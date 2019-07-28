// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

using System;

namespace Arcanum.DataContracts
{
	public abstract class DiscriminatedUnionCaseErrorInfo
	{
		public sealed class HasInvalidName : DiscriminatedUnionCaseErrorInfo
		{
			public DataCaseName.Invalid invalidCaseName { get; }

			/// <inheritdoc />
			public HasInvalidName (DiscriminatedUnionCaseInfo caseInfo)
			: base(caseInfo)
			{
				invalidCaseName = caseInfo.name as DataCaseName.Invalid
				?? throw new Exception(
					$"'{nameof(caseInfo)}' must be with invalid name, but accepted {caseInfo} with valid name {caseInfo.name}."
				);
			}

			/// <inheritdoc />
			public override String ToString ()
			{
				return
				$"Name '{invalidCaseName}' of discriminated union case {caseInfo} has invalid character at {invalidCaseName.invalidCharPosition}.";
			}
		}

		public sealed class HasDuplicateName : DiscriminatedUnionCaseErrorInfo
		{
			public DiscriminatedUnionCaseInfo sameNameCaseInfo { get; }

			/// <inheritdoc />
			public HasDuplicateName (
				DiscriminatedUnionCaseInfo caseInfo,
				DiscriminatedUnionCaseInfo sameNameCaseInfo
			) : base(caseInfo)
			{
				this.sameNameCaseInfo = sameNameCaseInfo;
			}

			/// <inheritdoc />
			public override String ToString ()
			{
				return
				$"Name '{caseInfo.name}' of discriminated union case {caseInfo} duplicates name of neighboring case {sameNameCaseInfo}.";
			}
		}

		public DiscriminatedUnionCaseInfo caseInfo { get; }

		private DiscriminatedUnionCaseErrorInfo (DiscriminatedUnionCaseInfo caseInfo)
		{
			this.caseInfo = caseInfo;
		}

		/// <inheritdoc />
		public abstract override String ToString ();
	}
}
