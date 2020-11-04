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
	public enum RangeEndPoint
	{
		Open = 0,
		Closed = 1,
		Unbounded = 2
	}

	internal static class RangeEndPointFlag
	{
		#region Static Fields and Constants

		private const int MaximumPointOpenShift = 1;
		private const int MinimumPointOpenShift = 3;
		public static readonly RangeEndPoint[] MinPointFlagArray = new RangeEndPoint[32];
		public static readonly RangeEndPoint[] MaxPointFlagArray = new RangeEndPoint[32];

		#endregion

		#region Ctors

		static RangeEndPointFlag()
		{
			MinPointFlagArray[CalcFlag(RangeEndPoint.Closed, RangeEndPoint.Closed, false)] = RangeEndPoint.Closed;
			MinPointFlagArray[CalcFlag(RangeEndPoint.Closed, RangeEndPoint.Open, false)] = RangeEndPoint.Closed;
			MinPointFlagArray[CalcFlag(RangeEndPoint.Closed, RangeEndPoint.Unbounded, false)] = RangeEndPoint.Closed;
			MinPointFlagArray[CalcFlag(RangeEndPoint.Open, RangeEndPoint.Closed, false)] = RangeEndPoint.Open;
			MinPointFlagArray[CalcFlag(RangeEndPoint.Open, RangeEndPoint.Open, false)] = RangeEndPoint.Open;
			MinPointFlagArray[CalcFlag(RangeEndPoint.Open, RangeEndPoint.Unbounded, false)] = RangeEndPoint.Open;
			MinPointFlagArray[CalcFlag(RangeEndPoint.Unbounded, RangeEndPoint.Closed, false)] = RangeEndPoint.Unbounded;
			MinPointFlagArray[CalcFlag(RangeEndPoint.Unbounded, RangeEndPoint.Open, false)] = RangeEndPoint.Unbounded;
			MinPointFlagArray[CalcFlag(RangeEndPoint.Unbounded, RangeEndPoint.Unbounded, false)] = RangeEndPoint.Unbounded;
			MinPointFlagArray[CalcFlag(RangeEndPoint.Closed, RangeEndPoint.Closed, true)] = RangeEndPoint.Closed;
			MinPointFlagArray[CalcFlag(RangeEndPoint.Closed, RangeEndPoint.Open, true)] = RangeEndPoint.Closed;
			MinPointFlagArray[CalcFlag(RangeEndPoint.Closed, RangeEndPoint.Unbounded, true)] = RangeEndPoint.Closed;
			MinPointFlagArray[CalcFlag(RangeEndPoint.Open, RangeEndPoint.Closed, true)] = RangeEndPoint.Open;
			MinPointFlagArray[CalcFlag(RangeEndPoint.Open, RangeEndPoint.Open, true)] = RangeEndPoint.Open;
			MinPointFlagArray[CalcFlag(RangeEndPoint.Open, RangeEndPoint.Unbounded, true)] = RangeEndPoint.Open;
			MinPointFlagArray[CalcFlag(RangeEndPoint.Unbounded, RangeEndPoint.Closed, true)] = RangeEndPoint.Unbounded;
			MinPointFlagArray[CalcFlag(RangeEndPoint.Unbounded, RangeEndPoint.Open, true)] = RangeEndPoint.Unbounded;
			MinPointFlagArray[CalcFlag(RangeEndPoint.Unbounded, RangeEndPoint.Unbounded, true)] = RangeEndPoint.Unbounded;

			MaxPointFlagArray[CalcFlag(RangeEndPoint.Closed, RangeEndPoint.Closed, false)] = RangeEndPoint.Closed;
			MaxPointFlagArray[CalcFlag(RangeEndPoint.Closed, RangeEndPoint.Open, false)] = RangeEndPoint.Open;
			MaxPointFlagArray[CalcFlag(RangeEndPoint.Closed, RangeEndPoint.Unbounded, false)] = RangeEndPoint.Unbounded;
			MaxPointFlagArray[CalcFlag(RangeEndPoint.Open, RangeEndPoint.Closed, false)] = RangeEndPoint.Closed;
			MaxPointFlagArray[CalcFlag(RangeEndPoint.Open, RangeEndPoint.Open, false)] = RangeEndPoint.Open;
			MaxPointFlagArray[CalcFlag(RangeEndPoint.Open, RangeEndPoint.Unbounded, false)] = RangeEndPoint.Unbounded;
			MaxPointFlagArray[CalcFlag(RangeEndPoint.Unbounded, RangeEndPoint.Closed, false)] = RangeEndPoint.Closed;
			MaxPointFlagArray[CalcFlag(RangeEndPoint.Unbounded, RangeEndPoint.Open, false)] = RangeEndPoint.Open;
			MaxPointFlagArray[CalcFlag(RangeEndPoint.Unbounded, RangeEndPoint.Unbounded, false)] = RangeEndPoint.Unbounded;
			MaxPointFlagArray[CalcFlag(RangeEndPoint.Closed, RangeEndPoint.Closed, true)] = RangeEndPoint.Closed;
			MaxPointFlagArray[CalcFlag(RangeEndPoint.Closed, RangeEndPoint.Open, true)] = RangeEndPoint.Open;
			MaxPointFlagArray[CalcFlag(RangeEndPoint.Closed, RangeEndPoint.Unbounded, true)] = RangeEndPoint.Unbounded;
			MaxPointFlagArray[CalcFlag(RangeEndPoint.Open, RangeEndPoint.Closed, true)] = RangeEndPoint.Closed;
			MaxPointFlagArray[CalcFlag(RangeEndPoint.Open, RangeEndPoint.Open, true)] = RangeEndPoint.Open;
			MaxPointFlagArray[CalcFlag(RangeEndPoint.Open, RangeEndPoint.Unbounded, true)] = RangeEndPoint.Unbounded;
			MaxPointFlagArray[CalcFlag(RangeEndPoint.Unbounded, RangeEndPoint.Closed, true)] = RangeEndPoint.Closed;
			MaxPointFlagArray[CalcFlag(RangeEndPoint.Unbounded, RangeEndPoint.Open, true)] = RangeEndPoint.Open;
			MaxPointFlagArray[CalcFlag(RangeEndPoint.Unbounded, RangeEndPoint.Unbounded, true)] = RangeEndPoint.Unbounded;
		}

		#endregion

		#region  Methods

		public static byte CalcFlag(RangeEndPoint minimumPoint, RangeEndPoint maximumPoint, bool isEmpty)
		{
			var minimumPointVal = (int) minimumPoint << MinimumPointOpenShift;
			var maximumPointVal = (int) maximumPoint << MaximumPointOpenShift;

			var isEmptyVal = isEmpty ? 1 : 0;

			return (byte) (minimumPointVal | maximumPointVal | isEmptyVal);
		}

		#endregion
	}

	[PublicAPI]
	public struct Range<T> : IComparable<Range<T>> where T : IComparable<T>
	{
		#region Ctors

		public Range(T minimum, T maximum)
		{
			Minimum = minimum;
			Maximum = maximum;

			_flags = CalculateFlags(Minimum, RangeEndPoint.Closed, Maximum, RangeEndPoint.Closed);
		}

		internal Range(T minimum, RangeEndPoint minimumPoint, T maximum, RangeEndPoint maximumPoint)
		{
			Minimum = minimum;
			Maximum = maximum;

			_flags = CalculateFlags(minimum, minimumPoint, maximum, maximumPoint);
		}

		#endregion

		#region Properties

		public static readonly Range<T> Empty = new Range<T>(default, RangeEndPoint.Open, default, RangeEndPoint.Open);

		public static readonly Range<T> Universe = new Range<T>(RangeMinMax.Get<T>().Minimum, RangeEndPoint.Unbounded, RangeMinMax.Get<T>().Maximum, RangeEndPoint.Unbounded);

		public readonly T Maximum;

		private void Verify()
		{
			if (IsEmpty == false)
				throw new InvalidOperationException("Empty range");
		}

		public readonly T Minimum;

		private readonly byte _flags;

		public RangeEndPoint MaximumPoint => RangeEndPointFlag.MaxPointFlagArray[_flags];

		public RangeEndPoint MinimumPoint => RangeEndPointFlag.MinPointFlagArray[_flags];

		#endregion

		#region Methods

		private static byte CalculateFlags(T minimum, RangeEndPoint minimumPoint, T maximum, RangeEndPoint maximumPoint)
		{
			var minMaxCompare = minimum.CompareTo(maximum);
			var isEmpty = false;

			if (minMaxCompare > 0)
				isEmpty = true;
			else if (minMaxCompare == 0)
				isEmpty = minimumPoint == RangeEndPoint.Open || maximumPoint == RangeEndPoint.Open;

			if (minimumPoint == RangeEndPoint.Unbounded || maximumPoint == RangeEndPoint.Unbounded)
				isEmpty = false;

			return RangeEndPointFlag.CalcFlag(minimumPoint, maximumPoint, isEmpty);
		}

		internal int CompareTo(Range<T> range)
		{
			if (MinimumPoint == RangeEndPoint.Unbounded)
			{
				if (range.MinimumPoint != RangeEndPoint.Unbounded)
					return -1;
			}

			if (range.MinimumPoint == RangeEndPoint.Unbounded)
			{
				if (MinimumPoint != RangeEndPoint.Unbounded)
					return 1;
			}

			var minCompare = Minimum.CompareTo(range.Minimum);

			if (minCompare == 0)
				minCompare = Range.CompareMinimumEndPoint(MinimumPoint, range.MinimumPoint);

			if (minCompare < 0)
				return -1;

			if (minCompare > 0)
				return 1;

			if (MaximumPoint == RangeEndPoint.Unbounded)
			{
				if (range.MaximumPoint != RangeEndPoint.Unbounded)
					return -1;
			}

			if (range.MaximumPoint == RangeEndPoint.Unbounded)
			{
				if (MaximumPoint != RangeEndPoint.Unbounded)
					return 1;
			}

			var maxCompare = Maximum.CompareTo(range.Maximum);

			if (maxCompare == 0)
				maxCompare = Range.CompareMaximumEndPoint(MaximumPoint, range.MaximumPoint);

			if (maxCompare < 0)
				return -1;

			if (maxCompare > 0)
				return 1;

			return 0;
		}

		int IComparable<Range<T>>.CompareTo(Range<T> range)
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

		public bool Contains(Range<T> range)
		{
			if (IsEmpty)
				return false;

			if (range.IsEmpty)
				return false;

			var minimumPoint = MinimumPoint;
			var minResult = minimumPoint == RangeEndPoint.Unbounded;

			if (minResult == false)
			{
				var minCompare = Minimum.CompareTo(range.Minimum);

				minResult = minimumPoint == RangeEndPoint.Closed ? minCompare <= 0 : minCompare < 0;

				if (minResult == false)
				{
					if (!(minCompare == 0 && minimumPoint == range.MinimumPoint))
						return false;
				}
			}

			var maximumPoint = MaximumPoint;
			var maxResult = maximumPoint == RangeEndPoint.Unbounded;

			if (maxResult == false)
			{
				var maxCompare = Maximum.CompareTo(range.Maximum);

				maxResult = maximumPoint == RangeEndPoint.Closed ? maxCompare >= 0 : maxCompare > 0;

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
			var minResult = minimumPoint == RangeEndPoint.Unbounded;

			if (minResult == false)
			{
				var minCompare = Minimum.CompareTo(value);

				minResult = minimumPoint == RangeEndPoint.Closed ? minCompare <= 0 : minCompare < 0;

				if (minResult == false)
					return false;
			}

			var maximumPoint = MaximumPoint;
			var maxResult = maximumPoint == RangeEndPoint.Unbounded;

			if (maxResult == false)
			{
				var maxCompare = Maximum.CompareTo(value);

				maxResult = maximumPoint == RangeEndPoint.Closed ? maxCompare >= 0 : maxCompare > 0;
			}

			return maxResult;
		}

		public bool Overlaps(Range<T> range)
		{
			return Range.HasIntersection(this, range);
		}

		public Range<T> IntersectWith(Range<T> range)
		{
			return Range.Intersect(this, range);
		}

		public Range<T> UnionWith(Range<T> range)
		{
			return Range.Union(this, range);
		}

		public Range<T> ComplementWith(Range<T> range)
		{
			return Range.Complement(this, range);
		}

		public static Range<T> operator ^(Range<T> left, Range<T> right)
		{
			return Range.Complement(left, right);
		}

		public static Range<T> operator |(Range<T> left, Range<T> right)
		{
			return Range.Union(left, right);
		}

		public static Range<T> operator &(Range<T> left, Range<T> right)
		{
			return Range.Intersect(left, right);
		}

		public bool IsEmpty => (_flags & 1) == 1;

		public override string ToString()
		{
			var leftParen = MinimumPoint == RangeEndPoint.Closed ? "[" : "(";
			var rightParen = MaximumPoint == RangeEndPoint.Closed ? "]" : ")";
			var minimum = MinimumPoint == RangeEndPoint.Unbounded ? "-INF" : Minimum.ToString();
			var maximum = MaximumPoint == RangeEndPoint.Unbounded ? "+INF" : Maximum.ToString();

			return $"{leftParen}{minimum};{maximum}{rightParen}";
		}

		public bool Equals(Range<T> range)
		{
			return EqualityComparer<T>.Default.Equals(Maximum, range.Maximum) && EqualityComparer<T>.Default.Equals(Minimum, range.Minimum) && _flags == range._flags;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
				return false;

			return obj is Range<T> range && Equals(range);
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

		#endregion
	}

	public static partial class Range
	{
		#region Static Fields and Constants

		private static readonly Regex ParseRegex = new Regex(@"\s*([\[\(])\s*(.*)\s*;\s*(.*)\s*([\]\)])\s*", RegexOptions.Compiled);

		#endregion

		#region  Methods

		internal static int CompareMaximumEndPoint(RangeEndPoint first, RangeEndPoint second)
		{
			if (first == second)
				return 0;

			if (first == RangeEndPoint.Unbounded)
				return 1;

			return second == RangeEndPoint.Closed ? -1 : 1;
		}

		internal static int CompareMinimumEndPoint(RangeEndPoint first, RangeEndPoint second)
		{
			if (first == second)
				return 0;

			if (second == RangeEndPoint.Unbounded)
				return 1;

			return first == RangeEndPoint.Closed ? -1 : 1;
		}

		public static Range<T> Complement<T>(Range<T> first, Range<T> second) where T : IComparable<T>
		{
			return ComplementImpl(first, second);
		}

		private static Range<T> ComplementImpl<T>(Range<T> first, Range<T> second) where T : IComparable<T>
		{
			if (first.Equals(second))
				return Range<T>.Empty;

			SortEndPoints(first, second, out var leftMin, out _, out _, out var rightMax);

			if (leftMin.RangeIndex == rightMax.RangeIndex)
			{
				if (leftMin.RangeIndex == 1)
					return Range<T>.Empty;

				if (leftMin.RangeIndex == 0)
					throw FormatNonContiguousException(nameof(Complement));
			}

			return new Range<T>(leftMin.Value, leftMin.EndPoint, rightMax.Value, rightMax.EndPoint);
		}

		public static Range<T> Create<T>(T minimum, T maximum) where T : IComparable<T>
		{
			return new Range<T>(minimum, maximum);
		}

		internal static Range<T> CreateMaximumUnbounded<T>(T minimum, RangeEndPoint minimumPoint = RangeEndPoint.Closed) where T : IComparable<T>
		{
			return new Range<T>(minimum, minimumPoint, minimum, RangeEndPoint.Unbounded);
		}

		internal static Range<T> CreateMinimumUnbounded<T>(T maximum, RangeEndPoint maximumPoint = RangeEndPoint.Closed) where T : IComparable<T>
		{
			return new Range<T>(maximum, RangeEndPoint.Unbounded, maximum, maximumPoint);
		}

		private static IEnumerable<Range<T>> Except<T>(Range<T> first, Range<T> second) where T : IComparable<T>
		{
			throw new NotImplementedException();
		}

		private static Exception FormatNonContiguousException(string operation)
		{
			return new ArgumentOutOfRangeException($"{operation} operation produces 2 non contiguous ranges.", (Exception) null);
		}

		public static bool HasIntersection<T>(Range<T> first, Range<T> second) where T : IComparable<T>
		{
			SortEndPoints(first, second, out _, out var rightMin, out var leftMax, out _);

			var compareTo = rightMin.Value.CompareTo(leftMax.Value);

			if (compareTo > 0)
				return false;

			if (compareTo < 0)
				return true;

			return !(rightMin.EndPoint == RangeEndPoint.Open || leftMax.EndPoint == RangeEndPoint.Open);
		}

		public static Range<T> Intersect<T>(Range<T> first, Range<T> second) where T : IComparable<T>
		{
			return IntersectImpl(first, second);
		}

		private static Range<T> IntersectImpl<T>(Range<T> first, Range<T> second) where T : IComparable<T>
		{
			if (first.Equals(second))
				return first;

			SortEndPoints(first, second, out _, out var rightMin, out var leftMax, out _);

			return rightMin.Value.CompareTo(leftMax.Value) > 0 ? Range<T>.Empty : new Range<T>(rightMin.Value, rightMin.EndPoint, leftMax.Value, leftMax.EndPoint);
		}

		private static RangeEndPoint InverseEndPoint(RangeEndPoint endPoint)
		{
			switch (endPoint)
			{
				case RangeEndPoint.Open:
					return RangeEndPoint.Closed;

				case RangeEndPoint.Closed:
					return RangeEndPoint.Open;

				case RangeEndPoint.Unbounded:
					return RangeEndPoint.Unbounded;

				default:
					throw new ArgumentOutOfRangeException(nameof(endPoint));
			}
		}

		public static bool Overlaps<T>(Range<T> first, Range<T> second) where T : IComparable<T>
		{
			SortEndPoints(first, second, out _, out var rightMin, out var leftMax, out _);

			var compare = rightMin.Value.CompareTo(leftMax.Value);

			if (compare > 0)
				return false;

			if (compare < 0)
				return true;

			return rightMin.EndPoint == RangeEndPoint.Closed && leftMax.EndPoint == RangeEndPoint.Closed;
		}

		public static Range<T> Parse<T>(string value) where T : IComparable<T>
		{
			return Parse(value, StaticParser<T>.Default);
		}

		public static Range<T> Parse<T>(string value, IParser<T> valueParser) where T : IComparable<T>
		{
			if (TryParse(value, valueParser, out var result))
				return result;

			throw new Exception("Parse exception");
		}

		private static void SortEndPoints<T>(Range<T> first, Range<T> second, out RangeEndPointValue<T> leftMin, out RangeEndPointValue<T> rightMin, out RangeEndPointValue<T> leftMax, out RangeEndPointValue<T> rightMax) where T : IComparable<T>
		{
			var firstMinimumPoint = first.MinimumPoint;
			var secondMinimumPoint = second.MinimumPoint;

			// Rightmost minimum
			if (firstMinimumPoint == RangeEndPoint.Unbounded)
			{
				rightMin = new RangeEndPointValue<T>(second.Minimum, secondMinimumPoint, 1);
				leftMin = new RangeEndPointValue<T>(first.Minimum, firstMinimumPoint, 0);
			}
			else if (secondMinimumPoint == RangeEndPoint.Unbounded)
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
			if (firstMaximumPoint == RangeEndPoint.Unbounded)
			{
				leftMax = new RangeEndPointValue<T>(second.Maximum, secondMaximumPoint, 1);
				rightMax = new RangeEndPointValue<T>(first.Maximum, firstMaximumPoint, 0);
			}
			else if (secondMaximumPoint == RangeEndPoint.Unbounded)
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

		internal static IEnumerable<RangeItem<TRangeItem, T>> Split<TRangeItem, T>(RangeItem<TRangeItem, T> first, RangeItem<TRangeItem, T> second) where T : IComparable<T>
		{
			if (first.Range.Equals(second.Range))
			{
				yield return first;
				yield return second;
				yield break;
			}

			SortEndPoints(first.Range, second.Range, out var leftMin, out var rightMin, out var leftMax, out var rightMax);

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
					yield return new RangeItem<TRangeItem, T>(leftMinItem, new Range<T>(leftMin.Value, leftMin.EndPoint, rightMin.Value, InverseEndPoint(rightMin.EndPoint)));
					yield return new RangeItem<TRangeItem, T>(leftMinItem, new Range<T>(rightMin.Value, rightMin.EndPoint, leftMax.Value, leftMax.EndPoint));
					yield return new RangeItem<TRangeItem, T>(leftMinItem, new Range<T>(leftMax.Value, InverseEndPoint(leftMax.EndPoint), rightMax.Value, rightMax.EndPoint));

					// Inner
					yield return new RangeItem<TRangeItem, T>(rightMinItem, new Range<T>(rightMin.Value, rightMin.EndPoint, leftMax.Value, leftMax.EndPoint));
				}
				else
				{
					// Left
					yield return new RangeItem<TRangeItem, T>(leftMinItem, new Range<T>(leftMin.Value, leftMin.EndPoint, rightMin.Value, InverseEndPoint(rightMin.EndPoint)));
					yield return new RangeItem<TRangeItem, T>(leftMinItem, new Range<T>(rightMin.Value, rightMin.EndPoint, leftMax.Value, leftMax.EndPoint));

					// Right
					yield return new RangeItem<TRangeItem, T>(rightMinItem, new Range<T>(rightMin.Value, rightMin.EndPoint, leftMax.Value, leftMax.EndPoint));
					yield return new RangeItem<TRangeItem, T>(rightMinItem, new Range<T>(leftMax.Value, InverseEndPoint(leftMax.EndPoint), rightMax.Value, rightMax.EndPoint));
				}
			}
		}

		internal static List<RangeItem<TItem, T>> Split<TItem, T>(IEnumerable<RangeItem<TItem, T>> rangeItems) where T : IComparable<T>
		{
			// TODO Very naive implementation. Rework.
			var rangeItemPairs = rangeItems.ToList();
			var result = new List<RangeItem<TItem, T>>();
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
			var distinctSet = new HashSet<RangeItem<TItem, T>>();
			var distinctPoint = new HashSet<SplitPointData<T>>();

			foreach (var rangeItemPair in rangeItemPairs)
			{
				{
					var splitPointData = new SplitPointData<T>(rangeItemPair.Range.Minimum, RangeEndPoint.Open);

					if (distinctPoint.Add(splitPointData))
						splitPoints.Add(splitPointData);
				}

				{
					var splitPointData = new SplitPointData<T>(rangeItemPair.Range.Minimum, RangeEndPoint.Closed);

					if (distinctPoint.Add(splitPointData))
						splitPoints.Add(splitPointData);
				}

				{
					var splitPointData = new SplitPointData<T>(rangeItemPair.Range.Maximum, RangeEndPoint.Open);

					if (distinctPoint.Add(splitPointData))
						splitPoints.Add(splitPointData);
				}

				{
					var splitPointData = new SplitPointData<T>(rangeItemPair.Range.Maximum, RangeEndPoint.Closed);

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
				var startEndPoint = prevSame ? RangeEndPoint.Open : RangeEndPoint.Closed;
				var currentPoint = splitPointData.Point;
				var same = prevPoint.CompareTo(currentPoint) == 0;
				var range = same ? new Range<T>(prevPoint, currentPoint) : new Range<T>(prevPoint, startEndPoint, currentPoint, RangeEndPoint.Open);

				prevPoint = currentPoint;
				prevSame = same;

				var items = rangeItemPairs.Where(ri => ri.Range.Contains(range)).Select(ri => new RangeItem<TItem, T>(ri.Item, range));

				result.AddRange(items.Where(distinctSet.Add));
			}

			return result.OrderBy(r => sortDictionary[r.Item]).ThenBy(r => r.Range).ToList();
		}

		public static bool TryParse<T>(string value, IParser<T> valueParser, out Range<T> result) where T : IComparable<T>
		{
			var match = ParseRegex.Match(value);

			if (match.Success == false)
			{
				result = Range<T>.Empty;

				return false;
			}

			var minimumEndPoint = match.Groups[1].Value == "[" ? RangeEndPoint.Closed : RangeEndPoint.Open;
			var maximumEndPoint = match.Groups[4].Value == "]" ? RangeEndPoint.Closed : RangeEndPoint.Open;

			var minimum = default(T);
			var maximum = default(T);

			var minimumUnbounded = string.Equals(match.Groups[2].Value, "-INF", StringComparison.OrdinalIgnoreCase);
			var maximumUnbounded = string.Equals(match.Groups[3].Value, "+INF", StringComparison.OrdinalIgnoreCase);

			if ((minimumUnbounded || valueParser.TryParse(match.Groups[2].Value, out minimum)) && (maximumUnbounded || valueParser.TryParse(match.Groups[3].Value, out maximum)))
			{
				if (minimumUnbounded)
				{
					minimum = Range<T>.Universe.Minimum;
					minimumEndPoint = RangeEndPoint.Unbounded;
				}

				if (maximumUnbounded)
				{
					maximum = Range<T>.Universe.Maximum;
					maximumEndPoint = RangeEndPoint.Unbounded;
				}

				result = new Range<T>(minimum, minimumEndPoint, maximum, maximumEndPoint);

				return true;
			}

			result = Range<T>.Empty;

			return false;
		}

		public static bool TryParse<T>(string value, out Range<T> result) where T : IComparable<T>
		{
			return TryParse(value, StaticParser<T>.Default, out result);
		}

		public static Range<T> Union<T>(Range<T> first, Range<T> second) where T : IComparable<T>
		{
			return UnionImpl(first, second);
		}

		private static Range<T> UnionImpl<T>(Range<T> first, Range<T> second) where T : IComparable<T>
		{
			if (first.Equals(second))
				return first;

			SortEndPoints(first, second, out var leftMin, out var rightMin, out var leftMax, out var rightMax);

			var compare = rightMin.Value.CompareTo(leftMax.Value);

			if (compare > 0)
				throw FormatNonContiguousException("Union");

			if (compare == 0)
			{
				if (rightMin.EndPoint == RangeEndPoint.Open && leftMax.EndPoint == RangeEndPoint.Open)
					throw FormatNonContiguousException("Union");
			}

			return new Range<T>(leftMin.Value, leftMin.EndPoint, rightMax.Value, rightMax.EndPoint);
		}

		public static Range<T> WithClosedMaximum<T>(this Range<T> range) where T : IComparable<T>
		{
			return range.IsEmpty ? range : new Range<T>(range.Minimum, range.MinimumPoint, range.Maximum, RangeEndPoint.Closed);
		}

		public static Range<T> WithClosedMinimum<T>(this Range<T> range) where T : IComparable<T>
		{
			return range.IsEmpty ? range : new Range<T>(range.Minimum, RangeEndPoint.Closed, range.Maximum, range.MaximumPoint);
		}

		public static Range<T> WithOpenMaximum<T>(this Range<T> range) where T : IComparable<T>
		{
			return range.IsEmpty ? range : new Range<T>(range.Minimum, range.MinimumPoint, range.Maximum, RangeEndPoint.Open);
		}

		public static Range<T> WithOpenMinimum<T>(this Range<T> range) where T : IComparable<T>
		{
			return range.IsEmpty ? range : new Range<T>(range.Minimum, RangeEndPoint.Open, range.Maximum, range.MaximumPoint);
		}

		internal static Range<T> WithUnboundedMaximum<T>(this Range<T> range) where T : IComparable<T>
		{
			return range.IsEmpty ? range : new Range<T>(range.Minimum, range.MinimumPoint, Range<T>.Universe.Maximum, RangeEndPoint.Unbounded);
		}

		internal static Range<T> WithUnboundedMinimum<T>(this Range<T> range) where T : IComparable<T>
		{
			return range.IsEmpty ? range : new Range<T>(Range<T>.Universe.Minimum, RangeEndPoint.Unbounded, range.Maximum, range.MaximumPoint);
		}

		#endregion

		#region  Nested Types

		private struct SplitPointData<T>
		{
			public SplitPointData(T point, RangeEndPoint endPoint)
			{
				Point = point;
				EndPoint = endPoint;
			}

			public readonly T Point;
			public readonly RangeEndPoint EndPoint;

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

			private static int GetEndPointValue(RangeEndPoint endPoint)
			{
				switch (endPoint)
				{
					case RangeEndPoint.Closed:
						return 0;

					case RangeEndPoint.Open:
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

		private struct RangeEndPointValue<T> where T : IComparable<T>
		{
			public RangeEndPointValue(T value, RangeEndPoint endPoint, int rangeIndex)
			{
				Value = value;
				EndPoint = endPoint;
				RangeIndex = rangeIndex;
			}

			public readonly T Value;
			public readonly RangeEndPoint EndPoint;
			public readonly int RangeIndex;
		}

		#endregion
	}

	internal static class RangeItem
	{
		#region  Methods

		public static RangeItem<TItem, T> Create<TItem, T>(TItem item, Range<T> range) where T : IComparable<T>
		{
			return new RangeItem<TItem, T>(item, range);
		}

		#endregion
	}

	internal struct RangeItem<TItem, T> : IComparable<RangeItem<TItem, T>> where T : IComparable<T>
	{
		public RangeItem(TItem item, Range<T> range)
		{
			Item = item;
			Range = range;
		}

		public readonly TItem Item;

		public readonly Range<T> Range;

		public int CompareTo(RangeItem<TItem, T> other)
		{
			return Range.CompareTo(other.Range);
		}

		public override string ToString()
		{
			return $"{Item}:{Range}";
		}

		public bool Equals(RangeItem<TItem, T> other)
		{
			return EqualityComparer<TItem>.Default.Equals(Item, other.Item) && Range.Equals(other.Range);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
				return false;

			return obj is RangeItem<TItem, T> item && Equals(item);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (EqualityComparer<TItem>.Default.GetHashCode(Item) * 397) ^ Range.GetHashCode();
			}
		}
	}
}