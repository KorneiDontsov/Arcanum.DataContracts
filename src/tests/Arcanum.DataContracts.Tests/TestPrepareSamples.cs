// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.DataContracts.Tests {
	using FluentAssertions;
	using JetBrains.Annotations;
	using System;
	using Xunit;
	using static Arcanum.DataContracts.Module;

	public class TestPrepareSamples {
		class DataSampleFactoryBuilder: IDataSampleFactoryBuilder {
			public static DataSampleFactoryBuilder shared { get; } =
				new DataSampleFactoryBuilder();

			/// <inheritdoc />
			public IDataSampleFactoryBuilder Self<TSample> (Func<TSample> createSample) where TSample: class =>
				this;

			/// <inheritdoc />
			public IDataSampleFactoryBuilder Item<TSample> (Func<TSample> createSample) where TSample: class =>
				this;

			/// <inheritdoc />
			public IDataSampleFactoryBuilder Member<TSample> (String name, Func<TSample> createSample)
				where TSample: class =>
				this;
		}

		class Data {
			[UsedImplicitly]
			public static void PrepareSamples (IDataSampleFactoryBuilder builder) =>
				builder.Should().BeSameAs(DataSampleFactoryBuilder.shared);
		}

		[Fact]
		public void Works () {
			var contract = GetDataTypeInfo(typeof(Data)).contract;
			contract.mayPrepareSamples.Should().NotBeNull();
			contract.mayPrepareSamples!(DataSampleFactoryBuilder.shared);
		}
	}
}
