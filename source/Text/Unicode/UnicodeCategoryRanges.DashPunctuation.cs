// <copyright file="UnicodeCategoryRanges.DashPunctuation.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Range = Zaaml.Core.Range<int>;

namespace Zaaml.Text.Unicode
{
	internal static partial class UnicodeCategoryRanges
	{
		public static Range[] DashPunctuation => DashPunctuationRange.Range;

		private static class DashPunctuationRange
		{
			public static readonly Range[] Range;

			static DashPunctuationRange()
			{
				Range = BuildRange(
					0x0000002d, 0x0000002d, 0x0000058a, 0x0000058a, 0x000005be, 0x000005be, 0x00001400, 0x00001400, 0x00001806, 0x00001806, 0x00002010, 0x00002015, 0x00002e17, 0x00002e17, 0x00002e1a, 0x00002e1a, 0x00002e3a, 0x00002e3b,
					0x00002e40, 0x00002e40, 0x00002e5d, 0x00002e5d, 0x0000301c, 0x0000301c, 0x00003030, 0x00003030, 0x000030a0, 0x000030a0, 0x0000fe31, 0x0000fe32, 0x0000fe58, 0x0000fe58, 0x0000fe63, 0x0000fe63, 0x0000ff0d, 0x0000ff0d,
					0x00010ead, 0x00010ead);
			}
		}
	}
}