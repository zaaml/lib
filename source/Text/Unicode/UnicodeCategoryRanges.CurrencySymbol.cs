// <copyright file="UnicodeCategoryRanges.CurrencySymbol.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Range = Zaaml.Core.Range<int>;

namespace Zaaml.Text.Unicode
{
	internal static partial class UnicodeCategoryRanges
	{
		public static Range[] CurrencySymbol => CurrencySymbolRange.Range;

		private static class CurrencySymbolRange
		{
			public static readonly Range[] Range;

			static CurrencySymbolRange()
			{
				Range = BuildRange(
					0x00000024, 0x00000024, 0x000000a2, 0x000000a5, 0x0000058f, 0x0000058f, 0x0000060b, 0x0000060b, 0x000007fe, 0x000007ff, 0x000009f2, 0x000009f3, 0x000009fb, 0x000009fb, 0x00000af1, 0x00000af1, 0x00000bf9, 0x00000bf9,
					0x00000e3f, 0x00000e3f, 0x000017db, 0x000017db, 0x000020a0, 0x000020c0, 0x0000a838, 0x0000a838, 0x0000fdfc, 0x0000fdfc, 0x0000fe69, 0x0000fe69, 0x0000ff04, 0x0000ff04, 0x0000ffe0, 0x0000ffe1, 0x0000ffe5, 0x0000ffe6,
					0x00011fdd, 0x00011fe0, 0x0001e2ff, 0x0001e2ff, 0x0001ecb0, 0x0001ecb0);
			}
		}
	}
}