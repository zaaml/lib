// <copyright file="UnicodeCategoryRanges.ParagraphSeparator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Range = Zaaml.Core.Range<int>;

namespace Zaaml.Text.Unicode
{
	internal static partial class UnicodeCategoryRanges
	{
		public static Range[] ParagraphSeparator => ParagraphSeparatorRange.Range;

		private static class ParagraphSeparatorRange
		{
			public static readonly Range[] Range;

			static ParagraphSeparatorRange()
			{
				Range = BuildRange(
					0x00002029, 0x00002029);
			}
		}
	}
}