// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Collections.Immutable;

namespace Arcanum.DataContracts
{
	public sealed class DiscriminatedUnionInfo
	{
		public DataTypeInfo dataTypeInfo { get; }

		public IImmutableList<DiscriminatedUnionCaseInfo> caseInfos { get; }

		public IImmutableDictionary<Type, DiscriminatedUnionCaseInfo> caseInfosByTypes { get; }

		public IImmutableDictionary<String, DiscriminatedUnionCaseInfo> caseInfosByNames { get; }

		public Type dataType => dataTypeInfo.dataType;

		internal DiscriminatedUnionInfo (
			DataTypeInfo dataTypeInfo,
			Func<DiscriminatedUnionInfo, IImmutableList<DiscriminatedUnionCaseInfo>> caseInfoListConstructor
		)
		{
			this.dataTypeInfo = dataTypeInfo;
			caseInfos = caseInfoListConstructor(this);
			caseInfosByTypes = caseInfos.ToImmutableDictionary(i => i.dataType);
			caseInfosByNames = caseInfos.ToImmutableDictionary(i => i.name);
		}
	}
}
