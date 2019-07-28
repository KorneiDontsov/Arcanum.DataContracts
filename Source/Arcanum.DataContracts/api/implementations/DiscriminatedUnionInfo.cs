// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace Arcanum.DataContracts
{
	public sealed class DiscriminatedUnionInfo
	{
		public DataTypeInfo dataTypeInfo { get; }

		public IImmutableList<DiscriminatedUnionCaseInfo> caseInfos { get; }

		public IImmutableDictionary<Type, DiscriminatedUnionCaseInfo> caseInfosByTypes { get; }

		public IImmutableDictionary<String, DiscriminatedUnionCaseInfo> caseInfosByNames { get; }

		public ImmutableList<DiscriminatedUnionCaseErrorInfo> invalidCaseErrorInfos { get; }

		public Type dataType => dataTypeInfo.dataType;

		public DiscriminatedUnionCaseInfo? asDiscriminatedUnionCaseInfo => dataTypeInfo.asDiscriminatedUnionCaseInfo;

		public Boolean hasErrors => invalidCaseErrorInfos.Count > 0 || caseInfos.Count is 0;

		internal DiscriminatedUnionInfo (
			DataTypeInfo dataTypeInfo,
			Func<DiscriminatedUnionInfo, IEnumerable<DiscriminatedUnionCaseInfo>> caseInfoListConstructor
		)
		{
			this.dataTypeInfo = dataTypeInfo;

			var builderOfCaseInfos = ImmutableList.CreateBuilder<DiscriminatedUnionCaseInfo>();
			var builderOfCaseInfosByTypes = ImmutableDictionary.CreateBuilder<Type, DiscriminatedUnionCaseInfo>();
			var builderOfCaseInfosByNames = ImmutableDictionary.CreateBuilder<String, DiscriminatedUnionCaseInfo>();

			var builderOfInvalidCaseErrorInfos = ImmutableList.CreateBuilder<DiscriminatedUnionCaseErrorInfo>();

			foreach (var caseInfo in caseInfoListConstructor(this))
			{
				if (caseInfo.name is DataCaseName.Invalid)
				{
					builderOfInvalidCaseErrorInfos.Add(new DiscriminatedUnionCaseErrorInfo.HasInvalidName(caseInfo));
				}
				else if (builderOfCaseInfosByNames.TryGetValue(caseInfo.name.nameString, out var sameNameCaseInfo))
				{
					builderOfInvalidCaseErrorInfos.Add(
						new DiscriminatedUnionCaseErrorInfo.HasDuplicateName(caseInfo, sameNameCaseInfo)
					);
				}
				else
				{
					builderOfCaseInfos.Add(caseInfo);
					builderOfCaseInfosByTypes.Add(caseInfo.dataType, caseInfo);
					builderOfCaseInfosByNames.Add(caseInfo.name.nameString, caseInfo);
				}
			}

			caseInfos = builderOfCaseInfos.ToImmutable();
			caseInfosByTypes = builderOfCaseInfosByTypes.ToImmutable();
			caseInfosByNames = builderOfCaseInfosByNames.ToImmutable();
			invalidCaseErrorInfos = builderOfInvalidCaseErrorInfos.ToImmutable();
		}

		/// <inheritdoc />
		public override String ToString () => dataType.ToString();

		/// <summary>
		///     Must not be invoked for discriminated union that doesn't have errors.
		/// </summary>
		public void AppendErrorString (StringBuilder sb)
		{
			if (invalidCaseErrorInfos.Count is 0)
			{
				throw new Exception($"Discriminated union {this} doesn't have errors.");
			}

			if (caseInfos.Count is 0)
			{
				sb = sb.AppendLine("Discriminated union doesn't contain valid cases.");
			}

			sb = sb.AppendLine("There are invalid cases with errors.");

			foreach (var invalidCaseErrorInfo in invalidCaseErrorInfos)
			{
				sb = sb.Append('\t').AppendLine(invalidCaseErrorInfo.ToString());
			}
		}

		/// <inheritdoc cref="AppendErrorString(StringBuilder)" />
		public String GetErrorString ()
		{
			var sb = new StringBuilder(256);
			AppendErrorString(sb);

			return sb.ToString();
		}
	}
}
