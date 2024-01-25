// <copyright file="UnicodeCategoryRanges.ConnectorPunctuation.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Range = Zaaml.Core.Range<int>;

namespace Zaaml.Text.Unicode
{
	internal static partial class UnicodeCategoryRanges
	{
		public static Range[] ConnectorPunctuation => ConnectorPunctuationRange.Range;

		private static class ConnectorPunctuationRange
		{
			public static readonly Range[] Range;

			static ConnectorPunctuationRange()
			{
				Range = BuildRange(
					0x0000005f, 0x0000005f, 0x0000203f, 0x00002040, 0x00002054, 0x00002054, 0x0000fe33, 0x0000fe34, 0x0000fe4d, 0x0000fe4f, 0x0000ff3f, 0x0000ff3f);
			}
		}
	}
}