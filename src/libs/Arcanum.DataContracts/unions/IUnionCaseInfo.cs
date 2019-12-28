// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.DataContracts {
	using Arcanum.Routes;
	using System;

	public interface IUnionCaseInfo {
		IDataTypeInfo dataTypeInfo { get; }

		IUnionInfo declaringUnionInfo { get; }

		String name { get; }

		Type dataType { get; }

		IUnionInfo? asDiscriminatedUnionInfo { get; }

		Boolean isDiscriminatedUnionInfo { get; }

		IUnionInfo rootUnionInfo { get; }

		IUnionCaseInfo? maybeDeclaringCaseInfo { get; }

		/// <exception cref="InvalidOperationException"/>
		Route route { get; }

		/// <inheritdoc cref = "Object.ToString()" />
		String ToString ();
	}
}
