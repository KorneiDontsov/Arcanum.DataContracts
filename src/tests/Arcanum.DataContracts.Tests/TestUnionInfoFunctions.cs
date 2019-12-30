// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.DataContracts.Tests {
	using FluentAssertions;
	using System;
	using Xunit;
	using static Arcanum.DataContracts.Module;

	public class TestUnionInfoFunctions {
		public abstract class UnionExample {
			public abstract class Case: UnionExample {
				public sealed class InnerCase: Case { }
			}
		}

		IUnionInfo unionInfo { get; } = GetDataTypeInfo(typeof(UnionExample)).asUnionInfo!;

		[Fact]
		public void NestedCaseInfoIsGotten () {
			var expected = GetDataTypeInfo(typeof(UnionExample.Case.InnerCase)).asUnionCaseInfo!;
			var actual = unionInfo.GetNestedCaseInfo("Case/InnerCase");
			actual.Should().BeSameAs(expected);
		}

		[Theory]
		[InlineData("UnknownCase")]
		[InlineData("Case/UnknownInnerCase")]
		public void GetNestedCaseInfoThrowsFormatException (String caseRouteStr) {
			Action action = () => unionInfo.GetNestedCaseInfo(caseRouteStr);
			action.Should().Throw<FormatException>();
		}
	}
}
