// <copyright file="Range.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Zaaml.Core.Text;

namespace Zaaml.Core
{
	internal enum IntervalEndPoint
	{
		Open = 0,
		Closed = 1,
		Unbounded = 2
	}

	internal static class IntervalEndPointFlag
	{
		#region Static Fields and Constants

		private const int MaximumPointOpenShift = 1;
		private const int MinimumPointOpenShift = 3;
		public static readonly IntervalEndPoint[] MinPointFlagArray = new IntervalEndPoint[32];
		public static readonly IntervalEndPoint[] MaxPointFlagArray = new IntervalEndPoint[32];

		#endregion

		#region Ctors

		static IntervalEndPointFlag()
		{
			MinPointFlagArray[CalcFlag(IntervalEndPoint.Closed, IntervalEndPoint.Closed, false)] = IntervalEndPoint.Closed;
			MinPointFlagArray[CalcFlag(IntervalEndPoint.Closed, IntervalEndPoint.Open, false)] = IntervalEndPoint.Closed;
			MinPointFlagArray[CalcFlag(IntervalEndPoint.Closed, IntervalEndPoint.Unbounded, false)] = IntervalEndPoint.Closed;
			MinPointFlagArray[CalcFlag(IntervalEndPoint.Open, IntervalEndPoint.Closed, false)] = IntervalEndPoint.Open;
			MinPointFlagArray[CalcFlag(IntervalEndPoint.Open, IntervalEndPoint.Open, false)] = IntervalEndPoint.Open;
			MinPointFlagArray[CalcFlag(IntervalEndPoint.Open, IntervalEndPoint.Unbounded, false)] = IntervalEndPoint.Open;
			MinPointFlagArray[CalcFlag(IntervalEndPoint.Unbounded, IntervalEndPoint.Closed, false)] = IntervalEndPoint.Unbounded;
			MinPointFlagArray[CalcFlag(IntervalEndPoint.Unbounded, IntervalEndPoint.Open, false)] = IntervalEndPoint.Unbounded;
			MinPointFlagArray[CalcFlag(IntervalEndPoint.Unbounded, IntervalEndPoint.Unbounded, false)] = IntervalEndPoint.Unbounded;
			MinPointFlagArray[CalcFlag(IntervalEndPoint.Closed, IntervalEndPoint.Closed, true)] = IntervalEndPoint.Closed;
			MinPointFlagArray[CalcFlag(IntervalEndPoint.Closed, IntervalEndPoint.Open, true)] = IntervalEndPoint.Closed;
			MinPointFlagArray[CalcFlag(IntervalEndPoint.Closed, IntervalEndPoint.Unbounded, true)] = IntervalEndPoint.Closed;
			MinPointFlagArray[CalcFlag(IntervalEndPoint.Open, IntervalEndPoint.Closed, true)] = IntervalEndPoint.Open;
			MinPointFlagArray[CalcFlag(IntervalEndPoint.Open, IntervalEndPoint.Open, true)] = IntervalEndPoint.Open;
			MinPointFlagArray[CalcFlag(IntervalEndPoint.Open, IntervalEndPoint.Unbounded, true)] = IntervalEndPoint.Open;
			MinPointFlagArray[CalcFlag(IntervalEndPoint.Unbounded, IntervalEndPoint.Closed, true)] = IntervalEndPoint.Unbounded;
			MinPointFlagArray[CalcFlag(IntervalEndPoint.Unbounded, IntervalEndPoint.Open, true)] = IntervalEndPoint.Unbounded;
			MinPointFlagArray[CalcFlag(IntervalEndPoint.Unbounded, IntervalEndPoint.Unbounded, true)] = IntervalEndPoint.Unbounded;

			MaxPointFlagArray[CalcFlag(IntervalEndPoint.Closed, IntervalEndPoint.Closed, false)] = IntervalEndPoint.Closed;
			MaxPointFlagArray[CalcFlag(IntervalEndPoint.Closed, IntervalEndPoint.Open, false)] = IntervalEndPoint.Open;
			MaxPointFlagArray[CalcFlag(IntervalEndPoint.Closed, IntervalEndPoint.Unbounded, false)] = IntervalEndPoint.Unbounded;
			MaxPointFlagArray[CalcFlag(IntervalEndPoint.Open, IntervalEndPoint.Closed, false)] = IntervalEndPoint.Closed;
			MaxPointFlagArray[CalcFlag(IntervalEndPoint.Open, IntervalEndPoint.Open, false)] = IntervalEndPoint.Open;
			MaxPointFlagArray[CalcFlag(IntervalEndPoint.Open, IntervalEndPoint.Unbounded, false)] = IntervalEndPoint.Unbounded;
			MaxPointFlagArray[CalcFlag(IntervalEndPoint.Unbounded, IntervalEndPoint.Closed, false)] = IntervalEndPoint.Closed;
			MaxPointFlagArray[CalcFlag(IntervalEndPoint.Unbounded, IntervalEndPoint.Open, false)] = IntervalEndPoint.Open;
			MaxPointFlagArray[CalcFlag(IntervalEndPoint.Unbounded, IntervalEndPoint.Unbounded, false)] = IntervalEndPoint.Unbounded;
			MaxPointFlagArray[CalcFlag(IntervalEndPoint.Closed, IntervalEndPoint.Closed, true)] = IntervalEndPoint.Closed;
			MaxPointFlagArray[CalcFlag(IntervalEndPoint.Closed, IntervalEndPoint.Open, true)] = IntervalEndPoint.Open;
			MaxPointFlagArray[CalcFlag(IntervalEndPoint.Closed, IntervalEndPoint.Unbounded, true)] = IntervalEndPoint.Unbounded;
			MaxPointFlagArray[CalcFlag(IntervalEndPoint.Open, IntervalEndPoint.Closed, true)] = IntervalEndPoint.Closed;
			MaxPointFlagArray[CalcFlag(IntervalEndPoint.Open, IntervalEndPoint.Open, true)] = IntervalEndPoint.Open;
			MaxPointFlagArray[CalcFlag(IntervalEndPoint.Open, IntervalEndPoint.Unbounded, true)] = IntervalEndPoint.Unbounded;
			MaxPointFlagArray[CalcFlag(IntervalEndPoint.Unbounded, IntervalEndPoint.Closed, true)] = IntervalEndPoint.Closed;
			MaxPointFlagArray[CalcFlag(IntervalEndPoint.Unbounded, IntervalEndPoint.Open, true)] = IntervalEndPoint.Open;
			MaxPointFlagArray[CalcFlag(IntervalEndPoint.Unbounded, IntervalEndPoint.Unbounded, true)] = IntervalEndPoint.Unbounded;
		}

