// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.DataContracts {
	using System.Reflection;

	static class AttributeFunctions {
		public static TAttr? MatchCustomAttribute<TAttr> (this MemberInfo type)
		where TAttr: class {
			foreach (var attribute in type.GetCustomAttributes())
				if (attribute is TAttr matched)
					return matched;

			return null;
		}
	}
}
