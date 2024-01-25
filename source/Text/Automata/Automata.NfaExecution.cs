// <copyright file="Automata.Pool.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		private protected readonly struct NfaExecution
		{
			public static readonly NfaExecution Next = new(ExecutionRailList.Next, null);
			public static readonly NfaExecution Empty = new(ExecutionRailList.Empty, null);

			public NfaExecution(ExecutionRailList executionRailList, DfaState targetDfa)
			{
				ExecutionRailList = executionRailList;
				TargetDfa = targetDfa;
			}

			public readonly ExecutionRailList ExecutionRailList;
			public readonly DfaState TargetDfa;
		}
	}
}