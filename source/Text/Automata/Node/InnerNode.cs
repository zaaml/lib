// <copyright file="InnerNode.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		private protected sealed class InnerNode : Node
		{
			public InnerNode(Automata<TInstruction, TOperand> automata, Graph graph) : base(automata, graph)
			{
			}

			protected override string KindString => $"_inner_{Index}";
		}
	}
}