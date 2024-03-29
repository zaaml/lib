// <copyright file="LazyNode.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		private protected sealed class LazyNode : Node
		{
			public LazyNode(Automata<TInstruction, TOperand> automata, SyntaxGraph syntaxGraph) : base(automata, syntaxGraph)
			{
			}

			protected override string KindString => "_lazy";
		}
	}
}