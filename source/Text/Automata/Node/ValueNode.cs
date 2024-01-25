// <copyright file="ValueNode.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		private protected sealed class ValueNode : Node
		{
			public ValueNode(Automata<TInstruction, TOperand> automata, SyntaxGraph syntaxGraph, ValueEntry valueEntry) : base(automata, syntaxGraph)
			{
				ValueEntry = valueEntry;
			}

			protected override string KindString => "_value";

			public ValueEntry ValueEntry { get; }
		}
	}
}