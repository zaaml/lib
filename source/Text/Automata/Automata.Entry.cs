// <copyright file="Automata.Entry.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Diagnostics;

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		[DebuggerDisplay("{" + nameof(DebuggerDisplay) + "}")]
		protected abstract class Entry
		{
			protected abstract string DebuggerDisplay { get; }

			public static implicit operator Entry(TOperand operand)
			{
				return new SingleMatchEntry(operand);
			}

			public static implicit operator Entry(Rule state)
			{
				return new RuleEntry(state);
			}

			public override string ToString()
			{
				return DebuggerDisplay;
			}
		}
	}
}