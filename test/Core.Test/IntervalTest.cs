// <copyright file="IntervalTest.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Linq;
using NUnit.Framework;

namespace Zaaml.Core.Test
{
	[TestFixture]
	public class IntervalTest
	{
		#region  Methods

		[Test(Description = "TestContainsInterval")]
		public void TestContainsInterval()
		{
			// Closed bounds
			var r1 = new Interval<int>(2, 8);

			Assert.That(r1.Contains(new Interval<int>(2, 8)), Is.True);
			Assert.That(r1.Contains(new Interval<int>(2, IntervalEndPoint.Open, 8, IntervalEndPoint.Open)), Is.True);
			Assert.That(r1.Contains(new Interval<int>(3, 7)), Is.True);
			Assert.That(r1.Contains(new Interval<int>(3, 9)), Is.False);
			Assert.That(r1.Contains(new Interval<int>(1, 7)), Is.False);

			// Open bounds
			r1 = new Interval<int>(2, IntervalEndPoint.Open, 8, IntervalEndPoint.Open);

			Assert.That(r1.Contains(new Interval<int>(2, 8)), Is.False);
			Assert.That(r1.Contains(new Interval<int>(2, IntervalEndPoint.Open, 8, IntervalEndPoint.Open)), Is.True);
			Assert.That(r1.Contains(new Interval<int>(3, 7)), Is.True);
			Assert.That(r1.Contains(new Interval<int>(3, 9)), Is.False);
			Assert.That(r1.Contains(new Interval<int>(1, 7)), Is.False);
		}

		[Test(Description = "TestContainsValue")]
		public void TestContainsValue()
		{
			// Closed bounds
			var r1 = new Interval<int>(2, 8);

			Assert.That(r1.Contains(2), Is.True);
			Assert.That(r1.Contains(8), Is.True);
			Assert.That(r1.Contains(3), Is.True);
			Assert.That(r1.Contains(7), Is.True);
			Assert.That(r1.Contains(1), Is.False);
			Assert.That(r1.Contains(9), Is.False);

			// Open bounds
			r1 = new Interval<int>(2, IntervalEndPoint.Open, 8, IntervalEndPoint.Open);

			Assert.That(r1.Contains(2), Is.False);
			Assert.That(r1.Contains(8), Is.False);

			Assert.That(r1.Contains(3), Is.True);
			Assert.That(r1.Contains(7), Is.True);
			Assert.That(r1.Contains(1), Is.False);
			Assert.That(r1.Contains(9), Is.False);

			// Unbounded Minimum
			r1 = new Interval<int>(2, IntervalEndPoint.Unbounded, 8, IntervalEndPoint.Closed);

			Assert.That(r1.Contains(3), Is.True);
			Assert.That(r1.Contains(0), Is.True);
			Assert.That(r1.Contains(9), Is.False);

			// Unbounded Maximum
			r1 = new Interval<int>(2, IntervalEndPoint.Closed, 8, IntervalEndPoint.Unbounded);

			Assert.That(r1.Contains(3), Is.True);
			Assert.That(r1.Contains(0), Is.False);
			Assert.That(r1.Contains(9), Is.True);

			// Unbounded
			r1 = new Interval<int>(2, IntervalEndPoint.Unbounded, 8, IntervalEndPoint.Unbounded);

			Assert.That(r1.Contains(2), Is.True);
			Assert.That(r1.Contains(8), Is.True);
			Assert.That(r1.Contains(3), Is.True);
			Assert.That(r1.Contains(7), Is.True);
			Assert.That(r1.Contains(1), Is.True);
			Assert.That(r1.Contains(9), Is.True);
		}

		[Test(Description = "TestFlags")]
		public void TestFlags()
		{
			var r1 = new Interval<int>(0, IntervalEndPoint.Open, 1, IntervalEndPoint.Open);

			Assert.That(r1.MinimumPoint, Is.EqualTo(IntervalEndPoint.Open));
			Assert.That(r1.MaximumPoint, Is.EqualTo(IntervalEndPoint.Open));
			Assert.That(r1.IsEmpty, Is.False);

			var r2 = new Interval<int>(1, IntervalEndPoint.Closed, 0, IntervalEndPoint.Closed);

			Assert.That(r2.MinimumPoint, Is.EqualTo(IntervalEndPoint.Closed));
			Assert.That(r2.MaximumPoint, Is.EqualTo(IntervalEndPoint.Closed));
			Assert.That(r2.IsEmpty, Is.True);

			var r3 = new Interval<int>(0, IntervalEndPoint.Unbounded, 1, IntervalEndPoint.Unbounded);

			Assert.That(r3.MinimumPoint, Is.EqualTo(IntervalEndPoint.Unbounded));
			Assert.That(r3.MaximumPoint, Is.EqualTo(IntervalEndPoint.Unbounded));
		}

