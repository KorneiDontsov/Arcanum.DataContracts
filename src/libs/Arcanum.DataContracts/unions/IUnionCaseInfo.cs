// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.DataContracts {
	using System;

	public interface IUnionCaseInfo {
		IDataTypeInfo dataTypeInfo { get; }

		IUnionInfo declaringUnionInfo { get; }

		String name { get; }

		Type dataType { get; }

		IUnionInfo? asUnionInfo { get; }

		Boolean isUnionInfo { get; }

		IUnionInfo rootUnionInfo { get; }

		IUnionCaseInfo? maybeDeclaringCaseInfo { get; }

		/// <inheritdoc cref = "Object.ToString()" />
		String ToString ();
	}
}
