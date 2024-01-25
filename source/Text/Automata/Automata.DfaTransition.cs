// <copyright file="Automata.Pool.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.Core;

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		private protected readonly struct DfaTransition
		{
			public readonly MemorySpan<int> ExecutionRail;
			public readonly DfaState TargetDfa;

			public DfaTransition(DfaState targetDfa, MemorySpan<int> executionRail)
			{
				TargetDfa = targetDfa;
				ExecutionRail = executionRail.DetachAllocator();
			}
		}
	}
}