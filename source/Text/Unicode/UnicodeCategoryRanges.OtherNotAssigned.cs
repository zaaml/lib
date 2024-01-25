// <copyright file="UnicodeCategoryRanges.OtherNotAssigned.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Range = Zaaml.Core.Range<int>;

namespace Zaaml.Text.Unicode
{
	internal static partial class UnicodeCategoryRanges
	{
		public static Range[] OtherNotAssigned => OtherNotAssignedRange.Range;

		private static class OtherNotAssignedRange
		{
			public static readonly Range[] Range;

			static OtherNotAssignedRange()
			{
				Range = BuildRange(
				);
			}
		}
	}
}