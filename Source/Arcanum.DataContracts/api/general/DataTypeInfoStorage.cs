// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.DataContracts {
	using System;
	using System.Collections.Concurrent;

	public sealed class DataTypeInfoStorage: IDataTypeInfoFactory {
		ConcurrentDictionary<Type, IDataTypeInfo> dataTypeInfoDict { get; }
			= new ConcurrentDictionary<Type, IDataTypeInfo>();

		public static IDataTypeInfoFactory shared { get; } = new DataTypeInfoStorage();

		public IDataTypeInfo Get (Type dataType) =>
			dataTypeInfoDict.GetOrAdd(dataType, dataType => DataTypeInfo.Create(dataType, factory: this));
	}
}
