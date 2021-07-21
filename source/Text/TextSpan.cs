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
			StartIndex = start.Index;
			Length = length;
		}

		public TextSpan(int start, int length)
		{
			StartIndex = start;
			Length = length;
		}

		public TextSpan(TextPoint start, TextPoint end)
		{
			if (start.Index < end.Index)
			{
				StartIndex = start;
				Length = end.Index - start.Index;
			}
			else
			{
				StartIndex = end;
				Length = start.Index - end.Index;
			}
		}

		private int StartIndex { get; }

		private int EndIndex => StartIndex + Length;

		public bool IsEmpty => StartIndex < 0;

		public TextPoint Start => new TextPoint(StartIndex);

		public TextPoint End => new TextPoint(EndIndex);

		public int Length { get; }

		public bool Contains(TextSpan span)
		{
			if (IsEmpty)
				return false;

			return StartIndex <= span.StartIndex && span.EndIndex <= EndIndex;
		}

		public bool Contains(TextPoint textPoint)
		{
			if (IsEmpty)
				return false;

			var pointIndex = textPoint.Index;

			return pointIndex >= StartIndex && pointIndex < EndIndex;
		}

		public static bool operator ==(TextSpan first, TextSpan second)
		{
			return first.StartIndex == second.StartIndex && first.Length == second.Length;
		}

		public static bool operator !=(TextSpan first, TextSpan second)
		{
			return first.StartIndex != second.StartIndex || first.Length != second.Length;
		}

		public override string ToString()
		{
			return IsEmpty ? "Empty" : $"[{StartIndex}-{EndIndex})";
		}

		internal Interval<int> Interval => IsEmpty ? Interval<int>.Empty : new Interval<int>(StartIndex, IntervalEndPoint.Closed, EndIndex, IntervalEndPoint.Open);

		internal TextSpan IntersectWith(TextSpan textSpan)
		{
			var interval = Interval;
			var textInterval = textSpan.Interval;
			var intersection = Core.Interval.Intersect(interval, textInterval).Normalize(IntervalEndPoint.Closed, IntervalEndPoint.Open);

			return intersection.IsEmpty ? Empty : new TextSpan(new TextPoint(intersection.Minimum), intersection.Maximum - intersection.Minimum);
		}

		public bool Equals(TextSpan other)
		{
			return StartIndex == other.StartIndex && Length == other.Length;
		}

		public override bool Equals(object obj)
		{
			return obj is TextSpan other && Equals(other);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(StartIndex, Length);
		}
	}
}