// <copyright file="EnterRuleNode.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		private protected sealed class EnterRuleNode : SubGraphNode
		{
			public EnterRuleNode(Automata<TInstruction, TOperand> automata, Graph graph, SubGraph subGraph) : base(automata, graph, subGraph)
			{
			}

			protected override string KindString => $"_enter({SubGraph})";
		}
	}
}