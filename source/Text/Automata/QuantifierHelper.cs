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

		private static readonly Range<int> OneOrMoreRange = new Range<int>(1, RangeEndPoint.Closed, int.MaxValue, RangeEndPoint.Unbounded);
		private static readonly Range<int> ZeroOrMoreRange = new Range<int>(0, RangeEndPoint.Closed, int.MaxValue, RangeEndPoint.Unbounded);
		private static readonly Range<int> ZeroOrOneRange = new Range<int>(0, 1);

		#endregion

		#region Methods

		public static Range<int> AtLeast(int count)
		{
			return new Range<int>(count, RangeEndPoint.Closed, int.MaxValue, RangeEndPoint.Unbounded);
		}

		public static Range<int> Between(int from, int to)
		{
			return new Range<int>(from, to);
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

		public static Range<int> Exact(int count)
		{
			return new Range<int>(count, count);
		}

		public static QuantifierKind GetKind(Range<int> range)
		{
			if (range.Equals(OneOrMoreRange))
				return QuantifierKind.OneOrMore;

			if (range.Equals(ZeroOrMoreRange))
				return QuantifierKind.ZeroOrMore;

			if (range.Equals(ZeroOrOneRange))
				return QuantifierKind.ZeroOrOne;

			return QuantifierKind.Generic;
		}

		public static Range<int> GetRange(QuantifierKind kind)
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