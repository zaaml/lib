// <copyright file="DelegateComparer.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;

namespace Zaaml.Core
{
	internal static class DelegateComparer
	{
		public static DelegateComparer<T> Create<T>(Func<T, T, int> comparer)
		{
			return new DelegateComparer<T>(comparer);
		}
	}

	internal sealed class DelegateComparer<T> : IComparer<T>
	{
		private readonly Func<T, T, int> _comparer;

		public DelegateComparer(Func<T, T, int> comparer)
		{
			_comparer = comparer;
		}

		public int Compare(T x, T y)
		{
			return _comparer(x, y);
		}
	}
}