		[Test(Description = "TestIntersection")]
		public void TestIntersection()
		{
			var r1 = new Interval<int>(2, 8);
			var r2 = new Interval<int>(4, 10);

			var i = Interval.Intersect(r1, r2);

			Assert.That(i.ToString(), Is.EqualTo("[4;8]"));

			i = Interval.Intersect(r2, r1);

			Assert.That(i.ToString(), Is.EqualTo("[4;8]"));

			r2 = new Interval<int>(2, 8);
			i = Interval.Intersect(r1, r2);

			Assert.That(i.ToString(), Is.EqualTo("[2;8]"));

			r1 = new Interval<int>(2, IntervalEndPoint.Open, 8, IntervalEndPoint.Open);
			i = Interval.Intersect(r1, r2);

			Assert.That(i.ToString(), Is.EqualTo("(2;8)"));

			r1 = new Interval<int>(1, IntervalEndPoint.Closed, 9, IntervalEndPoint.Closed);
			i = Interval.Intersect(r1, r2);

			Assert.That(i.ToString(), Is.EqualTo("[2;8]"));

			r2 = new Interval<int>(2, IntervalEndPoint.Open, 8, IntervalEndPoint.Open);
			i = Interval.Intersect(r1, r2);

			Assert.That(i.ToString(), Is.EqualTo("(2;8)"));

			r1 = new Interval<int>(2, 8);
			r2 = new Interval<int>(8, 10);

			i = Interval.Intersect(r1, r2);

			Assert.That(i.ToString(), Is.EqualTo("[8;8]"));

			r1 = new Interval<int>(2, IntervalEndPoint.Closed, 8, IntervalEndPoint.Open);
			i = Interval.Intersect(r1, r2);

			Assert.That(i.ToString(), Is.EqualTo("[8;8)"));
			Assert.That(i.IsEmpty, Is.True);
			Assert.That(i.Contains(8), Is.False);

			r1 = Interval.CreateMinimumUnbounded(8);
			r2 = Interval.CreateMaximumUnbounded(2);
			i = Interval.Intersect(r1, r2);

			Assert.That(i.ToString(), Is.EqualTo("[2;8]"));
		}

		[Test(Description = "TestUnion")]
		public void TestUnion()
		{
			Assert.That(Union("[2;8]", "[8;10]").ToString(), Is.EqualTo("[2;10]"));
			Assert.That(Union("(2;8)", "[8;10]").ToString(), Is.EqualTo("(2;10]"));
			Assert.That(Union("[2;8]", "(8;10)").ToString(), Is.EqualTo("[2;10)"));

			Assert.Catch<ArgumentOutOfRangeException>(() => Union("[2;8)", "(8;10]"));
		}

		[Test(Description = "TestOpenClose")]
		public void TestOpenClose()
		{
			Assert.That(OpenMinimum("[2;4]").ToString(), Is.EqualTo("(3;4]"));
			Assert.That(OpenMaximum("[2;4]").ToString(), Is.EqualTo("[2;5)"));
			Assert.That(CloseMinimum("(3;4]").ToString(), Is.EqualTo("[2;4]"));
			Assert.That(CloseMaximum("[2;5)").ToString(), Is.EqualTo("[2;4]"));
		}

		[Test(Description = "TestNormalize")]
		public void TestNormalize()
		{
			Assert.That(Normalize("[2;4]", IntervalEndPoint.Open, IntervalEndPoint.Closed).ToString(), Is.EqualTo("(3;4]"));
			Assert.That(Normalize("[2;4]", IntervalEndPoint.Closed, IntervalEndPoint.Open).ToString(), Is.EqualTo("[2;5)"));
      Assert.That(Normalize("(3;4]", IntervalEndPoint.Closed, IntervalEndPoint.Closed).ToString(), Is.EqualTo("[2;4]"));
      Assert.That(Normalize("[2;5)", IntervalEndPoint.Closed , IntervalEndPoint.Closed).ToString(), Is.EqualTo("[2;4]"));
		}

