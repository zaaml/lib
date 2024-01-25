// <copyright file="UnicodeCategoryRanges.Format.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Range = Zaaml.Core.Range<int>;

namespace Zaaml.Text.Unicode
{
	internal static partial class UnicodeCategoryRanges
	{
		public static Range[] Format => FormatRange.Range;

		private static class FormatRange
		{
			public static readonly Range[] Range;

			static FormatRange()
			{
				Range = BuildRange(
					0x000000ad, 0x000000ad, 0x00000600, 0x00000605, 0x0000061c, 0x0000061c, 0x000006dd, 0x000006dd, 0x0000070f, 0x0000070f, 0x00000890, 0x00000891, 0x000008e2, 0x000008e2, 0x0000180e, 0x0000180e, 0x0000200b, 0x0000200f,
					0x0000202a, 0x0000202e, 0x00002060, 0x00002064, 0x00002066, 0x0000206f, 0x0000feff, 0x0000feff, 0x0000fff9, 0x0000fffb, 0x000110bd, 0x000110bd, 0x000110cd, 0x000110cd, 0x00013430, 0x0001343f, 0x0001bca0, 0x0001bca3,
					0x0001d173, 0x0001d17a, 0x000e0001, 0x000e0001, 0x000e0020, 0x000e007f);
			}
		}
	}
}