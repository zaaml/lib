// <copyright file="QuantifierHelper.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using Zaaml.Core;

namespace Zaaml.Text
{
	internal static class QuantifierHelper
	{
		#region Static Fields and Constants

		private static readonly Interval<int> OneOrMoreRange = new Interval<int>(1, IntervalEndPoint.Closed, int.MaxValue, IntervalEndPoint.Unbounded);
		private static readonly Interval<int> ZeroOrMoreRange = new Interval<int>(0, IntervalEndPoint.Closed, int.MaxValue, IntervalEndPoint.Unbounded);
		private static readonly Interval<int> ZeroOrOneRange = new Interval<int>(0, 1);

		#endregion

		#region Methods

		public static Interval<int> AtLeast(int count)
		{
			return new Interval<int>(count, IntervalEndPoint.Closed, int.MaxValue, IntervalEndPoint.Unbounded);
		}

		public static Interval<int> Between(int from, int to)
		{
			return new Interval<int>(from, to);
		}

		public static bool CanCollapse(QuantifierKind inner, QuantifierKind outer)
		{
			return inner != QuantifierKind.Generic && outer != QuantifierKind.Generic;
		}

		public static QuantifierKind Collapse(QuantifierKind inner, QuantifierKind outer)
		{
			return (inner, outer) switch
			{
				(QuantifierKind.ZeroOrOne, QuantifierKind.ZeroOrOne) => QuantifierKind.ZeroOrOne,
				(QuantifierKind.ZeroOrOne, QuantifierKind.OneOrMore) => QuantifierKind.ZeroOrMore,
				(QuantifierKind.ZeroOrOne, QuantifierKind.ZeroOrMore) => QuantifierKind.ZeroOrMore,

				(QuantifierKind.OneOrMore, QuantifierKind.ZeroOrOne) => QuantifierKind.ZeroOrMore,
				(QuantifierKind.OneOrMore, QuantifierKind.OneOrMore) => QuantifierKind.OneOrMore,
				(QuantifierKind.OneOrMore, QuantifierKind.ZeroOrMore) => QuantifierKind.ZeroOrMore,

				(QuantifierKind.ZeroOrMore, QuantifierKind.ZeroOrOne) => QuantifierKind.ZeroOrMore,
				(QuantifierKind.ZeroOrMore, QuantifierKind.OneOrMore) => QuantifierKind.ZeroOrMore,
				(QuantifierKind.ZeroOrMore, QuantifierKind.ZeroOrMore) => QuantifierKind.ZeroOrMore,

				_ => throw new InvalidOperationException()
			};
		}

		public static Interval<int> Exact(int count)
		{
			return new Interval<int>(count, count);
		}

		public static QuantifierKind GetKind(Interval<int> range)
		{
			if (range.Equals(OneOrMoreRange))
				return QuantifierKind.OneOrMore;

			if (range.Equals(ZeroOrMoreRange))
				return QuantifierKind.ZeroOrMore;

			if (range.Equals(ZeroOrOneRange))
				return QuantifierKind.ZeroOrOne;

			return QuantifierKind.Generic;
		}

		public static Interval<int> GetRange(QuantifierKind kind)
		{
			switch (kind)
			{
				case QuantifierKind.ZeroOrOne:
					return ZeroOrOneRange;
				case QuantifierKind.ZeroOrMore:
					return ZeroOrMoreRange;
				case QuantifierKind.OneOrMore:
					return OneOrMoreRange;
				default:
					throw new ArgumentOutOfRangeException(nameof(kind));
			}
		}

		#endregion
	}
}