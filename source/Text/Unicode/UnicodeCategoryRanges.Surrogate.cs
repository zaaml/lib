// <copyright file="UnicodeCategoryRanges.Surrogate.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Range = Zaaml.Core.Range<int>;

namespace Zaaml.Text.Unicode
{
	internal static partial class UnicodeCategoryRanges
	{
		public static Range[] Surrogate => SurrogateRange.Range;

		private static class SurrogateRange
		{
			public static readonly Range[] Range;

			static SurrogateRange()
			{
				Range = BuildRange(
				);
			}
		}
	}
}