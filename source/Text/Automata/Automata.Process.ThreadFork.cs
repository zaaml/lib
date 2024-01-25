// <copyright file="Automata.Process.ThreadFork.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

// ReSharper disable ForCanBeConvertedToForeach

using System.Reflection;
using System;

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		partial class Process
		{
			private struct ThreadFork
			{
				public static readonly Type Type = typeof(ThreadFork);
				public static FieldInfo ThreadField = Type.GetField(nameof(Thread));
				public static FieldInfo ContextField = Type.GetField(nameof(Context));

				public static ThreadFork Empty = new(default, default, ResetForkNode.Empty);

				public Thread Thread;
				public ThreadContext Context;
				public ExecutionRailList ExecutionRailList;
				public ResetForkNode ResetForkNode;

				public ThreadFork(Thread thread, ThreadContext context, ResetForkNode resetForkNode)
				{
					Thread = thread;
					Context = context;
					ExecutionRailList = ExecutionRailList.Start;
					ResetForkNode = resetForkNode;
				}

				public ThreadFork(Thread thread, ThreadContext context, ExecutionRailList executionRailList, ResetForkNode resetForkNode)
				{
					Thread = thread;
					Context = context;
					ExecutionRailList = executionRailList;
					ResetForkNode = resetForkNode;
				}

				public void Dispose()
				{
					Thread.Dispose();
					Context.Dispose();
					ExecutionRailList.Dispose();
					ResetForkNode.Dispose();

					this = Empty;
				}

				public bool IsEmpty => Thread.Node == null;

				public override string ToString()
				{
					if (IsEmpty)
						return "Empty";

					return $"Node:{Thread.Node}, InstructionPointer:{Context.InstructionStreamPointer}, ForkCount:{ExecutionRailList.Count}";
				}
			}
		}
	}
}