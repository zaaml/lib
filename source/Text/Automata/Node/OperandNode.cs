// <copyright file="OperandNode.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		private protected sealed class OperandNode : Node
		{
			public OperandNode(Automata<TInstruction, TOperand> automata, Graph graph, MatchEntry matchEntry) : base(automata, graph)
			{
				MatchEntry = matchEntry;
			}

			protected override string KindString => $"_operand({MatchEntry})";

			public MatchEntry MatchEntry { get; }
		}
	}
}