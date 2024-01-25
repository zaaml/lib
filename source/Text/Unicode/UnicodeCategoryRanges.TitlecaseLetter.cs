// <copyright file="UnicodeCategoryRanges.TitlecaseLetter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Range = Zaaml.Core.Range<int>;

namespace Zaaml.Text.Unicode
{
	internal static partial class UnicodeCategoryRanges
	{
		public static Range[] TitlecaseLetter => TitlecaseLetterRange.Range;

		private static class TitlecaseLetterRange
		{
			public static readonly Range[] Range;

			static TitlecaseLetterRange()
			{
				Range = BuildRange(
					0x000001c5, 0x000001c5, 0x000001c8, 0x000001c8, 0x000001cb, 0x000001cb, 0x000001f2, 0x000001f2, 0x00001f88, 0x00001f8f, 0x00001f98, 0x00001f9f, 0x00001fa8, 0x00001faf, 0x00001fbc, 0x00001fbc, 0x00001fcc, 0x00001fcc,
					0x00001ffc, 0x00001ffc);
			}
		}
	}
}