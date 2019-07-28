// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Arcanum.DataContracts
{
	public sealed class DataTypeInfo
	{
		public Type dataType { get; }

		public DiscriminatedUnionInfo? asDiscriminatedUnionInfo { get; }

		public DiscriminatedUnionCaseInfo? asDiscriminatedUnionCaseInfo { get; }

		private DataTypeInfo (
			Type dataType,
			DiscriminatedUnionInfo? declaringUnionInfo = null
		)
		{
			this.dataType = dataType;

			if (declaringUnionInfo != null)
			{
				asDiscriminatedUnionCaseInfo = new DiscriminatedUnionCaseInfo(this, declaringUnionInfo);
			}

			if (dataType.IsAbstract
				&& getCaseTypes() is var caseTypes
				&& caseTypes != null)
			{
				asDiscriminatedUnionInfo = new DiscriminatedUnionInfo(
					dataTypeInfo: this,
					caseInfoListConstructor: discriminatedUnionInfo =>
					{
						return caseTypes
						.Select(
							caseType =>
							{
								return new DataTypeInfo(caseType, declaringUnionInfo: discriminatedUnionInfo)
								.asDiscriminatedUnionCaseInfo!;
							}
						)
						.ToImmutableList();
					}
				);
			}
			else
			{
				asDiscriminatedUnionInfo = null;
			}

			List<Type>? getCaseTypes ()
			{
				List<Type>? list = null;

				var nestedTypes = dataType.GetNestedTypes();
				foreach (var nestedType in nestedTypes)
				{
					if (nestedType.IsSubclassOf(dataType))
					{
						list ??= new List<Type>(capacity: 4);
						list.Add(nestedType);
					}
				}

				return list;
			}
		}

		internal static DataTypeInfo construct (Type dataType, IDataTypeInfoStorage? maybeStorage)
		{
			if (dataType.IsClass is false && dataType.IsValueType is false)
			{
				throw new Exception(
					$"{nameof(dataType)} must be class or structure. Accepted {dataType.AssemblyQualifiedName}."
				);
			}

			if (dataType.IsNested)
			{
				var declaringType = dataType.DeclaringType;
				var declaringDataTypeInfo = maybeStorage?.Get(declaringType) ?? construct(declaringType, null);
				if (declaringDataTypeInfo.asDiscriminatedUnionInfo != null
					&& declaringDataTypeInfo.asDiscriminatedUnionInfo.caseInfosByTypes.TryGetValue(
						dataType,
						out var caseInfo
					)
				)
				{
					return caseInfo.dataTypeInfo;
				}
			}

			return new DataTypeInfo(dataType);
		}

		public static DataTypeInfo Construct (Type dataType) => construct(dataType, maybeStorage: null);
	}
}
