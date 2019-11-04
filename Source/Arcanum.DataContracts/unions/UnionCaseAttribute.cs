// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.DataContracts {
	using System;

	public interface IUnionCaseAttribute {
		UnionCaseName name { get; }
	}

	public sealed class UnionCaseAttribute: Attribute, IUnionCaseAttribute {
		public UnionCaseName name { get; }

		/// <inheritdoc />
		/// <param name = "name"> Must contain only latin letters, digits and underscores. </param>
		public UnionCaseAttribute (String name) => this.name = UnionCaseName.Create(name);
	}
}