		#endregion

		#region  Methods

		public static byte CalcFlag(IntervalEndPoint minimumPoint, IntervalEndPoint maximumPoint, bool isEmpty)
		{
			var minimumPointVal = (int) minimumPoint << MinimumPointOpenShift;
			var maximumPointVal = (int) maximumPoint << MaximumPointOpenShift;

			var isEmptyVal = isEmpty ? 1 : 0;

			return (byte) (minimumPointVal | maximumPointVal | isEmptyVal);
		}

		#endregion
	}

	internal readonly struct Interval<T> : IComparable<Interval<T>> where T : IComparable<T>
	{
		#region Ctors

		public Interval(T minimum, T maximum)
		{
			Minimum = minimum;
			Maximum = maximum;

			_flags = CalculateFlags(Minimum, IntervalEndPoint.Closed, Maximum, IntervalEndPoint.Closed);
		}

		internal Interval(T minimum, IntervalEndPoint minimumPoint, T maximum, IntervalEndPoint maximumPoint)
		{
			Minimum = minimum;
			Maximum = maximum;

			_flags = CalculateFlags(minimum, minimumPoint, maximum, maximumPoint);
		}

		#endregion

		#region Properties

		public static readonly Interval<T> Empty = new Interval<T>(default, IntervalEndPoint.Open, default, IntervalEndPoint.Open);

		public static readonly Interval<T> Universe = new Interval<T>(IntervalMinMax.Get<T>().Minimum, IntervalEndPoint.Unbounded, IntervalMinMax.Get<T>().Maximum, IntervalEndPoint.Unbounded);

		public readonly T Maximum;

		private void Verify()
		{
			if (IsEmpty == false)
				throw new InvalidOperationException("Empty range");
		}

		public readonly T Minimum;

		private readonly byte _flags;

		public IntervalEndPoint MaximumPoint => IntervalEndPointFlag.MaxPointFlagArray[_flags];

		public IntervalEndPoint MinimumPoint => IntervalEndPointFlag.MinPointFlagArray[_flags];

		#endregion

		#region Methods

		private static byte CalculateFlags(T minimum, IntervalEndPoint minimumPoint, T maximum, IntervalEndPoint maximumPoint)
		{
			var minMaxCompare = minimum.CompareTo(maximum);
			var isEmpty = false;

			if (minMaxCompare > 0)
				isEmpty = true;
			else if (minMaxCompare == 0)
				isEmpty = minimumPoint == IntervalEndPoint.Open || maximumPoint == IntervalEndPoint.Open;

			if (minimumPoint == IntervalEndPoint.Unbounded || maximumPoint == IntervalEndPoint.Unbounded)
				isEmpty = false;

			return IntervalEndPointFlag.CalcFlag(minimumPoint, maximumPoint, isEmpty);
		}

		internal int CompareTo(Interval<T> range)
		{
			if (MinimumPoint == IntervalEndPoint.Unbounded)
			{
				if (range.MinimumPoint != IntervalEndPoint.Unbounded)
					return -1;
			}

			if (range.MinimumPoint == IntervalEndPoint.Unbounded)
			{
				if (MinimumPoint != IntervalEndPoint.Unbounded)
					return 1;
			}

			var minCompare = Minimum.CompareTo(range.Minimum);

			if (minCompare == 0)
				minCompare = Interval.CompareMinimumEndPoint(MinimumPoint, range.MinimumPoint);

			if (minCompare < 0)
				return -1;

			if (minCompare > 0)
				return 1;

			if (MaximumPoint == IntervalEndPoint.Unbounded)
			{
				if (range.MaximumPoint != IntervalEndPoint.Unbounded)
					return -1;
			}

			if (range.MaximumPoint == IntervalEndPoint.Unbounded)
			{
				if (MaximumPoint != IntervalEndPoint.Unbounded)
					return 1;
			}

			var maxCompare = Maximum.CompareTo(range.Maximum);

			if (maxCompare == 0)
				maxCompare = Interval.CompareMaximumEndPoint(MaximumPoint, range.MaximumPoint);

			if (maxCompare < 0)
				return -1;

			if (maxCompare > 0)
				return 1;

			return 0;
		}

		int IComparable<Interval<T>>.CompareTo(Interval<T> range)
		{
			return CompareTo(range);
		}

		public T Clamp(T value)
		{
			Verify();

			if (Minimum.CompareTo(value) > 0)
				return Minimum;

			if (Maximum.CompareTo(value) < 0)
				return Maximum;

			return value;
		}

		public bool Contains(Interval<T> range)
		{
			if (IsEmpty)
				return false;

			if (range.IsEmpty)
				return false;

			var minimumPoint = MinimumPoint;
			var minResult = minimumPoint == IntervalEndPoint.Unbounded;

			if (minResult == false)
			{
				var minCompare = Minimum.CompareTo(range.Minimum);

				minResult = minimumPoint == IntervalEndPoint.Closed ? minCompare <= 0 : minCompare < 0;

				if (minResult == false)
				{
					if (!(minCompare == 0 && minimumPoint == range.MinimumPoint))
						return false;
				}
			}

			var maximumPoint = MaximumPoint;
			var maxResult = maximumPoint == IntervalEndPoint.Unbounded;

			if (maxResult == false)
			{
				var maxCompare = Maximum.CompareTo(range.Maximum);

				maxResult = maximumPoint == IntervalEndPoint.Closed ? maxCompare >= 0 : maxCompare > 0;

				if (maxResult == false)
				{
					if (!(maxCompare == 0 && maximumPoint == range.MaximumPoint))
						return false;
				}
			}

			return true;
		}

		public bool Contains(T value)
		{
			if (IsEmpty)
				return false;

			var minimumPoint = MinimumPoint;
			var minResult = minimumPoint == IntervalEndPoint.Unbounded;

			if (minResult == false)
			{
				var minCompare = Minimum.CompareTo(value);

				minResult = minimumPoint == IntervalEndPoint.Closed ? minCompare <= 0 : minCompare < 0;

				if (minResult == false)
					return false;
			}

			var maximumPoint = MaximumPoint;
			var maxResult = maximumPoint == IntervalEndPoint.Unbounded;

			if (maxResult == false)
			{
				var maxCompare = Maximum.CompareTo(value);

				maxResult = maximumPoint == IntervalEndPoint.Closed ? maxCompare >= 0 : maxCompare > 0;
			}

			return maxResult;
		}

		public bool Overlaps(Interval<T> range)
		{
			return Interval.HasIntersection(this, range);
		}

		public Interval<T> IntersectWith(Interval<T> range)
		{
			return Interval.Intersect(this, range);
		}

		public Interval<T> UnionWith(Interval<T> range)
		{
			return Interval.Union(this, range);
		}

		public Interval<T> ExceptWith(Interval<T> range)
		{
			return Interval.Except(this, range);
		}

		//public Range<T> ComplementWith(Range<T> range)
		//{
		//	return Range.Complement(this, range);
		//}

		//public static Range<T> operator ^(Range<T> left, Range<T> right)
		//{
		//	return Range.Complement(left, right);
		//}

		public static Interval<T> operator |(Interval<T> left, Interval<T> right)
		{
			return Interval.Union(left, right);
		}

		public static Interval<T> operator &(Interval<T> left, Interval<T> right)
		{
			return Interval.Intersect(left, right);
		}

		public bool IsEmpty => (_flags & 1) == 1;

		public override string ToString()
		{
			var leftParen = MinimumPoint == IntervalEndPoint.Closed ? "[" : "(";
			var rightParen = MaximumPoint == IntervalEndPoint.Closed ? "]" : ")";
			var minimum = MinimumPoint == IntervalEndPoint.Unbounded ? "-INF" : Minimum.ToString();
			var maximum = MaximumPoint == IntervalEndPoint.Unbounded ? "+INF" : Maximum.ToString();

			return $"{leftParen}{minimum};{maximum}{rightParen}";
		}

		public bool Equals(Interval<T> range)
		{
			return EqualityComparer<T>.Default.Equals(Maximum, range.Maximum) && EqualityComparer<T>.Default.Equals(Minimum, range.Minimum) && _flags == range._flags;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
				return false;

			return obj is Interval<T> range && Equals(range);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = EqualityComparer<T>.Default.GetHashCode(Maximum);

				hashCode = (hashCode * 397) ^ EqualityComparer<T>.Default.GetHashCode(Minimum);
				hashCode = (hashCode * 397) ^ _flags.GetHashCode();

				return hashCode;
			}
		}

