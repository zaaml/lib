// <copyright file="UnicodeCategoryRanges.EnclosingMark.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Range = Zaaml.Core.Range<int>;

namespace Zaaml.Text.Unicode
{
	internal static partial class UnicodeCategoryRanges
	{
		public static Range[] EnclosingMark => EnclosingMarkRange.Range;

		private static class EnclosingMarkRange
		{
			public static readonly Range[] Range;

			static EnclosingMarkRange()
			{
				Range = BuildRange(
					0x00000488, 0x00000489, 0x00001abe, 0x00001abe, 0x000020dd, 0x000020e0, 0x000020e2, 0x000020e4, 0x0000a670, 0x0000a672);
			}
		}
	}
}