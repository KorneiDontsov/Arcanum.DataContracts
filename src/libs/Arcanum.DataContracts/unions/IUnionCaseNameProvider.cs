﻿// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.DataContracts {
	using System;

	public interface IUnionCaseNameProvider {
		String GetUnionCaseName (Type unionCaseType);
	}
}
