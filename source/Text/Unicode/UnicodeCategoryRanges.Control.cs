// <copyright file="UnicodeCategoryRanges.Control.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Range = Zaaml.Core.Range<int>;

namespace Zaaml.Text.Unicode
{
	internal static partial class UnicodeCategoryRanges
	{
		public static Range[] Control => ControlRange.Range;

		private static class ControlRange
		{
			public static readonly Range[] Range;

			static ControlRange()
			{
				Range = BuildRange(
					0x00000000, 0x0000001f, 0x0000007f, 0x0000009f);
			}
		}
	}
}