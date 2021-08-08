// <copyright file="BeginProductionNode.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		private protected sealed class BeginProductionNode : ProductionNode
		{
			public BeginProductionNode(Automata<TInstruction, TOperand> automata, Graph graph, Production production) : base(automata, graph, production)
			{
			}

			protected override string KindString => $"_begin({Production})";
		}
	}
}