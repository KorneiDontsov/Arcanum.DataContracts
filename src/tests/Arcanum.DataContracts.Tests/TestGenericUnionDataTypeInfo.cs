// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.DataContracts.Tests {
	using FluentAssertions;
	using System;
	using System.Linq;
	using Xunit;
	using static Arcanum.DataContracts.Module;

	public class TestGenericUnionDataTypeInfo {
		public abstract class GenericUnionExample<T> where T: struct {
			public sealed class Case1: GenericUnionExample<T> {
				public T item { get; }

				public Case1 (T item) => this.item = item;
			}
		}

		IDataTypeInfo dataTypeInfo { get; } = GetDataTypeInfo(typeof(GenericUnionExample<Byte>));

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
					(typeof(GenericUnionExample<Byte>.Case1), "Case1"));
	}
}
