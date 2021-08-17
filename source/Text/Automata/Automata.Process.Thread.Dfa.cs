// <copyright file="Automata.Process.Thread.Dfa.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using Zaaml.Core;

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		private readonly MemorySpanAllocator<int> _dfaAllocator = MemorySpanAllocator.Create(ArrayPool<int>.Shared);

		partial class Process
		{
			internal partial struct Thread
			{
				private int BuildExecutionPathsDfa(ref ThreadContext context, ref MemorySpan<int> executionPaths, out int executionPathLength)
				{
					var instructionPointer = context.InstructionStreamPointer;
					var dfaThread = Node.GetDfaThread(Stack);

					Loop_Read:

					var operand = context.InstructionStream.FetchReadOperand(ref instructionPointer);
					var nextDfa = dfaThread.Transitions[operand];

					Loop_Build:
					switch (nextDfa.Switch)
					{
						case Dfa.SwitchBuild:

							nextDfa = dfaThread.BuildNext(operand, ref executionPaths);

							goto Loop_Build;

						case Dfa.SwitchContinue:

							dfaThread = nextDfa;

							goto Loop_Read;

						case Dfa.SwitchBreak:

							var executionPathCount = nextDfa.GetExecutionPaths(ref this, ref context, ref executionPaths, out executionPathLength, out var precedence);

							return executionPathCount;
							//return precedence ? BuildExecutionPathsPrecedence(ref context, ref executionPaths, ref executionPathLength) : executionPathCount;

						default:
							
							executionPathLength = 0;

							return 0;
					}
				}

				public sealed class Dfa
				{
					const int MaxDfaDepth = 8;
					public const int SwitchBuild = 0;
					public const int SwitchContinue = 1;
					public const int SwitchBreak = 2;

					private static readonly Dfa Sentinel = new(true);
					private readonly Automata<TInstruction, TOperand> _automata;
					private readonly Dfa _source;
					private readonly ReadOnlyMemorySpan<int> _executionPath;
					private readonly bool _precedence;
					private readonly int _executionPathCount;
					private readonly int _depth;

					private Dfa()
					{
						Transitions = new Dfa[InstructionsRange.Maximum + 1];
						_executionPath = ReadOnlyMemorySpan<int>.Empty;

						for (var i = 0; i < Transitions.Length; i++)
							Transitions[i] = Sentinel;
					}

					public Dfa(Node node, AutomataStack stack) : this()
					{
						_automata = node.Automata;
						_depth = 0;
						Threads = new[] { new DfaThread(new Thread(node, stack), -1)  };
					}

					private Dfa(Automata<TInstruction, TOperand> automata, Dfa source, int operand, ref MemorySpan<int> executionPaths) : this()
					{
						_automata = automata;
						_source = source;
						_depth = source._depth + 1;
						_executionPathCount = 0;
						_executionPath = ReadOnlyMemorySpan<int>.Empty;

						var threadList = new List<DfaThread>();
						var dfaExecutionPathLength = 0;

						for (var index = 0; index < source.Threads.Length; index++)
						{
							ref var dfaThread = ref source.Threads[index];
							ref var thread = ref dfaThread.Thread;

							var count = thread.BuildExecutionPathsDfa(operand, ref executionPaths, out var executionPathLength, out var precedence, out var predicate);

							_precedence |= precedence;

							if (count == -1 || predicate)
							{
								_executionPathCount = source._executionPathCount;
								_executionPath = source._executionPath;
								Threads = Array.Empty<DfaThread>();
								Switch = SwitchBreak;

								return;
							}

							var executionPathSpan = executionPaths.Span.Slice(0, executionPathLength);
							var executionPathHead = 0;

							for (var i = 0; i < count; i++)
							{
								var nextThread = EvalDfaThread(ref thread, executionPathSpan, ref executionPathHead);

								threadList.Add(new DfaThread(nextThread, _depth == 1 ? i : dfaThread.ExecutionPath) );

								dfaExecutionPathLength += nextThread.EntryExecution.Length + 1;
							}
						}

						CollapseThreads(threadList, ref dfaExecutionPathLength);
						SelectPath(threadList, ref dfaExecutionPathLength);


						Threads = threadList.ToArray();

						if (Threads.Length == 0)
							Switch = SwitchBreak;
						else
						{
							if (_depth >= MaxDfaDepth)
								Switch = SwitchBreak;
							else
								Switch = Threads.Length == 1 || operand == 0 ? SwitchBreak : SwitchContinue;

							var dfaExecutionPath = _automata._dfaAllocator.Allocate(dfaExecutionPathLength);
							var executionPathHead = 0;

							for (var index = 0; index < Threads.Length; index++)
							{
								var entryExecutionSpan = Threads[index].Thread.EntryExecution.Span;

								entryExecutionSpan.CopyTo(dfaExecutionPath.Span.Slice(executionPathHead));
								dfaExecutionPath.Span[entryExecutionSpan.Length + executionPathHead++] = 0;
								executionPathHead += entryExecutionSpan.Length;
							}

							_executionPath = dfaExecutionPath;
							_executionPathCount = Threads.Length;
						}
					}

					private void SelectPath(List<DfaThread> threads, ref int dfaExecutionPathLength)
					{
						if (threads.Count <= 1 || _depth <= 1) 
							return;

						var firstPath = threads[0].ExecutionPath;

						for (var i = 1; i < threads.Count; i++)
						{
							if (firstPath == threads[i].ExecutionPath)
								continue;

							return;
						}

						threads.Clear();

						var dfa = this;

						while (dfa._depth != 1) 
							dfa = dfa._source;

						threads.Add(dfa.Threads[firstPath]);

						dfaExecutionPathLength = threads[0].Thread.EntryExecution.Length + 1;
					}

					private static void CollapseThreads(List<DfaThread> threads, ref int dfaExecutionPathLength)
					{
						if (threads.Count == 0)
							return;

						var firstThread = threads[0].Thread;

						for (var i = 1; i < threads.Count; i++)
						{
							var thread = threads[i].Thread;

							if (ReferenceEquals(firstThread.Node, thread.Node) && AutomataStack.FullEqualityComparer.Equals(firstThread.Stack, thread.Stack))
								continue;

							return;
						}

						threads.RemoveRange(1, threads.Count - 1);
						dfaExecutionPathLength = threads[0].Thread.EntryExecution.Length + 1;
					}

					private Dfa(bool sentinel)
					{
						Debug.Assert(sentinel);

						Transitions = null;
						Switch = SwitchBuild;
					}

					public int Switch { get; }

					private DfaThread[] Threads { get; }

					public Dfa[] Transitions { get; }

					public Dfa BuildNext(int operand, ref MemorySpan<int> executionPaths)
					{
						var buildNext = new Dfa(_automata, this, operand, ref executionPaths);

						Transitions[operand] = buildNext;

						return buildNext;
					}

					private Thread EvalDfaThread(ref Thread thread, ReadOnlySpan<int> executionPaths, ref int executionPathHead)
					{
						var start = executionPathHead;

						while (executionPaths[executionPathHead++] != 0)
						{
						}

						var prevEntryPointLength = thread.EntryExecution.IsEmpty ? 0 : thread.EntryExecution.Length;
						var executionPathLength = executionPathHead - start - 1;
						var executionPath = executionPaths.Slice(start, executionPathLength);
						var entryPointLength = prevEntryPointLength + executionPathLength;
						var dfaExecutionPath = _automata._dfaAllocator.Allocate(entryPointLength);

						thread.EntryExecution.Span.CopyTo(dfaExecutionPath.Span);

						executionPath.CopyTo(dfaExecutionPath.Span.Slice(prevEntryPointLength));

						var nextStack = thread.Stack.EvalDfa(executionPath, out var nextNode);

						return new Thread(nextNode, nextStack, dfaExecutionPath);
					}

					public int GetExecutionPaths(ref Thread thread, ref ThreadContext threadContext, ref MemorySpan<int> executionPaths, out int executionPathLength, out bool precedence)
					{
						precedence = _precedence;

						var executionPathSpan = _executionPath.Span;

						executionPathSpan.CopyTo(executionPaths.Span);

						executionPathLength = executionPathSpan.Length;

						return _executionPathCount;
					}

					private struct DfaThread
					{
						public DfaThread(Thread thread, int executionPath)
						{
							Thread = thread;
							ExecutionPath = executionPath;
						}

						public Thread Thread;
						public int ExecutionPath;
					}
				}
			}
		}
	}
}