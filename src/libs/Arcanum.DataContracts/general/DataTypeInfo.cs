// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.DataContracts {
	using Arcanum.Companions;
	using System;
	using System.Collections.Generic;
	using System.Collections.Immutable;
	using System.Linq;

	class DataTypeInfo: IDataTypeInfo {
		public Type dataType { get; }

		public IUnionInfo? asUnionInfo { get; }

		public Boolean isUnionInfo => asUnionInfo is { };

		public IUnionCaseInfo? asUnionCaseInfo { get; }

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
			asUnionInfo = AsUnionInfo(dataTypeInfo: this);
		}

		DataTypeInfo (Type dataType, UnionInfo declaringUnionInfo):
			this(dataType) =>
			asUnionCaseInfo = new UnionCaseInfo(this, declaringUnionInfo);

		/// <inheritdoc cref = "IDataTypeInfo.ToString()" />
		public override String ToString () => dataType.FullName;
	}
}
