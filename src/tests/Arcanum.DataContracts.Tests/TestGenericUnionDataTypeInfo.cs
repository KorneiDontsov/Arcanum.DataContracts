// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.DataContracts.Tests {
	using FluentAssertions;
	using System;
	using System.Linq;
	using Xunit;
	using static Arcanum.DataContracts.DataContractModule;

	public class TestGenericUnionDataTypeInfo {
		public abstract class GenericUnionExample<T> {
			public sealed class Case1: GenericUnionExample<T> {
				public T item { get; }

				public Case1 (T item) => this.item = item;
			}
		}

		IDataTypeInfo dataTypeInfo { get; } = GetDataTypeInfo(typeof(GenericUnionExample<Object>));

		[Fact]
		public void HasUnionInfo () =>
			dataTypeInfo.asUnionInfo.Should().NotBeNull();

		[Fact]
		public void HasNoUnionCaseInfo () =>
			dataTypeInfo.asUnionCaseInfo.Should().BeNull();

		[Fact]
		public void HasCases () =>
			dataTypeInfo.asUnionInfo!.caseInfos
				.Select(i => (type: i.dataType, i.name))
				.Should()
				.BeEquivalentTo(
					(typeof(GenericUnionExample<Object>.Case1), "Case1"));
	}
}
