// <copyright file="EndProductionNode.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		private protected sealed class EndProductionNode : ProductionNode
		{
			public EndProductionNode(Automata<TInstruction, TOperand> automata, SyntaxGraph syntaxGraph, Production production) : base(automata, syntaxGraph, production)
			{
			}

			protected override string KindString => "_endProduction";
		}
	}
}