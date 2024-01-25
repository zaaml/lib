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
					(OperandMatchEntry sx, OperandMatchEntry sy) => OperandMatchEntry.EqualityComparer.Equals(sx, sy),
					(SetMatchEntry sx, SetMatchEntry sy) => SetMatchEntry.EqualityComparer.Equals(sx, sy),
					(RangeMatchEntry rx, RangeMatchEntry ry) => RangeMatchEntry.EqualityComparer.Equals(rx, ry),
					(QuantifierEntry qx, QuantifierEntry qy) => QuantifierEntry.EqualityComparer.Equals(qx, qy),
					(SyntaxEntry rx, SyntaxEntry ry) => SyntaxEntry.EqualityComparer.Equals(rx, ry),
					_ => false
				};
			}

			public int GetHashCode(Entry m)
			{
				return m switch
				{
					OperandMatchEntry s => OperandMatchEntry.EqualityComparer.GetHashCode(s),
					RangeMatchEntry r => RangeMatchEntry.EqualityComparer.GetHashCode(r),
					SetMatchEntry s => SetMatchEntry.EqualityComparer.GetHashCode(s),
					QuantifierEntry q => QuantifierEntry.EqualityComparer.GetHashCode(q),
					SyntaxEntry r => SyntaxEntry.EqualityComparer.GetHashCode(r),
					_ => m.GetHashCode()
				};
			}
		}
	}
}