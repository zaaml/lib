// <copyright file="UnicodeCategoryRanges.OtherNumber.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Range = Zaaml.Core.Range<int>;

namespace Zaaml.Text.Unicode
{
	internal static partial class UnicodeCategoryRanges
	{
		public static Range[] OtherNumber => OtherNumberRange.Range;

		private static class OtherNumberRange
		{
			public static readonly Range[] Range;

			static OtherNumberRange()
			{
				Range = BuildRange(
					0x000000b2, 0x000000b3, 0x000000b9, 0x000000b9, 0x000000bc, 0x000000be, 0x000009f4, 0x000009f9, 0x00000b72, 0x00000b77, 0x00000bf0, 0x00000bf2, 0x00000c78, 0x00000c7e, 0x00000d58, 0x00000d5e, 0x00000d70, 0x00000d78,
					0x00000f2a, 0x00000f33, 0x00001369, 0x0000137c, 0x000017f0, 0x000017f9, 0x000019da, 0x000019da, 0x00002070, 0x00002070, 0x00002074, 0x00002079, 0x00002080, 0x00002089, 0x00002150, 0x0000215f, 0x00002189, 0x00002189,
					0x00002460, 0x0000249b, 0x000024ea, 0x000024ff, 0x00002776, 0x00002793, 0x00002cfd, 0x00002cfd, 0x00003192, 0x00003195, 0x00003220, 0x00003229, 0x00003248, 0x0000324f, 0x00003251, 0x0000325f, 0x00003280, 0x00003289,
					0x000032b1, 0x000032bf, 0x0000a830, 0x0000a835, 0x00010107, 0x00010133, 0x00010175, 0x00010178, 0x0001018a, 0x0001018b, 0x000102e1, 0x000102fb, 0x00010320, 0x00010323, 0x00010858, 0x0001085f, 0x00010879, 0x0001087f,
					0x000108a7, 0x000108af, 0x000108fb, 0x000108ff, 0x00010916, 0x0001091b, 0x000109bc, 0x000109bd, 0x000109c0, 0x000109cf, 0x000109d2, 0x000109ff, 0x00010a40, 0x00010a48, 0x00010a7d, 0x00010a7e, 0x00010a9d, 0x00010a9f,
					0x00010aeb, 0x00010aef, 0x00010b58, 0x00010b5f, 0x00010b78, 0x00010b7f, 0x00010ba9, 0x00010baf, 0x00010cfa, 0x00010cff, 0x00010e60, 0x00010e7e, 0x00010f1d, 0x00010f26, 0x00010f51, 0x00010f54, 0x00010fc5, 0x00010fcb,
					0x00011052, 0x00011065, 0x000111e1, 0x000111f4, 0x0001173a, 0x0001173b, 0x000118ea, 0x000118f2, 0x00011c5a, 0x00011c6c, 0x00011fc0, 0x00011fd4, 0x00016b5b, 0x00016b61, 0x00016e80, 0x00016e96, 0x0001d2c0, 0x0001d2d3,
					0x0001d2e0, 0x0001d2f3, 0x0001d360, 0x0001d378, 0x0001e8c7, 0x0001e8cf, 0x0001ec71, 0x0001ecab, 0x0001ecad, 0x0001ecaf, 0x0001ecb1, 0x0001ecb4, 0x0001ed01, 0x0001ed2d, 0x0001ed2f, 0x0001ed3d, 0x0001f100, 0x0001f10c);
			}
		}
	}
}