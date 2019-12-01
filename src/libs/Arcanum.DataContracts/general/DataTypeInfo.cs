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

		public DataTypeInfo (Type dataType) {
			static IUnionInfo? AsUnionInfo (DataTypeInfo dataTypeInfo) {
				static ImmutableList<Type> GetPotentialCaseTypes (Type dataType) =>
					dataType.EnumerateClosedNestedTypes()
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

			this.dataType = dataType;
			asUnionInfo = AsUnionInfo(dataTypeInfo: this);
		}

		DataTypeInfo (Type dataType, UnionInfo declaringUnionInfo):
			this(dataType) =>
			asUnionCaseInfo = new UnionCaseInfo(this, declaringUnionInfo);

		/// <inheritdoc cref = "IDataTypeInfo.ToString()" />
		public override String ToString () => dataType.ToString();
	}
}
