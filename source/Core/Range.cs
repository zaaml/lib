// <copyright file="Range.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Zaaml.Core
{
	public readonly struct Range<T> where T : IComparable<T>
	{
		public static readonly Range<T> Empty = new (true);

		public Range(T minimum, T maximum)
		{
			if (minimum.CompareTo(maximum) > 0)
				throw new InvalidOperationException();

			Minimum = minimum;
			Maximum = maximum;
		}

		private Range(bool isEmpty)
		{
			Debug.Assert(isEmpty);

			var minMax = IntervalMinMax.Get<T>();

			Minimum = minMax.Maximum;
			Maximum = minMax.Minimum;
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

		public bool IsEmpty => Minimum.CompareTo(Maximum) > 0;

		public override string ToString()
		{
			return $"[{Minimum};{Maximum}]";
		}
	}

	public static partial class Range
	{
		public static Range<T> Intersect<T>(Range<T> range1, Range<T> range2) where T : IComparable<T>
		{
			Interval<T> interval1 = range1;
			Interval<T> interval2 = range2;

			var intersection = Interval.Intersect(interval1, interval2);

			if (intersection.IsEmpty)
				return Range<T>.Empty;

			return new Range<T>(intersection.Minimum, intersection.Maximum);
		}
	}
}