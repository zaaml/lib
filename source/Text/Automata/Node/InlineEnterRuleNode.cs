// <copyright file="InlineEnterRuleNode.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		private protected sealed class InlineEnterRuleNode : RuleEntryNode
		{
			public InlineEnterRuleNode(Automata<TInstruction, TOperand> automata, Graph graph, RuleEntry ruleEntry) : base(automata, graph, ruleEntry)
			{
			}

			protected override string KindString => $"_inline_enter({RuleEntry})";
		}
	}
}