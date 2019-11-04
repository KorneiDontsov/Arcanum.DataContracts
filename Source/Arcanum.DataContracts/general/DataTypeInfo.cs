// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.DataContracts {
	using System;
	using System.Collections.Immutable;
	using System.Linq;

	class DataTypeInfo: IDataTypeInfo {
		public Type dataType { get; }

		public IUnionInfo? asUnionInfo { get; }

		public Boolean isUnionInfo => asUnionInfo is { };

		public IUnionCaseInfo? asUnionCaseInfo { get; }

		public Boolean isUnionCaseInfo => asUnionCaseInfo is { };

		DataTypeInfo (Type dataType, UnionInfo? declaringUnionInfo = null) {
			static IUnionInfo? AsUnionInfo (DataTypeInfo dataTypeInfo) {
				static ImmutableList<Type> GetPotentialCaseTypes (Type dataType) =>
					dataType.GetNestedTypes()
						.Where(nestedType => nestedType.IsSubclassOf(dataType))
						.ToImmutableList();

				static IUnionInfo CreateUnionInfo (DataTypeInfo dataTypeInfo, ImmutableList<Type> potentialCaseTypes) =>
					new UnionInfo(
						dataTypeInfo,
						enumerateCaseInfos: unionInfo =>
							from caseType in potentialCaseTypes
							let caseDataTypeInfo = new DataTypeInfo(caseType, declaringUnionInfo: unionInfo)
							where caseType.IsAbstract is false || caseDataTypeInfo.isUnionInfo
							select caseDataTypeInfo.asUnionCaseInfo!);

				return
					dataTypeInfo.dataType.IsAbstract
					&& GetPotentialCaseTypes(dataTypeInfo.dataType) is var potentialCaseTypes
					&& potentialCaseTypes.Count > 0
					&& CreateUnionInfo(dataTypeInfo: dataTypeInfo, potentialCaseTypes) is var unionInfo
					&& unionInfo.caseInfos.Count > 0
						? unionInfo
						: null;
			}

			static IUnionCaseInfo? AsUnionCaseInfo (DataTypeInfo dataTypeInfo, UnionInfo? declaringUnionInfo = null) =>
				declaringUnionInfo is null ? null : new UnionCaseInfo(dataTypeInfo, declaringUnionInfo);

			this.dataType = dataType;
			asUnionInfo = AsUnionInfo(dataTypeInfo: this);
			asUnionCaseInfo = AsUnionCaseInfo(dataTypeInfo: this, declaringUnionInfo);
		}

		internal static IDataTypeInfo Create (Type dataType, IDataTypeInfoFactory factory) {
			static Exception MustBeClassOrStructure (Type dataType) =>
				new Exception($"'dataType' must be class or structure. Accepted {dataType.AssemblyQualifiedName}.");

			static IUnionCaseInfo? TryFindCaseInfo (Type dataType, IDataTypeInfoFactory factory) =>
				factory.Get(dataType.DeclaringType).asUnionInfo?.caseInfosByTypes.GetValueOrDefault(dataType);

			return
				! dataType.IsClass && ! dataType.IsValueType ? throw MustBeClassOrStructure(dataType)
				: dataType.IsNested && TryFindCaseInfo(dataType, factory) is { } caseInfo ? caseInfo.dataTypeInfo
				: new DataTypeInfo(dataType);
		}

		/// <inheritdoc cref = "IDataTypeInfo.ToString()" />
		public override String ToString () => dataType.ToString();
	}
}
