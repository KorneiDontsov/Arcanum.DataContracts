// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.DataContracts {
	using System;
	using System.Reflection;

	static class DataContractModule {
		public static DataContract CreateDataContract (Type dataType) {
			var mayPrepareSamples =
				dataType.GetMethod(
						"PrepareSamples",
						BindingFlags.Static | BindingFlags.Public,
						null,
						new[] { typeof(IDataSampleFactoryBuilder) }
						, null)
					?.CreateDelegate(typeof(PrepareSamples)) as PrepareSamples;
			return new DataContract(mayPrepareSamples);
		}
	}
}
