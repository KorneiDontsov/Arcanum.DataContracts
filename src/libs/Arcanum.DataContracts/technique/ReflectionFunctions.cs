// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.DataContracts {
	using System;
	using System.Collections.Generic;
	using System.Linq;

	static class ReflectionFunctions {
		public static IEnumerable<Type> EnumerateClosedNestedTypes (this Type type) {
			static IEnumerable<Type> OfCommonType (Type type) {
				foreach (var nestedType in type.GetNestedTypes())
					if (! nestedType.IsGenericTypeDefinition)
						yield return nestedType;
			}

			static IEnumerable<Type> OfClosedGenericType (Type type) {
				static Boolean ArgsOfGenericTypeDefinitionsAreEqual (Type first, Type second) {
					static IEnumerable<Type[]> SelectArgConstraints (Type type) =>
						type.GetGenericArguments().Select(arg => arg.GetGenericParameterConstraints());

					return SelectArgConstraints(first).SequenceEqual(SelectArgConstraints(second));
				}

				var typeDefinition = type.GetGenericTypeDefinition();

				foreach (var nestedType in type.GetNestedTypes())
					if (! nestedType.IsGenericTypeDefinition)
						yield return nestedType;
					else if (ArgsOfGenericTypeDefinitionsAreEqual(typeDefinition, nestedType))
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
