// <copyright file="Automata.Process.ExecutionPathQueueCollectionBuilder.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using Zaaml.Core;
using Zaaml.Core.Pools;
using Zaaml.Core.Extensions;

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		partial class Process
		{
			private sealed partial class ExecutionPathGroupBuilder : PoolSharedObject<ExecutionPathGroupBuilder>
			{
				private readonly Automata<TInstruction, TOperand> _automata;
				private const int MaxLookAhead = 5;
				private bool _finished;
				private int _returnPathCount;
				public int ExecutionPathBuilderCount;
				public ExecutionPathQueueBuilder[] ExecutionPathBuilders = new ExecutionPathQueueBuilder[16];
				public ExecutionPathGroupBuilder Parent;
				public ExecutionPath[] ReturnPaths = new ExecutionPath[16];
#if DEV_EXP
				public ExecutionPathDfaBuilder DfaBuilder = ExecutionPathDfaBuilder.Empty;
#endif

				public ExecutionPathGroupBuilder(Automata<TInstruction, TOperand> automata, IPool<ExecutionPathGroupBuilder> pool) : base(pool)
				{
					_automata = automata;

					for (var i = 0; i < ExecutionPathBuilders.Length; i++)
						ExecutionPathBuilders[i] = new ExecutionPathQueueBuilder(this);
				}

				public ArrayPageSegment<int> Serialize(ArrayPageManager<int> arrayPageManager)
				{
					var serializeLength = SerializeLength;
					var plane = arrayPageManager.GetPage(serializeLength);
					var span = plane.Allocate(serializeLength, out var offset);

					Serialize(span);

					return new ArrayPageSegment<int>(plane, offset, span);
				}

				public int SerializeLength
				{
					get
					{
						if (_returnPathCount == 0)
							return 1 + ExecutionPathBuilderCount;

						return 1 + _returnPathCount + ExecutionPathBuilderCount * 2;
					}
				}

				public void Serialize(Span<int> binaryArray)
				{
					// layout [returnPathCount, [returnPathArray], [executionPath, returnPathIndex] ]
					// when no return path [0, [executionPath]]

					if (_returnPathCount == 0)
					{
						if (binaryArray.Length != 1 + ExecutionPathBuilderCount)
							throw new InvalidOperationException();
						
						var index = 0;

						binaryArray[index++] = 0;

						for (var i = 0; i < ExecutionPathBuilderCount; i++)
						{
							var executionPathQueueBuilder = ExecutionPathBuilders[i];

							binaryArray[index++] = executionPathQueueBuilder.ExecutionPath.Id;
						}
					}
					else
					{
						if (binaryArray.Length != 1 + _returnPathCount + ExecutionPathBuilderCount * 2)
							throw new InvalidOperationException();

						var index = 0;

						binaryArray[index++] = _returnPathCount;

						for (var i = 0; i < _returnPathCount; i++)
							binaryArray[index++] = ReturnPaths[i].Id;

						for (var i = 0; i < ExecutionPathBuilderCount; i++)
						{
							var executionPathQueueBuilder = ExecutionPathBuilders[i];

							binaryArray[index++] = executionPathQueueBuilder.ExecutionPath.Id;
							binaryArray[index++] = executionPathQueueBuilder.ReturnPathCount;
						}
					}
				}

				public void Deserialize(ReadOnlySpan<int> binaryArray)
				{
					var index = 0;

					_returnPathCount = binaryArray[index++];
					ExecutionPathBuilderCount = 0;

					if (_returnPathCount == 0)
					{
						var executionBuildersCount = binaryArray.Length - 1;

						for (var i = 0; i < executionBuildersCount; i++)
						{
							var executionPath = _automata._executionPathRegistry[binaryArray[index++]];

							AddExecutionPath(executionPath, 0);
						}
					}
					else
					{
						EnsureReturnPathArray(_returnPathCount);

						for (var i = 0; i < _returnPathCount; i++)
							ReturnPaths[i] = _automata._executionPathRegistry[binaryArray[index++]];

						var executionBuildersCount = (binaryArray.Length - 1 - _returnPathCount) / 2;

						for (var i = 0; i < executionBuildersCount; i++)
						{
							var executionPath = _automata._executionPathRegistry[binaryArray[index++]];
							var returnPathIndex = binaryArray[index++];

							AddExecutionPath(executionPath, returnPathIndex);
						}
					}
				}

				private void AddExecutionPath(ExecutionPath executionPath, int returnPathIndex)
				{
					if (executionPath.Safe == false)
						executionPath.MakeSafe();

					if (ExecutionPathBuilderCount == ExecutionPathBuilders.Length)
					{
						Array.Resize(ref ExecutionPathBuilders, ExecutionPathBuilders.Length * 2);

						for (var i = ExecutionPathBuilderCount; i < ExecutionPathBuilders.Length; i++)
							ExecutionPathBuilders[i] = new ExecutionPathQueueBuilder(this);
					}

					var builder = ExecutionPathBuilders[ExecutionPathBuilderCount++];

					builder.ExecutionPath = executionPath;
					builder.ReturnPathCount = returnPathIndex;
					builder.NextBuilder = null;
				}

				public static bool Dfa { get; set; } = false;

				public void BuildExecutionPath(Process process, ref bool continueBuild)
				{
					ref var currentThread = ref process._currentThread;
					var pathLookupInfo = new PathLookupInfo(currentThread.InstructionPointer, currentThread.InstructionStream, currentThread.CurrentNode, currentThread.Stack, process);

#if DEV_EXP
					if (Dfa)
					{
						DfaBuilder = AutomataDfa.BuildDfa(ref pathLookupInfo);

						//if (DfaBuilder.DfaState.Length > 0)
						{
							_finished = false;
							ExecutionPathBuilderCount = DfaBuilder.DfaState.Length;

							continueBuild = false;

							return;
						}
					}
					else
#endif
					{
						BuildExecutionPath(ref pathLookupInfo, 1, MaxLookAhead, continueBuild);

						continueBuild = false;
					}

					//FlattenBuilders();

					//CollapseForward();
				}

				private void BuildExecutionPath(ref PathLookupInfo pathLookup, int resolveLimit, int lookAhead, bool continueBuild)
				{
					if (continueBuild == false)
						BuildExecutionPathImpl(ref pathLookup);

					if (ExecutionPathBuilderCount == 1 && ReferenceEquals(ExecutionPathBuilders[0].ExecutionPath.Output, pathLookup.Process._entryPointSubGraph.EndNode))
					{
						_finished = true;
						
						return;
					}
					
					//if (ExecutionPathBuilderCount > resolveLimit && lookAhead > 0)
					//	ResolveBranch(resolveLimit, lookAhead, ref pathLookup);

					//CollapseSinglePath();
				}

				private void BuildExecutionPathImpl(ref PathLookupInfo pathLookup)
				{
					_finished = false;
					ExecutionPathBuilderCount = 0;
					_returnPathCount = 0;

					var instructionPointer = pathLookup.InstructionPointer;
					var operand = pathLookup.InstructionStream.ReadOperand(ref instructionPointer);
					var returnLeaveNode = FillReturnPath(ref pathLookup);

					if (operand >= 0)
					{
						BuildMatchPath(ref pathLookup, operand, instructionPointer);

						if (pathLookup.Process._partialRun && ReferenceEquals(pathLookup.Process._entryPointSubGraph.LeaveNode, returnLeaveNode))
							AddExecutionPath(pathLookup.Process._entryPointSubGraph.EndPath, _returnPathCount);

						if (pathLookup.Process._partialRun == false || ExecutionPathBuilderCount > 0)
							return;
					}

					if (BuildPredicatePath(ref pathLookup))
						return;

					BuildReturnPath(ref pathLookup, returnLeaveNode);
				}

				private void BuildMatchPath(ref PathLookupInfo pathLookup, int operand, int instructionPointer)
				{
					var currentNode = pathLookup.Node;

					foreach (var executionPath in currentNode.GetExecutionPathsFastSafe(operand))
					{
						if (executionPath.LookAhead == null)
							AddExecutionPath(executionPath, 0);
						else if (LookAhead(pathLookup.InstructionStream, executionPath.LookAhead, instructionPointer))
							AddExecutionPath(executionPath.LookAhead, 0);
					}

					var stack = pathLookup.Stack;
					var stackArray = stack.Array;
					var stackCount = stack.Count;
					var registry = _automata._subGraphRegistry;

					for (var iReturnPath = 1; iReturnPath <= _returnPathCount; iReturnPath++)
					{
						currentNode = registry[stackArray[stackCount - iReturnPath]].LeaveNode;

						foreach (var executionPath in currentNode.GetExecutionPathsFastSafe(operand))
						{
							if (executionPath.LookAhead == null)
								AddExecutionPath(executionPath, iReturnPath);
							else if (LookAhead(pathLookup.InstructionStream, executionPath.LookAhead, instructionPointer))
								AddExecutionPath(executionPath.LookAhead, iReturnPath);
						}
					}
				}

				private bool BuildPredicatePath(ref PathLookupInfo pathLookup)
				{
					var currentNode = pathLookup.Node;
					var subGraph = pathLookup.Process._entryPointSubGraph;

					foreach (var executionPath in currentNode.GetExecutionPathsFast())
						AddExecutionPath(executionPath, 0);

					if (ReferenceEquals(currentNode, subGraph.InitNode))
					{
						if (subGraph.EmptyPath.IsInvalid)
							return ExecutionPathBuilderCount > 0;

						ReturnPaths[_returnPathCount++] = subGraph.EmptyPath;

						AddExecutionPath(subGraph.EndPath, 1);

						return ExecutionPathBuilderCount > 0;
					}

					var stack = pathLookup.Stack;
					var stackCount = stack.Count;

					for (var iReturnPath = 1; iReturnPath <= _returnPathCount; iReturnPath++)
					{
						currentNode = stack.Get(stackCount - iReturnPath).LeaveNode;

						foreach (var executionPath in currentNode.GetExecutionPathsFast())
							AddExecutionPath(executionPath, iReturnPath);
					}

					if (ReferenceEquals(currentNode, subGraph.LeaveNode))
						AddExecutionPath(subGraph.EndPath, _returnPathCount);

					return ExecutionPathBuilderCount > 0;
				}

				private void BuildReturnPath(ref PathLookupInfo pathLookup, Node returnLeaveNode)
				{
					var currentNode = pathLookup.Node;
					var subGraph = pathLookup.Process._entryPointSubGraph;

					if (ReferenceEquals(currentNode, subGraph.InitNode))
					{
						if (subGraph.EmptyPath.IsInvalid)
							return;

						ReturnPaths[_returnPathCount++] = subGraph.EmptyPath;

						AddExecutionPath(subGraph.EndPath, 1);

						return;
					}

					var leaveNode = subGraph.LeaveNode;

					if (ReferenceEquals(returnLeaveNode, leaveNode))
						AddExecutionPath(subGraph.EndPath, _returnPathCount);
				}

				private void CollapseForward()
				{
					if (ExecutionPathBuilderCount != 1)
						return;

					var single = ExecutionPathBuilders[0];

					while (true)
					{
						var next = single.NextBuilder;

						if (next == null || next.ExecutionPathBuilderCount != 1)
							return;

						var nextBuilder = next.ExecutionPathBuilders[0];

						_returnPathCount = single.ReturnPathCount;

						if (ReturnPaths.Length == _returnPathCount)
							ResizeReturnPathArray(ref ReturnPaths);

						ReturnPaths[_returnPathCount++] = single.ExecutionPath;

						for (var i = 0; i < nextBuilder.ReturnPathCount; i++)
						{
							if (ReturnPaths.Length == _returnPathCount)
								ResizeReturnPathArray(ref ReturnPaths);

							ReturnPaths[_returnPathCount++] = nextBuilder.ReturnPaths[i];
						}

						single.ReturnPathCount = _returnPathCount;
						single.ExecutionPath = nextBuilder.ExecutionPath;
						single.NextBuilder = nextBuilder.NextBuilder;

						next.Dispose();
					}
				}

				private void CollapseSinglePath()
				{
					if (ExecutionPathBuilderCount == 1 && ExecutionPathBuilders[0].NextBuilder?.ExecutionPathBuilderCount == 1)
					{
						var firstBuilder = ExecutionPathBuilders[0];
						var nextBuilder = firstBuilder.NextBuilder.ExecutionPathBuilders[0];

						_returnPathCount = firstBuilder.ReturnPathCount;

						if (ReturnPaths.Length == _returnPathCount)
							ResizeReturnPathArray(ref ReturnPaths);

						ReturnPaths[_returnPathCount++] = firstBuilder.ExecutionPath;

						for (var i = 0; i < nextBuilder.ReturnPathCount; i++)
						{
							if (ReturnPaths.Length == _returnPathCount)
								ResizeReturnPathArray(ref ReturnPaths);

							ReturnPaths[_returnPathCount++] = nextBuilder.ReturnPaths[i];
						}

						firstBuilder.ReturnPathCount = _returnPathCount;
						firstBuilder.ExecutionPath = nextBuilder.ExecutionPath;
						firstBuilder.NextBuilder = firstBuilder.NextBuilder.DisposeExchange();
					}
				}

				protected override void OnReleased()
				{
					Parent = null;

					base.OnReleased();
				}

				private Node FillReturnPath(ref PathLookupInfo pathLookup)
				{
					var currentNode = pathLookup.Node;

					if (currentNode.HasReturnPathSafe == false)
						return currentNode;

					var stack = pathLookup.Stack;
					var stackIndex = stack.Count - 1;
					var stackArray = stack.Array;
					var returnPaths = ReturnPaths;
					var returnPathCount = _returnPathCount;
					var registry = _automata._subGraphRegistry;

					while (true)
					{
						if (returnPaths.Length == returnPathCount)
							ResizeReturnPathArray(ref returnPaths);

						var returnPath = currentNode.ReturnPathSafe;

						if (returnPath.Safe == false)
							returnPath.MakeSafe();

						returnPaths[returnPathCount++] = returnPath;
						currentNode = registry[stackArray[stackIndex]].LeaveNode;

						if (currentNode.HasReturnPathSafe == false || stackIndex == 0)
							break;

						stackIndex--;
					}

					_returnPathCount = returnPathCount;

					return currentNode;
				}

				private void FlattenBuilders()
				{
					var stack = new Stack<ExecutionPathQueueBuilder>();
					var leafList = new List<ExecutionPathQueueBuilder>();

					for (var i = 0; i < ExecutionPathBuilderCount; i++)
						stack.Push(ExecutionPathBuilders[i]);

					while (stack.Count > 0)
					{
						var builder = stack.Pop();
						var nextBuilder = builder.NextBuilder;

						if (nextBuilder != null)
							for (var j = 0; j < nextBuilder.ExecutionPathBuilderCount; j++)
								stack.Push(nextBuilder.ExecutionPathBuilders[j]);
						else
							leafList.Add(builder);
					}
				}

				private static bool LookAhead(InstructionStream instructionStream, ExecutionPath executionPath, int instructionPointer)
				{
					var lookAheadMatch = executionPath.LookAheadMatch;

					for (var i = 1; i < lookAheadMatch.Length; i++)
					{
						int operand;

						if ((operand = instructionStream.ReadOperand(ref instructionPointer)) < 0)
							return false;

						if (lookAheadMatch[i].Match(operand) == false)
							return false;
					}

					return true;
				}

				private void EnsureReturnPathArray(int minCapacity)
				{
					var capacity = ReturnPaths.Length;

					while (capacity < minCapacity) 
						capacity <<= 2;

					ResizeReturnPathArray(ref ReturnPaths, capacity);
				}

				private void ResizeReturnPathArray(ref ExecutionPath[] returnPaths)
				{
					ResizeReturnPathArray(ref returnPaths, returnPaths.Length * 2);
				}

				private void ResizeReturnPathArray(ref ExecutionPath[] returnPaths, int capacity)
				{
					Array.Resize(ref returnPaths, capacity);

					ReturnPaths = returnPaths;

					foreach (var executionPathBuilder in ExecutionPathBuilders)
						executionPathBuilder.ReturnPaths = returnPaths;
				}

				private void Resolve(ExecutionPathQueueBuilder pathBuilder,  int lookAhead, ref PathLookupInfo prevPathLookup)
				{
					var instructionPointer = prevPathLookup.InstructionPointer;
					var instructionStream = prevPathLookup.InstructionStream;
					var pathBuilderExecutionPath = pathBuilder.ExecutionPath;
					var stack = pathBuilder.ReturnPathCount > 0 || pathBuilderExecutionPath.EnterReturnNodes.Length > 0 ? prevPathLookup.Stack.Clone() : prevPathLookup.Stack;

					instructionStream.Move(pathBuilderExecutionPath.LookAheadMatch.Length, ref instructionPointer);

					for (var i = 0; i < pathBuilder.ReturnPathCount; i++)
						stack.Eval(pathBuilder.ReturnPaths[i]);

					var node = stack.Eval(pathBuilderExecutionPath);
					var pathLookupInfo = new PathLookupInfo(instructionPointer, instructionStream, node, stack, prevPathLookup.Process);

					BuildExecutionPath(ref pathLookupInfo, 0, lookAhead - 1, false);

					if (ReferenceEquals(prevPathLookup.Stack, pathLookupInfo.Stack) == false)
						pathLookupInfo.Stack.Dispose();
				}

				private void ResolveBranch(int resolveLimit, int lookAhead, ref PathLookupInfo prevPathLookup)
				{
					if (_finished)
						return;

					var builder = Pool.Get().AddReference();

					builder.Parent = this;

					for (var pathIndex = ExecutionPathBuilderCount - 1; pathIndex >= 0; pathIndex--)
					{
						var pathBuilder = ExecutionPathBuilders[pathIndex];

						builder.Resolve(pathBuilder, lookAhead, ref prevPathLookup);

						if (builder._finished || builder.ExecutionPathBuilderCount > 0)
						{
							pathBuilder.NextBuilder = builder;
							builder.Parent = this;
							builder = Pool.Get().AddReference();
						}
						else
						{
							for (var i = pathIndex; i < ExecutionPathBuilderCount - 1; i++) 
								ExecutionPathBuilders[i] = ExecutionPathBuilders[i + 1];

							ExecutionPathBuilders[ExecutionPathBuilderCount - 1] = pathBuilder;
							ExecutionPathBuilderCount--;
						}

						if (ExecutionPathBuilderCount == resolveLimit)
							break;
					}

					builder.Dispose();
				}

				public void EnqueueDfaPath(ExecutionPathQueue threadStartExecutionQueue, in ulong dfaTrail)
				{
#if DEV_EXP
					AutomataDfa.EnqueueDfaPath(threadStartExecutionQueue, dfaTrail);
#endif
				}
			}

			private readonly struct PathLookupInfo
			{
				public PathLookupInfo(int instructionPointer, InstructionStream instructionStream, Node node, AutomataStack stack, Process process)
				{
					InstructionPointer = instructionPointer;
					InstructionStream = instructionStream;
					Node = node;
					Stack = stack;
					Process = process;
				}

				public readonly int InstructionPointer;
				public readonly InstructionStream InstructionStream;
				public readonly Node Node;
				public readonly AutomataStack Stack;
				public readonly Process Process;
			}
		}
	}
}