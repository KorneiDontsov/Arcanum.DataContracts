// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

using System;

namespace Arcanum.DataContracts
{
	public abstract class DataCaseName : IEquatable<DataCaseName>, IEquatable<String>
	{
		public sealed class Valid : DataCaseName
		{
			/// <param name="nameString">Must contain only latin letters, digits and underscores.</param>
			internal Valid (String nameString) : base(nameString) { }
		}

		public sealed class Invalid : DataCaseName
		{
			public UInt32 invalidCharPosition { get; }

			internal Invalid (String nameString, UInt32 invalidCharPosition) : base(nameString)
			{
				this.invalidCharPosition = invalidCharPosition;
			}
		}

		public String nameString { get; }

		protected DataCaseName (String nameString)
		{
			this.nameString = nameString;
		}

		/// <summary>
		///     Returns <see cref="Valid" /> if <paramref name="nameString" /> contains only latin letters, digits and underscores;
		///     otherwise, returns <see cref="Invalid" />.
		/// </summary>
		public static DataCaseName Construct (String nameString)
		{
			return tryFindInvalidCharPosition(nameString, out var invalidCharPosition)
			? (DataCaseName)new Invalid(nameString, invalidCharPosition)
			: new Valid(nameString);


			static Boolean tryFindInvalidCharPosition (String @string, out UInt32 invalidCharPosition)
			{
				for (var i = 0; i < @string.Length; i++)
				{
					var c = @string[i];

					var charIsValid = c >= '0' && c <= '9' || c >= 'A' && c <= 'Z' || c == '_' || c >= 'a' && c <= 'z';

					if (!charIsValid)
					{
						invalidCharPosition = (UInt32)i;
						return true;
					}
				}

				invalidCharPosition = default;
				return false;
			}
		}

		public static implicit operator String (DataCaseName dataCaseName) => dataCaseName.nameString;

		/// <inheritdoc />
		public override String ToString () => nameString;

		/// <inheritdoc />
		public Boolean Equals (DataCaseName? other) => nameString == other?.nameString;

		public static Boolean operator == (DataCaseName? first, DataCaseName? second)
		{
			return first is null && second is null || first != null && first.Equals(second);
		}

		public static Boolean operator != (DataCaseName? first, DataCaseName? second) => !(first == second);

		/// <inheritdoc />
		public Boolean Equals (String? other) => nameString == other;

		/// <inheritdoc />
		public override Boolean Equals (Object obj)
		{
			return obj switch
			{
			DataCaseName other => Equals(other),
			String other => Equals(other),
			_ => false
			};
		}

		/// <inheritdoc />
		public override Int32 GetHashCode () => nameString?.GetHashCode() ?? 0;
	}
}
