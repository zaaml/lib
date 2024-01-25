// <copyright file="IntSpan.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Diagnostics;

namespace Zaaml.Core
{
	[DebuggerDisplay("{" + nameof(DebuggerDisplay) + "}")]
	internal readonly struct IntSpan
	{
		public static IntSpan Empty => new(true);

		public readonly int Start;
		public readonly int Length;

		public IntSpan(int start)
		{
			Start = start;
			Length = 0;
		}

		public IntSpan(int start, int length)
		{
			Start = start;
			Length = length;
		}

		private IntSpan(bool empty)
		{
			Debug.Assert(empty);

			Start = -1;
			Length = 0;
		}

		public IntSpan Slice(int start, int length)
		{
			if (start < 0 || start + length > Length)
				throw new ArgumentOutOfRangeException();

			return new IntSpan(Start + start, length);
		}

		public IntSpan Slice(int start)
		{
			if (start < 0 || start > Length)
				throw new ArgumentOutOfRangeException();

			return new IntSpan(Start + start, Length - start);
		}

		public bool IsEmpty => Start == -1;

		private void Verify()
		{
			if (Start == -1)
				throw new InvalidOperationException("Empty int span.");
		}

		public int End => Start + Length;

		public bool Contains(IntSpan span)
		{
			if (IsEmpty)
				return false;

			return Start <= span.Start && span.End <= End;
		}

		public bool Contains(int index)
		{
			if (IsEmpty)
				return false;

			return index >= Start && index < End;
		}

		public static bool operator ==(IntSpan first, IntSpan second)
		{
			return first.Equals(second);
		}

		public static bool operator !=(IntSpan first, IntSpan second)
		{
			return first.Equals(second) == false;
		}

		internal Interval<int> Interval => IsEmpty ? Interval<int>.Empty : new Interval<int>(Start, IntervalEndPoint.Closed, End, IntervalEndPoint.Open);

		public bool JoinsWith(IntSpan span)
		{
			var maxStart = Start > span.Start ? Start : span.Start;
			var minEnd = End < span.End ? End : span.End;

			return maxStart == minEnd;
		}

		public IntSpan Bounds(IntSpan span)
		{
			var minStart = Start < span.Start ? Start : span.Start;
			var maxEnd = End > span.End ? End : span.End;

			return FromBounds(minStart, maxEnd);
		}

		public IntSpan Junction(IntSpan span)
		{
			var maxStart = Start > span.Start ? Start : span.Start;
			var minEnd = End < span.End ? End : span.End;

			if (maxStart > minEnd)
			{
				var spanEnd = End < span.End ? End : span.End;
				var end = Start > span.Start ? Start : span.Start;

				return FromBounds(spanEnd, end);
			}

			return Empty;
		}

		public IntSpan Join(IntSpan span)
		{
			var maxStart = Start > span.Start ? Start : span.Start;
			var minEnd = End < span.End ? End : span.End;

			if (maxStart == minEnd)
			{
				var joinStart = Start < span.Start ? Start : span.Start;
				var joinEnd = End > span.End ? End : span.End;

				return FromBounds(joinStart, joinEnd);
			}

			return Empty;
		}

		public static IntSpan FromBounds(int start, int end)
		{
			return new IntSpan(start, end - start);
		}

		public bool IntersectsWith(IntSpan span)
		{
			return span.Start < End && span.End > Start;
		}

		public IntSpan Intersection(IntSpan span)
		{
			var start = Start > span.Start ? Start : span.Start;
			var end = End < span.End ? End : span.End;

			return start <= end ? FromBounds(start, end) : Empty;
		}

		public bool Equals(IntSpan other)
		{
			return Start == other.Start && Length == other.Length;
		}

		public override bool Equals(object obj)
		{
			return obj is IntSpan other && Equals(other);
		}

		private string DebuggerDisplay
		{
			get => IsEmpty ? "(Empty)" : $"[{Start};{End})";
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = Start.GetHashCode();

				hashCode = (hashCode * 397) ^ Length.GetHashCode();

				return hashCode;
			}
		}

		public IntSpan Remove(IntSpan span)
		{
			var intersection = Intersection(span);

			if (intersection.IsEmpty)
				return span.Start < Start ? new IntSpan(Start - span.Length, Length) : this;

			if (this == intersection)
				return Empty;

			var start = Start < span.Start ? Start : span.Start;

			return new IntSpan(start, Length - intersection.Length);
		}

		public IntSpan Insert(IntSpan span)
		{
			if (span.Start < Start)
				return new IntSpan(Start + span.Length, Length);

			if (span.Start >= End)
				return this;

			return new IntSpan(Start, Length + span.Length);
		}

		public override string ToString()
		{
			return DebuggerDisplay;
		}
	}

	internal static class IntSpanExtensions
	{
		public static IntSpan AsIntSpan(this Range<int> range)
		{
			return range.IsEmpty ? IntSpan.Empty : IntSpan.FromBounds(range.Minimum, range.Maximum + 1);
		}

		public static Range<int> AsRange(this IntSpan span)
		{
			return span.IsEmpty ? Range<int>.Empty : new Range<int>(span.Start, span.End - 1);
		}
	}
}