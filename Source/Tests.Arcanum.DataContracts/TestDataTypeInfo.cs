// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Tests.Arcanum.DataContracts {
	using FluentAssertions;
	using FluentAssertions.Execution;
	using global::Arcanum.DataContracts;
	using JetBrains.Annotations;
	using System;
	using System.Linq;
	using Xunit;

	public abstract class TestDataTypeInfo {
		#region OfAnyData
		public abstract partial class Base {
			public abstract class OfAnyData {
				Type dataType { get; }
				protected IDataTypeInfo dataTypeInfo { get; }

				protected OfAnyData (Type dataType, IDataTypeInfoFactory factory) {
					this.dataType = dataType;
					dataTypeInfo = factory.Get(dataType);
				}

				[Fact]
				public void IsNotNull () =>
					dataTypeInfo.Should().NotBeNull();

				[Fact]
				public void HasDataType () =>
					dataTypeInfo.dataType.Should().Be(dataType);
			}
		}

		public abstract partial class FromFactory {
			static IDataTypeInfoFactory factory { get; } = DataTypeInfoFactory.shared;
		}

		public abstract partial class FromStorage {
			static IDataTypeInfoFactory factory { get; } = DataTypeInfoStorage.shared;
		}
		#endregion

		#region OfSimpleData
		[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
		sealed class SimpleDataExample {
			public Boolean isTrue { get; set; }
			public UInt32 someInteger { get; set; }
			public String? someText { get; set; }
		}

		partial class Base {
			public abstract class OfSimpleData: OfAnyData {
				protected OfSimpleData (IDataTypeInfoFactory factory):
					base(typeof(SimpleDataExample), factory) { }

				[Fact]
				public void DoesntHaveDiscriminatedUnionInfo () =>
					dataTypeInfo.asUnionInfo.Should().BeNull();

				[Fact]
				public void DoesntHaveDiscriminatedUnionCaseInfo () =>
					dataTypeInfo.asUnionCaseInfo.Should().BeNull();
			}
		}

		partial class FromFactory {
			public sealed class OfSimpleData: Base.OfSimpleData {
				public OfSimpleData (): base(factory) { }
			}
		}

		partial class FromStorage {
			public sealed class OfSimpleData: Base.OfSimpleData {
				public OfSimpleData (): base(factory) { }
			}
		}
		#endregion

		#region OfDiscriminatedUnionData
		abstract class DiscriminatedUnionDataExample {
			[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
			[DataCase("Case1Name")]
			public sealed class Case1: DiscriminatedUnionDataExample {
				public Byte[]? salt { get; set; }
			}

			[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
			public abstract class Case2Base: DiscriminatedUnionDataExample {
				public SByte[]? extraSalt { get; set; }
			}

			[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
			public sealed class Case2: Case2Base {
				public String? saltText { get; set; }
			}
		}

		partial class Base {
			public abstract class OfDiscriminatedUnionData: OfAnyData {
				protected OfDiscriminatedUnionData (IDataTypeInfoFactory factory):
					base(typeof(DiscriminatedUnionDataExample), factory) { }

				[Fact]
				public void DoesntHaveDiscriminatedUnionCaseInfo () =>
					dataTypeInfo.asUnionCaseInfo.Should().BeNull();

				[Fact]
				public void HasDiscriminatedUnionInfo () =>
					dataTypeInfo.asUnionInfo.Should().NotBeNull();

				[Fact]
				public void HasDiscriminatedUnionInfoThatDoesntHaveErrors () {
					using (new AssertionScope()) {
						dataTypeInfo.asUnionInfo!.hasErrors.Should().BeFalse();
						dataTypeInfo.asUnionInfo!.invalidCaseErrors.Count.Should().Be(0);
					}
				}

				[Fact]
				public void HasDiscriminatedUnionInfoThatHasDataType () =>
					dataTypeInfo.asUnionInfo!.dataType.Should().Be(typeof(DiscriminatedUnionDataExample));

				[Fact]
				public void HasDiscriminatedUnionInfoThatHasDataTypeInfo () =>
					dataTypeInfo.asUnionInfo!.dataTypeInfo.Should().BeSameAs(dataTypeInfo);

				[Fact]
				public void HasDiscriminatedUnionInfoThatHasCases () {
					var discriminatedUnionInfo = dataTypeInfo.asUnionInfo!;

					using (new AssertionScope()) {
						discriminatedUnionInfo.caseInfos
							.Select(i => (type: i.dataType, i.name.nameString, i.declaringUnionInfo))
							.Should()
							.BeEquivalentTo(
								(
									type: typeof(DiscriminatedUnionDataExample.Case1),
									name: "Case1Name",
									declaringUnionInfo: discriminatedUnionInfo
								),
								(
									type: typeof(DiscriminatedUnionDataExample.Case2),
									name: "Case2",
									declaringUnionInfo: discriminatedUnionInfo
								));

						discriminatedUnionInfo.caseInfos.Should()
							.OnlyContain(caseInfo => caseInfo.dataTypeInfo.dataType == caseInfo.dataType)
							.And.OnlyContain(caseInfo => caseInfo.dataTypeInfo.asUnionInfo == null)
							.And.OnlyContain(caseInfo => caseInfo.dataTypeInfo.isUnionInfo == false)
							.And.OnlyContain(
								caseInfo => ReferenceEquals(
									caseInfo.dataTypeInfo.asUnionCaseInfo, caseInfo));

						_ = discriminatedUnionInfo.caseInfosByTypes.Should()
							.OnlyContain(i => i.Key == i.Value.dataType);

						_ = discriminatedUnionInfo.caseInfosByTypes.Values.Should()
							.BeEquivalentTo(discriminatedUnionInfo.caseInfos);

						_ = discriminatedUnionInfo.caseInfosByNames.Should()
							.OnlyContain(i => i.Key == i.Value.name.nameString);

						_ = discriminatedUnionInfo.caseInfosByNames.Values.Should()
							.BeEquivalentTo(discriminatedUnionInfo.caseInfos);
					}
				}
			}
		}

		partial class FromFactory {
			public sealed class OfDiscriminatedUnionData: Base.OfDiscriminatedUnionData {
				public OfDiscriminatedUnionData (): base(factory) { }
			}
		}

		partial class FromStorage {
			public sealed class OfDiscriminatedUnionData: Base.OfDiscriminatedUnionData {
				public OfDiscriminatedUnionData (): base(factory) { }
			}
		}
		#endregion

		#region OfDiscriminatedUnionDataCase
		partial class Base {
			public abstract class OfDiscriminatedUnionDataCase: OfAnyData {
				protected OfDiscriminatedUnionDataCase (IDataTypeInfoFactory factory)
					: base(typeof(DiscriminatedUnionDataExample.Case1), factory) { }

				[Fact]
				public void DoesntHaveDiscriminatedUnionInfo () {
					_ = dataTypeInfo.asUnionInfo.Should()
						.BeNull();
				}

				[Fact]
				public void HasDiscriminatedUnionCaseInfo () {
					_ = dataTypeInfo.asUnionCaseInfo.Should()
						.NotBeNull();
				}

				[Fact]
				public void HasDiscriminatedUnionCaseInfoThatHasDataTypeInfo () {
					_ = dataTypeInfo.asUnionCaseInfo!.dataTypeInfo.Should()
						.BeSameAs(dataTypeInfo);
				}

				[Fact]
				public void HasDiscriminatedUnionCaseInfoThatHasDataType () {
					_ = dataTypeInfo.asUnionCaseInfo!.dataType.Should()
						.Be(typeof(DiscriminatedUnionDataExample.Case1));
				}

				[Fact]
				public void HasDiscriminatedUnionCaseInfoThatHasName () {
					_ = dataTypeInfo.asUnionCaseInfo!.name.nameString.Should()
						.Be("Case1Name");
				}

				[Fact]
				public void HasDiscriminatedUnionCaseInfoThatHasDeclaringUnionInfo () {
					_ = dataTypeInfo.asUnionCaseInfo!.declaringUnionInfo.dataType.Should()
						.Be(typeof(DiscriminatedUnionDataExample));
				}
			}
		}

		partial class FromFactory {
			public sealed class OfDiscriminatedUnionDataCase: Base.OfDiscriminatedUnionDataCase {
				public OfDiscriminatedUnionDataCase (): base(factory) { }
			}
		}

		partial class FromStorage {
			public sealed class OfDiscriminatedUnionDataCase: Base.OfDiscriminatedUnionDataCase {
				public OfDiscriminatedUnionDataCase (): base(factory) { }
			}
		}
		#endregion
	}
}
