// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Reflection;

namespace Arcanum.DataContracts
{
	public sealed class DiscriminatedUnionCaseInfo
	{
		public DataTypeInfo typeInfo { get; }

		public DiscriminatedUnionInfo declaringUnionInfo { get; }

		public String name { get; }

		public Type type => typeInfo.type;

		internal DiscriminatedUnionCaseInfo (DataTypeInfo typeInfo, DiscriminatedUnionInfo declaringUnionInfo)
		{
			this.typeInfo = typeInfo;
			this.declaringUnionInfo = declaringUnionInfo;

			name = (
				typeInfo.type.GetCustomAttribute(typeof(IDataCaseAttribute), inherit: true)
				as IDataCaseAttribute
			)
			?.name
			?? typeInfo.type.Name;
		}
	}
}
