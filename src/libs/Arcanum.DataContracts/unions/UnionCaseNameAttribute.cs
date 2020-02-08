// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.DataContracts {
	using System;

	[AttributeUsage(AttributeTargets.Class, Inherited = false)]
	public sealed class UnionCaseNameAttribute: Attribute, IUnionCaseNameProvider {
		public String value { get; }

		public UnionCaseNameAttribute (String value) => this.value = value;

		String IUnionCaseNameProvider.GetUnionCaseName (Type unionCaseType) => value;
	}
}
