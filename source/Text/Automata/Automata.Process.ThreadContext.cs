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
			private protected struct ThreadContext
			{
				public static ThreadContext Empty;

				public static readonly FieldInfo InstructionStreamPointerFieldInfo = typeof(ThreadContext).GetField(nameof(InstructionStreamPointer), BF.IPNP);
				public static readonly FieldInfo InstructionStreamFieldInfo = typeof(ThreadContext).GetField(nameof(InstructionStream), BF.IPNP);
				public static readonly MethodInfo CompleteBlockMethodInfo = typeof(ThreadContext).GetMethod(nameof(CompleteBlock), BF.IPNP);
				public int Index;
				public Process Process;
				public int ExecutionStreamPointer;
				public int InstructionStreamPointer;
				public int PredicateResultStreamPointer;
				public ExecutionStream ExecutionStream;
				public AutomataContext AutomataContext;
				public AutomataContextState AutomataContextState;
				public InstructionStream InstructionStream;
				public List<ExecutionPath> ExecutionPathRegistry;
				public PredicateResultStream PredicateResultStream;
				public int ExecutionMethodIndex;
				public bool IsExecutionStreamRunning;
				public bool IsCompleteBlock;

				public ThreadContext(Process process, InstructionStream instructionStream, ExecutionStream executionStream, PredicateResultStream predicateResultStream, AutomataContext automataContext)
				{
					Index = 0;
					Process = process;
					AutomataContext = automataContext;
					AutomataContextState = default;
					ExecutionStream = executionStream;
					InstructionStream = instructionStream;
					PredicateResultStream = predicateResultStream;
					ExecutionStreamPointer = 0;
					InstructionStreamPointer = 0;
					PredicateResultStreamPointer = 0;
					ExecutionPathRegistry = Process._automata._executionPathRegistry;
					ExecutionMethodIndex = Process.ILGenerator.MainExecutionMethodIndex;
					IsExecutionStreamRunning = false;
					IsCompleteBlock = false;

					InstructionStream.LockPointer(InstructionStreamPointer);

					InstructionStream.AddReference();
				}

				public ThreadContext(int index, Process process, InstructionStream instructionStream, int instructionPointer, ExecutionStream executionStream, int executionStreamPointer, PredicateResultStream predicateResultStream,
					int predicateResultStreamPointer, AutomataContext context, AutomataContextState contextState)
				{
					Debug.Assert(index > 0);

					Index = index;
					Process = process;
					AutomataContext = context;
					AutomataContextState = contextState;
					ExecutionStream = executionStream;
					InstructionStream = instructionStream;
					PredicateResultStream = predicateResultStream;
					ExecutionStreamPointer = executionStreamPointer;
					InstructionStreamPointer = instructionPointer;
					PredicateResultStreamPointer = predicateResultStreamPointer;
					ExecutionPathRegistry = Process._automata._executionPathRegistry;
					ExecutionMethodIndex = Process.ILGenerator.ParallelExecutionMethodIndex;

					IsExecutionStreamRunning = false;
					IsCompleteBlock = false;

					InstructionStream.LockPointer(InstructionStreamPointer);

					InstructionStream.AddReference();
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public void EnqueueParallelPath(ExecutionPath executionPath)
				{
					ExecutionStream.Enqueue(executionPath.Id, ExecutionStreamPointer++);

					if (executionPath.ForkPredicatePath)
						executionPath.AddReference();
				}

				public ref TInstruction Instruction => ref InstructionStream.PeekInstructionRef(InstructionStreamPointer);

				public int InstructionOperand
				{
					[MethodImpl(MethodImplOptions.AggressiveInlining)]
					get { return InstructionStream.PeekOperand(InstructionStreamPointer); }
				}

				public int FetchInstructionOperand
				{
					[MethodImpl(MethodImplOptions.AggressiveInlining)]
					get { return InstructionStream.FetchPeekOperand(InstructionStreamPointer); }
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public int FetchPeekOperand(int delta)
				{
					return InstructionStream.FetchPeekOperand(InstructionStreamPointer + delta);
				}

				public TInstruction FetchInstruction
				{
					[MethodImpl(MethodImplOptions.AggressiveInlining)]
					get { return InstructionStream.FetchPeekInstruction(InstructionStreamPointer); }
				}

				public int InstructionStreamPosition => InstructionStream.GetPosition(InstructionStreamPointer);

				public ref TInstruction GetInstructionOperand(out int operand)
				{
					return ref InstructionStream.PeekInstructionOperand(InstructionStreamPointer, out operand);
				}

				public void CompleteBlock()
				{
					IsCompleteBlock = true;
				}

				public void RunExecutionStream(ref Thread thread)
				{
					IsExecutionStreamRunning = true;

					var executionQueue = ExecutionStream.GetSpan(ExecutionStreamPointer);

					InstructionStream.UnlockPointer(InstructionStreamPointer);

					PredicateResultStreamPointer = 0;

					foreach (var executionPathId in executionQueue)
						thread.Node = Execute(executionPathId);

					ExecutionStreamPointer = 0;
					PredicateResultStreamPointer = 0;

					InstructionStream.LockPointer(InstructionStreamPointer);

					IsExecutionStreamRunning = false;
				}

				public void Dispose()
				{
					InstructionStream.UnlockPointer(InstructionStreamPointer);
					InstructionStream.ReleaseReference();
					
					AutomataContext.DisposeContextStateInternal(AutomataContextState);
					
					IsCompleteBlock = false;
					Process = null;
					AutomataContext = null;
					AutomataContextState = null;
					ExecutionStream = null;
					InstructionStream = null;
					PredicateResultStream = null;
				}

				public ThreadContext Fork()
				{
					return new ThreadContext(Index + 1, Process, InstructionStream, InstructionStreamPointer, ExecutionStream, ExecutionStreamPointer, PredicateResultStream, PredicateResultStreamPointer, AutomataContext, AutomataContext.CloneContextStateInternal(AutomataContextState));
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public Node Execute(int executionPathId)
				{
					var executionPath = ExecutionPathRegistry[executionPathId];

					return executionPath.Execute(ExecutionMethodIndex, Process);
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