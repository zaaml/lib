// <copyright file="Automata.Process.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Zaaml.Core;
using Zaaml.Core.Collections;
using Zaaml.Core.Extensions;

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		#region Nested Types

		private protected sealed partial class Process
		{
			#region Static Fields and Constants

			public static readonly Process DummyProcess = new Process(null);

			#endregion

			#region Fields

			private readonly Automata<TInstruction, TOperand> _automata;

			//private readonly DebugId<Process> Id = DebugId<Process>.Create(typeof(TOperand).ToString());

			private readonly List<ForkThreadData> _forkThreads = new List<ForkThreadData>
			{
				new ForkThreadData(),
				new ForkThreadData(),
				new ForkThreadData(),
				new ForkThreadData(),
				new ForkThreadData(),
				new ForkThreadData(),
				new ForkThreadData(),
				new ForkThreadData(),
			};

			private Thread _currentThread = Thread.Empty;
			private EntryPointSubGraph _entryPointSubGraph;
			private ExecutionPathMethodCollection _executionMethods;
			private Thread _mainThread = Thread.Empty;

			private LinkedListStack<Thread> _parallelThreads;
			private bool _partialRun;
			private PredicateResultQueue _predicateResultQueue;
			private ProcessResources _processResources;
			private ReferenceCounter _referenceCount = new ReferenceCounter();
			private ExecutionPathGroupBuilder _executionPathQueueBuilder;
			internal AutomataContext Context;
			internal bool ExecuteThreadQueue;

			#endregion

			#region Ctors

			public Process(Automata<TInstruction, TOperand> automata)
			{
				_automata = automata;
			}

			#endregion

			#region Properties

			public AutomataContextState ContextState => _currentThread.ContextState;

			public ref TInstruction Instruction => ref _currentThread.Instruction;

			public int InstructionOperand => _currentThread.InstructionOperand;

			public int InstructionPointer => _currentThread.InstructionPointer;

			public int InstructionStreamPosition => _currentThread.InstructionStreamPosition;

			public bool IsMainThread => _currentThread.Parent == null;

			#endregion

			#region Methods

			public ref TInstruction GetInstructionOperand(out int operand)
			{
				return ref _currentThread.GetInstructionOperand(out operand);
			}

			public void AddReference()
			{
				_referenceCount.AddReference();
			}

			public void AdvanceInstructionPosition()
			{
				var instructionQueue = _currentThread.InstructionStream;

				_currentThread.InstructionStream = instructionQueue.Advance(_currentThread.InstructionPointer, _automata).AddReference();

				instructionQueue.ReleaseReference();
			}

			[UsedImplicitly]
			private ForkNode BuildForkNode(int nodeIndex, ExecutionPath executionPath, PredicateResult predicateResult)
			{
				var forkNode = _processResources.ForkNodePool.Get().Mount(nodeIndex, executionPath, predicateResult);

				predicateResult.Dispose();

				return forkNode;
			}

			private void ClearThreads()
			{
				while (_parallelThreads.Count > 0)
					_parallelThreads.Pop().Dispose(this);
			}

			private List<DebugExecutionAlternative> DebugExecutionPaths()
			{
				var alt = new List<DebugExecutionAlternative>();
				var executionPathBuilderCount = _executionPathQueueBuilder.ExecutionPathBuilderCount;

				for (var j = 0; j < executionPathBuilderCount; j++)
					alt.Add(new DebugExecutionAlternative(_executionPathQueueBuilder.ExecutionPathBuilders[j]));

				return alt;
			}

			internal PredicateResult DequePredicateResult()
			{
				if (_predicateResultQueue.Queue.Count == 0)
					_predicateResultQueue = _predicateResultQueue.Next;

				if (_predicateResultQueue == null)
					throw new InvalidOperationException();

				var result = _predicateResultQueue.Queue.Dequeue();

				return result;
			}

			private void Dispose()
			{
				ClearThreads();

				_parallelThreads = _parallelThreads.DisposeExchange();
				_executionPathQueueBuilder = _executionPathQueueBuilder.DisposeExchange();
				_currentThread.Dispose(this);
				_currentThread = Thread.Empty;
				_executionMethods = null;
				_partialRun = false;

				Context.ProcessField = DummyProcess;
				Context = null;

				_processResources = null;
				_automata.ReleaseProcess(this);
			}

			internal void EnqueuePredicateResult(PredicateResult predicateResult)
			{
				_currentThread.EnqueuePredicateResult(predicateResult);
			}

			private StatusKind ExecuteInstruction(bool main)
			{
				var continueBuild = false;

				while (true)
				{
					_executionPathQueueBuilder.BuildExecutionPath(this, ref continueBuild);

#if DEBUG
					//var dbgPaths = DebugExecutionPaths();
#endif

					switch (_executionPathQueueBuilder.ExecutionPathBuilderCount)
					{
						case 0:

							if (ReferenceEquals(_currentThread.CurrentNode, _entryPointSubGraph.EndNode))
								return StatusKind.Finished;

							return StatusKind.Unexpected;

						case 1:

							_currentThread.InstructionStream.UnlockPointer(_currentThread.InstructionPointer);

#if DEV_EXP
							var dfaPathArray = _executionPathQueueBuilder.DfaBuilder.ExecutionPathsArray;

							if (dfaPathArray != null)
							{
								if (main)
								{
									var dfaExecutionPaths = dfaPathArray;

									for (var i = 0; i < dfaExecutionPaths.Length; i++)
									{
										ExecuteMain(dfaExecutionPaths[i]);

										if (_currentThread.CurrentNode is ForkNode forkNode)
										{
											for (var j = i + 1; j < dfaExecutionPaths.Length; j++)
												forkNode.ForkExecutionPaths.Add(dfaExecutionPaths[i]);

											_currentThread.InstructionStream.LockPointer(_currentThread.InstructionPointer);
											
											return ForkThreadNode(forkNode, true);
										}
									}
								}
								else
								{
									var dfaExecutionPaths = dfaPathArray;

									for (var i = 0; i < dfaExecutionPaths.Length; i++)
									{
										ExecuteParallel(dfaExecutionPaths[i]);

										if (_currentThread.CurrentNode is ForkNode forkNode)
										{
											for (var j = i + 1; j < dfaExecutionPaths.Length; j++)
												forkNode.ForkExecutionPaths.Add(dfaExecutionPaths[i]);

											_currentThread.InstructionStream.LockPointer(_currentThread.InstructionPointer);

											return ForkThreadNode(forkNode, true);
										}
									}
								}
							}
							else
#endif
							{
								var executionPathQueueBuilder = _executionPathQueueBuilder.ExecutionPathBuilders[0];
								var returnPathCount = executionPathQueueBuilder.ReturnPathCount;

								if (main)
								{
									for (var i = 0; i < returnPathCount; i++)
									{
										ExecuteMain(executionPathQueueBuilder.ReturnPaths[i]);

										if (_currentThread.CurrentNode is ForkNode forkNode)
										{
											for (var j = i + 1; j < returnPathCount; j++)
												forkNode.ForkExecutionPaths.Add(executionPathQueueBuilder.ReturnPaths[i]);

											forkNode.ForkExecutionPaths.Add(executionPathQueueBuilder.ExecutionPath);

											_currentThread.InstructionStream.LockPointer(_currentThread.InstructionPointer);

											executionPathQueueBuilder.NextBuilder = executionPathQueueBuilder.NextBuilder?.DisposeExchange();

											return ForkThreadNode(forkNode, true);
										}
									}

									ExecuteMain(executionPathQueueBuilder.ExecutionPath);
								}
								else
								{
									for (var i = 0; i < returnPathCount; i++)
									{
										ExecuteParallel(executionPathQueueBuilder.ReturnPaths[i]);

										if (_currentThread.CurrentNode is ForkNode forkNode)
										{
											for (var j = i + 1; j < returnPathCount; j++)
												forkNode.ForkExecutionPaths.Add(executionPathQueueBuilder.ReturnPaths[i]);

											forkNode.ForkExecutionPaths.Add(executionPathQueueBuilder.ExecutionPath);

											_currentThread.InstructionStream.LockPointer(_currentThread.InstructionPointer);

											executionPathQueueBuilder.NextBuilder = executionPathQueueBuilder.NextBuilder?.DisposeExchange();

											return ForkThreadNode(forkNode, false);
										}
									}

									ExecuteParallel(executionPathQueueBuilder.ExecutionPath);
								}
							}

							_currentThread.InstructionStream.LockPointer(_currentThread.InstructionPointer);

							if (_currentThread.CurrentNode == null)
								return StatusKind.Unexpected;

							if (ReferenceEquals(_currentThread.CurrentNode, _entryPointSubGraph.EndNode))
								return StatusKind.Finished;

							//if (executionPathQueueBuilder.NextBuilder != null)
							//{
							//	continueBuild = true;

							//	_executionPathQueueBuilder = _executionPathQueueBuilder.DisposeExchange(executionPathQueueBuilder.NextBuilder);
							//}

							{
								if (_currentThread.CurrentNode is ForkNode forkNode)
									return ForkThreadNode(forkNode, main);
							}

							continue;

						default:

#if DEV_EXP
							if (_executionPathQueueBuilder.DfaBuilder.DfaState.Length > 1)
								return ForkDfaThread(main);
#endif

							return ForkThread(main);
					}
				}
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			private void ExecuteMain(ExecutionPath executionPath)
			{
				_currentThread.CurrentNode = _executionMethods.GetExecutionPathMethod(executionPath).ExecuteMain(this, _currentThread.Stack);
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			private void ExecuteParallel(ExecutionPath executionPath)
			{
				_currentThread.EnqueuePath(executionPath);

				if (executionPath.ForkPredicatePath)
					executionPath.AddReference();

				_currentThread.CurrentNode = _executionMethods.GetExecutionPathMethod(executionPath).ExecuteParallel(this, _currentThread.Stack);
			}

			private void ExecuteThread()
			{
				var thread = _currentThread;

				_currentThread = _mainThread;
				_mainThread = Thread.Empty;

				var instructionQueue = thread.InstructionStream;

				thread.InstructionStream = _currentThread.InstructionStream;

				_currentThread.InstructionStream = instructionQueue;

				var currentParent = thread.Parent;
				var executionQueue = thread.ExecutionQueue;

				_predicateResultQueue = thread.PredicateResultQueue;

				while (currentParent != null)
				{
					if (currentParent.PredicateResultQueue != null && ReferenceEquals(_predicateResultQueue, currentParent.PredicateResultQueue) == false)
					{
						currentParent.PredicateResultQueue.Next = _predicateResultQueue;
						_predicateResultQueue = currentParent.PredicateResultQueue;
					}

					currentParent.ExecutionQueue.Next = executionQueue;
					executionQueue = currentParent.ExecutionQueue;

					currentParent = currentParent.Parent;
				}

				ExecuteThreadQueue = true;

				while (executionQueue != null)
				{
					var executionQueueList = executionQueue.List;

					// ReSharper disable once ForCanBeConvertedToForeach
					for (var i = 0; i < executionQueueList.Count; i++)
						ExecuteMain(executionQueueList[i]);

					executionQueue = executionQueue.Next;
				}

				ExecuteThreadQueue = false;

				_predicateResultQueue = null;

				thread.Dispose(this);
			}

			public AutomataResult ForkFinish()
			{
				ExecuteThread();

				return _processResources.SuccessAutomataResultPool.Get().Mount(this);
			}

			public AutomataResult ForkRunNext()
			{
				_currentThread.Dispose(this);

				return Run();
			}

			private Thread PopParallelThread()
			{
				var thread = _parallelThreads.Pop();

				if (thread.DfaTrails != null)
				{
					var dfaTrail = thread.DfaTrails[thread.DfaTrailIndex];

					if (thread.DfaTrailIndex + 1 < thread.DfaTrails.Length)
					{
						var contextState = thread.ContextState != null ? Context.CloneContextStateInternal(thread.ContextState) : null;

						_parallelThreads.Push(new Thread(thread.Parent, thread.CurrentNode, thread.InstructionStream, thread.InstructionPointer, contextState, thread.DfaTrails, thread.DfaTrailIndex + 1));
					}

					thread.EnsureStartExecutionQueue();

					_executionPathQueueBuilder.EnqueueDfaPath(thread.StartExecutionQueue, dfaTrail);
				}

				return thread;
			}

#if DEV_EXP
			private StatusKind ForkDfaThread(bool main)
			{
				var threadParent = ForkThreadParent();
				var contextState = _currentThread.ContextState != null ? Context.CloneContextStateInternal(_currentThread.ContextState) : null;
				var dfaThread = new Thread(threadParent, _currentThread.CurrentNode, _currentThread.InstructionStream, _currentThread.InstructionPointer, contextState, _executionPathQueueBuilder.DfaBuilder.DfaState, 0);

				_parallelThreads.PushRef(ref dfaThread);

				if (main)
					_mainThread = _currentThread;
				else
					_currentThread.Dispose(this);

				return StatusKind.Fork;
			}
#endif

			private StatusKind ForkThread(bool main)
			{
				var threadParent = ForkThreadParent();
				var executionPathBuilderCount = _executionPathQueueBuilder.ExecutionPathBuilderCount;

				while (_forkThreads.Count < executionPathBuilderCount)
					_forkThreads.Add(new ForkThreadData());

				for (var j = 0; j < executionPathBuilderCount; j++)
				{
					var executionPathQueue = _executionPathQueueBuilder.ExecutionPathBuilders[j];
					var returnPathCount = executionPathQueue.ReturnPathCount;
					var contextState = _currentThread.ContextState != null ? Context.CloneContextStateInternal(_currentThread.ContextState) : null;
					var forkThreadData = _forkThreads[executionPathBuilderCount - j - 1];
					var thread = new Thread(threadParent, _currentThread.CurrentNode, _currentThread.InstructionStream, _currentThread.InstructionPointer, contextState);

					thread.EnsureStartExecutionQueue();
					forkThreadData.Weight = 0;

					for (var i = 0; i < returnPathCount; i++)
					{
						var executionPath = executionPathQueue.ReturnPaths[i];

						thread.EnqueueStartPath(executionPath);
						forkThreadData.Weight += executionPath.Weight;
					}

					thread.EnqueueStartPath(executionPathQueue.ExecutionPath);
					forkThreadData.Weight += executionPathQueue.ExecutionPath.Weight;
					forkThreadData.Thread = thread;
				}

				foreach (var forkThreadData in _forkThreads.Take(executionPathBuilderCount).OrderBy(t => t.Weight))
					_parallelThreads.PushRef(ref forkThreadData.Thread);

				if (main)
					_mainThread = _currentThread;
				else
					_currentThread.Dispose(this);

				return StatusKind.Fork;
			}

			private StatusKind ForkThreadNode(ForkNode forkNode, bool main)
			{
				var forkPredicateResult = (IForkPredicateResult)forkNode.PredicateResult;
				var startNode = (PredicateNode)forkNode.ExecutionPath.Nodes[forkNode.NodeIndex];
				var threadParent = ForkThreadParent();

				for (var i = 1; i >= 0; i--)
				{
					var predicateEntry = i == 0 ? forkPredicateResult.First : forkPredicateResult.Second;
					var predicateNode = _processResources.ForkPredicateNodePool.Get().Mount(predicateEntry);
					var predicateExecutionPath = _processResources.ForkExecutionPathPool.Get().Mount(startNode, predicateNode);

					predicateNode.CopyLookup(forkNode);

					if (startNode.ForkPathIndex == -1)
					{
						lock (startNode)
						{
							if (startNode.ForkPathIndex == -1)
							{
								_automata.RegisterExecutionPath(predicateExecutionPath);
								startNode.ForkPathIndex = predicateExecutionPath.Id;
							}
							else
								predicateExecutionPath.Id = startNode.ForkPathIndex;
						}
					}
					else
						predicateExecutionPath.Id = startNode.ForkPathIndex;

					predicateNode.ForkPathIndex = startNode.ForkPathIndex;

					var contextState = _currentThread.ContextState != null ? Context.CloneContextStateInternal(_currentThread.ContextState) : null;
					var thread = new Thread(threadParent, forkNode, _currentThread.InstructionStream, _currentThread.InstructionPointer, contextState);

					thread.EnsureStartExecutionQueue();
					thread.EnqueueStartPath(predicateExecutionPath);

					// ReSharper disable once ForCanBeConvertedToForeach
					for (var j = 0; j < forkNode.ForkExecutionPaths.Count; j++)
						thread.EnqueueStartPath(forkNode.ForkExecutionPaths[j]);

					_parallelThreads.PushRef(ref thread);
				}

				if (main)
					_mainThread = _currentThread;
				else
					_currentThread.Dispose(this);

				forkNode.Release();

				return StatusKind.Fork;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			private ThreadParent ForkThreadParent()
			{
				ThreadParent threadParent;

				if (_currentThread.OwnStack || _currentThread.OwnExecutionQueue || _currentThread.OwnPredicateResult)
				{
					threadParent = _processResources.ThreadParentPool.Get();
					threadParent.Mount(ref _currentThread);
				}
				else
					threadParent = _currentThread.Parent;

				return threadParent;
			}

			public StateEntryContext GetTopStateEntryContext(FiniteState state)
			{
				var stack = _currentThread.Stack;
				var subGraphRegistry = _automata._subGraphRegistry;

				for (var i = stack.Count - 1; i >= 0; i--)
				{
					var subGraph = subGraphRegistry[stack.Array[i]];

					if (ReferenceEquals(subGraph.State, state))
						return subGraph.StateEntry?.StateEntryContext;
				}

				return null;
			}

			internal void Initialize(IInstructionReader instructionReader, AutomataContext context)
			{
				if (ReferenceEquals(context.ProcessField, DummyProcess) == false)
					throw new InvalidOperationException("Context is busy");

				_processResources = ProcessResources.ThreadLocalInstance.Value;
				_executionPathQueueBuilder = _processResources.ExecutionPathGroupBuilderPool.Get().AddReference();
				_parallelThreads = _processResources.ThreadListPool.Get().AddReference();
				_executionMethods = _automata.GetExecutionMethods(context);

				Context = context;
				Context.ProcessField = this;

				var entryPoint = context.EntryPoint;

				if (!(entryPoint is FiniteState state))
					return;

				var instructionQueue = _processResources.InstructionQueuePool.Get().Mount(instructionReader, _automata);

				_entryPointSubGraph = _automata.EnsureSubGraph(state);

				var initNode = _entryPointSubGraph.InitNode;

				if (initNode.Safe == false)
					initNode.MakeSafe();

				_currentThread = new Thread(initNode, instructionQueue, context.CreateContextStateInternal())
				{
					Stack = _processResources.StackPool.Get().AddReference(),
					ExecutionQueue = _processResources.ExecutionPathPool.Get().AddReference(),
					PredicateResultQueue = _processResources.PredicateResultPool.Get().AddReference()
				};
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public void MoveInstructionPointer()
			{
				_currentThread.InstructionPointer++;
				//_currentThread.InstructionStream.Move(ref _currentThread.InstructionPointer);
			}

			public AutomataResult PartialRun()
			{
				_partialRun = true;

				return Run();
			}

			public void ReleaseReference()
			{
				if (_referenceCount.ReleaseReference() == 0)
					Dispose();
			}

			public AutomataResult Run()
			{
				try
				{
					loopSingle:

					//switch (ExecuteInstruction(true))
					switch (_parallelThreads.Count > 0 ? StatusKind.Fork : ExecuteInstruction(true))
					{
						case StatusKind.Fork:
							{
								loopParallel:

								_currentThread = PopParallelThread();

								if (_parallelThreads.Count == 0)
								{
									if (StartThread() == false)
										break;

									if (_currentThread.CurrentNode is ForkNode forkNode)
									{
										ForkThreadNode(forkNode, false);

										goto loopParallel;
									}

									ExecuteThread();

									goto loopSingle;
								}

								if (StartThread())
								{
									if (_currentThread.CurrentNode is ForkNode forkNode)
									{
										ForkThreadNode(forkNode, false);

										goto loopParallel;
									}

									switch (ExecuteInstruction(false))
									{
										case StatusKind.Fork:
											goto loopParallel;

										case StatusKind.Finished:

											if (_parallelThreads.Count > 0 && _partialRun)
												return _processResources.ForkAutomataResultPool.Get().Mount(this);

											ExecuteThread();

											return _processResources.SuccessAutomataResultPool.Get().Mount(this);
									}
								}

								_currentThread.Dispose(this);

								if (_parallelThreads.Count == 0)
									break;

								goto loopParallel;
							}

						case StatusKind.Finished:

							return _processResources.SuccessAutomataResultPool.Get().Mount(this);
					}
				}
				catch (Exception e)
				{
					return _processResources.ExceptionAutomataResultPool.Get().Mount(e, this);
				}

				return _processResources.ExceptionAutomataResultPool.Get().Mount(new InvalidOperationException(), this);
			}

			private bool StartThread()
			{
				_currentThread.Stack = _currentThread.Parent.Stack.Clone();
				_currentThread.ExecutionQueue = _currentThread.Parent.ExecutionQueue.Pool.Get().AddReference();

				var executionPaths = _currentThread.StartExecutionQueue?.List;

				if (executionPaths == null)
					return true;

				for (var index = 0; index < executionPaths.Count; index++)
				{
					var executionPath = executionPaths[index];

					ExecuteParallel(executionPath);

					switch (_currentThread.CurrentNode)
					{
						case null:
							return false;
						
						case ForkNode forkNode:
						{
							for (var j = index + 1; j < executionPaths.Count; j++)
								forkNode.ForkExecutionPaths.Add(executionPaths[j]);

							return true;
						}
					}
				}

				return true;
			}

			#endregion

			#region Nested Types

			private sealed class ProcessResources
			{
				#region Static Fields and Constants

				public static readonly ThreadLocal<ProcessResources> ThreadLocalInstance = new ThreadLocal<ProcessResources>(() => new ProcessResources());

				#endregion

				#region Fields

				private readonly LinkedListStackNodePool<Thread> _linkedListStackNodeThreadPool = new LinkedListStackNodePool<Thread>(16);
				public readonly Pool<ExceptionAutomataResult> ExceptionAutomataResultPool;
				public readonly Pool<ExecutionPathQueue> ExecutionPathPool;
				public readonly Pool<ForkAutomataResult> ForkAutomataResultPool;
				public readonly Pool<ExecutionPath> ForkExecutionPathPool;
				public readonly Pool<ForkNode> ForkNodePool;
				public readonly Pool<PredicateNode> ForkPredicateNodePool;
				public readonly Pool<InstructionStream> InstructionQueuePool;
				public readonly Pool<PredicateResultQueue> PredicateResultPool;
				public readonly Pool<AutomataStack> StackPool;
				public readonly Pool<SuccessAutomataResult> SuccessAutomataResultPool;
				public readonly Pool<LinkedListStack<Thread>> ThreadListPool;
				public readonly Pool<ThreadParent> ThreadParentPool;
				public readonly Pool<ExecutionPathGroupBuilder> ExecutionPathGroupBuilderPool;

				#endregion

				#region Ctors

				private ProcessResources()
				{
					var automata = Instance;

					StackPool = new Pool<AutomataStack>(p => new AutomataStack(automata, p));
					ThreadParentPool = new Pool<ThreadParent>(p => new ThreadParent(p));
					ExecutionPathPool = new Pool<ExecutionPathQueue>(p => new ExecutionPathQueue(p));
					PredicateResultPool = new Pool<PredicateResultQueue>(p => new PredicateResultQueue(p));
					InstructionQueuePool = automata?.CreateInstructionQueuePool() ?? new Pool<InstructionStream>(p => new InstructionStream(p));
					ForkExecutionPathPool = new Pool<ExecutionPath>(p => new ExecutionPath(p));
					ForkPredicateNodePool = new Pool<PredicateNode>(p => new PredicateNode(automata, p));
					ForkNodePool = new Pool<ForkNode>(p => new ForkNode(automata, p));
					ThreadListPool = new Pool<LinkedListStack<Thread>>(p => new LinkedListStack<Thread>(_linkedListStackNodeThreadPool, p));
					SuccessAutomataResultPool = new Pool<SuccessAutomataResult>(p => new SuccessAutomataResult(p));
					ExceptionAutomataResultPool = new Pool<ExceptionAutomataResult>(p => new ExceptionAutomataResult(p));
					ForkAutomataResultPool = new Pool<ForkAutomataResult>(p => new ForkAutomataResult(p));
					ExecutionPathGroupBuilderPool = new Pool<ExecutionPathGroupBuilder>(p => new ExecutionPathGroupBuilder(automata, p));
				}

				#endregion
			}

			private sealed class DebugExecutionAlternative
			{
				#region Ctors

				public DebugExecutionAlternative(List<ExecutionPath> executionPaths)
				{
					ExecutionPaths = executionPaths;
					DebugNodes = string.Join("\n ", executionPaths.SelectMany(p => p.Nodes).Select(n => n.ToString()));
				}

				public DebugExecutionAlternative(ExecutionPathQueueBuilder executionPathQueue)
				{
					ExecutionPaths = new List<ExecutionPath>();

					while (true)
					{
						var returnPathCount = executionPathQueue.ReturnPathCount;

						for (var i = 0; i < returnPathCount; i++)
							ExecutionPaths.Add(executionPathQueue.ReturnPaths[i]);

						ExecutionPaths.Add(executionPathQueue.ExecutionPath);

						if (executionPathQueue.NextBuilder != null && executionPathQueue.NextBuilder.ExecutionPathBuilderCount == 1)
						{
							executionPathQueue = executionPathQueue.NextBuilder.ExecutionPathBuilders[0];
						}
						else
							break;
					}

					DebugNodes = string.Join("\n ", ExecutionPaths.SelectMany(p => p.Nodes).Select(n => n.ToString()));
				}

				#endregion

				#region Properties

				public string DebugNodes { get; }

				public List<ExecutionPath> ExecutionPaths { get; }

				#endregion

				#region Methods

				public override string ToString()
				{
					return DebugNodes;
				}

				#endregion
			}

			private sealed class ForkThreadData
			{
				#region Fields

				public Thread Thread;
				public int Weight;

				#endregion
			}

			#endregion
		}

		private enum StatusKind
		{
			Fork,
			Finished,
			Unexpected
		}

		#endregion
	}
}