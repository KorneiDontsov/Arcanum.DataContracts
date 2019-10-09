// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.DataContracts {
	using System;
	using System.Collections.Generic;
	using System.Collections.Immutable;
	using System.Linq;
	using System.Text;

	sealed class UnionInfo: IUnionInfo {
		public IDataTypeInfo dataTypeInfo { get; }

		public IImmutableList<IUnionCaseInfo> caseInfos { get; }

		public IImmutableDictionary<Type, IUnionCaseInfo> caseInfosByTypes { get; }

		public IImmutableDictionary<String, IUnionCaseInfo> caseInfosByNames { get; }

		public IImmutableList<UnionCaseError> invalidCaseErrors { get; }

		public Type dataType => dataTypeInfo.dataType;

		public IUnionCaseInfo? asUnionCaseInfo => dataTypeInfo.asUnionCaseInfo;

		public Boolean isUnionCaseInfo => asUnionCaseInfo != null;

		public IUnionInfo rootUnionInfo => asUnionCaseInfo?.rootUnionInfo ?? this;

		public Boolean hasErrors => invalidCaseErrors.Count > 0 || caseInfos.Count is 0;

		internal UnionInfo (
		IDataTypeInfo dataTypeInfo,
		Func<UnionInfo, IEnumerable<IUnionCaseInfo>> enumerateCaseInfos) {
			this.dataTypeInfo = dataTypeInfo;

			var caseInfosB = ImmutableList.CreateBuilder<IUnionCaseInfo>();
			var caseInfosOfTypesB = ImmutableDictionary.CreateBuilder<Type, IUnionCaseInfo>();
			var caseInfosByNamesB = ImmutableDictionary.CreateBuilder<String, IUnionCaseInfo>();

			var invalidCaseErrorInfosB = ImmutableList.CreateBuilder<UnionCaseError>();

			foreach (var caseInfo in enumerateCaseInfos(this))
				if (caseInfo.name is DataCaseName.Invalid)
					invalidCaseErrorInfosB.Add(new UnionCaseError.HasInvalidName(caseInfo));
				else if (caseInfosByNamesB.TryGetValue(caseInfo.name.nameString, out var sameNameCaseInfo))
					invalidCaseErrorInfosB.Add(new UnionCaseError.HasDuplicateName(caseInfo, sameNameCaseInfo));
				else {
					caseInfosB.Add(caseInfo);
					caseInfosOfTypesB.Add(caseInfo.dataType, caseInfo);
					caseInfosByNamesB.Add(caseInfo.name.nameString, caseInfo);
				}

			caseInfos = caseInfosB.ToImmutable();
			caseInfosByTypes = caseInfosOfTypesB.ToImmutable();
			caseInfosByNames = caseInfosByNamesB.ToImmutable();
			invalidCaseErrors = invalidCaseErrorInfosB.ToImmutable();
		}

		/// <inheritdoc />
		public override String ToString () => dataType.ToString();

		/// <summary>
		///     Must not be invoked for union that doesn't have errors.
		/// </summary>
		public String GetErrorString () {
			if (invalidCaseErrors.Count is 0) throw new Exception($"Union {this} doesn't have errors.");

			var errorSb = new StringBuilder(256);
			errorSb = caseInfos.Count > 0 ? errorSb : errorSb.AppendLine("Union doesn't contain valid cases.");
			errorSb = invalidCaseErrors.Aggregate(
				seed: errorSb.AppendLine("There are invalid cases with errors."),
				func: (sb, invalidCaseErrorInfo) =>
					sb.Append('\t').AppendLine(invalidCaseErrorInfo.ToString()));

			return errorSb.ToString();
		}
	}
}
