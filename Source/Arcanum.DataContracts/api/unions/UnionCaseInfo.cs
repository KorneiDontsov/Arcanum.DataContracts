// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.DataContracts {
	using System;

	sealed class UnionCaseInfo: IUnionCaseInfo {
		public IDataTypeInfo dataTypeInfo { get; }

		public IUnionInfo declaringUnionInfo { get; }

		public UnionCaseName name { get; }

		public Type dataType => dataTypeInfo.dataType;

		public IUnionInfo? asDiscriminatedUnionInfo => dataTypeInfo.asUnionInfo;

		public Boolean isDiscriminatedUnionInfo => asDiscriminatedUnionInfo != null;

		IUnionInfo? lazyRootUnionInfo;

		public IUnionInfo rootUnionInfo {
			get {
				IUnionInfo Init () {
					var currentUnionInfo = declaringUnionInfo;
					while (currentUnionInfo.asUnionCaseInfo is { declaringUnionInfo: var nextUnionInfo })
						currentUnionInfo = nextUnionInfo;

					return currentUnionInfo;
				}

				return lazyRootUnionInfo ??= Init();
			}
		}

		public IUnionCaseInfo? maybeDeclaringCaseInfo => declaringUnionInfo.asUnionCaseInfo;

		internal UnionCaseInfo (IDataTypeInfo dataTypeInfo, IUnionInfo declaringUnionInfo) {
			this.dataTypeInfo = dataTypeInfo;
			this.declaringUnionInfo = declaringUnionInfo;

			name =
				dataTypeInfo.dataType.TryFindAttributeByAbstraction<IUnionCaseAttribute>()?.name
				?? UnionCaseName.Create(nameString: dataTypeInfo.dataType.Name);
		}
	}
}
