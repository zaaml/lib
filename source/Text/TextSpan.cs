// <copyright file="TextSpan.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using Zaaml.Core;

namespace Zaaml.Text
{
	public readonly struct TextSpan
	{
		public static readonly TextSpan Empty = new(TextPoint.Empty, 0);

		public TextSpan(TextPoint start, int length)
		{
			Start = start;
			Length = length;
		}

		public TextSpan(TextPoint start, TextPoint end)
		{
			if (start.Index < end.Index)
			{
				Start = start;
				Length = end.Index - start.Index;
			}
			else
			{
				Start = end;
				Length = start.Index - end.Index;
			}
		}

		public bool IsEmpty => Start.IsEmpty;

		public TextPoint Start { get; }

		public TextPoint End => Start + Length;

		public int Length { get; }

		public bool Contains(TextSpan span)
		{
			if (IsEmpty)
				return false;

			return Start <= span.Start && span.End <= End;
		}

		public bool Contains(TextPoint textPoint)
		{
			if (IsEmpty)
				return false;

			return textPoint >= Start && textPoint < End;
		}

		public static bool operator ==(TextSpan first, TextSpan second)
		{
			return first.Start == second.Start && first.Length == second.Length;
		}

		public static bool operator !=(TextSpan first, TextSpan second)
		{
			return first.Start != second.Start || first.Length != second.Length;
		}

		public override string ToString()
		{
			return IsEmpty ? "Empty" : $"[{Start}-{End})";
		}

		internal Interval<int> Interval => IsEmpty ? Interval<int>.Empty : new Interval<int>(Start, IntervalEndPoint.Closed, End, IntervalEndPoint.Open);

		internal TextSpan IntersectWith(TextSpan textSpan)
		{
			var interval = Interval;
			var textInterval = textSpan.Interval;
			var intersection = Core.Interval.Intersect(interval, textInterval).Normalize(IntervalEndPoint.Closed, IntervalEndPoint.Open);

			return intersection.IsEmpty ? Empty : new TextSpan(new TextPoint(intersection.Minimum), intersection.Maximum - intersection.Minimum);
		}

		public bool Equals(TextSpan other)
		{
			return Start.Equals(other.Start) && Length.Equals(other.Length);
		}

		public override bool Equals(object obj)
		{
			return obj is TextSpan other && Equals(other);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(Start, Length);
		}
	}
}