// <copyright file="UnicodeCategoryRanges.PrivateUse.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Range = Zaaml.Core.Range<int>;

namespace Zaaml.Text.Unicode
{
	internal static partial class UnicodeCategoryRanges
	{
		public static Range[] PrivateUse => PrivateUseRange.Range;

		private static class PrivateUseRange
		{
			public static readonly Range[] Range;

			static PrivateUseRange()
			{
				Range = BuildRange(
					0x0000e000, 0x0000f8ff, 0x000f0000, 0x000ffffd, 0x00100000, 0x0010fffd);
			}
		}
	}
}