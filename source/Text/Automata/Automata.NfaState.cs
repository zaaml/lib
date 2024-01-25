// <copyright file="Automata.Pool.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.Core.Collections;

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		private protected sealed class NfaState
		{
			public readonly ExecutionRailNode ExecutionRailNode;
			public readonly DfaTransition[] Transitions;
			public readonly IntTrie<NfaExecution> Trie = new();

			public NfaState(DfaTransition[] transitions, ExecutionRailNode executionRailNode)
			{
				Transitions = transitions;
				ExecutionRailNode = executionRailNode;
			}
		}
	}
}