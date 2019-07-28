// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Collections.Concurrent;

namespace Arcanum.DataContracts
{
	public sealed class DataTypeInfoStorage : IDataTypeInfoStorage
	{
		private ConcurrentDictionary<Type, DataTypeInfo> _dataTypeInfoDict { get; }
			= new ConcurrentDictionary<Type, DataTypeInfo>();

		public static IDataTypeInfoStorage shared { get; } = new DataTypeInfoStorage();

		/// <inheritdoc />
		public DataTypeInfo Get (Type dataType)
		{
			return _dataTypeInfoDict.GetOrAdd(
				dataType,
				dataType => DataTypeInfo.construct(dataType, maybeStorage: this)
			);
		}
	}
}
