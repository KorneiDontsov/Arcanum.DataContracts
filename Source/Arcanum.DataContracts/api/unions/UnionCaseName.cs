// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.DataContracts {
	using System;

	public abstract class UnionCaseName: IEquatable<UnionCaseName?>, IEquatable<String?> {
		public String nameString { get; }

		UnionCaseName (String nameString) => this.nameString = nameString;

		/// <summary>
		///     Returns <see cref = "Valid" /> if <paramref name = "nameString" /> contains only latin letters, digits and
		///     underscores;
		///     otherwise, returns <see cref = "Invalid" />.
		/// </summary>
		public static UnionCaseName Create (String nameString) =>
			UnionCaseUtils.TryFindInvalidCharPosition(nameString)
				switch {
					{ } invalidCharPosition => (UnionCaseName) new Invalid(nameString, invalidCharPosition),
					null => new Valid(nameString)
				};

		public static implicit operator String (UnionCaseName unionCaseName) => unionCaseName.nameString;

		/// <inheritdoc />
		public override String ToString () => nameString;

		/// <inheritdoc />
		public Boolean Equals (UnionCaseName? other) =>
			nameString == other?.nameString;

		public static Boolean operator == (UnionCaseName? first, UnionCaseName? second) =>
			first is null && second is null || first is { } && first.Equals(second);

		public static Boolean operator != (UnionCaseName? first, UnionCaseName? second) =>
			! (first == second);

		/// <inheritdoc />
		public Boolean Equals (String? other) =>
			nameString == other;

		/// <inheritdoc />
		public override Boolean Equals (Object? obj) =>
			obj switch {
				UnionCaseName other => Equals(other),
				String other => Equals(other),
				_ => false
			};

		/// <inheritdoc />
		public override Int32 GetHashCode () => nameString?.GetHashCode() ?? 0;

		#region cases
		sealed class Valid: UnionCaseName {
			/// <param name = "nameString"> Must contain only latin letters, digits and underscores. </param>
			internal Valid (String nameString): base(nameString) { }
		}

		public sealed class Invalid: UnionCaseName {
			public UInt32 invalidCharPosition { get; }

			internal Invalid (String nameString, UInt32 invalidCharPosition): base(nameString) =>
				this.invalidCharPosition = invalidCharPosition;
		}
		#endregion
	}
}
