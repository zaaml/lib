// <copyright file="UnicodeCategoryRanges.MathSymbol.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Range = Zaaml.Core.Range<int>;

namespace Zaaml.Text.Unicode
{
	internal static partial class UnicodeCategoryRanges
	{
		public static Range[] MathSymbol => MathSymbolRange.Range;

		private static class MathSymbolRange
		{
			public static readonly Range[] Range;

			static MathSymbolRange()
			{
				Range = BuildRange(
					0x0000002b, 0x0000002b, 0x0000003c, 0x0000003e, 0x0000007c, 0x0000007c, 0x0000007e, 0x0000007e, 0x000000ac, 0x000000ac, 0x000000b1, 0x000000b1, 0x000000d7, 0x000000d7, 0x000000f7, 0x000000f7, 0x000003f6, 0x000003f6,
					0x00000606, 0x00000608, 0x00002044, 0x00002044, 0x00002052, 0x00002052, 0x0000207a, 0x0000207c, 0x0000208a, 0x0000208c, 0x00002118, 0x00002118, 0x00002140, 0x00002144, 0x0000214b, 0x0000214b, 0x00002190, 0x00002194,
					0x0000219a, 0x0000219b, 0x000021a0, 0x000021a0, 0x000021a3, 0x000021a3, 0x000021a6, 0x000021a6, 0x000021ae, 0x000021ae, 0x000021ce, 0x000021cf, 0x000021d2, 0x000021d2, 0x000021d4, 0x000021d4, 0x000021f4, 0x000022ff,
					0x00002320, 0x00002321, 0x0000237c, 0x0000237c, 0x0000239b, 0x000023b3, 0x000023dc, 0x000023e1, 0x000025b7, 0x000025b7, 0x000025c1, 0x000025c1, 0x000025f8, 0x000025ff, 0x0000266f, 0x0000266f, 0x000027c0, 0x000027c4,
					0x000027c7, 0x000027e5, 0x000027f0, 0x000027ff, 0x00002900, 0x00002982, 0x00002999, 0x000029d7, 0x000029dc, 0x000029fb, 0x000029fe, 0x00002aff, 0x00002b30, 0x00002b44, 0x00002b47, 0x00002b4c, 0x0000fb29, 0x0000fb29,
					0x0000fe62, 0x0000fe62, 0x0000fe64, 0x0000fe66, 0x0000ff0b, 0x0000ff0b, 0x0000ff1c, 0x0000ff1e, 0x0000ff5c, 0x0000ff5c, 0x0000ff5e, 0x0000ff5e, 0x0000ffe2, 0x0000ffe2, 0x0000ffe9, 0x0000ffec, 0x0001d6c1, 0x0001d6c1,
					0x0001d6db, 0x0001d6db, 0x0001d6fb, 0x0001d6fb, 0x0001d715, 0x0001d715, 0x0001d735, 0x0001d735, 0x0001d74f, 0x0001d74f, 0x0001d76f, 0x0001d76f, 0x0001d789, 0x0001d789, 0x0001d7a9, 0x0001d7a9, 0x0001d7c3, 0x0001d7c3,
					0x0001eef0, 0x0001eef1);
			}
		}
	}
}