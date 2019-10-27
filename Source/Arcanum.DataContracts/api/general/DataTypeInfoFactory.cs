// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.DataContracts {
	using System;

	public sealed class DataTypeInfoFactory: IDataTypeInfoFactory {
		public static IDataTypeInfoFactory shared { get; } = new DataTypeInfoFactory();

		public IDataTypeInfo Get (Type dataType) => DataTypeInfo.Create(dataType, factory: this);
	}
}
