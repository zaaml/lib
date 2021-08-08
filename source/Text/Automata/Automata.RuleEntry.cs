// <copyright file="Automata.RuleEntry.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		protected class RuleEntry : PrimitiveEntry
		{
			public RuleEntry(Rule rule)
			{
				Rule = rule;
			}

			protected override string DebuggerDisplay => Rule.Name;

			public Rule Rule { get; }

			internal RuleEntryContext RuleEntryContext { get; set; }

			internal bool SkipStack { get; set; }
		}

		protected abstract class RuleEntryContext
		{
		}
	}
}