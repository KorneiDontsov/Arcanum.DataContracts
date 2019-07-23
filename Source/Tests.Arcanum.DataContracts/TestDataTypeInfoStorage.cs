// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

using Arcanum.DataContracts;

using FluentAssertions;

using Xunit;

namespace Tests.Arcanum.DataContracts
{
	public abstract class TestDataTypeInfoStorage
	{
		public abstract class Base
		{
			private sealed class SomeData { }

			protected IDataTypeInfoStorage storage { get; }

			protected Base (IDataTypeInfoStorage storage)
			{
				this.storage = storage;
			}

			[Fact]
			public void DoesntGiveNullInsteadInfo ()
			{
				var someDataTypeInfo = storage.Get(typeof(SomeData));

				_ = someDataTypeInfo.Should().NotBeNull();
			}

			[Fact]
			public void DoCache ()
			{
				var someDataTypeInfo = storage.Get(typeof(SomeData));
				var sameDataTypeInfo = storage.Get(typeof(SomeData));

				_ = someDataTypeInfo.Should().BeSameAs(sameDataTypeInfo);
			}
		}

		public sealed class Shared : Base
		{
			public Shared () : base(DataTypeInfoStorage.shared) { }
		}
	}
}
