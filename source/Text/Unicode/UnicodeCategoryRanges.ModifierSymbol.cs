// <copyright file="UnicodeCategoryRanges.ModifierSymbol.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Range = Zaaml.Core.Range<int>;

namespace Zaaml.Text.Unicode
{
	internal static partial class UnicodeCategoryRanges
	{
		public static Range[] ModifierSymbol => ModifierSymbolRange.Range;

		private static class ModifierSymbolRange
		{
			public static readonly Range[] Range;

			static ModifierSymbolRange()
			{
				Range = BuildRange(
					0x0000005e, 0x0000005e, 0x00000060, 0x00000060, 0x000000a8, 0x000000a8, 0x000000af, 0x000000af, 0x000000b4, 0x000000b4, 0x000000b8, 0x000000b8, 0x000002c2, 0x000002c5, 0x000002d2, 0x000002df, 0x000002e5, 0x000002eb,
					0x000002ed, 0x000002ed, 0x000002ef, 0x000002ff, 0x00000375, 0x00000375, 0x00000384, 0x00000385, 0x00000888, 0x00000888, 0x00001fbd, 0x00001fbd, 0x00001fbf, 0x00001fc1, 0x00001fcd, 0x00001fcf, 0x00001fdd, 0x00001fdf,
					0x00001fed, 0x00001fef, 0x00001ffd, 0x00001ffe, 0x0000309b, 0x0000309c, 0x0000a700, 0x0000a716, 0x0000a720, 0x0000a721, 0x0000a789, 0x0000a78a, 0x0000ab5b, 0x0000ab5b, 0x0000ab6a, 0x0000ab6b, 0x0000fbb2, 0x0000fbc2,
					0x0000ff3e, 0x0000ff3e, 0x0000ff40, 0x0000ff40, 0x0000ffe3, 0x0000ffe3, 0x0001f3fb, 0x0001f3ff);
			}
		}
	}
}