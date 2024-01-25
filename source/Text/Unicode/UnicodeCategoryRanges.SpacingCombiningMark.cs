// <copyright file="UnicodeCategoryRanges.SpacingCombiningMark.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Range = Zaaml.Core.Range<int>;

namespace Zaaml.Text.Unicode
{
	internal static partial class UnicodeCategoryRanges
	{
		public static Range[] SpacingCombiningMark => SpacingCombiningMarkRange.Range;

		private static class SpacingCombiningMarkRange
		{
			public static readonly Range[] Range;

			static SpacingCombiningMarkRange()
			{
				Range = BuildRange(
					0x00000903, 0x00000903, 0x0000093b, 0x0000093b, 0x0000093e, 0x00000940, 0x00000949, 0x0000094c, 0x0000094e, 0x0000094f, 0x00000982, 0x00000983, 0x000009be, 0x000009c0, 0x000009c7, 0x000009c8, 0x000009cb, 0x000009cc,
					0x000009d7, 0x000009d7, 0x00000a03, 0x00000a03, 0x00000a3e, 0x00000a40, 0x00000a83, 0x00000a83, 0x00000abe, 0x00000ac0, 0x00000ac9, 0x00000ac9, 0x00000acb, 0x00000acc, 0x00000b02, 0x00000b03, 0x00000b3e, 0x00000b3e,
					0x00000b40, 0x00000b40, 0x00000b47, 0x00000b48, 0x00000b4b, 0x00000b4c, 0x00000b57, 0x00000b57, 0x00000bbe, 0x00000bbf, 0x00000bc1, 0x00000bc2, 0x00000bc6, 0x00000bc8, 0x00000bca, 0x00000bcc, 0x00000bd7, 0x00000bd7,
					0x00000c01, 0x00000c03, 0x00000c41, 0x00000c44, 0x00000c82, 0x00000c83, 0x00000cbe, 0x00000cbe, 0x00000cc0, 0x00000cc4, 0x00000cc7, 0x00000cc8, 0x00000cca, 0x00000ccb, 0x00000cd5, 0x00000cd6, 0x00000cf3, 0x00000cf3,
					0x00000d02, 0x00000d03, 0x00000d3e, 0x00000d40, 0x00000d46, 0x00000d48, 0x00000d4a, 0x00000d4c, 0x00000d57, 0x00000d57, 0x00000d82, 0x00000d83, 0x00000dcf, 0x00000dd1, 0x00000dd8, 0x00000ddf, 0x00000df2, 0x00000df3,
					0x00000f3e, 0x00000f3f, 0x00000f7f, 0x00000f7f, 0x0000102b, 0x0000102c, 0x00001031, 0x00001031, 0x00001038, 0x00001038, 0x0000103b, 0x0000103c, 0x00001056, 0x00001057, 0x00001062, 0x00001064, 0x00001067, 0x0000106d,
					0x00001083, 0x00001084, 0x00001087, 0x0000108c, 0x0000108f, 0x0000108f, 0x0000109a, 0x0000109c, 0x00001715, 0x00001715, 0x00001734, 0x00001734, 0x000017b6, 0x000017b6, 0x000017be, 0x000017c5, 0x000017c7, 0x000017c8,
					0x00001923, 0x00001926, 0x00001929, 0x0000192b, 0x00001930, 0x00001931, 0x00001933, 0x00001938, 0x00001a19, 0x00001a1a, 0x00001a55, 0x00001a55, 0x00001a57, 0x00001a57, 0x00001a61, 0x00001a61, 0x00001a63, 0x00001a64,
					0x00001a6d, 0x00001a72, 0x00001b04, 0x00001b04, 0x00001b35, 0x00001b35, 0x00001b3b, 0x00001b3b, 0x00001b3d, 0x00001b41, 0x00001b43, 0x00001b44, 0x00001b82, 0x00001b82, 0x00001ba1, 0x00001ba1, 0x00001ba6, 0x00001ba7,
					0x00001baa, 0x00001baa, 0x00001be7, 0x00001be7, 0x00001bea, 0x00001bec, 0x00001bee, 0x00001bee, 0x00001bf2, 0x00001bf3, 0x00001c24, 0x00001c2b, 0x00001c34, 0x00001c35, 0x00001ce1, 0x00001ce1, 0x00001cf7, 0x00001cf7,
					0x0000302e, 0x0000302f, 0x0000a823, 0x0000a824, 0x0000a827, 0x0000a827, 0x0000a880, 0x0000a881, 0x0000a8b4, 0x0000a8c3, 0x0000a952, 0x0000a953, 0x0000a983, 0x0000a983, 0x0000a9b4, 0x0000a9b5, 0x0000a9ba, 0x0000a9bb,
					0x0000a9be, 0x0000a9c0, 0x0000aa2f, 0x0000aa30, 0x0000aa33, 0x0000aa34, 0x0000aa4d, 0x0000aa4d, 0x0000aa7b, 0x0000aa7b, 0x0000aa7d, 0x0000aa7d, 0x0000aaeb, 0x0000aaeb, 0x0000aaee, 0x0000aaef, 0x0000aaf5, 0x0000aaf5,
					0x0000abe3, 0x0000abe4, 0x0000abe6, 0x0000abe7, 0x0000abe9, 0x0000abea, 0x0000abec, 0x0000abec, 0x00011000, 0x00011000, 0x00011002, 0x00011002, 0x00011082, 0x00011082, 0x000110b0, 0x000110b2, 0x000110b7, 0x000110b8,
					0x0001112c, 0x0001112c, 0x00011145, 0x00011146, 0x00011182, 0x00011182, 0x000111b3, 0x000111b5, 0x000111bf, 0x000111c0, 0x000111ce, 0x000111ce, 0x0001122c, 0x0001122e, 0x00011232, 0x00011233, 0x00011235, 0x00011235,
					0x000112e0, 0x000112e2, 0x00011302, 0x00011303, 0x0001133e, 0x0001133f, 0x00011341, 0x00011344, 0x00011347, 0x00011348, 0x0001134b, 0x0001134d, 0x00011357, 0x00011357, 0x00011362, 0x00011363, 0x00011435, 0x00011437,
					0x00011440, 0x00011441, 0x00011445, 0x00011445, 0x000114b0, 0x000114b2, 0x000114b9, 0x000114b9, 0x000114bb, 0x000114be, 0x000114c1, 0x000114c1, 0x000115af, 0x000115b1, 0x000115b8, 0x000115bb, 0x000115be, 0x000115be,
					0x00011630, 0x00011632, 0x0001163b, 0x0001163c, 0x0001163e, 0x0001163e, 0x000116ac, 0x000116ac, 0x000116ae, 0x000116af, 0x000116b6, 0x000116b6, 0x00011720, 0x00011721, 0x00011726, 0x00011726, 0x0001182c, 0x0001182e,
					0x00011838, 0x00011838, 0x00011930, 0x00011935, 0x00011937, 0x00011938, 0x0001193d, 0x0001193d, 0x00011940, 0x00011940, 0x00011942, 0x00011942, 0x000119d1, 0x000119d3, 0x000119dc, 0x000119df, 0x000119e4, 0x000119e4,
					0x00011a39, 0x00011a39, 0x00011a57, 0x00011a58, 0x00011a97, 0x00011a97, 0x00011c2f, 0x00011c2f, 0x00011c3e, 0x00011c3e, 0x00011ca9, 0x00011ca9, 0x00011cb1, 0x00011cb1, 0x00011cb4, 0x00011cb4, 0x00011d8a, 0x00011d8e,
					0x00011d93, 0x00011d94, 0x00011d96, 0x00011d96, 0x00011ef5, 0x00011ef6, 0x00011f03, 0x00011f03, 0x00011f34, 0x00011f35, 0x00011f3e, 0x00011f3f, 0x00011f41, 0x00011f41, 0x00016f51, 0x00016f87, 0x00016ff0, 0x00016ff1,
					0x0001d165, 0x0001d166, 0x0001d16d, 0x0001d172);
			}
		}
	}
}