// <copyright file="Automata.Pool.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Runtime.CompilerServices;
using Zaaml.Core;

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		private protected sealed class ExecutionRailNode : IDisposable
		{
			public static readonly ExecutionRailNode Empty = new();
			public readonly DfaState Dfa;
			public readonly ExecutionRailNodePool Pool;
			public MemorySpan<int> ExecutionRail;
			public ExecutionRailNode Next;
			public ExecutionRailNodeStat RunStat;

			public ExecutionRailNode()
			{
			}

			public ExecutionRailNode(ExecutionRailNodePool pool)
			{
				Pool = pool;
			}

			public ExecutionRailNode(DfaState dfa)
			{
				Dfa = dfa;
				RunStat = new ExecutionRailNodeStat();
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public void Dispose()
			{
				if (Pool == null)
					return;

				ExecutionRail = ExecutionRail.DisposeExchange();

				Pool.Return(this);
			}
		}

		private protected sealed class ExecutionRailNodeStat
		{
			public int FailCount;
			public int FailLength = int.MaxValue;
			public int RunCount;

			public float FailRatio => FailCount < 64 ? .0f : (float)FailCount / RunCount;

			public void Run()
			{
				RunCount++;
			}

			public void Reset()
			{
				FailCount = 0;
				FailLength = 0;
				RunCount = 0;
			}

			public void Fail(int failLength)
			{
				if (FailLength < failLength)
				{
					RunCount = 0;
					FailCount = 0;
				}

				FailLength = failLength;
				FailCount++;
			}
		}
	}
}