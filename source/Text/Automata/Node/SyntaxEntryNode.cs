// <copyright file="RuleEntryNode.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		private protected abstract class SyntaxEntryNode : Node
		{
			public readonly SyntaxEntry RuleEntry;

			protected SyntaxEntryNode(Automata<TInstruction, TOperand> automata, SyntaxGraph syntaxGraph, SyntaxEntry ruleEntry) : base(automata, syntaxGraph)
			{
				RuleEntry = ruleEntry;
			}
		}
	}
}