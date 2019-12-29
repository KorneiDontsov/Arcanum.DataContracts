// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.DataContracts {
	using System;

	class UnionCaseInfo: IUnionCaseInfo {
		public IDataTypeInfo dataTypeInfo { get; }

		public IUnionInfo declaringUnionInfo { get; }

		public String name { get; }

		internal UnionCaseInfo (IDataTypeInfo dataTypeInfo, IUnionInfo declaringUnionInfo) {
			this.dataTypeInfo = dataTypeInfo;
			this.declaringUnionInfo = declaringUnionInfo;
			name =
				dataTypeInfo.dataType.MatchCustomAttribute<IUnionCaseAttribute>()?.name
				?? dataTypeInfo.dataType.Name;
		}

		/// <inheritdoc cref = "IUnionCaseInfo.ToString()" />
		public override String ToString () => $"union case {dataTypeInfo}";

		public Type dataType => dataTypeInfo.dataType;

		public IUnionInfo? asUnionInfo => dataTypeInfo.asUnionInfo;

		public Boolean isUnionInfo => asUnionInfo is { };

		public IUnionCaseInfo? maybeDeclaringCaseInfo => declaringUnionInfo.asUnionCaseInfo;

		IUnionInfo? lazyRootUnionInfo;

		public IUnionInfo rootUnionInfo {
			get {
				IUnionInfo Create () {
					var currentUnionInfo = declaringUnionInfo;
					while (currentUnionInfo.asUnionCaseInfo is { declaringUnionInfo: var nextUnionInfo })
						currentUnionInfo = nextUnionInfo;

					return currentUnionInfo;
				}

				return lazyRootUnionInfo ??= Create();
			}
		}
	}
}
