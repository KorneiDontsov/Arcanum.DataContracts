// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.Companions {
	using System;
	using System.Collections.Generic;

	class TypeArrayEqualityComparer: IEqualityComparer<Type[]> {
		public static TypeArrayEqualityComparer shared { get; } = new TypeArrayEqualityComparer();

		/// <inheritdoc />
		public Boolean Equals (Type[]? x, Type[]? y) {
			if (x is null) return y is null;
			if (y is null || x.Length != y.Length)
				return false;
			else {
				for (UInt32 i = 0; i < x.Length; ++ i)
					if (x[i] != y[i])
						return false;
				return true;
			}
		}

		/// <inheritdoc />
		public Int32 GetHashCode (Type[]? obj) => throw new NotSupportedException();
	}
}
