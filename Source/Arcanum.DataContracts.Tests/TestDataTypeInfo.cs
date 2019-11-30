// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.DataContracts.Tests {
	using FluentAssertions;
	using FluentAssertions.Execution;
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
				public void DoesntHaveUnionInfo () =>
					dataTypeInfo.asUnionInfo.Should().BeNull();

				[Fact]
				public void DoesntHaveUnionCaseInfo () =>
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

		#region OfUnionData
		abstract class UnionDataExample {
			[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
			[UnionCase("Case1Name")]
			public sealed class Case1: UnionDataExample {
				public Byte[]? salt { get; set; }
			}

			[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
			public abstract class Case2Base: UnionDataExample {
				public SByte[]? extraSalt { get; set; }
			}

			[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
			public sealed class Case2: Case2Base {
				public String? saltText { get; set; }
			}
		}

		partial class Base {
			public abstract class OfUnionData: OfAnyData {
				protected OfUnionData (IDataTypeInfoFactory factory):
					base(typeof(UnionDataExample), factory) { }

				[Fact]
				public void DoesntHaveUnionCaseInfo () =>
					dataTypeInfo.asUnionCaseInfo.Should().BeNull();

				[Fact]
				public void HasUnionInfo () =>
					dataTypeInfo.asUnionInfo.Should().NotBeNull();

				[Fact]
				public void HasUnionInfoThatDoesntHaveErrors () {
					using (new AssertionScope()) {
						dataTypeInfo.asUnionInfo!.hasErrors.Should().BeFalse();
						dataTypeInfo.asUnionInfo!.invalidCaseErrors.Count.Should().Be(0);
					}
				}

				[Fact]
				public void HasUnionInfoThatHasDataType () =>
					dataTypeInfo.asUnionInfo!.dataType.Should().Be(typeof(UnionDataExample));

				[Fact]
				public void HasUnionInfoThatHasDataTypeInfo () =>
					dataTypeInfo.asUnionInfo!.dataTypeInfo.Should().BeSameAs(dataTypeInfo);

				[Fact]
				public void HasUnionInfoThatHasCases () {
					var unionInfo = dataTypeInfo.asUnionInfo!;

					using (new AssertionScope()) {
						unionInfo.caseInfos
							.Select(i => (type: i.dataType, i.name, i.declaringUnionInfo))
							.Should()
							.BeEquivalentTo(
								(
									type: typeof(UnionDataExample.Case1),
									name: "Case1Name",
									declaringUnionInfo: unionInfo
								),
								(
									type: typeof(UnionDataExample.Case2),
									name: "Case2",
									declaringUnionInfo: unionInfo
								));

						unionInfo.caseInfos.Should()
							.OnlyContain(caseInfo => caseInfo.dataTypeInfo.dataType == caseInfo.dataType)
							.And.OnlyContain(caseInfo => caseInfo.dataTypeInfo.asUnionInfo == null)
							.And.OnlyContain(caseInfo => caseInfo.dataTypeInfo.isUnionInfo == false)
							.And.OnlyContain(
								caseInfo => ReferenceEquals(
									caseInfo.dataTypeInfo.asUnionCaseInfo, caseInfo));

						unionInfo.caseInfosByTypes.Should()
							.OnlyContain(i => i.Key == i.Value.dataType);

						unionInfo.caseInfosByTypes.Values.Should()
							.BeEquivalentTo(unionInfo.caseInfos);

						unionInfo.caseInfosByNames.Should()
							.OnlyContain(i => i.Key == i.Value.name);

						unionInfo.caseInfosByNames.Values.Should()
							.BeEquivalentTo(unionInfo.caseInfos);
					}
				}
			}
		}

		partial class FromFactory {
			public sealed class OfUnionData: Base.OfUnionData {
				public OfUnionData (): base(factory) { }
			}
		}

		partial class FromStorage {
			public sealed class OfUnionData: Base.OfUnionData {
				public OfUnionData (): base(factory) { }
			}
		}
		#endregion

		#region OfUnionDataCase
		partial class Base {
			public abstract class OfUnionDataCase: OfAnyData {
				protected OfUnionDataCase (IDataTypeInfoFactory factory)
					: base(typeof(UnionDataExample.Case1), factory) { }

				[Fact]
				public void DoesntHaveUnionInfo () =>
					dataTypeInfo.asUnionInfo.Should().BeNull();

				[Fact]
				public void HasUnionCaseInfo () =>
					dataTypeInfo.asUnionCaseInfo.Should().NotBeNull();

				[Fact]
				public void HasUnionCaseInfoThatHasDataTypeInfo () =>
					dataTypeInfo.asUnionCaseInfo!.dataTypeInfo.Should().BeSameAs(dataTypeInfo);

				[Fact]
				public void HasUnionCaseInfoThatHasDataType () =>
					dataTypeInfo.asUnionCaseInfo!.dataType.Should().Be(typeof(UnionDataExample.Case1));

				[Fact]
				public void HasUnionCaseInfoThatHasName () =>
					dataTypeInfo.asUnionCaseInfo!.name.Should().Be("Case1Name");

				[Fact]
				public void HasUnionCaseInfoThatHasDeclaringUnionInfo () =>
					dataTypeInfo.asUnionCaseInfo!.declaringUnionInfo.dataType.Should().Be(typeof(UnionDataExample));
			}
		}

		partial class FromFactory {
			public sealed class OfUnionDataCase: Base.OfUnionDataCase {
				public OfUnionDataCase (): base(factory) { }
			}
		}

		partial class FromStorage {
			public sealed class OfUnionDataCase: Base.OfUnionDataCase {
				public OfUnionDataCase (): base(factory) { }
			}
		}
		#endregion
	}
}
