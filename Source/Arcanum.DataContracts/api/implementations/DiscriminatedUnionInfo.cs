// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Collections.Immutable;

namespace Arcanum.DataContracts
{
	public sealed class DiscriminatedUnionInfo
	{
		public DataTypeInfo typeInfo { get; }

		public IImmutableList<DiscriminatedUnionCaseInfo> caseInfos { get; }

		public IImmutableDictionary<Type, DiscriminatedUnionCaseInfo> caseInfosByTypes { get; }

		public IImmutableDictionary<String, DiscriminatedUnionCaseInfo> caseInfosByNames { get; }

		public Type dataType => typeInfo.dataType;

		internal DiscriminatedUnionInfo (
			DataTypeInfo typeInfo,
			Func<DiscriminatedUnionInfo, IImmutableList<DiscriminatedUnionCaseInfo>> caseInfoListConstructor
		)
		{
			this.typeInfo = typeInfo;
			caseInfos = caseInfoListConstructor(this);
			caseInfosByTypes = caseInfos.ToImmutableDictionary(i => i.dataType);
			caseInfosByNames = caseInfos.ToImmutableDictionary(i => i.name);
		}
	}
}
