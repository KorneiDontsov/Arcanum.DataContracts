// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.DataContracts {
	using Arcanum.Routes;
	using System;
	using System.Collections.Generic;

	public static class UnionCaseInfoFunctions {
		/// <exception cref = "FormatException"> One of union cases contains '/'. </exception>
		public static Route GetNestedCaseRoute (this IUnionCaseInfo unionCaseInfo) {
			var caseNames = new List<String>();
			var currentCaseInfo = unionCaseInfo;
			while (true) {
				caseNames.Add(currentCaseInfo.name);

				if (currentCaseInfo.maybeDeclaringCaseInfo is {} parentCaseInfo)
					currentCaseInfo = parentCaseInfo;
				else {
					caseNames.Reverse();
					return Route.Join(caseNames);
				}
			}
		}
	}
}
