// <copyright file="InlineEnterRuleNode.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		private protected sealed class InlineEnterSyntaxNode : SyntaxEntryNode
		{
			public InlineEnterSyntaxNode(Automata<TInstruction, TOperand> automata, SyntaxGraph syntaxGraph, SyntaxEntry ruleEntry) : base(automata, syntaxGraph, ruleEntry)
			{
			}

			protected override string KindString => $"_inline_enter({RuleEntry})";
		}
	}
}