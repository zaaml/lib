// <copyright file="Automata.Process.Thread.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

// ReSharper disable ForCanBeConvertedToForeach

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Zaaml.Core;
using Zaaml.Core.Reflection;

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		partial class Process
		{
			internal partial struct Thread
			{
				public static readonly FieldInfo StackField = typeof(Thread).GetField(nameof(Stack), BF.IPNP);
				public Node Node;
				public AutomataStack Stack;
				public PrecedenceContextStack Precedence;
				public ReadOnlyMemorySpan<int> EntryExecution;

				public Thread(Node node, AutomataStack stack)
				{
					Node = node;
					Stack = stack.AddReference();
					Precedence = null;
					EntryExecution = MemorySpan<int>.Empty;
				}

				public Thread(Node node, AutomataStack stack, PrecedenceContextStack precedence)
				{
					Node = node;
					Stack = stack.AddReference();
					Precedence = precedence.AddReference();
					EntryExecution = MemorySpan<int>.Empty;
				}

				public Thread(Node node, AutomataStack stack, ReadOnlyMemorySpan<int> entryExecution)
				{
					Node = node;
					Stack = stack.AddReference();
					Precedence = null;
					EntryExecution = entryExecution;
				}

				public Thread(Node node, AutomataStack stack, PrecedenceContextStack precedence, ReadOnlyMemorySpan<int> entryExecution)
				{
					Node = node;
					Stack = stack.AddReference();
					Precedence = precedence.AddReference();
					EntryExecution = entryExecution;
				}

				private int BuildExecutionPathNoReturn(int operand, ref ThreadContext context, ref MemorySpan<int> executionPathsMemorySpan, out int executionPathLength, out bool precedence, out bool predicate)
				{
					var executionPaths = executionPathsMemorySpan.Span;
					var pathCount = 0;
					var executionPathGroup = Node.GetExecutionPathsFastSafe(operand);

					precedence = false;
					predicate = false;

					executionPathLength = 0;

					for (var index = 0; index < executionPathGroup.Length; index++)
					{
						var executionPath = executionPathGroup[index];

						precedence |= executionPath.HasPrecedenceNodes;
						predicate |= executionPath.Predicate != null;

						if (context.InstructionStream != null)
						{
							var lookAhead = executionPath.LookAheadPath;

							if (lookAhead != null)
							{
								if (LookAhead(lookAhead, ref context) == false)
									continue;

								executionPath = lookAhead;
							}

							if (executionPath.OutputEnd && operand != 0 && context.Process._processKind != ProcessKind.SubProcess)
								continue;
						}

						executionPaths[executionPathLength++] = executionPath.Id;
						executionPaths[executionPathLength++] = 0;

						pathCount++;
					}

					return pathCount;
				}

				private int BuildExecutionPathReturn(int operand, ref ThreadContext context, ref MemorySpan<int> executionPathsMemorySpan, out int executionPathLength, out bool precedence, out bool predicate)
				{
					var executionPaths = executionPathsMemorySpan.Span;
					var currentNode = Node;
					var pathCount = 0;
					var returnDepth = 0;
					var returnExecutionPathsHead = executionPaths.Length;
					var executionPathsHead = 0;

					precedence = false;
					predicate = false;

					while (true)
					{
						var executionPathGroup = currentNode.GetExecutionPathsFastSafe(operand);

						for (var index = 0; index < executionPathGroup.Length; index++)
						{
							var executionPath = executionPathGroup[index];

							precedence |= executionPath.HasPrecedenceNodes;
							predicate |= executionPath.Predicate != null;

							if (executionPath.OutputReturn)
							{
								executionPaths[--returnExecutionPathsHead] = returnDepth;
								executionPaths[--returnExecutionPathsHead] = executionPath.Id;
							}
							else
							{
								if (context.InstructionStream != null)
								{
									var lookAhead = executionPath.LookAheadPath;

									if (lookAhead != null)
									{
										if (LookAhead(lookAhead, ref context) == false)
											continue;

										executionPath = lookAhead;
									}

									if (executionPath.OutputEnd && operand != 0 && context.Process._processKind != ProcessKind.SubProcess)
										continue;
								}

								for (var i = 0; i < returnDepth; i++)
									executionPaths[executionPathsHead + returnDepth + i + 2] = executionPaths[executionPathsHead + i];

								executionPaths[executionPathsHead + returnDepth] = executionPath.Id;
								executionPathsHead += returnDepth + 1;

								executionPaths[executionPathsHead++] = 0;

								pathCount++;
							}
						}

						if (returnExecutionPathsHead == executionPaths.Length)
							break;

						var path = executionPaths[returnExecutionPathsHead++];
						var rd = executionPaths[returnExecutionPathsHead++];

						if (context.InstructionStream == null && rd >= Stack.HashCodeDepth)
						{
							executionPathLength = 0;

							return -1;
						}

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
					Precedence.ReleaseReference();

					EntryExecution = ReadOnlyMemorySpan<int>.Empty;
					Precedence = null;
					Stack = null;
					Node = null;
				}

				private ThreadStatusKind Run(ref ThreadContext context, ReadOnlySpan<int> executionPaths)
				{
					context.InstructionStream.UnlockPointer(context.InstructionStreamPointer);

					for (var index = 0; index < executionPaths.Length; index++)
					{
						var executionPath = executionPaths[index];

						Node = context.Execute(executionPath, ref this);

						switch (Node.ThreadStatusKind)
						{
							case ThreadStatusKind.Run:
								continue;

							case ThreadStatusKind.Fork:

								context.InstructionStream.LockPointer(context.InstructionStreamPointer);

								return ForkThreadNode((ForkNode)Node, executionPaths.Slice(index+1), ref context);

							default:

								context.InstructionStream.LockPointer(context.InstructionStreamPointer);

								return Node.ThreadStatusKind;
						}
					}

					context.InstructionStream.LockPointer(context.InstructionStreamPointer);

					return Node.ThreadStatusKind;
				}

				private bool TryEnterPrecedence(int precedenceEnterNodeId)
				{
					var precedenceEnterNode = (PrecedenceEnterNode)Node.Automata._nodeRegistry[precedenceEnterNodeId];

					return Precedence.TryEnter(precedenceEnterNode.Precedence);
				}

				private void LeavePrecedence(int precedenceEnterNodeId)
				{
					var precedenceLeaveNode = (PrecedenceLeaveNode)Node.Automata._nodeRegistry[precedenceEnterNodeId];

					Precedence.Leave(precedenceLeaveNode.Precedence);
				}

				private ThreadStatusKind ForkThreadNode(ForkNode forkNode, ReadOnlySpan<int> executionPaths, ref ThreadContext context)
				{
					var forkPredicateResult = (IForkPredicateResult)forkNode.PredicateResult;
					var startNode = forkNode.PredicateNode;
					var processResources = context.Process._processResources;
					var forkPaths = processResources.DynamicMemorySpanAllocator.Allocate(executionPaths.Length * 2 + 2 + 2);
					var forkPathsSpan = forkPaths.Span;
					var forkPathsHead = 0;

					for (var i = 0; i <= 1; i++)
					{
						var predicateEntry = i == 0 ? forkPredicateResult.First : forkPredicateResult.Second;
						var predicateNode = processResources.ForkPredicateNodePool.Get().Mount(predicateEntry);
						var predicateExecutionPath = processResources.ForkExecutionPathPool.Get().Mount(startNode, predicateNode);

						predicateNode.CopyLookup(forkNode);

						forkPathsSpan[forkPathsHead++] = predicateExecutionPath.Id;

						foreach (var executionPath in executionPaths)
							forkPathsSpan[forkPathsHead++] = executionPath;

						forkPathsSpan[forkPathsHead++] = 0;
					}

					forkNode.Release();

					return context.Process.ForkThread(ref this, ref context, forkPaths.Span);
				}

				private static bool LookAhead(ExecutionPath executionPath, ref ThreadContext context)
				{
					const int aheadLength = 4;

					var instructionPointer = context.InstructionStreamPointer;
					var operandSpan = context.InstructionStream.PrefetchReadOperandSpan(context.InstructionStreamPointer, aheadLength);
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

				public ThreadStatusKind Run(ref ThreadContext context)
				{
					Debug.Assert(ReferenceEquals(Stack, context.Process._stack));

					if (EntryExecution.IsEmpty == false)
					{
						var entryStatus = Run(ref context, EntryExecution);

						if (entryStatus != ThreadStatusKind.Run)
							return entryStatus;
					}

					var executionPaths = context.Process._executionPathBuilder;

					while (true)
					{
						var executionPathCount = BuildExecutionPaths(ref context, ref executionPaths, out var executionPathLength);
						
						switch (executionPathCount)
						{
							case 0:

								return Node.ThreadStatusKind == ThreadStatusKind.Finished ? ThreadStatusKind.Finished : ThreadStatusKind.Unexpected;

							case 1:

								if (Run(ref context, executionPaths.Span.Slice(0, executionPathLength - 1)) == ThreadStatusKind.Run)
									continue;

								return Node.ThreadStatusKind;

							default:
								var forkPaths = executionPaths.Span.Slice(0, executionPathLength);
#if false
								var fp = DumpForkPaths(ref this, ref context, forkPaths);
#endif
								return context.Process.ForkThread(ref this, ref context, forkPaths);
						}
					}
				}

				[UsedImplicitly]
				private static string DumpForkPaths(ref Thread thread, ref ThreadContext context, ReadOnlySpan<int> executionPaths)
				{
					var stringBuilder = new StringBuilder();
					var forkPath = 1;
					var executionPath = new List<ExecutionPath>();

					for (var i = 0; i < executionPaths.Length; i++)
					{
						var executionPathId = executionPaths[i];

						if (executionPathId == 0)
						{
							stringBuilder.Append($"ForkPath: {forkPath}\r\n");
							stringBuilder.Append(string.Join("\r\n", executionPath.SelectMany(e => e.Nodes).Select(n => n.ToString())));
							stringBuilder.Append("\r\n--------------------------------------------------------------\r\n");

							executionPath.Clear();
							forkPath++;
						}
						else
							executionPath.Add(context.Process._automata._executionPathRegistry[executionPathId]);
					}

					return stringBuilder.ToString();
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				private int BuildExecutionPaths(ref ThreadContext context, ref MemorySpan<int> executionPathsMemorySpan, out int executionPathLength)
				{
#if false
					Node.EnsureSafe();

					var operand = context.InstructionStream.FetchPeekOperand(context.InstructionStreamPointer);
					var executionPathCount = Node.HasReturnPathSafe
						? BuildExecutionPathReturn(operand, ref context, ref executionPathsMemorySpan, out executionPathLength, out var precedence, out _)
						: BuildExecutionPathNoReturn(operand, ref context, ref executionPathsMemorySpan, out executionPathLength, out precedence, out _);

					if (precedence == false)
						return executionPathCount > 1 ? BuildExecutionPathsDfa(ref context, ref executionPathsMemorySpan, out executionPathLength) : executionPathCount;

					executionPathCount = BuildExecutionPathsPrecedence(ref context, ref executionPathsMemorySpan, ref executionPathLength);

					return executionPathCount > 1 ? BuildExecutionPathsDfa(ref context, ref executionPathsMemorySpan, out executionPathLength) : executionPathCount;
#else
					Node.EnsureSafe();

					var operand = context.InstructionStream.FetchPeekOperand(context.InstructionStreamPointer);
					
					var executionPathCount = Node.HasReturnPathSafe
						? BuildExecutionPathReturn(operand, ref context, ref executionPathsMemorySpan, out executionPathLength, out var precedence, out _)
						: BuildExecutionPathNoReturn(operand, ref context, ref executionPathsMemorySpan, out executionPathLength, out precedence, out _);

					//if (precedence == false)
					//	return executionPathCount;

					//executionPathCount = BuildExecutionPathsPrecedence(ref context, ref executionPathsMemorySpan, ref executionPathLength);

					return executionPathCount;
#endif
				}

				private int BuildExecutionPathsPrecedence(ref ThreadContext threadContext, ref MemorySpan<int> executionPathsMemorySpan, ref int executionPathLength)
				{
					var executionPathRegistry = threadContext.Process._automata._executionPathRegistry;
					var executionPathSpan = executionPathsMemorySpan.Span.Slice(0, executionPathLength);
					var targetSpan = executionPathsMemorySpan.Span;
					var executionPathCount = 0;
					executionPathLength = 0;

					var precedenceContext = threadContext.Process._precedenceContext;
					var skipPath = false;
					var prevExecutionPathLength = 0;

					precedenceContext.CopyFrom(Precedence);

					for (var i = 0; i < executionPathSpan.Length; i++)
					{
						var executionPathId = executionPathSpan[i];

						if (executionPathId == 0)
						{
							if (skipPath == false)
							{
								executionPathCount++;
								targetSpan[executionPathLength++] = 0;
								prevExecutionPathLength = executionPathLength;
							}
							else
							{
								executionPathLength = prevExecutionPathLength;
								skipPath = false;
							}

							precedenceContext.CopyFrom(Precedence);

							continue;
						}

						if (skipPath)
							continue;

						targetSpan[executionPathLength++] = executionPathId;

						var executionPath = executionPathRegistry[executionPathId];

						if (executionPath.HasPrecedenceNodes == false)
							continue;

						foreach (var precedenceNode in executionPath.PrecedenceNodes)
						{
							if (precedenceNode is PrecedenceLeaveNode)
								precedenceContext.Leave(precedenceNode.Precedence);
							else if (precedenceContext.TryEnter(precedenceNode.Precedence) == false)
							{
								skipPath = true;

								break;
							}
						}
					}

					return executionPathCount;
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				private int BuildExecutionPathsDfa(int operand, ref MemorySpan<int> executionPathsMemorySpan, out int executionPathLength, out bool precedence, out bool predicate)
				{
					Node.EnsureSafe();

					return Node.HasReturnPathSafe
						? BuildExecutionPathReturn(operand, ref ThreadContext.Empty, ref executionPathsMemorySpan, out executionPathLength, out precedence, out predicate)
						: BuildExecutionPathNoReturn(operand, ref ThreadContext.Empty, ref executionPathsMemorySpan, out executionPathLength, out precedence, out predicate);
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

					var forkThread = new Thread(Node, stack, Precedence.Clone(), entryPath);

					stack.ReleaseReference();

					return forkThread;
				}
			}
		}
	}
}