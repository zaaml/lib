// <copyright file="UnicodeCategoryRanges.Number.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Range = Zaaml.Core.Range<int>;

namespace Zaaml.Text.Unicode
{
	internal static partial class UnicodeCategoryRanges
	{
		public static Range[] Number => NumberRange.Range;

		private static class NumberRange
		{
			public static readonly Range[] Range;

			static NumberRange()
			{
				Range = BuildRange(
					0x00000030, 0x00000039, 0x000000b2, 0x000000b3, 0x000000b9, 0x000000b9, 0x000000bc, 0x000000be, 0x00000660, 0x00000669, 0x000006f0, 0x000006f9, 0x000007c0, 0x000007c9, 0x00000966, 0x0000096f, 0x000009e6, 0x000009ef,
					0x000009f4, 0x000009f9, 0x00000a66, 0x00000a6f, 0x00000ae6, 0x00000aef, 0x00000b66, 0x00000b6f, 0x00000b72, 0x00000b77, 0x00000be6, 0x00000bf2, 0x00000c66, 0x00000c6f, 0x00000c78, 0x00000c7e, 0x00000ce6, 0x00000cef,
					0x00000d58, 0x00000d5e, 0x00000d66, 0x00000d78, 0x00000de6, 0x00000def, 0x00000e50, 0x00000e59, 0x00000ed0, 0x00000ed9, 0x00000f20, 0x00000f33, 0x00001040, 0x00001049, 0x00001090, 0x00001099, 0x00001369, 0x0000137c,
					0x000016ee, 0x000016f0, 0x000017e0, 0x000017e9, 0x000017f0, 0x000017f9, 0x00001810, 0x00001819, 0x00001946, 0x0000194f, 0x000019d0, 0x000019da, 0x00001a80, 0x00001a89, 0x00001a90, 0x00001a99, 0x00001b50, 0x00001b59,
					0x00001bb0, 0x00001bb9, 0x00001c40, 0x00001c49, 0x00001c50, 0x00001c59, 0x00002070, 0x00002070, 0x00002074, 0x00002079, 0x00002080, 0x00002089, 0x00002150, 0x00002182, 0x00002185, 0x00002189, 0x00002460, 0x0000249b,
					0x000024ea, 0x000024ff, 0x00002776, 0x00002793, 0x00002cfd, 0x00002cfd, 0x00003007, 0x00003007, 0x00003021, 0x00003029, 0x00003038, 0x0000303a, 0x00003192, 0x00003195, 0x00003220, 0x00003229, 0x00003248, 0x0000324f,
					0x00003251, 0x0000325f, 0x00003280, 0x00003289, 0x000032b1, 0x000032bf, 0x0000a620, 0x0000a629, 0x0000a6e6, 0x0000a6ef, 0x0000a830, 0x0000a835, 0x0000a8d0, 0x0000a8d9, 0x0000a900, 0x0000a909, 0x0000a9d0, 0x0000a9d9,
					0x0000a9f0, 0x0000a9f9, 0x0000aa50, 0x0000aa59, 0x0000abf0, 0x0000abf9, 0x0000ff10, 0x0000ff19, 0x00010107, 0x00010133, 0x00010140, 0x00010178, 0x0001018a, 0x0001018b, 0x000102e1, 0x000102fb, 0x00010320, 0x00010323,
					0x00010341, 0x00010341, 0x0001034a, 0x0001034a, 0x000103d1, 0x000103d5, 0x000104a0, 0x000104a9, 0x00010858, 0x0001085f, 0x00010879, 0x0001087f, 0x000108a7, 0x000108af, 0x000108fb, 0x000108ff, 0x00010916, 0x0001091b,
					0x000109bc, 0x000109bd, 0x000109c0, 0x000109cf, 0x000109d2, 0x000109ff, 0x00010a40, 0x00010a48, 0x00010a7d, 0x00010a7e, 0x00010a9d, 0x00010a9f, 0x00010aeb, 0x00010aef, 0x00010b58, 0x00010b5f, 0x00010b78, 0x00010b7f,
					0x00010ba9, 0x00010baf, 0x00010cfa, 0x00010cff, 0x00010d30, 0x00010d39, 0x00010e60, 0x00010e7e, 0x00010f1d, 0x00010f26, 0x00010f51, 0x00010f54, 0x00010fc5, 0x00010fcb, 0x00011052, 0x0001106f, 0x000110f0, 0x000110f9,
					0x00011136, 0x0001113f, 0x000111d0, 0x000111d9, 0x000111e1, 0x000111f4, 0x000112f0, 0x000112f9, 0x00011450, 0x00011459, 0x000114d0, 0x000114d9, 0x00011650, 0x00011659, 0x000116c0, 0x000116c9, 0x00011730, 0x0001173b,
					0x000118e0, 0x000118f2, 0x00011950, 0x00011959, 0x00011c50, 0x00011c6c, 0x00011d50, 0x00011d59, 0x00011da0, 0x00011da9, 0x00011f50, 0x00011f59, 0x00011fc0, 0x00011fd4, 0x00012400, 0x0001246e, 0x00016a60, 0x00016a69,
					0x00016ac0, 0x00016ac9, 0x00016b50, 0x00016b59, 0x00016b5b, 0x00016b61, 0x00016e80, 0x00016e96, 0x0001d2c0, 0x0001d2d3, 0x0001d2e0, 0x0001d2f3, 0x0001d360, 0x0001d378, 0x0001d7ce, 0x0001d7ff, 0x0001e140, 0x0001e149,
					0x0001e2f0, 0x0001e2f9, 0x0001e4f0, 0x0001e4f9, 0x0001e8c7, 0x0001e8cf, 0x0001e950, 0x0001e959, 0x0001ec71, 0x0001ecab, 0x0001ecad, 0x0001ecaf, 0x0001ecb1, 0x0001ecb4, 0x0001ed01, 0x0001ed2d, 0x0001ed2f, 0x0001ed3d,
					0x0001f100, 0x0001f10c, 0x0001fbf0, 0x0001fbf9);
			}
		}
	}
}