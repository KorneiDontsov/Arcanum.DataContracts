// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.DataContracts {
	using System;
	using System.Collections.Generic;
	using System.Collections.Immutable;
	using System.Linq;
	using System.Text;

	class UnionInfo: IUnionInfo {
		public IDataTypeInfo dataTypeInfo { get; }

		public IImmutableList<IUnionCaseInfo> caseInfos { get; }

		public IImmutableDictionary<Type, IUnionCaseInfo> caseInfosByTypes { get; }

		public IImmutableDictionary<String, IUnionCaseInfo> caseInfosByNames { get; }

		public IImmutableList<UnionCaseError> invalidCaseErrors { get; }

		public Type dataType => dataTypeInfo.dataType;

		public IUnionCaseInfo? asUnionCaseInfo => dataTypeInfo.asUnionCaseInfo;

		public Boolean isUnionCaseInfo => asUnionCaseInfo is { };

		public IUnionInfo rootUnionInfo => asUnionCaseInfo?.rootUnionInfo ?? this;

		public Boolean hasErrors => invalidCaseErrors.Count > 0 || caseInfos.Count is 0;

		internal UnionInfo
		(IDataTypeInfo dataTypeInfo,
		 Func<UnionInfo, IEnumerable<IUnionCaseInfo>> enumerateCaseInfos) {
			this.dataTypeInfo = dataTypeInfo;

			var caseInfosB = ImmutableList.CreateBuilder<IUnionCaseInfo>();
			var caseInfosByTypesB = ImmutableDictionary.CreateBuilder<Type, IUnionCaseInfo>();
			var caseInfosByNamesB = ImmutableDictionary.CreateBuilder<String, IUnionCaseInfo>();

			var invalidCaseErrorInfosB = ImmutableList.CreateBuilder<UnionCaseError>();

			foreach (var caseInfo in enumerateCaseInfos(this))
				if (caseInfosByNamesB.GetValueOrDefault(caseInfo.name) is { } sameNameCaseInfo)
					invalidCaseErrorInfosB.Add(new UnionCaseError.HasDuplicateName(caseInfo, sameNameCaseInfo));
				else {
					caseInfosB.Add(caseInfo);
					caseInfosByTypesB.Add(caseInfo.dataType, caseInfo);
					caseInfosByNamesB.Add(caseInfo.name, caseInfo);
				}

			caseInfos = caseInfosB.ToImmutable();
			caseInfosByTypes = caseInfosByTypesB.ToImmutable();
			caseInfosByNames = caseInfosByNamesB.ToImmutable();
			invalidCaseErrors = invalidCaseErrorInfosB.ToImmutable();
		}

		/// <inheritdoc />
		public override String ToString () => dataType.ToString();

		/// <summary>
		///     Must not be invoked for union that doesn't have errors.
		/// </summary>
		public String GetErrorString () {
			if (invalidCaseErrors.Count is 0)
				throw new Exception($"Union {this} doesn't have errors.");
			else {
				var empty = new StringBuilder(256);
				var er1 = caseInfos.Count > 0 ? empty : empty.AppendLine("Union doesn't contain valid cases.");
				var er2 = invalidCaseErrors.Aggregate(
					seed: er1.AppendLine("There are invalid cases with errors."),
					(sb,
					 invalidCaseErrorInfo) =>
						sb.Append('\t').AppendLine(invalidCaseErrorInfo.ToString()));
				return er2.ToString();
			}
		}
	}
}
