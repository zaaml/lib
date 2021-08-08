// <copyright file="SubGraphNode.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		private protected abstract class SubGraphNode : Node
		{
			public readonly SubGraph SubGraph;

			protected SubGraphNode(Automata<TInstruction, TOperand> automata, Graph graph, SubGraph subGraph, ThreadStatusKind threadStatusKind = ThreadStatusKind.Run) : base(automata, graph, threadStatusKind)
			{
				SubGraph = subGraph;
			}
		}
	}
}