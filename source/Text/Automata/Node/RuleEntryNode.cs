// <copyright file="RuleEntryNode.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		private protected abstract class RuleEntryNode : Node
		{
			public readonly RuleEntry RuleEntry;

			protected RuleEntryNode(Automata<TInstruction, TOperand> automata, Graph graph, RuleEntry ruleEntry) : base(automata, graph)
			{
				RuleEntry = ruleEntry;
			}
		}
	}
}