		public static implicit operator Interval<T>(Range<T> range)
		{
			return Interval.Create(range.Minimum, range.Maximum);
		}

		internal Range<T> AsRange()
		{
			return new Range<T>(Minimum, Maximum);
		}

		#endregion
	}

	internal static partial class Interval
	{
		#region Static Fields and Constants

		private static readonly Regex ParseRegex = new Regex(@"\s*([\[\(])\s*(.*)\s*;\s*(.*)\s*([\]\)])\s*", RegexOptions.Compiled);

		#endregion

		#region  Methods

		internal static int CompareMaximumEndPoint(IntervalEndPoint first, IntervalEndPoint second)
		{
			if (first == second)
				return 0;

			if (first == IntervalEndPoint.Unbounded)
				return 1;

			return second == IntervalEndPoint.Closed ? -1 : 1;
		}

		internal static int CompareMinimumEndPoint(IntervalEndPoint first, IntervalEndPoint second)
		{
			if (first == second)
				return 0;

			if (second == IntervalEndPoint.Unbounded)
				return 1;

			return first == IntervalEndPoint.Closed ? -1 : 1;
		}

		//public static Range<T> Complement<T>(Range<T> first, Range<T> second) where T : IComparable<T>
		//{
		//	return ComplementImpl(first, second);
		//}

		//private static Range<T> ComplementImpl<T>(Range<T> first, Range<T> second) where T : IComparable<T>
		//{
		//	if (first.Equals(second))
		//		return Range<T>.Empty;

		//	SortEndPoints(first, second, out var leftMin, out _, out _, out var rightMax);

		//	if (leftMin.RangeIndex == rightMax.RangeIndex)
		//	{
		//		if (leftMin.RangeIndex == 1)
		//			return Range<T>.Empty;

		//		if (leftMin.RangeIndex == 0)
		//			throw FormatNonContiguousException(nameof(Complement));
		//	}

		//	return new Range<T>(leftMin.Value, leftMin.EndPoint, rightMax.Value, rightMax.EndPoint);
		//}

		public static Interval<T> Create<T>(T minimum, T maximum) where T : IComparable<T>
		{
			return new Interval<T>(minimum, maximum);
		}

		internal static Interval<T> CreateMaximumUnbounded<T>(T minimum, IntervalEndPoint minimumPoint = IntervalEndPoint.Closed) where T : IComparable<T>
		{
			return new Interval<T>(minimum, minimumPoint, minimum, IntervalEndPoint.Unbounded);
		}

		internal static Interval<T> CreateMinimumUnbounded<T>(T maximum, IntervalEndPoint maximumPoint = IntervalEndPoint.Closed) where T : IComparable<T>
		{
			return new Interval<T>(maximum, IntervalEndPoint.Unbounded, maximum, maximumPoint);
		}

		public static Interval<T> Except<T>(Interval<T> first, Interval<T> second) where T : IComparable<T>
		{
			return ExceptImpl(first, second);
		}

		private static Interval<T> ExceptImpl<T>(Interval<T> first, Interval<T> second) where T : IComparable<T>
		{
			if (first.Equals(second))
				return Interval<T>.Empty;

			var compareMinimum = first.Minimum.CompareTo(second.Minimum);
			var compareMaximum = first.Maximum.CompareTo(second.Maximum);

			if (compareMinimum == 0 && first.MinimumPoint == second.MinimumPoint)
			{
				// Case a:
				// first  [0;10]    [0----------10]
				// second  [0;5]    [0-----5]
				// result                 (5----10]
				// calculations: minimums equals, first maximum > second maximum, result = ~(secondMax; firstMax]
				if (compareMaximum > 0)
					return new Interval<T>(second.Maximum, InverseEndPoint(second.MaximumPoint), first.Maximum, first.MaximumPoint);

				// Case b:
				// first   [0;5]    [0-----5]
				// second [0;10]    [0----------10]
				// result                 (5----10]
				// calculations: minimums equals, first maximum < second maximum, result = ~(firstMax; secondMax]
				if (compareMaximum < 0)
					return new Interval<T>(first.Maximum, InverseEndPoint(first.MaximumPoint), second.Maximum, second.MaximumPoint);
			}

			if (compareMaximum == 0 && first.MaximumPoint == second.MaximumPoint)
			{
				// Case c:
				// first  [0;10]    [0----------10]
				// second [5;10]         [5-----10]
				// result           [0----5)
				// calculations: maximums equals, first minimum < second minimum, result = [firstMin; secondMin)~
				if (compareMinimum < 0)
					return new Interval<T>(first.Minimum, first.MinimumPoint, second.Minimum, InverseEndPoint(second.MinimumPoint));

				// Case d:
				// first  [5;10]         [5-----10]
				// second [0;10]    [0----------10]     
				// result           [0----5)
				// calculations: maximums equals, first minimum > second minimum, result = [secondMin; firstMin)~
				if (compareMinimum > 0)
					return new Interval<T>(second.Minimum, second.MinimumPoint, first.Minimum, InverseEndPoint(first.MinimumPoint));
			}

			throw FormatNonContiguousException("Except");
		}

		private static Exception FormatNonContiguousException(string operation)
		{
			return new ArgumentOutOfRangeException($"{operation} operation produces 2 non contiguous ranges.", (Exception) null);
		}

		public static bool HasIntersection<T>(Interval<T> first, Interval<T> second) where T : IComparable<T>
		{
			SortEndPoints(first, second, out _, out var rightMin, out var leftMax, out _);

			var compareTo = rightMin.Value.CompareTo(leftMax.Value);

			if (compareTo > 0)
				return false;

			if (compareTo < 0)
				return true;

			return !(rightMin.EndPoint == IntervalEndPoint.Open || leftMax.EndPoint == IntervalEndPoint.Open);
		}

		public static Interval<T> Intersect<T>(Interval<T> first, Interval<T> second) where T : IComparable<T>
		{
			return IntersectImpl(first, second);
		}

		private static Interval<T> IntersectImpl<T>(Interval<T> first, Interval<T> second) where T : IComparable<T>
		{
			if (first.Equals(second))
				return first;

			SortEndPoints(first, second, out _, out var rightMin, out var leftMax, out _);

			return rightMin.Value.CompareTo(leftMax.Value) > 0 ? Interval<T>.Empty : new Interval<T>(rightMin.Value, rightMin.EndPoint, leftMax.Value, leftMax.EndPoint);
		}

		private static IntervalEndPoint InverseEndPoint(IntervalEndPoint endPoint)
		{
			switch (endPoint)
			{
				case IntervalEndPoint.Open:
					return IntervalEndPoint.Closed;

				case IntervalEndPoint.Closed:
					return IntervalEndPoint.Open;

				case IntervalEndPoint.Unbounded:
					return IntervalEndPoint.Unbounded;

				default:
					throw new ArgumentOutOfRangeException(nameof(endPoint));
			}
		}

		public static bool Overlaps<T>(Interval<T> first, Interval<T> second) where T : IComparable<T>
		{
			SortEndPoints(first, second, out _, out var rightMin, out var leftMax, out _);

			var compare = rightMin.Value.CompareTo(leftMax.Value);

			if (compare > 0)
				return false;

			if (compare < 0)
				return true;

			return rightMin.EndPoint == IntervalEndPoint.Closed && leftMax.EndPoint == IntervalEndPoint.Closed;
		}

		public static Interval<T> Parse<T>(string value) where T : IComparable<T>
		{
			return Parse(value, StaticParser<T>.Default);
		}

		public static Interval<T> Parse<T>(string value, IParser<T> valueParser) where T : IComparable<T>
		{
			if (TryParse(value, valueParser, out var result))
				return result;

			throw new Exception("Parse exception");
		}

		private static void SortEndPoints<T>(Interval<T> first, Interval<T> second, out RangeEndPointValue<T> leftMin, out RangeEndPointValue<T> rightMin, out RangeEndPointValue<T> leftMax, out RangeEndPointValue<T> rightMax) where T : IComparable<T>
		{
			var firstMinimumPoint = first.MinimumPoint;
			var secondMinimumPoint = second.MinimumPoint;

			// Rightmost minimum
			if (firstMinimumPoint == IntervalEndPoint.Unbounded)
			{
				rightMin = new RangeEndPointValue<T>(second.Minimum, secondMinimumPoint, 1);
				leftMin = new RangeEndPointValue<T>(first.Minimum, firstMinimumPoint, 0);
			}
			else if (secondMinimumPoint == IntervalEndPoint.Unbounded)
			{
				leftMin = new RangeEndPointValue<T>(second.Minimum, secondMinimumPoint, 1);
				rightMin = new RangeEndPointValue<T>(first.Minimum, firstMinimumPoint, 0);
			}
			else
			{
				var firstSecondMinCompare = first.Minimum.CompareTo(second.Minimum);

				if (firstSecondMinCompare == 0)
					firstSecondMinCompare = CompareMinimumEndPoint(firstMinimumPoint, secondMinimumPoint);

				if (firstSecondMinCompare > 0)
				{
					leftMin = new RangeEndPointValue<T>(second.Minimum, secondMinimumPoint, 1);
					rightMin = new RangeEndPointValue<T>(first.Minimum, firstMinimumPoint, 0);
				}
				else
				{
					rightMin = new RangeEndPointValue<T>(second.Minimum, secondMinimumPoint, 1);
					leftMin = new RangeEndPointValue<T>(first.Minimum, firstMinimumPoint, 0);
				}
			}

			var firstMaximumPoint = first.MaximumPoint;
			var secondMaximumPoint = second.MaximumPoint;

			// Leftmost maximum
			if (firstMaximumPoint == IntervalEndPoint.Unbounded)
			{
				leftMax = new RangeEndPointValue<T>(second.Maximum, secondMaximumPoint, 1);
				rightMax = new RangeEndPointValue<T>(first.Maximum, firstMaximumPoint, 0);
			}
			else if (secondMaximumPoint == IntervalEndPoint.Unbounded)
			{
				rightMax = new RangeEndPointValue<T>(second.Maximum, secondMaximumPoint, 1);
				leftMax = new RangeEndPointValue<T>(first.Maximum, firstMaximumPoint, 0);
			}
			else
			{
				var firstSecondMaxCompare = first.Maximum.CompareTo(second.Maximum);

				if (firstSecondMaxCompare == 0)
					firstSecondMaxCompare = CompareMaximumEndPoint(firstMaximumPoint, secondMaximumPoint);

				if (firstSecondMaxCompare < 0)
				{
					rightMax = new RangeEndPointValue<T>(second.Maximum, secondMaximumPoint, 1);
					leftMax = new RangeEndPointValue<T>(first.Maximum, firstMaximumPoint, 0);
				}
				else
				{
					leftMax = new RangeEndPointValue<T>(second.Maximum, secondMaximumPoint, 1);
					rightMax = new RangeEndPointValue<T>(first.Maximum, firstMaximumPoint, 0);
				}
			}
		}

		internal static IEnumerable<IntervalItem<TRangeItem, T>> Split<TRangeItem, T>(IntervalItem<TRangeItem, T> first, IntervalItem<TRangeItem, T> second) where T : IComparable<T>
		{
			if (first.Interval.Equals(second.Interval))
			{
				yield return first;
				yield return second;
				yield break;
			}

			SortEndPoints(first.Interval, second.Interval, out var leftMin, out var rightMin, out var leftMax, out var rightMax);

			var minMaxCompare = rightMin.Value.CompareTo(leftMax.Value);

			if (minMaxCompare > 0)
			{
				yield return first;
				yield return second;
			}
			else
			{
				var leftMinItem = leftMin.RangeIndex == 0 ? first.Item : second.Item;
				var rightMinItem = rightMin.RangeIndex == 0 ? first.Item : second.Item;

				if (leftMin.RangeIndex == rightMax.RangeIndex)
				{
					// Outer
					yield return new IntervalItem<TRangeItem, T>(leftMinItem, new Interval<T>(leftMin.Value, leftMin.EndPoint, rightMin.Value, InverseEndPoint(rightMin.EndPoint)));
					yield return new IntervalItem<TRangeItem, T>(leftMinItem, new Interval<T>(rightMin.Value, rightMin.EndPoint, leftMax.Value, leftMax.EndPoint));
					yield return new IntervalItem<TRangeItem, T>(leftMinItem, new Interval<T>(leftMax.Value, InverseEndPoint(leftMax.EndPoint), rightMax.Value, rightMax.EndPoint));

					// Inner
					yield return new IntervalItem<TRangeItem, T>(rightMinItem, new Interval<T>(rightMin.Value, rightMin.EndPoint, leftMax.Value, leftMax.EndPoint));
				}
				else
				{
					// Left
					yield return new IntervalItem<TRangeItem, T>(leftMinItem, new Interval<T>(leftMin.Value, leftMin.EndPoint, rightMin.Value, InverseEndPoint(rightMin.EndPoint)));
					yield return new IntervalItem<TRangeItem, T>(leftMinItem, new Interval<T>(rightMin.Value, rightMin.EndPoint, leftMax.Value, leftMax.EndPoint));

					// Right
					yield return new IntervalItem<TRangeItem, T>(rightMinItem, new Interval<T>(rightMin.Value, rightMin.EndPoint, leftMax.Value, leftMax.EndPoint));
					yield return new IntervalItem<TRangeItem, T>(rightMinItem, new Interval<T>(leftMax.Value, InverseEndPoint(leftMax.EndPoint), rightMax.Value, rightMax.EndPoint));
				}
			}
		}

		internal static List<IntervalItem<TItem, T>> Split<TItem, T>(IEnumerable<IntervalItem<TItem, T>> rangeItems) where T : IComparable<T>
		{
			// TODO Very naive implementation. Rework.
			var rangeItemPairs = rangeItems.ToList();
			var result = new List<IntervalItem<TItem, T>>();
			var sortDictionary = new Dictionary<TItem, int>();

			for (var index = 0; index < rangeItemPairs.Count; index++)
			{
				var rangeItemPair = rangeItemPairs[index];

				if (sortDictionary.ContainsKey(rangeItemPair.Item) == false)
					sortDictionary.Add(rangeItemPair.Item, index);
			}

			if (rangeItemPairs.Count < 2)
			{
				result.AddRange(rangeItemPairs);

				return result;
			}

			var splitPoints = new List<SplitPointData<T>>();
			var distinctSet = new HashSet<IntervalItem<TItem, T>>();
			var distinctPoint = new HashSet<SplitPointData<T>>();

			foreach (var rangeItemPair in rangeItemPairs)
			{
				{
					var splitPointData = new SplitPointData<T>(rangeItemPair.Interval.Minimum, IntervalEndPoint.Open);

					if (distinctPoint.Add(splitPointData))
						splitPoints.Add(splitPointData);
				}

				{
					var splitPointData = new SplitPointData<T>(rangeItemPair.Interval.Minimum, IntervalEndPoint.Closed);

					if (distinctPoint.Add(splitPointData))
						splitPoints.Add(splitPointData);
				}

				{
					var splitPointData = new SplitPointData<T>(rangeItemPair.Interval.Maximum, IntervalEndPoint.Open);

					if (distinctPoint.Add(splitPointData))
						splitPoints.Add(splitPointData);
				}

				{
					var splitPointData = new SplitPointData<T>(rangeItemPair.Interval.Maximum, IntervalEndPoint.Closed);

					if (distinctPoint.Add(splitPointData))
						splitPoints.Add(splitPointData);
				}
			}

			splitPoints.Sort(SplitDataPointComparer<T>.Default);

			var prevPoint = splitPoints[0].Point;
			var prevSame = false;

			for (var iPoint = 1; iPoint < splitPoints.Count; iPoint++)
			{
				var splitPointData = splitPoints[iPoint];
				var startEndPoint = prevSame ? IntervalEndPoint.Open : IntervalEndPoint.Closed;
				var currentPoint = splitPointData.Point;
				var same = prevPoint.CompareTo(currentPoint) == 0;
				var range = same ? new Interval<T>(prevPoint, currentPoint) : new Interval<T>(prevPoint, startEndPoint, currentPoint, IntervalEndPoint.Open);

				prevPoint = currentPoint;
				prevSame = same;

				var items = rangeItemPairs.Where(ri => ri.Interval.Contains(range)).Select(ri => new IntervalItem<TItem, T>(ri.Item, range));

				result.AddRange(items.Where(distinctSet.Add));
			}

			return result.OrderBy(r => sortDictionary[r.Item]).ThenBy(r => r.Interval).ToList();
		}

		public static bool TryParse<T>(string value, IParser<T> valueParser, out Interval<T> result) where T : IComparable<T>
		{
			var match = ParseRegex.Match(value);

			if (match.Success == false)
			{
				result = Interval<T>.Empty;

				return false;
			}

			var minimumEndPoint = match.Groups[1].Value == "[" ? IntervalEndPoint.Closed : IntervalEndPoint.Open;
			var maximumEndPoint = match.Groups[4].Value == "]" ? IntervalEndPoint.Closed : IntervalEndPoint.Open;

			var minimum = default(T);
			var maximum = default(T);

			var minimumUnbounded = string.Equals(match.Groups[2].Value, "-INF", StringComparison.OrdinalIgnoreCase);
			var maximumUnbounded = string.Equals(match.Groups[3].Value, "+INF", StringComparison.OrdinalIgnoreCase);

			if ((minimumUnbounded || valueParser.TryParse(match.Groups[2].Value, out minimum)) && (maximumUnbounded || valueParser.TryParse(match.Groups[3].Value, out maximum)))
			{
				if (minimumUnbounded)
				{
					minimum = Interval<T>.Universe.Minimum;
					minimumEndPoint = IntervalEndPoint.Unbounded;
				}

				if (maximumUnbounded)
				{
					maximum = Interval<T>.Universe.Maximum;
					maximumEndPoint = IntervalEndPoint.Unbounded;
				}

				result = new Interval<T>(minimum, minimumEndPoint, maximum, maximumEndPoint);

				return true;
			}

			result = Interval<T>.Empty;

			return false;
		}

		public static bool TryParse<T>(string value, out Interval<T> result) where T : IComparable<T>
		{
			return TryParse(value, StaticParser<T>.Default, out result);
		}

		public static Interval<T> Union<T>(Interval<T> first, Interval<T> second) where T : IComparable<T>
		{
			return UnionImpl(first, second);
		}

		private static Interval<T> UnionImpl<T>(Interval<T> first, Interval<T> second) where T : IComparable<T>
		{
			if (first.Equals(second))
				return first;

			SortEndPoints(first, second, out var leftMin, out var rightMin, out var leftMax, out var rightMax);

			var compare = rightMin.Value.CompareTo(leftMax.Value);

			if (compare > 0)
				throw FormatNonContiguousException("Union");

			if (compare == 0)
			{
				if (rightMin.EndPoint == IntervalEndPoint.Open && leftMax.EndPoint == IntervalEndPoint.Open)
					throw FormatNonContiguousException("Union");
			}

			return new Interval<T>(leftMin.Value, leftMin.EndPoint, rightMax.Value, rightMax.EndPoint);
		}

		public static int Count(this Interval<byte> range)
		{
			return range.Maximum - range.Minimum + GetEndPointCount(range);
		}
		public static int Count(this Interval<sbyte> range)
		{
			return range.Maximum - range.Minimum + GetEndPointCount(range);
		}
		public static int Count(this Interval<char> range)
		{
			return range.Maximum - range.Minimum + GetEndPointCount(range);
		}
		public static int Count(this Interval<short> range)
		{
			return range.Maximum - range.Minimum + GetEndPointCount(range);
		}
		public static int Count(this Interval<ushort> range)
		{
			return range.Maximum - range.Minimum + GetEndPointCount(range);
		}
		public static int Count(this Interval<int> range)
		{
			return range.Maximum - range.Minimum + GetEndPointCount(range);
		}
		public static long Count(this Interval<uint> range)
		{
			return range.Maximum - range.Minimum + GetEndPointCount(range);
		}
		public static long Count(this Interval<long> range)
		{
			return range.Maximum - range.Minimum + GetEndPointCount(range);
		}
		public static ulong Count(this Interval<ulong> range)
		{
			return range.Maximum - range.Minimum + (ulong)GetEndPointCount(range);
		}

		private static int GetEndPointCount<T>(this Interval<T> range) where T : IComparable<T>
		{
			return GetEndPointCount(range.MinimumPoint) + GetEndPointCount(range.MaximumPoint) - 1;
		}

		private static int GetEndPointCount(IntervalEndPoint endPoint)
		{
			switch (endPoint)
			{
				case IntervalEndPoint.Open:
					return 0;
				case IntervalEndPoint.Closed:
					return 1;
				case IntervalEndPoint.Unbounded:
					return 0;
				default:
					throw new ArgumentOutOfRangeException(nameof(endPoint), endPoint, null);
			}
		}

		#endregion

		#region  Nested Types

		private readonly struct SplitPointData<T>
		{
			public SplitPointData(T point, IntervalEndPoint endPoint)
			{
				Point = point;
				EndPoint = endPoint;
			}

			public readonly T Point;
			public readonly IntervalEndPoint EndPoint;

			public override string ToString()
			{
				return $"{Point}";
			}

			public bool Equals(SplitPointData<T> other)
			{
				return EqualityComparer<T>.Default.Equals(Point, other.Point) && EndPoint == other.EndPoint;
			}

			public override bool Equals(object obj)
			{
				if (ReferenceEquals(null, obj))
					return false;

				return obj is SplitPointData<T> data && Equals(data);
			}

			public override int GetHashCode()
			{
				unchecked
				{
					return (EqualityComparer<T>.Default.GetHashCode(Point) * 397) ^ (int) EndPoint;
				}
			}
		}

		private class SplitDataPointComparer<T> : IComparer<SplitPointData<T>> where T : IComparable<T>
		{
			#region Static Fields and Constants

			public static readonly IComparer<SplitPointData<T>> Default = new SplitDataPointComparer<T>();

			#endregion

			#region Ctors

			private SplitDataPointComparer()
			{
			}

			#endregion

			#region  Methods

			private static int GetEndPointValue(IntervalEndPoint endPoint)
			{
				switch (endPoint)
				{
					case IntervalEndPoint.Closed:
						return 0;

					case IntervalEndPoint.Open:
						return 1;

					default:
						throw new ArgumentOutOfRangeException(nameof(endPoint), endPoint, null);
				}
			}

			#endregion

			#region Interface Implementations

			#region IComparer<SplitPointData<T>>

			public int Compare(SplitPointData<T> x, SplitPointData<T> y)
			{
				var pointCompare = x.Point.CompareTo(y.Point);

				return pointCompare == 0 ? Comparer<int>.Default.Compare(GetEndPointValue(x.EndPoint), GetEndPointValue(y.EndPoint)) : pointCompare;
			}

			#endregion

			#endregion
		}

		private readonly struct RangeEndPointValue<T> where T : IComparable<T>
		{
			public RangeEndPointValue(T value, IntervalEndPoint endPoint, int rangeIndex)
			{
				Value = value;
				EndPoint = endPoint;
				RangeIndex = rangeIndex;
			}

			public readonly T Value;
			public readonly IntervalEndPoint EndPoint;
			public readonly int RangeIndex;
		}

		#endregion
	}

	internal static class IntervalItem
	{
		#region  Methods

		public static IntervalItem<TItem, T> Create<TItem, T>(TItem item, Interval<T> range) where T : IComparable<T>
		{
			return new IntervalItem<TItem, T>(item, range);
		}

		#endregion
	}

	internal readonly struct IntervalItem<TItem, T> : IComparable<IntervalItem<TItem, T>> where T : IComparable<T>
	{
		public IntervalItem(TItem item, Interval<T> interval)
		{
			Item = item;
			Interval = interval;
		}

		public readonly TItem Item;

		public readonly Interval<T> Interval;

		public int CompareTo(IntervalItem<TItem, T> other)
		{
			return Interval.CompareTo(other.Interval);
		}

		public override string ToString()
		{
			return $"{Item}:{Interval}";
		}

		public bool Equals(IntervalItem<TItem, T> other)
		{
			return EqualityComparer<TItem>.Default.Equals(Item, other.Item) && Interval.Equals(other.Interval);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
				return false;

			return obj is IntervalItem<TItem, T> item && Equals(item);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (EqualityComparer<TItem>.Default.GetHashCode(Item) * 397) ^ Interval.GetHashCode();
			}
		}
	}
}