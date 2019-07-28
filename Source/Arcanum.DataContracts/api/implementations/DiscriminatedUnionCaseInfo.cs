// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Linq;
using System.Reflection;

namespace Arcanum.DataContracts
{
	public sealed class DiscriminatedUnionCaseInfo
	{
		public DataTypeInfo dataTypeInfo { get; }

		public DiscriminatedUnionInfo declaringUnionInfo { get; }

		public DataCaseName name { get; }

		public Type dataType => dataTypeInfo.dataType;

		public DiscriminatedUnionInfo? asDiscriminatedUnionInfo => dataTypeInfo.asDiscriminatedUnionInfo;

		public Boolean isDiscriminatedUnionInfo => asDiscriminatedUnionInfo != null;

		private DiscriminatedUnionInfo? _lazyRootUnionInfo { get; set; }

		public DiscriminatedUnionInfo rootUnionInfo
		{
			get
			{
				if (_lazyRootUnionInfo is null)
				{
					DiscriminatedUnionInfo iUnionInfo;
					for (iUnionInfo = declaringUnionInfo;
						iUnionInfo.asDiscriminatedUnionCaseInfo != null;
						iUnionInfo = iUnionInfo.asDiscriminatedUnionCaseInfo.declaringUnionInfo) { }

					_lazyRootUnionInfo = iUnionInfo;
				}

				return _lazyRootUnionInfo;
			}
		}

		public DiscriminatedUnionCaseInfo? maybeDeclaringCaseInfo => declaringUnionInfo.asDiscriminatedUnionCaseInfo;

		internal DiscriminatedUnionCaseInfo (DataTypeInfo dataTypeInfo, DiscriminatedUnionInfo declaringUnionInfo)
		{
			this.dataTypeInfo = dataTypeInfo;
			this.declaringUnionInfo = declaringUnionInfo;

			name = dataTypeInfo.dataType
			.GetCustomAttributes()
			.OfType<IDataCaseAttribute>()
			.FirstOrDefault()
			?.name
			?? DataCaseName.Construct(nameString: dataTypeInfo.dataType.Name);
		}

		/// <inheritdoc />
		public override String ToString () => dataType.ToString();
	}
}
