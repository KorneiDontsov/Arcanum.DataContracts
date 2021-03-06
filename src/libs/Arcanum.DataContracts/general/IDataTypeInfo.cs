﻿// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.DataContracts {
	using System;

	public interface IDataTypeInfo {
		Type dataType { get; }

		DataContract contract { get; }

		IUnionInfo? asUnionInfo { get; }

		Boolean isUnionInfo { get; }

		IUnionCaseInfo? asUnionCaseInfo { get; }

		Boolean isUnionCaseInfo { get; }

		/// <inheritdoc cref = "Object.ToString()" />
		String ToString ();
	}
}
