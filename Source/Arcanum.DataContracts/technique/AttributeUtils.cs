// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.DataContracts {
	using System.Reflection;

	static class AttributeUtils {
		public static TAttrAbst? MatchCustomAttribute<TAttrAbst> (this MemberInfo type)
		where TAttrAbst: class {
			foreach (var attribute in type.GetCustomAttributes())
				if (attribute is TAttrAbst implOfAbst)
					return implOfAbst;

			return null;
		}
	}
}
