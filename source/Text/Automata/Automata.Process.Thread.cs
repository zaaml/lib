// <copyright file="Automata.Process.Thread.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

// ReSharper disable ForCanBeConvertedToForeach

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		#region Nested Types

		partial class Process
		{
			#region Nested Types

			private struct Thread
			{
				public static readonly Thread Empty = new Thread();

				#region Fields

				public PredicateResultQueue PredicateResultQueue;
				public ExecutionPathQueue ExecutionQueue;
				public ExecutionPathQueue StartExecutionQueue;
				public Node CurrentNode;
				public int InstructionPointer;
				public ThreadParent Parent;
				public AutomataStack Stack;
				public AutomataContextState ContextState;
				public InstructionStream InstructionStream;

				public ulong[] DfaTrails;
				public int DfaTrailIndex;

				#endregion

				public Thread(Node node, InstructionStream instructionStream, AutomataContextState contextState) : this()
				{
					CurrentNode = node;
					InstructionPointer = InstructionStream.InitializeInstructionPointer();
					InstructionStream = instructionStream.AddReference();
					ContextState = contextState;
				}

				public Thread(ThreadParent parent, Node node, InstructionStream instructionStream, int instructionPointer, AutomataContextState contextState) : this()
				{
					CurrentNode = node;
					InstructionPointer = instructionPointer;
					ContextState = contextState;
					InstructionStream = instructionStream.AddReference();

					InstructionStream.LockPointer(InstructionPointer);

					if (parent == null)
						return;

					Parent = parent;
					Parent.AddReference();
				}

				public Thread(ThreadParent parent, Node node, InstructionStream instructionStream, int instructionPointer, AutomataContextState contextState, ulong[] dfaTrails, int dfaTrailIndex) : this()
				{
					CurrentNode = node;
					InstructionPointer = instructionPointer;
					ContextState = contextState;
					InstructionStream = instructionStream.AddReference();

					InstructionStream.LockPointer(InstructionPointer);

					DfaTrails = dfaTrails;
					DfaTrailIndex = dfaTrailIndex;

					if (parent == null)
						return;

					Parent = parent;
					Parent.AddReference();
				}

				#region Methods

				public void EnsureStartExecutionQueue()
				{
					StartExecutionQueue ??= Parent.ExecutionQueue.Pool.Get().AddReference();
				}

				public void EnqueueStartPath(ExecutionPath executionPath)
				{
					StartExecutionQueue.List.Add(executionPath);
				}

				public void EnqueuePath(ExecutionPath executionPath)
				{
					ExecutionQueue.List.Add(executionPath);
				}

				public void EnqueuePredicateResult(PredicateResult predicateResult)
				{
					PredicateResultQueue ??= Parent.PredicateResultQueue.Pool.Get().AddReference();
					PredicateResultQueue.Queue.Enqueue(predicateResult);
				}

				#endregion

				public ref TInstruction Instruction => ref InstructionStream.PeekInstruction(InstructionPointer);

				public int InstructionOperand => InstructionStream.PeekOperand(InstructionPointer);

				public int InstructionStreamPosition => InstructionStream.GetPosition(InstructionPointer);

				public ref TInstruction GetInstructionOperand(out int operand)
				{
					return ref InstructionStream.PeekInstructionOperand(InstructionPointer, out operand);
				}

				public bool OwnStack => Stack != null && ReferenceEquals(Stack, Parent?.Stack) == false;

				public bool OwnPredicateResult => PredicateResultQueue != null && ReferenceEquals(PredicateResultQueue, Parent?.PredicateResultQueue) == false;

				public bool OwnExecutionQueue => ExecutionQueue != null;

				public void Dispose(Process process)
				{
					if (Parent == null)
					{
						Stack?.ReleaseReference();
						ExecutionQueue?.ReleaseReference();
						PredicateResultQueue?.ReleaseReference();
					}
					else
					{
						if (ReferenceEquals(Stack, Parent.Stack) == false)
							Stack?.ReleaseReference();

						if (ReferenceEquals(ExecutionQueue, Parent.ExecutionQueue) == false)
							ExecutionQueue?.ReleaseReference();

						if (ReferenceEquals(PredicateResultQueue, Parent.PredicateResultQueue) == false)
							PredicateResultQueue?.ReleaseReference();

						Parent.ReleaseReference();
					}

					InstructionStream.UnlockPointer(InstructionPointer);

					StartExecutionQueue?.ReleaseReference();
					InstructionStream.ReleaseReference();

					process.Context.DisposeContextStateInternal(ContextState);

					DfaTrails = null;
					DfaTrailIndex = -1;

					Stack = null;
					Parent = null;
					ContextState = null;
					ExecutionQueue = null;
					InstructionStream = null;
					StartExecutionQueue = null;
				}
			}

			#endregion
		}

		#endregion
	}
}