// <copyright file="Automata.MatchEntryGroupingEqualityComparer.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		protected sealed class EntryEqualityComparer : IEqualityComparer<Entry>
		{
			public static readonly IEqualityComparer<Entry> Instance = new EntryEqualityComparer();

			private EntryEqualityComparer()
			{
			}

			public bool Equals(Entry x, Entry y)
			{
				if (ReferenceEquals(x, y))
					return true;

				return (x, y) switch
				{
					(RangeMatchEntry rx, RangeMatchEntry ry) => RangeMatchEntry.EqualityComparer.Equals(rx, ry),
					(SingleMatchEntry sx, SingleMatchEntry sy) => SingleMatchEntry.EqualityComparer.Equals(sx, sy),
					(QuantifierEntry qx, QuantifierEntry qy) => QuantifierEntry.EqualityComparer.Equals(qx, qy),
					_ => false
				};
			}

			public int GetHashCode(Entry m)
			{
				return m switch
				{
					RangeMatchEntry r => RangeMatchEntry.EqualityComparer.GetHashCode(r),
					SingleMatchEntry s => SingleMatchEntry.EqualityComparer.GetHashCode(s),
					QuantifierEntry q => QuantifierEntry.EqualityComparer.GetHashCode(q),
					_ => m.GetHashCode()
				};
			}
		}
	}
}