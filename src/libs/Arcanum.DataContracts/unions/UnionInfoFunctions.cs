// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.DataContracts {
	using Arcanum.Routes;
	using System;
	using System.Collections.Immutable;

	public static class UnionInfoFunctions {
		/// <exception cref = "FormatException">
		///     <paramref name = "caseRoute" /> doesn't match any case in <paramref name = "unionInfo" />.
		/// </exception>
		public static IUnionCaseInfo GetNestedCaseInfo (this IUnionInfo unionInfo, Route caseRoute) {
			// ReSharper disable once SuggestVarOrType_SimpleTypes
			var caseRouteEnumerator = caseRoute.nodes.GetEnumerator();
			if (! caseRouteEnumerator.MoveNext()) throw new FormatException("Route is empty.");
			else {
				var currentUnionInfo = unionInfo;
				while (true) {
					var caseName = caseRouteEnumerator.Current;
					var nextCaseInfo = currentUnionInfo.caseInfosByNames.GetValueOrDefault(caseName);
					if (nextCaseInfo is null) {
						var msg =
							$"Route '{caseRoute}' doesn't match any case in {unionInfo}: "
							+ $"'{currentUnionInfo}' doesn't contain '{caseName}'.";
						throw new FormatException(msg);
					}
					else if (! caseRouteEnumerator.MoveNext()) return nextCaseInfo;
					else {
						if (nextCaseInfo.asUnionInfo is {} nextUnionInfo)
							currentUnionInfo = nextUnionInfo;
						else {
							var msg =
								$"Route '{caseRoute}' doesn't match any case in {unionInfo}: "
								+ $"'{nextCaseInfo}' is not a union so it cannot have case '{caseName}'.";
							throw new FormatException(msg);
						}
					}
				}
			}
		}
	}
}
