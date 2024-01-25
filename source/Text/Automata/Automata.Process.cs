// <copyright file="Automata.Process.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Threading;
using Zaaml.Core;

// ReSharper disable ForCanBeConvertedToForeach

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		private protected partial class Process
		{
			private readonly Automata<TInstruction, TOperand> _automata;
			private readonly AutomataStackCleaner _automataStackCleaner;
			private readonly AutomataContext _context;
			private readonly ExecutionRailBuilder _executionRailBuilder;
			private readonly ExecutionStream _executionStream;
			private readonly NfaTransitionBuilder _nfaTransitionBuilder;
			private readonly PrecedenceContextCleaner _precedenceContextCleaner;
			private readonly PrecedenceStackCleaner _precedenceStackCleaner;
			private readonly PredicateResultStream _predicateResultStream;
			private readonly ProcessKind _processKind;
			private readonly ProcessResources _processResources;
			private readonly AutomataTelemetry _telemetry;
			private readonly AutomataStack _evalStack;
			private readonly List<ResetForkNode> _resetForkStack = new();
			private CancellationToken _cancellationToken;

			private ReferenceCounter _referenceCount;
			private ThreadCollection _threads;

			public Process(IInstructionReader instructionReader, AutomataContext context)
			{
				_context = context;
				_automata = context.Automata;
				_processKind = context.ProcessKind;
				_processResources = ProcessResources.Get(_automata);

				_nfaTransitionBuilder = _processResources.NfaTransitionBuilderPool.Rent();

				var initNode = _automata.EnsureSubGraph(context.Rule).InitNode;
				var instructionStream = _processResources.InstructionStreamPool.Rent().Mount(instructionReader, _automata);

				_executionStream = _processResources.ExecutionStreamPool.Rent();
				_executionRailBuilder = _processResources.ThreadTransitionPool.Rent();
				_predicateResultStream = _processResources.PredicateResultStreamPool.Rent();
				_automataStackCleaner = new AutomataStackCleaner(_processResources.AutomataStackCleanNodePool);
				_precedenceStackCleaner = new PrecedenceStackCleaner(_processResources.PrecedenceStackCleanerNodePool);
				_precedenceContextCleaner = new PrecedenceContextCleaner(_processResources.PrecedenceContextCleanerNodePool, _precedenceStackCleaner);
				_evalStack = _automataStackCleaner.Add(_processResources.StackPool.Rent().Allocate(StackCapacity));

				var stack = _automataStackCleaner.Add(_processResources.StackPool.Rent().Allocate(StackCapacity));
				var precedenceContext = _precedenceContextCleaner.Add(_processResources.PrecedencePool.Rent().Allocate(PrecedenceStackCapacity));

				var thread = new Thread(initNode, stack, precedenceContext);
				var threadContext = new ThreadContext(this, instructionStream, _executionStream, _predicateResultStream, context);

				_executionStream.AddReference();
				_predicateResultStream.AddReference();

				_threads = new ThreadCollection(this, thread, threadContext, _processResources.ResetForkNodePool.Rent());
				_telemetry = (AutomataTelemetry)context.ServiceProvider?.GetService(typeof(AutomataTelemetry));
			}

			public AutomataContextState ContextState => CurrentThreadContext.AutomataContextState;

			private ref ThreadContext CurrentThreadContext => ref _threads.Context;
			
			private ref Thread CurrentThread => ref _threads.Thread;

			private protected virtual ProcessILGenerator CreateILGenerator(Automata<TInstruction, TOperand> automata)
			{
				return new ProcessILGenerator(automata);
			}

			private protected ProcessILGenerator ILGenerator => StaticILGenerator ??= CreateILGenerator(_automata);

			private protected int MainMethodIndex => ILGenerator.MainExecutionMethodIndex;
			
			private protected int ParallelMethodIndex => ILGenerator.ParallelExecutionMethodIndex;

			public TInstruction Instruction => CurrentThreadContext.Instruction;

			public int InstructionOperand => CurrentThreadContext.InstructionOperand;

			public int InstructionPointer => CurrentThreadContext.InstructionStreamPointer;

			public ref TInstruction InstructionReference => ref CurrentThreadContext.Instruction;

			public int InstructionStreamPosition => CurrentThreadContext.InstructionStreamPosition;

			public InstructionStream InstructionStream => CurrentThreadContext.InstructionStream;

			public bool IsMainThread => CurrentThreadContext.Index == 0;

			public int StackCapacity => 0xFFFF;

			public int PrecedenceStackCapacity => 0xFFFF;

			private bool DeferExecution => false;

			public CancellationToken CancellationToken => _cancellationToken;

			public void AddReference()
			{
				_referenceCount.AddReference();
			}

			public void AdvanceInstructionPosition(int position)
			{
				ref var threadContext = ref CurrentThreadContext;
				var instructionQueue = threadContext.InstructionStream;

				threadContext.InstructionStream = instructionQueue.Advance(position, threadContext.InstructionStreamPointer, _automata);
				threadContext.InstructionStream.AddReference();

				instructionQueue.ReleaseReference();
			}

			[UsedImplicitly]
			private ForkNode BuildForkNode(int nodeIndex, ExecutionPath executionPath, PredicateResult predicateResult)
			{
				return _processResources.ForkNodePool.Rent().Mount(nodeIndex, executionPath, predicateResult);
			}

			private ForkNode BuildPredicateForkNode(PredicateNode predicateNode, PredicateResult predicateResult)
			{
				return _processResources.ForkNodePool.Rent().Mount(predicateNode, predicateResult);
			}

			private PredicateResult CallPredicate(ExecutionPath executionPath)
			{
				var predicateNode = (PredicateNode)executionPath.Nodes[executionPath.Nodes.Length - 1];
				var predicateResult = predicateNode.PredicateEntry.PassInternal(_context);

				return predicateResult;
			}

			internal PredicateResult DequeuePredicateResult()
			{
				return CurrentThreadContext.DequeuePredicateResult();
			}

			private static Node ExecuteForkPathMain(ExecutionPath executionPath, Process process)
			{
				var predicateNode = (PredicateNode)executionPath.Nodes[0];

				return process.ExecuteForkPathMain(predicateNode, executionPath);
			}

			private static Node ExecuteForkPathParallel(ExecutionPath executionPath, Process process)
			{
				
				var predicateNode = (PredicateNode)executionPath.Nodes[0];

				return process.ExecuteForkPathParallel(predicateNode, executionPath);
			}

			private Node ExecuteForkPathParallel(PredicateNode node, ExecutionPath executionPath)
			{
				try
				{
					CurrentThreadContext.EnqueueParallelPath(executionPath);

					var predicateResult = node.PredicateEntry.PassInternal(_context);

					if (predicateResult == null)
						return UnexpectedNode.Instance;

					if (predicateResult.IsFork())
						return BuildPredicateForkNode(node, predicateResult);

					if (node.PredicateEntry.ConsumeResult)
						EnqueuePredicateResult(predicateResult);

					return node;
				}
				finally
				{
					executionPath.Release();
				}
			}

			private Node ExecuteForkPathMain(PredicateNode node, ExecutionPath executionPath)
			{
				try
				{
					if (GetExecuteThreadQueue())
					{
						if (node.PredicateEntry.ConsumeResult)
						{
							if (node.PredicateEntry.PopResult)
							{
								var predicateResult = DequeuePredicateResult();

								ConsumePredicateResult(predicateResult, node.PredicateEntry.GetActualPredicateEntry());
							}
						}
					}
					else
					{
						var predicateResult = node.PredicateEntry.PassInternal(_context);

						if (predicateResult == null)
							return UnexpectedNode.Instance;

						if (predicateResult.IsFork())
							return BuildPredicateForkNode(node, predicateResult);

						if (node.PredicateEntry.ConsumeResult)
							ConsumePredicateResult(predicateResult, node.PredicateEntry.GetActualPredicateEntry());

						predicateResult.Dispose();
					}

					return node;
				}
				finally
				{
					executionPath.Release();
				}
			}

			private protected virtual void ConsumePredicateResult(PredicateResult predicateResult, PredicateEntryBase predicateEntry)
			{
			}

			protected virtual void Dispose()
			{
				_evalStack.Dispose();
				_nfaTransitionBuilder.Dispose();
				_executionStream.ReleaseReference();
				_predicateResultStream.ReleaseReference();
				_executionRailBuilder.Dispose();
				_threads.Dispose();
				_automataStackCleaner.Clean();
				_precedenceStackCleaner.Clean();
				_precedenceContextCleaner.Clean();
			}

			internal void EnqueuePredicateResult(PredicateResult predicateResult)
			{
				CurrentThreadContext.EnqueuePredicateResult(predicateResult);
			}

			public AutomataResult ForkFinish()
			{
				RunExecutionStream();
				OnFinished();

				return _processResources.SuccessAutomataResultPool.Rent().Mount(this);
			}

			public AutomataResult ForkRunNext()
			{
				return RunPrivate();
			}

			private ThreadStatusKind ForkThread(ref Thread thread, ref ThreadContext context, ExecutionRailList executionRailList)
			{
				//var resetForkNode = _processResources.ResetForkNodePool.Rent();
				//var head = _resetForkStack.Count - 1;

				//resetForkNode.FrameHead = _resetForkStack[head];
				//resetForkNode.Next = resetForkNode.FrameHead.Next;
				//resetForkNode.FrameHead.Next = resetForkNode;

				var resetForkNode = ResetForkNode.Empty;

				return _threads.ForkThread(ref thread, ref context, executionRailList, resetForkNode);
			}

			private bool GetExecuteThreadQueue()
			{
				return CurrentThreadContext.IsExecutionStreamRunning;
			}

			public ref TInstruction GetInstructionOperand(out int operand)
			{
				return ref CurrentThreadContext.GetInstructionOperand(out operand);
			}

			public void ReleaseReference()
			{
				if (_referenceCount.ReleaseReference() == 0)
					Dispose();
			}

			private AutomataResult RunPrivate()
			{
				return Run(CancellationToken);
			}

			private void EnterForkFrame()
			{
				if (GetExecuteThreadQueue())
					return;

				//var resetForkNode = _processResources.ResetForkNodePool.Rent();

				//resetForkNode.ThreadHead = -1;

				//_resetForkStack.Add(resetForkNode);
			}

			private void LeaveForkFrame()
			{
				if (GetExecuteThreadQueue())
					return;

				//var resetForkNode = _resetForkStack[_resetForkStack.Count - 1];

				//_resetForkStack.RemoveAt(_resetForkStack.Count - 1);

				//while (resetForkNode != null)
				//{
				//	if (resetForkNode.ThreadHead != -1)
				//		_threads.ResetFork(resetForkNode.ThreadHead);

				//	var next = resetForkNode.Next;
					
				//	resetForkNode.Next = null;
				//	_processResources.ResetForkNodePool.Release(resetForkNode);

				//	resetForkNode = next;
				//}
			}

			private void ClearForkFrame()
			{
				foreach (var resetForkNode in _resetForkStack) 
					resetForkNode.Dispose();

				_resetForkStack.Clear();
			}

			public AutomataResult Run(CancellationToken cancellationToken)
			{
				_cancellationToken = cancellationToken;
				_telemetry?.StartSimulation();

				try
				{
					EnterForkFrame();

					_threads.Init();

					InitDeferExecution();

					while (true)
					{
						if (_cancellationToken.IsCancellationRequested)
							return _processResources.ExceptionAutomataResultPool.Rent().Mount(new OperationCanceledException(), this);

						ref var threadFork = ref _threads.Pop();

						if (threadFork.IsEmpty)
							break;

						ref var thread = ref threadFork.Thread;
						ref var threadContext = ref threadFork.Context;

						continue_run:

						switch (thread.Run(ref threadContext))
						{
							case ThreadStatusKind.Run:
								throw new InvalidOperationException();

							case ThreadStatusKind.Fork:
								continue;

							case ThreadStatusKind.Block:
								threadContext.IsCompleteBlock = false;

								RunExecutionStream();

								threadFork = ref _threads.Peek();
								thread = ref threadFork.Thread;
								threadContext = ref threadFork.Context;

								goto continue_run;

							case ThreadStatusKind.Finished:

								if (_processKind == ProcessKind.SubProcess && _threads.Parallel)
									return _processResources.ForkAutomataResultPool.Rent().Mount(this);

								_telemetry?.StopSimulation();
								_telemetry?.StartExecution();

								RunExecutionStream();
								OnFinished();

								_telemetry?.StopExecution();

								return _processResources.SuccessAutomataResultPool.Rent().Mount(this);

							case ThreadStatusKind.Unexpected:
								continue;
						}
					}
				}
				catch (Exception e)
				{
					return _processResources.ExceptionAutomataResultPool.Rent().Mount(e, this);
				}
				finally
				{
					ClearForkFrame();
				}

				return _processResources.ExceptionAutomataResultPool.Rent().Mount(new InvalidOperationException(), this);
			}

			private void InitDeferExecution()
			{
				if (DeferExecution == false)
					return;

				ref var mainThreadFork = ref _threads.Pop();

				var root = new ExecutionRailNode
				{
					ExecutionRail = MemorySpan<int>.Empty,
					Next = new ExecutionRailNode
					{
						ExecutionRail = MemorySpan<int>.Empty,
					}
				};

				ForkThread(ref mainThreadFork.Thread, ref mainThreadFork.Context, new ExecutionRailList(2, root));
			}

			protected virtual void OnFinished()
			{
			}

			private void RunExecutionStream()
			{
				_threads.RunExecutionStream();
			}

			private bool ShouldPopPredicateResult(ExecutionPath executionPath)
			{
				var predicateNode = (PredicateNode)executionPath.Nodes[executionPath.Nodes.Length - 1];

				return predicateNode.PredicateEntry.PopResult;
			}
		}
	}
}