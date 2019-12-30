// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.DataContracts.Tests {
	using Arcanum.Routes;
	using FluentAssertions;
	using Xunit;
	using static Arcanum.DataContracts.Module;

	public class TestUnionCaseInfoFunctions {
		public abstract class UnionExample {
			public abstract class Case: UnionExample {
				public sealed class InnerCase: Case { }
			}
		}

		[Fact]
		public void NestedCaseRouteIsGotten () {
			var unionCaseInfo = GetDataTypeInfo(typeof(UnionExample.Case.InnerCase)).asUnionCaseInfo!;
			Route expected = "Case/InnerCase";

			var actual = unionCaseInfo.GetNestedCaseRoute();

			actual.Should().Be(expected);
		}
	}
}