    private static Interval<int> Normalize(string interval, IntervalEndPoint minimumPoint, IntervalEndPoint maximumPoint)
    {
      return Interval.Parse<int>(interval).Normalize(minimumPoint, maximumPoint);
		}

    private static Interval<int> OpenMinimum(string interval)
    {
      return Interval.Parse<int>(interval).OpenMinimum(true);
    }

    private static Interval<int> OpenMaximum(string interval)
    {
      return Interval.Parse<int>(interval).OpenMaximum(true);
    }

    private static Interval<int> CloseMinimum(string interval)
    {
      return Interval.Parse<int>(interval).CloseMinimum(true);
    }

    private static Interval<int> CloseMaximum(string interval)
    {
      return Interval.Parse<int>(interval).CloseMaximum(true);
    }

		private static Interval<int> Union(string left, string right)
		{
			return Interval.Parse<int>(left).UnionWith(Interval.Parse<int>(right));
		}

		private static Interval<int> Intersect(string left, string right)
		{
			return Interval.Parse<int>(left).IntersectWith(Interval.Parse<int>(right));
		}

		//private static Interval<int> Complement(string left, string right)
		//{
		//	return Interval.Parse<int>(left).ComplementWith(Interval.Parse<int>(right));
		//}

		private static Interval<int> Except(string left, string right)
		{
			return Interval.Parse<int>(left).ExceptWith(Interval.Parse<int>(right));
		}

		//[Test(Description = "TestComplement")]
		//public void TestComplement()
		//{
		//	Assert.Catch<ArgumentOutOfIntervalException>(() => Complement("[2;8]", "[3;7]"));
		//	Assert.True(Complement("[3;7]", "[2;8]").IsEmpty);
		//}

		[Test(Description = "TestExcept")]
		public void TestExcept()
		{
			Assert.That(Except("[0;10]", "[5;10]").ToString(), Is.EqualTo("[0;5)"));
			Assert.That(Except("[5;10]", "[0;10]").ToString(), Is.EqualTo("[0;5)"));

      Assert.That(Except("[0;10]", "[0;5]").ToString(), Is.EqualTo("(5;10]"));
      Assert.That(Except("[0;5]", "[0;10]").ToString(), Is.EqualTo("(5;10]"));
		}

		[Test(Description = "TestHasIntersection")]
		public void TestHasIntersection()
		{
			var r1 = new Interval<int>(2, 8);
			var r2 = new Interval<int>(8, 10);

			Assert.That(Interval.HasIntersection(r1, r2), Is.True);

			r2 = new Interval<int>(8, IntervalEndPoint.Open, 10, IntervalEndPoint.Closed );

			Assert.That(Interval.HasIntersection(r1, r2), Is.False);

			r1 = Interval.CreateMaximumUnbounded(2);
			r2 = Interval.CreateMinimumUnbounded(10);

			Assert.That(Interval.HasIntersection(r1, r2), Is.True);
		}

		[Test(Description = "TestSplitInterval")]
		public void TestSplitInterval()
		{
			var a = IntervalItem.Create("a", new Interval<int>(2, 8));
			var b = IntervalItem.Create("b", new Interval<int>(4, 6));

			var split = Interval.Split(a,b).ToArray();
			Assert.That(string.Join(",", split), Is.EqualTo("a:[2;4),a:[4;6],a:(6;8],b:[4;6]"));

			split = Interval.Split(b, a).ToArray();
			Assert.That(string.Join(",", split), Is.EqualTo("a:[2;4),a:[4;6],a:(6;8],b:[4;6]"));

			a = IntervalItem.Create("a", new Interval<int>(2, 8));
			b = IntervalItem.Create("b", new Interval<int>(4, 10));

			split = Interval.Split(a, b).ToArray();
			Assert.That(string.Join(",", split), Is.EqualTo("a:[2;4),a:[4;8],b:[4;8],b:(8;10]"));

			split = Interval.Split(b, a).ToArray();
			Assert.That(string.Join(",", split), Is.EqualTo("a:[2;4),a:[4;8],b:[4;8],b:(8;10]"));

			a = IntervalItem.Create("a", new Interval<int>(2, 4));
			b = IntervalItem.Create("b", new Interval<int>(6, 8));
			split = Interval.Split(b, a).ToArray();
			Assert.That(string.Join(",", split), Is.EqualTo("b:[6;8],a:[2;4]"));
		}

		#endregion
	}
}