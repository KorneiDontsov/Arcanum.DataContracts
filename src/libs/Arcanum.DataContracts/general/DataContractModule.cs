// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.DataContracts {
	using System;
	using System.Collections.Concurrent;
	using System.Collections.Immutable;

	public static class DataContractModule {
		static ConcurrentDictionary<Type, IDataTypeInfo> dataTypeInfoStorage { get; }
			= new ConcurrentDictionary<Type, IDataTypeInfo>();

		public static IDataTypeInfo GetDataTypeInfo (Type dataType) {
			static Exception MustBeClassOrStructure (Type dataType) =>
				new Exception($"'dataType' must be class or structure. Accepted {dataType.AssemblyQualifiedName}.");

			static IUnionCaseInfo? MaybeCaseInfo (Type dataType) =>
				GetDataTypeInfo(dataType.DeclaringType).asUnionInfo?.caseInfosByTypes.GetValueOrDefault(dataType);

			static IDataTypeInfo CreateDataTypeInfo (Type dataType) =>
				! dataType.IsClass && ! dataType.IsValueType ? throw MustBeClassOrStructure(dataType)
				: dataType.IsNested && MaybeCaseInfo(dataType) is { } caseInfo ? caseInfo.dataTypeInfo
				: new DataTypeInfo(dataType);

			return dataTypeInfoStorage.GetOrAdd(dataType, it => CreateDataTypeInfo(it));
		}
	}
}
