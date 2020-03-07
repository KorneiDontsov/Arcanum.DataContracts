// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.DataContracts {
	using Arcanum.Companions;
	using System;
	using System.Collections.Generic;
	using System.Collections.Immutable;
	using System.Linq;
	using static Arcanum.DataContracts.DataContractModule;

	class DataTypeInfo: IDataTypeInfo {
		/// <inheritdoc />
		public Type dataType { get; }

		/// <inheritdoc />
		public DataContract contract { get; }

		/// <inheritdoc />
		public IUnionInfo? asUnionInfo { get; }

		/// <inheritdoc />
		public Boolean isUnionInfo => asUnionInfo is { };

		/// <inheritdoc />
		public IUnionCaseInfo? asUnionCaseInfo { get; }

		/// <inheritdoc />
		public Boolean isUnionCaseInfo => asUnionCaseInfo is { };

		public DataTypeInfo (Type dataType) {
			static ImmutableList<Type> GetPotentialCaseTypes (Type dataType) =>
				dataType.EnumerateClosedNestedTypes()
					.Where(t => t.IsSubclassOf(dataType) && ! t.HasCompanion<INotUnionCaseCompanion>())
					.ToImmutableList();

			static IUnionInfo CreateUnionInfo (DataTypeInfo dataTypeInfo, ImmutableList<Type> potentialCaseTypes) {
				IEnumerable<IUnionCaseInfo> EnumerateCaseInfos (UnionInfo unionInfo) {
					foreach (var caseType in potentialCaseTypes) {
						var caseDataTypeInfo = new DataTypeInfo(caseType, declaringUnionInfo: unionInfo);
						if (! caseType.IsAbstract || caseDataTypeInfo.isUnionInfo)
							yield return caseDataTypeInfo.asUnionCaseInfo!;
					}
				}

				return new UnionInfo(dataTypeInfo, EnumerateCaseInfos);
			}

			static IUnionInfo? AsUnionInfo (DataTypeInfo dataTypeInfo) =>
				dataTypeInfo.dataType.IsAbstract
				&& GetPotentialCaseTypes(dataTypeInfo.dataType) is var potentialCaseTypes
				&& potentialCaseTypes.Count > 0
				&& CreateUnionInfo(dataTypeInfo, potentialCaseTypes) is var unionInfo
				&& unionInfo.caseInfos.Count > 0
					? unionInfo
					: null;

			this.dataType = dataType;
			contract = CreateDataContract(dataType);
			asUnionInfo = AsUnionInfo(dataTypeInfo: this);
		}

		DataTypeInfo (Type dataType, UnionInfo declaringUnionInfo):
			this(dataType) =>
			asUnionCaseInfo = new UnionCaseInfo(this, declaringUnionInfo);

		/// <inheritdoc cref = "IDataTypeInfo.ToString()" />
		public override String ToString () => dataType.FullName;
	}
}
