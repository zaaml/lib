// <copyright file="InitRuleNode.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		private protected sealed class InitSyntaxNode : EntryPointSubGraphNode
		{
			public InitSyntaxNode(Automata<TInstruction, TOperand> automata, SubGraph subGraph) : base(automata, subGraph)
			{
			}

			protected override string KindString => $"_init({SubGraph})";
		}
	}
}