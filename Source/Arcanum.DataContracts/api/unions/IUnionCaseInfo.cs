// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.DataContracts {
	using System;

	public interface IUnionCaseInfo {
		IDataTypeInfo dataTypeInfo { get; }
		
		IUnionInfo declaringUnionInfo { get; }
		
		DataCaseName name { get; }
		
		Type dataType { get; }
		
		IUnionInfo? asDiscriminatedUnionInfo { get; }
		
		Boolean isDiscriminatedUnionInfo { get; }
		
		IUnionInfo rootUnionInfo { get; }
		
		IUnionCaseInfo? maybeDeclaringCaseInfo { get; }
	}
}
