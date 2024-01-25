// <copyright file="UnicodeCategoryRanges.Separator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Range = Zaaml.Core.Range<int>;

namespace Zaaml.Text.Unicode
{
	internal static partial class UnicodeCategoryRanges
	{
		public static Range[] Separator => SeparatorRange.Range;

		private static class SeparatorRange
		{
			public static readonly Range[] Range;

			static SeparatorRange()
			{
				Range = BuildRange(
					0x00000020, 0x00000020, 0x000000a0, 0x000000a0, 0x00001680, 0x00001680, 0x00002000, 0x0000200a, 0x00002028, 0x00002029, 0x0000202f, 0x0000202f, 0x0000205f, 0x0000205f, 0x00003000, 0x00003000);
			}
		}
	}
}