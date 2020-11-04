// <copyright file="ReferenceCounter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

#if DEBUG
using System;
#endif

namespace Zaaml.Core
{
	internal struct ReferenceCounter
	{
		private int _referenceCount;

		public int AddReference()
		{
			_referenceCount++;

			return _referenceCount;
		}

		public int ReleaseReference()
		{
#if DEBUG
			if (_referenceCount == 0)
				throw new InvalidOperationException();
#endif

			_referenceCount--;

			return _referenceCount;
		}

		public override string ToString()
		{
			return _referenceCount.ToString();
		}
	}
}