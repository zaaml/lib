// <copyright file="InnerNode.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		private protected sealed class InnerNode : Node
		{
			public InnerNode(Automata<TInstruction, TOperand> automata, SyntaxGraph syntaxGraph) : base(automata, syntaxGraph)
			{
			}

			protected override string KindString => $"_inner";
		}
	}
}