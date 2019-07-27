// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Linq;

using Arcanum.DataContracts;

using FluentAssertions;
using FluentAssertions.Execution;

using JetBrains.Annotations;

using Xunit;

namespace Tests.Arcanum.DataContracts
{
	public abstract class TestDataTypeInfo
	{
		#region OfAnyData
		public abstract partial class Base
		{
			public abstract class OfAnyData
			{
				private Type _dataType { get; }

				protected DataTypeInfo dataTypeInfo { get; }

				protected OfAnyData (Type dataType, Func<Type, DataTypeInfo> constructor)
				{
					_dataType = dataType;
					dataTypeInfo = constructor(dataType);
				}

				[Fact]
				public void IsNotNull ()
				{
					_ = dataTypeInfo.Should().NotBeNull();
				}

				[Fact]
				public void HasDataType ()
				{
					_ = dataTypeInfo.dataType.Should().Be(_dataType);
				}
			}
		}

		public abstract partial class Constructed
		{
			private static Func<Type, DataTypeInfo> _constructor { get; } = DataTypeInfo.Construct;
		}

		public abstract partial class FromSharedStorage
		{
			private static Func<Type, DataTypeInfo> _constructor { get; } = DataTypeInfoStorage.shared.Get;
		}
		#endregion

		#region OfSimpleData
		[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
		private sealed class SimpleDataExample
		{
			public Boolean isTrue { get; set; }

			public UInt32 someInteger { get; set; }

			public String? someText { get; set; }
		}

		partial class Base
		{
			public abstract class OfSimpleData : OfAnyData
			{
				protected OfSimpleData (Func<Type, DataTypeInfo> constructor)
				: base(typeof(SimpleDataExample), constructor) { }

				[Fact]
				public void DoesntHaveDiscriminatedUnionInfo ()
				{
					_ = dataTypeInfo.asDiscriminatedUnionInfo.Should().BeNull();
				}

				[Fact]
				public void DoesntHaveDiscriminatedUnionCaseInfo ()
				{
					_ = dataTypeInfo.asDiscriminatedUnionCaseInfo.Should().BeNull();
				}
			}
		}

		partial class Constructed
		{
			public sealed class OfSimpleData : Base.OfSimpleData
			{
				public OfSimpleData () : base(_constructor) { }
			}
		}

		partial class FromSharedStorage
		{
			public sealed class OfSimpleData : Base.OfSimpleData
			{
				public OfSimpleData () : base(_constructor) { }
			}
		}
		#endregion

		#region OfDiscriminatedUnionData
		private abstract class DiscriminatedUnionDataExample
		{
			[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
			[DataCase("Case1Name")]
			public sealed class Case1 : DiscriminatedUnionDataExample
			{
				public Byte[]? salt { get; set; }
			}

			[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
			public abstract class Case2Base : DiscriminatedUnionDataExample
			{
				public SByte[]? extraSalt { get; set; }
			}

			[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
			public sealed class Case2 : Case2Base
			{
				public String? saltText { get; set; }
			}
		}

		partial class Base
		{
			public abstract class OfDiscriminatedUnionData : OfAnyData
			{
				protected OfDiscriminatedUnionData (Func<Type, DataTypeInfo> constructor)
				: base(typeof(DiscriminatedUnionDataExample), constructor) { }

				[Fact]
				public void DoesntHaveDiscriminatedUnionCaseInfo ()
				{
					_ = dataTypeInfo.asDiscriminatedUnionCaseInfo.Should().BeNull();
				}

				[Fact]
				public void HasDiscriminatedUnionInfo ()
				{
					_ = dataTypeInfo.asDiscriminatedUnionInfo.Should().NotBeNull();
				}

				[Fact]
				public void HasDiscriminatedUnionInfoThatHasDataType ()
				{
					_ = dataTypeInfo.asDiscriminatedUnionInfo!.dataType.Should()
					.Be(typeof(DiscriminatedUnionDataExample));
				}

				[Fact]
				public void HasDiscriminatedUnionInfoThatHasDataTypeInfo ()
				{
					_ = dataTypeInfo.asDiscriminatedUnionInfo!.dataTypeInfo.Should().BeSameAs(dataTypeInfo);
				}

				[Fact]
				public void HasDiscriminatedUnionInfoThatHasCases ()
				{
					var discriminatedUnionInfo = dataTypeInfo.asDiscriminatedUnionInfo!;


					using (new AssertionScope())
					{
						_ = discriminatedUnionInfo.caseInfos
						.Select(i => (type: i.dataType, i.name, i.declaringUnionInfo))
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
							)
						);

						_ = discriminatedUnionInfo.caseInfos.Should()
						.OnlyContain(caseInfo => caseInfo.dataTypeInfo.dataType == caseInfo.dataType)
						.And.OnlyContain(caseInfo => caseInfo.dataTypeInfo.asDiscriminatedUnionInfo == null)
						.And.OnlyContain(
							caseInfo => ReferenceEquals(caseInfo.dataTypeInfo.asDiscriminatedUnionCaseInfo, caseInfo)
						);

						_ = discriminatedUnionInfo.caseInfosByTypes.Should()
						.OnlyContain(i => i.Key == i.Value.dataType);

						_ = discriminatedUnionInfo.caseInfosByTypes.Values.Should()
						.BeEquivalentTo(discriminatedUnionInfo.caseInfos);

						_ = discriminatedUnionInfo.caseInfosByNames.Should()
						.OnlyContain(i => i.Key == i.Value.name);

						_ = discriminatedUnionInfo.caseInfosByNames.Values.Should()
						.BeEquivalentTo(discriminatedUnionInfo.caseInfos);
					}
				}
			}
		}

		partial class Constructed
		{
			public sealed class OfDiscriminatedUnionData : Base.OfDiscriminatedUnionData
			{
				public OfDiscriminatedUnionData () : base(_constructor) { }
			}
		}

		partial class FromSharedStorage
		{
			public sealed class OfDiscriminatedUnionData : Base.OfDiscriminatedUnionData
			{
				public OfDiscriminatedUnionData () : base(_constructor) { }
			}
		}
		#endregion

		#region OfDiscriminatedUnionDataCase
		partial class Base
		{
			public abstract class OfDiscriminatedUnionDataCase : OfAnyData
			{
				protected OfDiscriminatedUnionDataCase (Func<Type, DataTypeInfo> constructor)
				: base(typeof(DiscriminatedUnionDataExample.Case1), constructor) { }

				[Fact]
				public void DoesntHaveDiscriminatedUnionInfo ()
				{
					_ = dataTypeInfo.asDiscriminatedUnionInfo.Should().BeNull();
				}

				[Fact]
				public void HasDiscriminatedUnionCaseInfo ()
				{
					_ = dataTypeInfo.asDiscriminatedUnionCaseInfo.Should().NotBeNull();
				}

				[Fact]
				public void HasDiscriminatedUnionCaseInfoThatHasDataTypeInfo ()
				{
					_ = dataTypeInfo.asDiscriminatedUnionCaseInfo!.dataTypeInfo.Should().BeSameAs(dataTypeInfo);
				}

				[Fact]
				public void HasDiscriminatedUnionCaseInfoThatHasDataType ()
				{
					_ = dataTypeInfo.asDiscriminatedUnionCaseInfo!.dataType.Should()
					.Be(typeof(DiscriminatedUnionDataExample.Case1));
				}

				[Fact]
				public void HasDiscriminatedUnionCaseInfoThatHasName ()
				{
					_ = dataTypeInfo.asDiscriminatedUnionCaseInfo!.name.Should().Be("Case1Name");
				}

				[Fact]
				public void HasDiscriminatedUnionCaseInfoThatHasDeclaringUnionInfo ()
				{
					_ = dataTypeInfo.asDiscriminatedUnionCaseInfo!.declaringUnionInfo.dataType.Should()
					.Be(typeof(DiscriminatedUnionDataExample));
				}
			}
		}

		partial class Constructed
		{
			public sealed class OfDiscriminatedUnionDataCase : Base.OfDiscriminatedUnionDataCase
			{
				public OfDiscriminatedUnionDataCase () : base(_constructor) { }
			}
		}

		partial class FromSharedStorage
		{
			public sealed class OfDiscriminatedUnionDataCase : Base.OfDiscriminatedUnionDataCase
			{
				public OfDiscriminatedUnionDataCase () : base(_constructor) { }
			}
		}
		#endregion
	}
}
