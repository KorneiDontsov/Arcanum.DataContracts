// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.DataContracts {
	using System;

	public abstract class UnionCaseError {
		public IUnionCaseInfo caseInfo { get; }

		UnionCaseError (IUnionCaseInfo caseInfo) => this.caseInfo = caseInfo;

		/// <inheritdoc />
		public override abstract String ToString ();

		#region cases
		public sealed class HasDuplicateName: UnionCaseError {
			public IUnionCaseInfo sameNameCaseInfo { get; }

			/// <inheritdoc />
			public HasDuplicateName (IUnionCaseInfo caseInfo, IUnionCaseInfo sameNameCaseInfo): base(caseInfo) =>
				this.sameNameCaseInfo = sameNameCaseInfo;

			/// <inheritdoc />
			public override String ToString () =>
				$"Name '{caseInfo.name}' of discriminated union case {caseInfo} duplicates name of neighboring case " +
				$"{sameNameCaseInfo}.";
		}
		#endregion
	}
}
