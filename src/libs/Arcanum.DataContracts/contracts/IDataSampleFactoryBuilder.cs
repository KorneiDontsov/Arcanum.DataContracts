// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.DataContracts {
	using System;

	public interface IDataSampleFactoryBuilder {
		IDataSampleFactoryBuilder Self<TSample> (Func<TSample> createSample) where TSample: class;

		IDataSampleFactoryBuilder Item<TSample> (Func<TSample> createSample) where TSample: class;

		IDataSampleFactoryBuilder Member<TSample> (String name, Func<TSample> createSample) where TSample: class;
	}
}
