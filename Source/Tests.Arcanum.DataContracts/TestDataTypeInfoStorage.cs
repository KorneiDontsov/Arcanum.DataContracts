// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Tests.Arcanum.DataContracts {
	using FluentAssertions;
	using global::Arcanum.DataContracts;
	using Xunit;

	public abstract class TestDataTypeInfoStorage {
		public abstract class Base {
			sealed class SomeData { }

			protected IDataTypeInfoFactory storage { get; }

			protected Base (IDataTypeInfoFactory storage) => this.storage = storage;

			[Fact]
			public void DoesntGiveNullInsteadInfo () {
				var someDataTypeInfo = storage.Get(typeof(SomeData));
				someDataTypeInfo.Should().NotBeNull();
			}

			[Fact]
			public void DoCache () {
				var someDataTypeInfo = storage.Get(typeof(SomeData));
				var sameDataTypeInfo = storage.Get(typeof(SomeData));
				someDataTypeInfo.Should().BeSameAs(sameDataTypeInfo);
			}
		}

		public sealed class Shared: Base {
			public Shared (): base(DataTypeInfoStorage.shared) { }
		}
	}
}
