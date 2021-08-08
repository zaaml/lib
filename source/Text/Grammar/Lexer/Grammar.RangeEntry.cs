// <copyright file="Grammar.RangeEntry.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal partial class Grammar<TToken>
	{
		protected internal sealed partial class RangeEntry : PrimitiveMatchEntry
		{
			public RangeEntry(char first, char last)
			{
				First = first;
				Last = last;
			}

			public char First { get; }

			public char Last { get; }
		}
	}
}