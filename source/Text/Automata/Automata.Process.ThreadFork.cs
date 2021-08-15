// <copyright file="Automata.Process.ThreadFork.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

// ReSharper disable ForCanBeConvertedToForeach

using Zaaml.Core;

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		partial class Process
		{
			private struct ThreadFork
			{
				public static ThreadFork Empty = new(-1, default, default, default);

				public int Count;
				public Thread Thread;
				public ThreadContext Context;
				public int ExecutionPathsHead;
				public MemorySpan<int> ExecutionPaths;

				public ThreadFork(Thread thread, ThreadContext context)
				{
					Count = 1;
					Thread = thread;
					Context = context;
					ExecutionPaths = MemorySpan<int>.Empty;
					ExecutionPathsHead = 0;
				}

				public ThreadFork(int count, Thread thread, ThreadContext context, MemorySpan<int> executionPaths)
				{
					Count = count;
					Thread = thread;
					Context = context;
					ExecutionPaths = executionPaths;
					ExecutionPathsHead = 0;
				}

				public void Dispose()
				{
					Thread.Dispose();
					Context.Dispose();
					ExecutionPaths.Dispose();

					Thread = default;
					Context = default;
					ExecutionPaths = default;
				}

				public bool IsEmpty => Count < 0;
			}
		}
	}
}