// <copyright file="ReferenceCounter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Core
{
#if DEBUG
	internal struct ReferenceCounter
	{
		private int _referenceCount;

		public int AddReference()
		{
			return ++_referenceCount;
		}

		public int ReleaseReference()
		{
			if (_referenceCount == 0)
				throw new InvalidOperationException();

			return --_referenceCount;
		}

		public int ReferenceCount => _referenceCount;

		public override string ToString()
		{
			return _referenceCount.ToString();
		}
	}
#else
	internal struct ReferenceCounter
	{
		public int ReferenceCount;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int AddReference()
		{
			return ++ReferenceCount;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int ReleaseReference()
		{
			return --ReferenceCount;
		}
	}
#endif
}