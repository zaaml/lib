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

			Assert.True(r1.Contains(new Interval<int>(2, 8)));
			Assert.True(r1.Contains(new Interval<int>(2, IntervalEndPoint.Open, 8, IntervalEndPoint.Open)));
			Assert.True(r1.Contains(new Interval<int>(3, 7)));
			Assert.False(r1.Contains(new Interval<int>(3, 9)));
			Assert.False(r1.Contains(new Interval<int>(1, 7)));

			// Open bounds
			r1 = new Interval<int>(2, IntervalEndPoint.Open, 8, IntervalEndPoint.Open);

			Assert.False(r1.Contains(new Interval<int>(2, 8)));
			Assert.True(r1.Contains(new Interval<int>(2, IntervalEndPoint.Open, 8, IntervalEndPoint.Open)));
			Assert.True(r1.Contains(new Interval<int>(3, 7)));
			Assert.False(r1.Contains(new Interval<int>(3, 9)));
			Assert.False(r1.Contains(new Interval<int>(1, 7)));
		}

		[Test(Description = "TestContainsValue")]
		public void TestContainsValue()
		{
			// Closed bounds
			var r1 = new Interval<int>(2, 8);

			Assert.True(r1.Contains(2));
			Assert.True(r1.Contains(8));
			Assert.True(r1.Contains(3));
			Assert.True(r1.Contains(7));
			Assert.False(r1.Contains(1));
			Assert.False(r1.Contains(9));

			// Open bounds
			r1 = new Interval<int>(2, IntervalEndPoint.Open, 8, IntervalEndPoint.Open);

			Assert.False(r1.Contains(2));
			Assert.False(r1.Contains(8));

			Assert.True(r1.Contains(3));
			Assert.True(r1.Contains(7));
			Assert.False(r1.Contains(1));
			Assert.False(r1.Contains(9));

			// Unbounded Minimum
			r1 = new Interval<int>(2, IntervalEndPoint.Unbounded, 8, IntervalEndPoint.Closed);

			Assert.True(r1.Contains(3));
			Assert.True(r1.Contains(0));
			Assert.False(r1.Contains(9));

			// Unbounded Maximum
			r1 = new Interval<int>(2, IntervalEndPoint.Closed, 8, IntervalEndPoint.Unbounded);

			Assert.True(r1.Contains(3));
			Assert.False(r1.Contains(0));
			Assert.True(r1.Contains(9));

			// Unbounded
			r1 = new Interval<int>(2, IntervalEndPoint.Unbounded, 8, IntervalEndPoint.Unbounded);

			Assert.True(r1.Contains(2));
			Assert.True(r1.Contains(8));
			Assert.True(r1.Contains(3));
			Assert.True(r1.Contains(7));
			Assert.True(r1.Contains(1));
			Assert.True(r1.Contains(9));
		}

		[Test(Description = "TestFlags")]
		public void TestFlags()
		{
			var r1 = new Interval<int>(0, IntervalEndPoint.Open, 1, IntervalEndPoint.Open);

			Assert.AreEqual(IntervalEndPoint.Open, r1.MinimumPoint);
			Assert.AreEqual(IntervalEndPoint.Open, r1.MaximumPoint);
			Assert.False(r1.IsEmpty);

			var r2 = new Interval<int>(1, IntervalEndPoint.Closed, 0, IntervalEndPoint.Closed);

			Assert.AreEqual(IntervalEndPoint.Closed, r2.MinimumPoint);
			Assert.AreEqual(IntervalEndPoint.Closed, r2.MaximumPoint);
			Assert.True(r2.IsEmpty);

			var r3 = new Interval<int>(0, IntervalEndPoint.Unbounded, 1, IntervalEndPoint.Unbounded);

			Assert.AreEqual(IntervalEndPoint.Unbounded, r3.MinimumPoint);
			Assert.AreEqual(IntervalEndPoint.Unbounded, r3.MaximumPoint);
		}

		[Test(Description = "TestIntersection")]
		public void TestIntersection()
		{
			var r1 = new Interval<int>(2, 8);
			var r2 = new Interval<int>(4, 10);

			var i = Interval.Intersect(r1, r2);

			Assert.AreEqual("[4;8]", i.ToString());

			i = Interval.Intersect(r2, r1);

			Assert.AreEqual("[4;8]", i.ToString());

			r2 = new Interval<int>(2, 8);
			i = Interval.Intersect(r1, r2);

			Assert.AreEqual("[2;8]", i.ToString());

			r1 = new Interval<int>(2, IntervalEndPoint.Open, 8, IntervalEndPoint.Open);
			i = Interval.Intersect(r1, r2);

			Assert.AreEqual("(2;8)", i.ToString());

			r1 = new Interval<int>(1, IntervalEndPoint.Closed, 9, IntervalEndPoint.Closed);
			i = Interval.Intersect(r1, r2);

			Assert.AreEqual("[2;8]", i.ToString());

			r2 = new Interval<int>(2, IntervalEndPoint.Open, 8, IntervalEndPoint.Open);
			i = Interval.Intersect(r1, r2);

			Assert.AreEqual("(2;8)", i.ToString());

			r1 = new Interval<int>(2, 8);
			r2 = new Interval<int>(8, 10);

			i = Interval.Intersect(r1, r2);

			Assert.AreEqual("[8;8]", i.ToString());

			r1 = new Interval<int>(2, IntervalEndPoint.Closed, 8, IntervalEndPoint.Open);
			i = Interval.Intersect(r1, r2);

			Assert.AreEqual("[8;8)", i.ToString());
			Assert.True(i.IsEmpty);
			Assert.False(i.Contains(8));

			r1 = Interval.CreateMinimumUnbounded(8);
			r2 = Interval.CreateMaximumUnbounded(2);
			i = Interval.Intersect(r1, r2);

			Assert.AreEqual("[2;8]", i.ToString());
		}

		[Test(Description = "TestUnion")]
		public void TestUnion()
		{
			Assert.AreEqual("[2;10]", Union("[2;8]", "[8;10]").ToString());
			Assert.AreEqual("(2;10]", Union("(2;8)", "[8;10]").ToString());
			Assert.AreEqual("[2;10)", Union("[2;8]", "(8;10)").ToString());

			Assert.Catch<ArgumentOutOfRangeException>(() => Union("[2;8)", "(8;10]"));
		}

		[Test(Description = "TestOpenClose")]
		public void TestOpenClose()
		{
			Assert.AreEqual("(3;4]", OpenMinimum("[2;4]").ToString());
			Assert.AreEqual("[2;5)", OpenMaximum("[2;4]").ToString());
			Assert.AreEqual("[2;4]", CloseMinimum("(3;4]").ToString());
			Assert.AreEqual("[2;4]", CloseMaximum("[2;5)").ToString());
		}

		[Test(Description = "TestNormalize")]
		public void TestNormalize()
		{
			Assert.AreEqual("(3;4]", Normalize("[2;4]", IntervalEndPoint.Open, IntervalEndPoint.Closed).ToString());
			Assert.AreEqual("[2;5)", Normalize("[2;4]", IntervalEndPoint.Closed, IntervalEndPoint.Open).ToString());
      Assert.AreEqual("[2;4]", Normalize("(3;4]", IntervalEndPoint.Closed, IntervalEndPoint.Closed).ToString());
      Assert.AreEqual("[2;4]", Normalize("[2;5)", IntervalEndPoint.Closed , IntervalEndPoint.Closed).ToString());
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
			Assert.AreEqual("[0;5)", Except("[0;10]", "[5;10]").ToString());
			Assert.AreEqual("[0;5)", Except("[5;10]", "[0;10]").ToString());

      Assert.AreEqual("(5;10]", Except("[0;10]", "[0;5]").ToString());
      Assert.AreEqual("(5;10]", Except("[0;5]", "[0;10]").ToString());
		}

		[Test(Description = "TestHasIntersection")]
		public void TestHasIntersection()
		{
			var r1 = new Interval<int>(2, 8);
			var r2 = new Interval<int>(8, 10);

			Assert.True(Interval.HasIntersection(r1, r2));

			r2 = new Interval<int>(8, IntervalEndPoint.Open, 10, IntervalEndPoint.Closed );

			Assert.False(Interval.HasIntersection(r1, r2));

			r1 = Interval.CreateMaximumUnbounded(2);
			r2 = Interval.CreateMinimumUnbounded(10);

			Assert.True(Interval.HasIntersection(r1, r2));
		}

		[Test(Description = "TestSplitInterval")]
		public void TestSplitInterval()
		{
			var a = IntervalItem.Create("a", new Interval<int>(2, 8));
			var b = IntervalItem.Create("b", new Interval<int>(4, 6));

			var split = Interval.Split(a,b).ToArray();
			Assert.AreEqual("a:[2;4),a:[4;6],a:(6;8],b:[4;6]", string.Join(",", split));

			split = Interval.Split(b, a).ToArray();
			Assert.AreEqual("a:[2;4),a:[4;6],a:(6;8],b:[4;6]", string.Join(",", split));

			a = IntervalItem.Create("a", new Interval<int>(2, 8));
			b = IntervalItem.Create("b", new Interval<int>(4, 10));

			split = Interval.Split(a, b).ToArray();
			Assert.AreEqual("a:[2;4),a:[4;8],b:[4;8],b:(8;10]", string.Join(",", split));

			split = Interval.Split(b, a).ToArray();
			Assert.AreEqual("a:[2;4),a:[4;8],b:[4;8],b:(8;10]", string.Join(",", split));

			a = IntervalItem.Create("a", new Interval<int>(2, 4));
			b = IntervalItem.Create("b", new Interval<int>(6, 8));
			split = Interval.Split(b, a).ToArray();
			Assert.AreEqual("b:[6;8],a:[2;4]", string.Join(",", split));
		}

		#endregion
	}
}