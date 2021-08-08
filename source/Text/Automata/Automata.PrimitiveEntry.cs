// <copyright file="Automata.PrimitiveEntry.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		protected abstract class PrimitiveEntry : Entry
		{
			public static implicit operator PrimitiveEntry(Rule state)
			{
				return new RuleEntry(state);
			}

			public static implicit operator PrimitiveEntry(TOperand input)
			{
				return new SingleMatchEntry(input);
			}
		}
	}
}