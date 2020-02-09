// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.DataContracts {
	using Arcanum.Companions;
	using System;
	using System.Collections.Generic;
	using System.Linq;

	static class ReflectionFunctions {
		public static Boolean HasSameGenSignatureAs (this Type type, Type other) {
			static IEnumerable<Type[]> SelectConstraints (Type type) {
				var t = ! type.IsGenericTypeDefinition ? type.GetGenericTypeDefinition() : type;
				return t.GetGenericArguments().Select(arg => arg.GetGenericParameterConstraints());
			}

			var firstConstraints = SelectConstraints(type);
			var secondConstraints = SelectConstraints(other);
			return firstConstraints.SequenceEqual(secondConstraints, TypeArrayEqualityComparer.shared);
		}

		public static IEnumerable<Type> EnumerateClosedNestedTypes (this Type type) {
			static IEnumerable<Type> OfCommonType (Type type) {
				foreach (var nestedType in type.GetNestedTypes())
					if (! nestedType.IsGenericTypeDefinition)
						yield return nestedType;
			}

			static IEnumerable<Type> OfClosedGenericType (Type type) {
				foreach (var nestedType in type.GetNestedTypes())
					if (! nestedType.IsGenericTypeDefinition)
						yield return nestedType;
					else if (nestedType.HasSameGenSignatureAs(type))
						yield return nestedType.MakeGenericType(type.GenericTypeArguments);
			}

			if (type.IsGenericTypeDefinition)
				throw new Exception($"Open generic type {type} cannot have closed nested types.");
			else if (type.IsGenericType)
				return OfClosedGenericType(type);
			else
				return OfCommonType(type);
		}
	}
}
