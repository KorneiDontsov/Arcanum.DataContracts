// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

using System;

namespace Arcanum.DataContracts
{
	public interface IDataCaseAttribute
	{
		String name { get; }
	}

	public sealed class DataCaseAttribute : Attribute, IDataCaseAttribute
	{
		public String name { get; }

		public DataCaseAttribute (String name) => this.name = name;
	}
}
