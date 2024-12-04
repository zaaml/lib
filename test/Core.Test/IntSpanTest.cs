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
			Assert.That(StrRemoveSpan("[10;20)", "[0;5)"), Is.EqualTo("[5;15)"));
			Assert.That(StrRemoveSpan("[10;20)", "[25;30)"), Is.EqualTo("[10;20)"));
			Assert.That(StrRemoveSpan("[10;20)", "[10;20)"), Is.EqualTo("(Empty)"));
			Assert.That(StrRemoveSpan("[10;20)", "[0;30)"), Is.EqualTo("(Empty)"));
			
			Assert.That(StrRemoveSpan("[10;25)", "[15;20)"), Is.EqualTo("[10;20)"));
			Assert.That(StrRemoveSpan("[10;25)", "[15;30)"), Is.EqualTo("[10;15)"));
			Assert.That(StrRemoveSpan("[10;25)", "[5;15)"), Is.EqualTo("[5;15)"));
			
			Assert.That(StrRemoveSpan("[1;2)", "[0;1)"), Is.EqualTo("[0;1)"));
		}

		[Test(Description = "TestInsertSpan")]
		public void TestInsertSpan()
		{
			var r = StrInsertSpan("[10;20)", "[9;14)");

			Assert.That(StrInsertSpan("[10;20)", "[0;5)"), Is.EqualTo("[15;25)"));
			Assert.That(StrInsertSpan("[10;20)", "[20;25)"), Is.EqualTo("[10;20)"));
			Assert.That(StrInsertSpan("[10;20)", "[19;24)"), Is.EqualTo("[10;25)"));
			Assert.That(StrInsertSpan("[10;20)", "[9;14)"), Is.EqualTo("[15;25)"));
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