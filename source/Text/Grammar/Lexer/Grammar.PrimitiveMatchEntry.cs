// <copyright file="Grammar.PrimitiveMatchEntry.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.Text
{
	internal partial class Grammar<TToken>
	{
		protected internal abstract class PrimitiveMatchEntry : MatchEntry
		{
			public static implicit operator PrimitiveMatchEntry(char c)
			{
				return new CharEntry(c);
			}

#if NETCOREAPP3_1 || NET5_0_OR_GREATER
			public static implicit operator PrimitiveMatchEntry(Range range)
			{
				return new RangeEntry((char)range.Start.Value, (char)range.End.Value);
			}
#endif
		}
	}
}