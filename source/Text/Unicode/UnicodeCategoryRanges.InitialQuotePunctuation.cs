// <copyright file="UnicodeCategoryRanges.InitialQuotePunctuation.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Range = Zaaml.Core.Range<int>;

namespace Zaaml.Text.Unicode
{
	internal static partial class UnicodeCategoryRanges
	{
		public static Range[] InitialQuotePunctuation => InitialQuotePunctuationRange.Range;

		private static class InitialQuotePunctuationRange
		{
			public static readonly Range[] Range;

			static InitialQuotePunctuationRange()
			{
				Range = BuildRange(
					0x000000ab, 0x000000ab, 0x00002018, 0x00002018, 0x0000201b, 0x0000201c, 0x0000201f, 0x0000201f, 0x00002039, 0x00002039, 0x00002e02, 0x00002e02, 0x00002e04, 0x00002e04, 0x00002e09, 0x00002e09, 0x00002e0c, 0x00002e0c,
					0x00002e1c, 0x00002e1c, 0x00002e20, 0x00002e20);
			}
		}
	}
}