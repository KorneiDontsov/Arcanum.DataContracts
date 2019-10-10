// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.DataContracts {
	using System;

	public static class UnionCaseUtils {
		public static UInt32? TryFindInvalidCharPosition (String caseNameString) {
			for (var i = 0; i < caseNameString.Length; i += 1) {
				var @char = caseNameString[i];
				var charIsValid =
					@char == '_'
					|| @char >= 'A' && @char <= 'Z'
					|| @char >= 'a' && @char <= 'z'
					|| @char >= '0' && @char <= '9';

				if (! charIsValid) return (UInt32) i;
			}

			return null;
		}
	}
}
