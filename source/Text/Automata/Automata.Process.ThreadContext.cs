// <copyright file="Automata.Process.ThreadContext.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

// ReSharper disable ForCanBeConvertedToForeach

using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using Zaaml.Core.Reflection;

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		partial class Process
		{
			internal struct ThreadContext
			{
				public static ThreadContext Empty;

				public static readonly FieldInfo InstructionStreamPointerFieldInfo = typeof(ThreadContext).GetField(nameof(InstructionStreamPointer), BF.IPNP);
				public static readonly FieldInfo InstructionStreamFieldInfo = typeof(ThreadContext).GetField(nameof(InstructionStream), BF.IPNP);
				public int Index;
				public Process Process;
				public int ExecutionStreamPointer;
				public int InstructionStreamPointer;
				public int PredicateResultStreamPointer;
				public ExecutionStream ExecutionStream;
				public AutomataContextState ContextState;
				public InstructionStream InstructionStream;
				public List<ExecutionPath> ExecutionPathRegistry;
				public PredicateResultStream PredicateResultStream;
				public ExecutionPathMethodCollection ExecutionMethodRegistry;
				public bool IsExecutionStreamRunning;

				public ThreadContext(Process process, InstructionStream instructionStream, ExecutionStream executionStream, PredicateResultStream predicateResultStream, AutomataContextState contextState)
				{
					Index = 0;
					Process = process;
					ContextState = contextState;
					ExecutionStream = executionStream.AddReference();
					InstructionStream = instructionStream.AddReference();
					PredicateResultStream = predicateResultStream.AddReference();
					ExecutionStreamPointer = 0;
					InstructionStreamPointer = 0;
					PredicateResultStreamPointer = 0;
					ExecutionPathRegistry = Process._automata._executionPathRegistry;
					ExecutionMethodRegistry = Process.ILGenerator.MainExecutionMethods;
					IsExecutionStreamRunning = false;

					InstructionStream.LockPointer(InstructionStreamPointer);
				}

				public ThreadContext(int index, Process process, InstructionStream instructionStream, int instructionPointer, ExecutionStream executionStream, int executionStreamPointer, PredicateResultStream predicateResultStream,
					int predicateResultStreamPointer, AutomataContextState contextState)
				{
					Debug.Assert(index > 0);

					Index = index;
					Process = process;
					ContextState = contextState;
					ExecutionStream = executionStream.AddReference();
					InstructionStream = instructionStream.AddReference();
					PredicateResultStream = predicateResultStream.AddReference();
					ExecutionStreamPointer = executionStreamPointer;
					InstructionStreamPointer = instructionPointer;
					PredicateResultStreamPointer = predicateResultStreamPointer;
					ExecutionPathRegistry = Process._automata._executionPathRegistry;
					ExecutionMethodRegistry = Process.ILGenerator.ParallelExecutionMethods;

					IsExecutionStreamRunning = false;
					InstructionStream.LockPointer(InstructionStreamPointer);
				}

				public void EnqueueParallelPath(ExecutionPath executionPath)
				{
					ExecutionStream.Enqueue(executionPath, ref ExecutionStreamPointer);

					if (executionPath.ForkPredicatePath)
						executionPath.AddReference();
				}

				public ref TInstruction Instruction => ref InstructionStream.PeekInstructionRef(InstructionStreamPointer);

				public int InstructionOperand => InstructionStream.PeekOperand(InstructionStreamPointer);

				public int InstructionStreamPosition => InstructionStream.GetPosition(InstructionStreamPointer);

				public ref TInstruction GetInstructionOperand(out int operand)
				{
					return ref InstructionStream.PeekInstructionOperand(InstructionStreamPointer, out operand);
				}

				public void RunExecutionStream(ref Thread thread)
				{
					Debug.Assert(ReferenceEquals(thread.Stack, Process._stack));

					IsExecutionStreamRunning = true;

					var executionQueue = ExecutionStream.GetSpan(ExecutionStreamPointer);

					InstructionStream.UnlockPointer(InstructionStreamPointer);

					PredicateResultStreamPointer = 0;

					foreach (var executionPathId in executionQueue)
						thread.Node = Execute(executionPathId, ref thread);

					ExecutionStreamPointer = 0;
					PredicateResultStreamPointer = 0;

					InstructionStream.LockPointer(InstructionStreamPointer);

					IsExecutionStreamRunning = false;
				}

				public void Dispose()
				{
					ExecutionStream.ReleaseReference();
					InstructionStream.UnlockPointer(InstructionStreamPointer);
					InstructionStream.ReleaseReference();
					PredicateResultStream.ReleaseReference();
					Process._context.DisposeContextStateInternal(ContextState);

					Process = null;
					ContextState = null;
					ExecutionStream = null;
					InstructionStream = null;
					PredicateResultStream = null;
				}

				public ThreadContext Fork()
				{
					return new ThreadContext(Index + 1, Process, InstructionStream, InstructionStreamPointer, ExecutionStream, ExecutionStreamPointer, PredicateResultStream, PredicateResultStreamPointer, ContextState);
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public Node Execute(int executionPathId, ref Thread thread)
				{
					//if (Index == 0)
					//	Debug.WriteLine(executionPathId);

					return ExecutionMethodRegistry.GetExecutionPathMethod(ExecutionPathRegistry[executionPathId]).Execute(Process, ref thread, ref this);
				}

				public void EnqueuePredicateResult(PredicateResult predicateResult)
				{
					PredicateResultStream.Enqueue(predicateResult, ref PredicateResultStreamPointer);
				}

				public PredicateResult DequeuePredicateResult()
				{
					return PredicateResultStream.Dequeue(ref PredicateResultStreamPointer);
				}
			}
		}
	}
}