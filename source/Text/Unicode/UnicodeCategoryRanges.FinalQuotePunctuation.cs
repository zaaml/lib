// <copyright file="UnicodeCategoryRanges.FinalQuotePunctuation.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Range = Zaaml.Core.Range<int>;

namespace Zaaml.Text.Unicode
{
	internal static partial class UnicodeCategoryRanges
	{
		public static Range[] FinalQuotePunctuation => FinalQuotePunctuationRange.Range;

		private static class FinalQuotePunctuationRange
		{
			public static readonly Range[] Range;

			static FinalQuotePunctuationRange()
			{
				Range = BuildRange(
					0x000000bb, 0x000000bb, 0x00002019, 0x00002019, 0x0000201d, 0x0000201d, 0x0000203a, 0x0000203a, 0x00002e03, 0x00002e03, 0x00002e05, 0x00002e05, 0x00002e0a, 0x00002e0a, 0x00002e0d, 0x00002e0d, 0x00002e1d, 0x00002e1d,
					0x00002e21, 0x00002e21);
			}
		}
	}
}