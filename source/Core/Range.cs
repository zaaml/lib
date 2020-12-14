// <copyright file="Range.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.Core
{
	public readonly struct Range<T> where T : IComparable<T>
	{
		public Range(T minimum, T maximum)
		{
			Minimum = minimum;
			Maximum = maximum;
		}

		public readonly T Minimum;

		public readonly T Maximum;

		public bool Contains(T value)
		{
			Interval<T> interval = this;

			return interval.Contains(value);
		}

		public bool Contains(Range<T> range)
		{
			Interval<T> interval = this;

			return interval.Contains(range);
		}

		public T Clamp(T value)
		{
			Interval<T> interval = this;

			return interval.Clamp(value);
		}
	}
}