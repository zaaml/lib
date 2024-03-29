// <copyright file="EndRuleNode.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		private protected sealed class ExitSyntaxNode : EntryPointSubGraphNode
		{
			public ExitSyntaxNode(Automata<TInstruction, TOperand> automata, SubGraph subGraph) : base(automata, subGraph, ThreadStatusKind.Finished)
			{
			}

			protected override string KindString => $"_exit({SubGraph})";
		}
	}
}