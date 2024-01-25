// <copyright file="UnicodeCategoryRanges.LetterNumber.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Range = Zaaml.Core.Range<int>;

namespace Zaaml.Text.Unicode
{
	internal static partial class UnicodeCategoryRanges
	{
		public static Range[] LetterNumber => LetterNumberRange.Range;

		private static class LetterNumberRange
		{
			public static readonly Range[] Range;

			static LetterNumberRange()
			{
				Range = BuildRange(
					0x000016ee, 0x000016f0, 0x00002160, 0x00002182, 0x00002185, 0x00002188, 0x00003007, 0x00003007, 0x00003021, 0x00003029, 0x00003038, 0x0000303a, 0x0000a6e6, 0x0000a6ef, 0x00010140, 0x00010174, 0x00010341, 0x00010341,
					0x0001034a, 0x0001034a, 0x000103d1, 0x000103d5, 0x00012400, 0x0001246e);
			}
		}
	}
}