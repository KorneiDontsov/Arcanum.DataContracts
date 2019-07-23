// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

using System;

namespace Arcanum.DataContracts
{
	public interface IDataTypeInfoStorage
	{
		DataTypeInfo Get (Type dataType);
	}
}
