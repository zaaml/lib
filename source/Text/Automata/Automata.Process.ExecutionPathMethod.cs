// <copyright file="Automata.Process.ExecutionPathMethod.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		private protected delegate Node ExecutionPathMethodDelegate(ExecutionPath executionPath, Process process);

		private protected partial class Process
		{
			private static readonly ExecutionPathMethodDelegate ExecuteForkPathMainDelegate = ExecuteForkPathMain;
			private static readonly ExecutionPathMethodDelegate ExecuteForkPathParallelDelegate = ExecuteForkPathParallel;
		}
	}
}