// <copyright file="CharRangeEntry.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal class CharRangeEntry : CharSetEntry
	{
		public CharRangeEntry(CharEntry min, CharEntry max)
		{
			Min = min;
			Max = max;
		}

		public CharEntry Max { get; }

		public CharEntry Min { get; }

		public override string Format(bool set)
		{
			return set ? $"{Min.Format(true)}-{Max.Format(true)}" : $"{Min.Format(false)}..{Max.Format(false)}";
		}
	}
}