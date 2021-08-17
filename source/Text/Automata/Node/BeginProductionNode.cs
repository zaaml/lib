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

		private protected abstract class PrecedenceNode : Node
		{
			public PrecedencePredicate Precedence { get; }

			protected PrecedenceNode(Automata<TInstruction, TOperand> automata, Graph graph, PrecedencePredicate precedence) : base(automata, graph)
			{
				Precedence = precedence;
			}
		}

		private protected sealed class PrecedenceEnterNode : PrecedenceNode
		{
			public PrecedenceEnterNode(Automata<TInstruction, TOperand> automata, Graph graph, PrecedencePredicate precedence) : base(automata, graph, precedence)
			{
			}

			protected override string KindString => $"_precedenceEnter({Precedence})";
		}

		private protected sealed class PrecedenceLeaveNode : PrecedenceNode
		{
			public PrecedenceLeaveNode(Automata<TInstruction, TOperand> automata, Graph graph, PrecedencePredicate precedence) : base(automata, graph, precedence)
			{
			}

			protected override string KindString => $"_precedenceLeave({Precedence})";
		}
	}
}