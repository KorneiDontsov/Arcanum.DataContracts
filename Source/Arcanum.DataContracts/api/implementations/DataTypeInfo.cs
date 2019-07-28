// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
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
						return
						from caseType in caseTypes
						let dataTypeInfo = new DataTypeInfo(caseType, declaringUnionInfo: discriminatedUnionInfo)
						where caseType.IsAbstract is false || dataTypeInfo.asDiscriminatedUnionInfo != null
						select dataTypeInfo.asDiscriminatedUnionCaseInfo!;
					}
				);

				// if all cases is abstract and not discriminated unions.
				if (asDiscriminatedUnionInfo.caseInfos.Count is 0) asDiscriminatedUnionInfo = null;
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

		/// <inheritdoc />
		public override String ToString () => dataType.ToString();
	}
}
