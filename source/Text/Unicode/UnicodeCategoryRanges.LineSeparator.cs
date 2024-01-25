// <copyright file="UnicodeCategoryRanges.LineSeparator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Range = Zaaml.Core.Range<int>;

namespace Zaaml.Text.Unicode
{
	internal static partial class UnicodeCategoryRanges
	{
		public static Range[] LineSeparator => LineSeparatorRange.Range;

		private static class LineSeparatorRange
		{
			public static readonly Range[] Range;

			static LineSeparatorRange()
			{
				Range = BuildRange(
					0x00002028, 0x00002028);
			}
		}
	}
}