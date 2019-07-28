// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

using System;

namespace Arcanum.DataContracts
{
	public interface IDataCaseAttribute
	{
		DataCaseName name { get; }
	}

	public sealed class DataCaseAttribute : Attribute, IDataCaseAttribute
	{
		public DataCaseName name { get; }

		/// <inheritdoc />
		/// <param name="name">Must contain only latin letters, digits and underscores.</param>
		public DataCaseAttribute (String name) => this.name = DataCaseName.Construct(name);
	}
}
