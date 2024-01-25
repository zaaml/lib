// <copyright file="EntryPointSubGraphNode.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		private protected abstract class EntryPointSubGraphNode : SubGraphNode
		{
			protected EntryPointSubGraphNode(Automata<TInstruction, TOperand> automata, SubGraph subGraph, ThreadStatusKind threadStatusKind = ThreadStatusKind.Run) : base(automata, null, subGraph, threadStatusKind)
			{
			}
		}
	}
}