// <copyright file="EnterRuleNode.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		private protected sealed class EnterSyntaxNode : SubGraphNode
		{
			public EnterSyntaxNode(Automata<TInstruction, TOperand> automata, SyntaxGraph syntaxGraph, SubGraph subGraph) : base(automata, syntaxGraph, subGraph)
			{
			}

			protected override string KindString => $"_enter({SubGraph})";
		}
	}
}