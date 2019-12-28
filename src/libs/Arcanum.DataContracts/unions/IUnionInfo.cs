// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.DataContracts {
	using System;
	using System.Collections.Immutable;

	public interface IUnionInfo {
		IDataTypeInfo dataTypeInfo { get; }

		IImmutableList<IUnionCaseInfo> caseInfos { get; }

		IImmutableDictionary<Type, IUnionCaseInfo> caseInfosByTypes { get; }

		IImmutableDictionary<String, IUnionCaseInfo> caseInfosByNames { get; }

		IImmutableList<UnionCaseError> invalidCaseErrors { get; }

		Type dataType { get; }

		IUnionCaseInfo? asUnionCaseInfo { get; }

		Boolean isUnionCaseInfo { get; }

		IUnionInfo rootUnionInfo { get; }

		Boolean hasErrors { get; }

		/// <inheritdoc cref = "Object.ToString()" />
		String ToString ();

		String GetErrorString ();
	}
}
