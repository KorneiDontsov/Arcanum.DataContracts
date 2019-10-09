﻿// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.DataContracts {
	using System;

	public abstract class DataCaseName: IEquatable<DataCaseName?>, IEquatable<String?> {
		public String nameString { get; }

		DataCaseName (String nameString) => this.nameString = nameString;

		/// <summary>
		///     Returns <see cref = "Valid" /> if <paramref name = "nameString" /> contains only latin letters, digits and
		///     underscores;
		///     otherwise, returns <see cref = "Invalid" />.
		/// </summary>
		public static DataCaseName Construct (String nameString) {
			static Boolean HasInvalidCharPosition (String @string, out UInt32 invalidCharPosition) {
				for (var i = 0; i < @string.Length; i += 1) {
					var @char = @string[i];
					var charIsValid =
						@char == '_'
						|| @char >= 'A' && @char <= 'Z'
						|| @char >= 'a' && @char <= 'z'
						|| @char >= '0' && @char <= '9';

					if (! charIsValid) {
						invalidCharPosition = (UInt32) i;
						return true;
					}
				}

				invalidCharPosition = default;
				return false;
			}

			return HasInvalidCharPosition(nameString, out var invalidCharPosition)
				? (DataCaseName) new Invalid(nameString, invalidCharPosition)
				: new Valid(nameString);
		}

		public static implicit operator String (DataCaseName dataCaseName) => dataCaseName.nameString;

		/// <inheritdoc />
		public override String ToString () => nameString;

		/// <inheritdoc />
		public Boolean Equals (DataCaseName? other) =>
			nameString == other?.nameString;

		public static Boolean operator == (DataCaseName? first, DataCaseName? second) =>
			first is null && second is null || first is { } && first.Equals(second);

		public static Boolean operator != (DataCaseName? first, DataCaseName? second) =>
			! (first == second);

		/// <inheritdoc />
		public Boolean Equals (String? other) =>
			nameString == other;

		/// <inheritdoc />
		public override Boolean Equals (Object? obj) =>
			obj switch {
				DataCaseName other => Equals(other),
				String other => Equals(other),
				_ => false
			};

		/// <inheritdoc />
		public override Int32 GetHashCode () => nameString?.GetHashCode() ?? 0;

		#region cases
		sealed class Valid: DataCaseName {
			/// <param name = "nameString"> Must contain only latin letters, digits and underscores. </param>
			internal Valid (String nameString): base(nameString) { }
		}

		public sealed class Invalid: DataCaseName {
			public UInt32 invalidCharPosition { get; }

			internal Invalid (String nameString, UInt32 invalidCharPosition): base(nameString) =>
				this.invalidCharPosition = invalidCharPosition;
		}
		#endregion
	}
}
