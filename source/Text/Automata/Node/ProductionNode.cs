// <copyright file="ProductionNode.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		private protected abstract class ProductionNode : Node
		{
			protected ProductionNode(Automata<TInstruction, TOperand> automata, SyntaxGraph syntaxGraph, Production production) : base(automata, syntaxGraph)
			{
				Production = production;
			}

			public Production Production { get; }
		}
	}
}