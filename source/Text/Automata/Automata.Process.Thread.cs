// <copyright file="Automata.Process.Thread.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

// ReSharper disable ForCanBeConvertedToForeach

using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Zaaml.Core;
using Zaaml.Core.Extensions;
using Zaaml.Core.Reflection;

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		partial class Process
		{
			private protected partial struct Thread
			{
				public static readonly FieldInfo StackField = typeof(Thread).GetField(nameof(Stack), BF.IPNP);

				public Node Node;
				public DfaState Dfa;
				public AutomataStack Stack;
				public PrecedenceContext Precedence;
				public ExecutionRailNode ExecutionRailNode;

				public Thread(Node node, AutomataStack stack)
				{
					Dfa = default;
					Node = node;
					Stack = stack;
					Precedence = null;
					ExecutionRailNode = ExecutionRailNode.Empty;
				}

				public Thread(Node node, AutomataStack stack, PrecedenceContext precedence)
				{
					Dfa = default;
					node.EnsureSafe();

					Node = node;
					Stack = stack;
					Precedence = precedence;
					ExecutionRailNode = ExecutionRailNode.Empty;
				}

				public Thread(Node node, AutomataStack stack, ExecutionRailNode executionRailNode)
				{
					Dfa = default;
					Node = node;
					Stack = stack;
					Precedence = null;
					ExecutionRailNode = executionRailNode;
				}

				public Thread(DfaState dfa, Node node, AutomataStack stack, PrecedenceContext precedence, ExecutionRailNode executionRailNode)
				{
					Dfa = dfa;
					Node = node;
					Stack = stack;
					Precedence = precedence;
					ExecutionRailNode = executionRailNode;
				}

				public void Dispose()
				{
					Stack.Dispose();
					Precedence.Dispose();
					ExecutionRailNode.Dispose();
				}

				private ThreadStatusKind Run(ref ThreadContext context, ReadOnlySpan<int> executionPaths)
				{
					context.InstructionStream.UnlockPointer(context.InstructionStreamPointer);

					for (var index = 0; index < executionPaths.Length; index++)
					{
						var executionPath = executionPaths[index];

						Node = context.Execute(executionPath);

						switch (Node.ThreadStatusKind)
						{
							case ThreadStatusKind.Run:
								continue;

							case ThreadStatusKind.Fork:

								context.InstructionStream.LockPointer(context.InstructionStreamPointer);

								return ForkThreadNode((ForkNode)Node, executionPaths.Slice(index + 1), ref context);

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

				private bool TryEnterPrecedenceCode(int precedenceCode)
				{
					return Precedence.TryEnterCode(precedenceCode);
				}

				private void LeavePrecedenceCode(int precedenceCode)
				{
					Precedence.LeaveCode(precedenceCode);
				}

				private void LeavePrecedence(int precedenceEnterNodeId)
				{
					var precedenceLeaveNode = (PrecedenceLeaveNode)Node.Automata._nodeRegistry[precedenceEnterNodeId];

					Precedence.Leave(precedenceLeaveNode.Precedence);
				}

				private ThreadStatusKind ForkThreadNode(ForkNode forkNode, ReadOnlySpan<int> executionPaths, ref ThreadContext context)
				{
					throw Error.Refactoring;

					//var forkPredicateResult = (IForkPredicateResult)forkNode.PredicateResult;
					//var startNode = forkNode.PredicateNode;
					//var processResources = context.Process._processResources;
					//var forkPaths = processResources.ExecutionPathSpanAllocator.Allocate(executionPaths.Length * 2 + 2);
					//var forkPathsSpan = forkPaths.Span;
					//var forkPathsHead = 0;

					//for (var i = 0; i <= 1; i++)
					//{
					//	var predicateEntry = i == 0 ? forkPredicateResult.First : forkPredicateResult.Second;
					//	var predicateNode = processResources.ForkPredicateNodePool.Rent().Mount(predicateEntry);
					//	var predicateExecutionPath = processResources.ForkExecutionPathPool.Rent().Mount(startNode, predicateNode);

					//	predicateNode.CopyLookup(forkNode);

					//	forkPathsSpan[forkPathsHead++] = predicateExecutionPath.Id;

					//	foreach (var executionPath in executionPaths)
					//		forkPathsSpan[forkPathsHead++] = executionPath;
					//}

					//forkNode.Release();

					//return context.Process.ForkThread(ref this, ref context, forkPaths, forkPathsHead);
				}

				public ThreadStatusKind Run(ref ThreadContext context)
				{
					var startExecutionRailNode = ExecutionRailNode;
					var startExecutionRail = startExecutionRailNode.ExecutionRail;
					var pointer = context.InstructionStreamPointer;
					var position = context.InstructionStreamPosition;
					var startNode = Node;
					
					ExecutionRailNodeStat runStat = default;

					if (startExecutionRail.IsEmpty == false)
					{
						try
						{
							var entryStatus = Run(ref context, startExecutionRail.SpanSafe);

							Dfa = startExecutionRailNode.Dfa;

							runStat = startExecutionRailNode.RunStat;
							
							if (entryStatus != ThreadStatusKind.Run)
							{
								if (entryStatus == ThreadStatusKind.Block)
									return ThreadStatusKind.Block;

								if (entryStatus == ThreadStatusKind.Finished && context.Process._processKind == ProcessKind.SubProcess)
									return entryStatus;

								return context.FetchInstructionOperand == 0 ? entryStatus : ThreadStatusKind.Unexpected;
							}
						}
						finally
						{
							ExecutionRailNode = ExecutionRailNode.DisposeExchange(ExecutionRailNode.Empty);
						}
					}

					var process = context.Process;
					var transitionBuilder = process._nfaTransitionBuilder;
					var executionRailBuilder = process._executionRailBuilder;

					runStat?.Run();

					while (true)
					{
						if (context.IsCompleteBlock)
							return ThreadStatusKind.Block;

						switch (executionRailBuilder.Build(ref this, ref context, transitionBuilder))
						{
							case 0:
#if false
								var executionTrace = DumpTrace(ref this, ref context);
								context.InstructionStream.Dump(position, context.InstructionStreamPosition - position)
#endif
								var backtrackingLength = context.InstructionStreamPointer - pointer;

								runStat?.Fail(backtrackingLength);

#if false
								DumpTrace(ref this, ref context);
#endif

								Dfa = default;

								process._telemetry?.Backtracking(backtrackingLength);

								return Node.ThreadStatusKind == ThreadStatusKind.Finished ? ThreadStatusKind.Finished : ThreadStatusKind.Unexpected;

							case 1:
#if false
								var fp = DumpPath(ref this, ref context, executionPaths.ExecutionRail);
#endif

								Dfa = executionRailBuilder.Dfa;

								var executionPaths = executionRailBuilder.ExecutionRail.SpanSafe;
								
								if (Run(ref context, executionPaths) == ThreadStatusKind.Run)
									continue;

								if (Node.ThreadStatusKind == ThreadStatusKind.Finished && context.Process._processKind == ProcessKind.SubProcess)
									return ThreadStatusKind.Finished;

								return context.FetchInstructionOperand == 0 ? Node.ThreadStatusKind : ThreadStatusKind.Unexpected;

							default:

								process._telemetry?.Fork();

								var executionRailList = executionRailBuilder.DetachRailList();

								Dfa = default;

#if false
								var fp = DumpForkPaths(ref this, ref context, executionRailList);
#endif
								return process.ForkThread(ref this, ref context, executionRailList);
						}
					}
				}

				private int GetBacktrackingLength(ref ThreadContext context)
				{
					if (context.Index == 0)
						return context.InstructionStreamPointer;

					ref var prevContext = ref context.Process._threads.PrevContext();

					return context.InstructionStreamPointer - prevContext.InstructionStreamPointer;
				}

				[UsedImplicitly]
				private static string DumpTrace(ref Thread thread, ref ThreadContext context)
				{
					if (context.Index > 0)
					{
						ref var prevContext = ref context.Process._threads.PrevContext();
						ref var prevThread = ref context.Process._threads.PrevThread();
						var prevPosition = prevContext.InstructionStreamPosition;
						var currPosition = context.InstructionStreamPosition;
						var prevPointer = prevContext.InstructionStreamPointer;
						var currPointer = context.InstructionStreamPointer;

						var text = context.InstructionStream.Dump(prevPosition, currPosition - prevPosition);
					}

					var prevInstructionPointer = context.Index == 0 ? 0 : context.Process._threads.PrevContext().ExecutionStreamPointer;
					var executionTrace = context.ExecutionStream.GetSpan(prevInstructionPointer, context.ExecutionStreamPointer - prevInstructionPointer);
					var contextExecutionPathRegistry = context.ExecutionPathRegistry;

					return string.Join("\r\n", executionTrace.ToArray().SelectMany(e => contextExecutionPathRegistry[e].Nodes).Select(n => n.ToString()));
				}

				[UsedImplicitly]
				private static string DumpForkPaths(ref Thread thread, ref ThreadContext context, ExecutionRailList executionPaths)
				{
					var stringBuilder = new StringBuilder();
					var count = executionPaths.Count;

					for (var i = 0; i < count; i++)
					{
						executionPaths = executionPaths.MoveNext(out var railNode);

						stringBuilder.Append($"ForkPath: {i + 1}\r\n");
						stringBuilder.Append(DumpPath(ref thread, ref context, railNode.ExecutionRail));
						stringBuilder.Append("\r\n--------------------------------------------------------------\r\n");
					}

					return stringBuilder.ToString();
				}

				[UsedImplicitly]
				private static string DumpPath(ref Thread thread, ref ThreadContext context, MemorySpan<int> executionPath)
				{
					var contextExecutionPathRegistry = context.ExecutionPathRegistry;

					return string.Join("\r\n", executionPath.SpanSafe.ToArray().SelectMany(e => contextExecutionPathRegistry[e].Nodes).Select(n => n.ToString()));
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public void StackForkExchange(ref Thread threadSource)
				{
					Stack.Unfork(threadSource.Stack);
					Precedence.Unfork(threadSource.Precedence);

					(Stack, threadSource.Stack) = (threadSource.Stack, Stack);
					(Precedence, threadSource.Precedence) = (threadSource.Precedence, Precedence);
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public Thread Fork(ExecutionRailNode executionRailNode)
				{
					var stack = Stack;
					var precedence = Precedence;

					Stack = stack.Fork();
					Precedence = precedence.Fork();

					return new Thread(Dfa, Node, stack, precedence, executionRailNode);
				}
			}
		}
	}
}