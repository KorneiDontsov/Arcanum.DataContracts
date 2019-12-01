// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.DataContracts.Tests {
	using FluentAssertions;
	using Xunit;
	using static Arcanum.DataContracts.DataContractModule;

	public class TestDataTypeInfoStorage {
		sealed class SomeData { }

		[Fact]
		public void DoesntGiveNullInsteadInfo () {
			var someDataTypeInfo = GetDataTypeInfo(typeof(SomeData));
			someDataTypeInfo.Should().NotBeNull();
		}

		[Fact]
		public void DoCache () {
			var someDataTypeInfo = GetDataTypeInfo(typeof(SomeData));
			var sameDataTypeInfo = GetDataTypeInfo(typeof(SomeData));
			someDataTypeInfo.Should().BeSameAs(sameDataTypeInfo);
		}
	}
}
