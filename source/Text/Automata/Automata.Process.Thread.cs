// <copyright file="Automata.Process.Thread.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

// ReSharper disable ForCanBeConvertedToForeach

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Zaaml.Core;

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		partial class Process
		{
			private struct Thread
			{
				public Node Node;
				public AutomataStack Stack;
				public ReadOnlyMemorySpan<int> EntryExecution;

				public Thread(Node node, AutomataStack stack)
				{
					Node = node;
					Stack = stack.AddReference();
					EntryExecution = MemorySpan<int>.Empty;
				}

				public Thread(Node node, AutomataStack stack, ReadOnlyMemorySpan<int> entryExecution)
				{
					Node = node;
					Stack = stack.AddReference();
					EntryExecution = entryExecution;
				}

				private int BuildExecutionPathNoReturn(ref ThreadContext context, ref MemorySpan<int> executionPathsMemorySpan, out int executionPathLength)
				{
					var executionPaths = executionPathsMemorySpan.Span;
					var instructionPointer = context.InstructionStreamPointer;
					var aheadLength = context.Process._automata.ExecutionPathLookAheadLength;
					var operandSpan = context.InstructionStream.PrefetchReadOperandSpan(instructionPointer, aheadLength);
					var operand = operandSpan[0];
					var pathCount = 0;
					var executionPathGroup = Node.GetExecutionPathsFastSafe(operand);

					executionPathLength = 0;

					for (var index = 0; index < executionPathGroup.Length; index++)
					{
						var executionPath = executionPathGroup[index];

						if (executionPath.OutputEnd && operand != 0 && context.Process._processKind == ProcessKind.Process)
							continue;

						var lookAhead = executionPath.LookAheadPath;

						if (lookAhead != null)
						{
							if (LookAhead(lookAhead, instructionPointer, aheadLength, operandSpan, ref context) == false)
								continue;

							executionPath = lookAhead;
						}

						executionPaths[executionPathLength++] = executionPath.Id;

						pathCount++;
					}

					return pathCount;
				}

				private int BuildExecutionPathReturn(ref ThreadContext context, ref MemorySpan<int> executionPathsMemorySpan, out int executionPathLength)
				{
					var executionPaths = executionPathsMemorySpan.Span;
					var instructionPointer = context.InstructionStreamPointer;
					var currentNode = Node;
					var aheadLength = context.Process._automata.ExecutionPathLookAheadLength;
					var operandSpan = context.InstructionStream.PrefetchReadOperandSpan(instructionPointer, aheadLength);
					var operand = operandSpan[0];
					var pathCount = 0;
					var returnDepth = 0;
					var returnExecutionPathsHead = executionPaths.Length;
					var executionPathsHead = 0;

					while (true)
					{
						var executionPathGroup = currentNode.GetExecutionPathsFastSafe(operand);

						for (var index = 0; index < executionPathGroup.Length; index++)
						{
							var executionPath = executionPathGroup[index];

							if (executionPath.OutputReturn)
							{
								executionPaths[--returnExecutionPathsHead] = returnDepth;
								executionPaths[--returnExecutionPathsHead] = executionPath.Id;
							}
							else
							{
								if (executionPath.OutputEnd && operand != 0 && context.Process._processKind == ProcessKind.Process)
									continue;

								var lookAhead = executionPath.LookAheadPath;

								if (lookAhead != null)
								{
									if (LookAhead(lookAhead, instructionPointer, aheadLength, operandSpan, ref context) == false)
										continue;

									executionPath = lookAhead;
								}

								for (var i = 0; i < returnDepth; i++)
									executionPaths[executionPathsHead + returnDepth + i + 1] = executionPaths[executionPathsHead + i];

								executionPaths[executionPathsHead + returnDepth] = executionPath.Id;
								executionPathsHead += returnDepth + 1;

								pathCount++;
							}
						}

						if (returnExecutionPathsHead == executionPaths.Length)
							break;

						var path = executionPaths[returnExecutionPathsHead++];
						var rd = executionPaths[returnExecutionPathsHead++];

						currentNode = Stack.PeekLeaveNode(rd);
						currentNode.EnsureSafe();

						executionPaths[executionPathsHead + rd] = path;

						returnDepth = rd + 1;
					}

					executionPathLength = executionPathsHead;

					return pathCount;
				}

				public void Dispose()
				{
					Stack.ReleaseReference();

					EntryExecution = ReadOnlyMemorySpan<int>.Empty;
					Stack = null;
					Node = null;
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				private void Eval(ExecutionPath executionPath, ref ThreadContext context)
				{
					Node = Stack.Eval(executionPath);
					context.InstructionStreamPointer += executionPath.LookAheadMatch.Length;
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				private void Eval(ExecutionPath executionPath)
				{
					Node = Stack.Eval(executionPath);
				}

				private ThreadStatusKind Run(ref ThreadContext context, ReadOnlySpan<int> executionPaths)
				{
					context.InstructionStream.UnlockPointer(context.InstructionStreamPointer);

					for (var index = 0; index < executionPaths.Length; index++)
					{
						var executionPath = executionPaths[index];

						Node = context.Execute(executionPath);

						if (Node.ThreadStatusKind == ThreadStatusKind.Fork)
							return ForkThreadNode((ForkNode)Node, executionPaths.Slice(index), ref context);
					}

					context.InstructionStream.LockPointer(context.InstructionStreamPointer);

					return Node.ThreadStatusKind;
				}

				private ThreadStatusKind ForkThreadNode(ForkNode forkNode, ReadOnlySpan<int> executionPaths, ref ThreadContext context)
				{
					context.InstructionStream.LockPointer(context.InstructionStreamPointer);

					var forkPredicateResult = (IForkPredicateResult)forkNode.PredicateResult;
					var startNode = (PredicateNode)forkNode.ExecutionPath.Nodes[forkNode.NodeIndex];
					var processResources = context.Process._processResources;
					var forkPaths = processResources.IntMemorySpanAllocator.Allocate(executionPaths.Length * 2 + 2);
					var forkPathsSpan = forkPaths.Span;
					var forkPathsHead = 0;

					for (var i = 0; i <= 1; i++)
					{
						var predicateEntry = i == 0 ? forkPredicateResult.First : forkPredicateResult.Second;
						var predicateNode = processResources.ForkPredicateNodePool.Get().Mount(predicateEntry);
						var predicateExecutionPath = processResources.ForkExecutionPathPool.Get().Mount(startNode, predicateNode);

						predicateNode.CopyLookup(forkNode);

						if (startNode.ForkPathIndex == -1)
						{
							lock (startNode)
							{
								if (startNode.ForkPathIndex == -1)
								{
									context.Process._automata.RegisterExecutionPath(predicateExecutionPath);
									startNode.ForkPathIndex = predicateExecutionPath.Id;
								}
								else
									predicateExecutionPath.Id = startNode.ForkPathIndex;
							}
						}
						else
							predicateExecutionPath.Id = startNode.ForkPathIndex;

						predicateNode.ForkPathIndex = startNode.ForkPathIndex;

						forkPathsSpan[forkPathsHead++] = predicateExecutionPath.Id;

						foreach (var executionPath in executionPaths)
							forkPathsSpan[forkPathsHead++] = executionPath;
					}

					forkNode.Release();

					return context.Process.ForkThread(ref this, ref context, forkPaths.Span);
				}

				private static bool LookAhead(ExecutionPath executionPath, int instructionPointer, int aheadLength, ReadOnlySpan<int> operandSpan, ref ThreadContext context)
				{
					var lookAheadMatch = executionPath.LookAheadMatch;
					var localIndex = 1;

					for (var i = 1; i < lookAheadMatch.Length; i++)
					{
						if (localIndex >= operandSpan.Length)
						{
							operandSpan = context.InstructionStream.PrefetchReadOperandSpan(instructionPointer + i, aheadLength);
							localIndex = 0;
						}

						var operand = operandSpan[localIndex++];

						if (lookAheadMatch[i].Match(operand) == false)
							return false;
					}

					return true;
				}

				private ThreadStatusKind RunEval(ref ThreadContext context)
				{
					var process = context.Process;

					if (process._processKind == ProcessKind.SubProcess || process._automata.HasPredicates)
						return ThreadStatusKind.Run;

					var executionStream = context.ExecutionStream;
					var executionMethods = process.ILGenerator.MainExecutionMethods;
					var executionPaths = context.Process._executionPathBuilder;
					var executionPathRegistry = process._automata._executionPathRegistry;
					var executionStreamPointer = 0;
					var evalThread = Fork(ReadOnlyMemorySpan<int>.Empty);
					var instructionStreamPointer = context.InstructionStreamPointer;

					while (true)
					{
						switch (evalThread.BuildExecutionPaths(ref context, ref executionPaths, out var executionPathLength))
						{
							case 0:

								goto loop_exit;

							case 1:

								foreach (var executionPathId in executionPaths.Span.Slice(0, executionPathLength))
								{
									var executionPath = executionPathRegistry[executionPathId];

									executionStream.Enqueue(executionPath, ref executionStreamPointer);

									evalThread.Eval(executionPath, ref context);
								}

								break;

							default:

								goto loop_exit;
						}

						if (executionStreamPointer <= 256)
							continue;

						var executionStreamSpan = executionStream.GetSpan(executionStreamPointer);

						context.InstructionStreamPointer = instructionStreamPointer;

						foreach (var executionPathId in executionStreamSpan)
						{
							var executionPath = executionPathRegistry[executionPathId];

							Node = executionMethods.GetExecutionPathMethod(executionPath).Execute(process);
						}

						instructionStreamPointer = context.InstructionStreamPointer;

						executionStreamPointer = 0;
					}

					loop_exit:
					{
						var executionStreamSpan = executionStream.GetSpan(executionStreamPointer);

						context.InstructionStreamPointer = instructionStreamPointer;

						foreach (var executionPathId in executionStreamSpan)
						{
							var executionPath = executionPathRegistry[executionPathId];

							Node = executionMethods.GetExecutionPathMethod(executionPath).Execute(process);
						}
					}

					(Stack, evalThread.Stack) = (evalThread.Stack, Stack);

					evalThread.Dispose();

					return Node.ThreadStatusKind;
				}

				public ThreadStatusKind Run(ref ThreadContext context)
				{
					Debug.Assert(ReferenceEquals(Stack, context.Process._stack));

					if (EntryExecution.IsEmpty == false)
					{
						var entryStatus = Run(ref context, EntryExecution);

						if (entryStatus != ThreadStatusKind.Run)
							return entryStatus;
					}

					//if (RunEval(ref context) != ThreadStatusKind.Run)
					//	return Node.ThreadStatusKind;

					var executionPaths = context.Process._executionPathBuilder;

					while (true)
					{
						switch (BuildExecutionPaths(ref context, ref executionPaths, out var executionPathLength))
						{
							case 0:

								return Node.ThreadStatusKind == ThreadStatusKind.Finished ? ThreadStatusKind.Finished : ThreadStatusKind.Unexpected;

							case 1:

								if (Run(ref context, executionPaths.Span.Slice(0, executionPathLength)) == ThreadStatusKind.Run)
									continue;

								return Node.ThreadStatusKind;

							default:

								return context.Process.ForkThread(ref this, ref context, executionPaths.Span.Slice(0, executionPathLength));
						}
					}
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				private int BuildExecutionPaths(ref ThreadContext context, ref MemorySpan<int> executionPathsMemorySpan, out int executionPathLength)
				{
					Node.EnsureSafe();

					return Node.HasReturnPathSafe ? BuildExecutionPathReturn(ref context, ref executionPathsMemorySpan, out executionPathLength) : BuildExecutionPathNoReturn(ref context, ref executionPathsMemorySpan, out executionPathLength);
				}

				public Thread Clone()
				{
					return new Thread { Node = Node, Stack = Stack.Clone() };
				}

				public void StackExchange(ref Thread threadSource)
				{
					threadSource.Stack.CopyFrom(Stack);
					(Stack, threadSource.Stack) = (threadSource.Stack, Stack);
				}

				public Thread Fork(ReadOnlyMemorySpan<int> entryPath)
				{
					var stack = Stack;

					Stack = Stack.Fork().AddReference();

					var forkThread = new Thread(Node, stack, entryPath);

					stack.ReleaseReference();

					return forkThread;
				}
			}
		}
	}
}