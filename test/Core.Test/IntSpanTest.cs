// <copyright file="IntSpanTest.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using NUnit.Framework;

namespace Zaaml.Core.Test
{
	[TestFixture]
	internal class IntSpanTest
	{
		private static string StrRemoveSpan(string s1, string s2)
		{
			var r = s1.ParseIntSpan().Remove(s2.ParseIntSpan());

			return r.ToString();
		}

		private static string StrInsertSpan(string s1, string s2)
		{
			var r = s1.ParseIntSpan().Insert(s2.ParseIntSpan());

			return r.ToString();
		}

		[Test(Description = "TestRemoveSpan")]
		public void TestRemoveSpan()
		{
			Assert.AreEqual("[5;15)", StrRemoveSpan("[10;20)", "[0;5)"));
			Assert.AreEqual("[10;20)", StrRemoveSpan("[10;20)", "[25;30)"));
			Assert.AreEqual("(Empty)", StrRemoveSpan("[10;20)", "[10;20)"));
			Assert.AreEqual("(Empty)", StrRemoveSpan("[10;20)", "[0;30)"));
			
			Assert.AreEqual("[10;20)", StrRemoveSpan("[10;25)", "[15;20)"));
			Assert.AreEqual("[10;15)", StrRemoveSpan("[10;25)", "[15;30)"));
			Assert.AreEqual("[5;15)", StrRemoveSpan("[10;25)", "[5;15)"));
			
			Assert.AreEqual("[0;1)", StrRemoveSpan("[1;2)", "[0;1)"));
		}

		[Test(Description = "TestInsertSpan")]
		public void TestInsertSpan()
		{
			var r = StrInsertSpan("[10;20)", "[9;14)");

			Assert.AreEqual("[15;25)", StrInsertSpan("[10;20)", "[0;5)"));
			Assert.AreEqual("[10;20)", StrInsertSpan("[10;20)", "[20;25)"));
			Assert.AreEqual("[10;25)", StrInsertSpan("[10;20)", "[19;24)"));
			Assert.AreEqual("[15;25)", StrInsertSpan("[10;20)", "[9;14)"));
		}
	}

	internal static class IntSpanStringExtensions
	{
		public static IntSpan ParseIntSpan(this string str)
		{
			var interval = Interval.Parse<int>(str).Normalize(IntervalEndPoint.Closed, IntervalEndPoint.Closed);

			return new IntSpan(interval.Minimum, interval.Maximum - interval.Minimum + 1);
		}
	}
}