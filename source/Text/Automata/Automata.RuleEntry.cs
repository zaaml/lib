// <copyright file="Automata.StateEntry.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		#region Nested Types

		protected class RuleEntry : PrimitiveEntry
		{
			#region Ctors

			public RuleEntry(Rule rule)
			{
				Rule = rule;
			}

			#endregion

			#region Properties

			protected override string DebuggerDisplay => Rule.Name;

			internal bool SkipStack { get; set; }

			public Rule Rule { get; }

			internal RuleEntryContext RuleEntryContext { get; set; }

			#endregion
		}

		protected abstract class RuleEntryContext
		{
		}

		#endregion
	}
}