// <copyright file="ActionNode.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.Core;

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		private protected sealed class ActionNode : Node
		{
			public ActionNode(Automata<TInstruction, TOperand> automata, SyntaxGraph syntaxGraph, ActionEntry actionEntry) : base(automata, syntaxGraph)
			{
				ActionEntry = actionEntry;
			}

			[UsedImplicitly]
			public ActionEntry ActionEntry { get; }

			protected override string KindString => "_action";
		}
	}
}