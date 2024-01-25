// <copyright file="Automata.Process.ThreadTransitionBuilder.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Runtime.CompilerServices;
using Zaaml.Core;

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		private protected partial class Process
		{
			private abstract class TransitionBuilder
			{
				protected static bool LookAhead(ExecutionPath executionPath, ref ThreadContext context)
				{
					const int aheadLength = 4;

					var instructionPointer = context.InstructionStreamPointer;
					var instructionStream = context.InstructionStream;
					var operandSpan = instructionStream.PrefetchReadOperandSpan(instructionPointer, aheadLength);
					var lookAheadMatch = executionPath.LookAheadMatch;
					var localIndex = 1;

					for (var i = 1; i < lookAheadMatch.Length; i++)
					{
						if (localIndex >= operandSpan.Length)
						{
							operandSpan = instructionStream.PrefetchReadOperandSpan(instructionPointer + i, aheadLength);
							localIndex = 0;
						}

						var operand = operandSpan[localIndex++];

						if (lookAheadMatch[i].Match(operand) == false)
							return false;
					}

					return true;
				}

				public abstract void Build(ref Thread thread, ref ThreadContext context, ExecutionRailBuilder transitionBuilder);
			}

			private sealed class PartialTransitionBuilder : TransitionBuilder
			{
				public static TransitionBuilder Instance = new PartialTransitionBuilder();

				public override void Build(ref Thread thread, ref ThreadContext context, ExecutionRailBuilder transitionBuilder)
				{
					var node = thread.Node;
					var stack = thread.Stack;

					node.EnsureSafe();

					var operand = context.FetchInstructionOperand;
					var hasReturnPaths = false;

					if (node.HasReturnPathSafe)
					{
						var returnDepth = 0;
						var retNode = node;

						while (retNode.HasReturnPathSafe)
						{
							retNode = stack.PeekLeaveNode(returnDepth++);

							var retPathGroup = retNode.GetExecutionPathsFastSafe(operand);

							for (var index = 0; index < retPathGroup.Length; index++)
							{
								var executionPath = retPathGroup[index];
								var lookAhead = executionPath.LookAheadPath;

								if (lookAhead != null && LookAhead(lookAhead, ref context) == false)
									continue;

								if (executionPath.OutputEnd && operand != 0 && context.Process._processKind != ProcessKind.SubProcess)
									continue;

								hasReturnPaths = true;

								break;
							}

							if (hasReturnPaths)
								break;
						}
					}

					var executionPathGroup = node.GetExecutionPathsFastSafe(operand);

					if (executionPathGroup.Length == 0)
						return;

					foreach (var path in executionPathGroup)
					{
						var executionPath = path;

						if (executionPath.OutputReturn)
						{
							if (hasReturnPaths == false)
								continue;
						}
						else
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

						transitionBuilder.AddExecutionPath(executionPath.Id);
					}
				}
			}

			private sealed class FullTransitionBuilder : TransitionBuilder
			{
				private readonly MemorySpan<int> _staticExecutionPaths;

				public FullTransitionBuilder()
				{
					_staticExecutionPaths = new MemorySpan<int>(new int[65536]);
				}

				public override void Build(ref Thread thread, ref ThreadContext context, ExecutionRailBuilder transitionBuilder)
				{
					var executionPaths = _staticExecutionPaths;
					var count = BuildAllExecutionPaths(thread.Node, thread.Stack, ref context, ref executionPaths, out var length);

					if (count < 2)
						transitionBuilder.AddExecutionRail(executionPaths.Slice(0, length - 1));

					transitionBuilder.Reset();
				}

				private int BuildAllExecutionPathNoReturn(int operand, Node node, AutomataStack stack, ref ThreadContext context, ref MemorySpan<int> executionPathsMemorySpan, out int executionPathLength)
				{
					var pathCount = 0;
					var executionPaths = executionPathsMemorySpan.Span;
					var executionPathGroup = node.GetExecutionPathsFastSafe(operand);

					executionPathLength = 0;

					for (var index = 0; index < executionPathGroup.Length; index++)
					{
						var executionPath = executionPathGroup[index];

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

				private int BuildAllExecutionPathReturn(int operand, Node node, AutomataStack stack, ref ThreadContext context, ref MemorySpan<int> executionPathsMemorySpan, out int executionPathLength)
				{
					var pathCount = 0;
					var returnDepth = 0;
					var currentNode = node;
					var executionPaths = executionPathsMemorySpan.Span;
					var executionPathsHead = 0;
					var returnExecutionPathsHead = executionPaths.Length;

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

						currentNode = stack.PeekLeaveNode(rd);
						currentNode.EnsureSafe();

						executionPaths[executionPathsHead + rd] = path;

						returnDepth = rd + 1;
					}

					executionPathLength = executionPathsHead;

					return pathCount;
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				private int BuildAllExecutionPaths(Node node, AutomataStack stack, ref ThreadContext context, ref MemorySpan<int> executionPathsMemorySpan, out int executionPathLength)
				{
					node.EnsureSafe();

					var operand = context.InstructionStream.FetchPeekOperand(context.InstructionStreamPointer);

					var executionPathCount = node.HasReturnPathSafe
						? BuildAllExecutionPathReturn(operand, node, stack, ref context, ref executionPathsMemorySpan, out executionPathLength)
						: BuildAllExecutionPathNoReturn(operand, node, stack, ref context, ref executionPathsMemorySpan, out executionPathLength);

					return executionPathCount;
				}
			}
		}
	}
}