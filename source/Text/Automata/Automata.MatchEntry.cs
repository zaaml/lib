// <copyright file="Automata.MatchEntry.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		protected abstract class MatchEntry : PrimitiveEntry
		{
			public abstract bool Match(TOperand operand);

			public abstract bool Match(int operand);

			public QuantifierEntry OneOrMore(QuantifierMode mode = QuantifierMode.Greedy)
			{
				return new QuantifierEntry(this, QuantifierKind.OneOrMore, mode);
			}

			public QuantifierEntry ZeroOrMore(QuantifierMode mode = QuantifierMode.Greedy)
			{
				return new QuantifierEntry(this, QuantifierKind.ZeroOrMore, mode);
			}

			public QuantifierEntry ZeroOrOne(QuantifierMode mode = QuantifierMode.Greedy)
			{
				return new QuantifierEntry(this, QuantifierKind.ZeroOrOne, mode);
			}
		}

		protected abstract class ValueEntry : PrimitiveEntry
		{
		}
	}
}