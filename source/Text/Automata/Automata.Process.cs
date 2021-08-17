// <copyright file="Automata.Process.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Zaaml.Core;
using Zaaml.Core.Utils;

// ReSharper disable ForCanBeConvertedToForeach

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		private protected enum ExecutionPathMethodKind
		{
			Main,
			Parallel
		}

		private protected partial class Process
		{
			internal sealed class ExecutionPathMethodCollection
			{
				private readonly ProcessILGenerator _ilBuilder;
				private readonly ExecutionPathMethodKind _kind;
				private ExecutionPathMethod[] _executionPaths;

				public ExecutionPathMethodCollection(ExecutionPathMethodKind kind, ProcessILGenerator ilBuilder)
				{
					_kind = kind;
					_ilBuilder = ilBuilder;
					_executionPaths = Array.Empty<ExecutionPathMethod>();
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public ExecutionPathMethod GetExecutionPathMethod(ExecutionPath executionPath)
				{
					if (executionPath.Id < _executionPaths.Length)
						return _executionPaths[executionPath.Id] ??= new ExecutionPathMethod(_kind, _ilBuilder, executionPath);

					ArrayUtils.EnsureArrayLength(ref _executionPaths, executionPath.Id + 1, true);

					return _executionPaths[executionPath.Id] ??= new ExecutionPathMethod(_kind, _ilBuilder, executionPath);
				}
			}

			private readonly Automata<TInstruction, TOperand> _automata;
			private readonly AutomataContext _context;
			private readonly ProcessKind _processKind;
			private readonly ProcessResources _processResources;
			private readonly AutomataStack _stack;
			private MemorySpan<int> _executionPathBuilder;
			private ReferenceCounter _referenceCount;
			private ThreadCollection _threads;
			private PrecedenceContextStack _precedenceContext;

			public Process(IInstructionReader instructionReader, AutomataContext context, ProcessILGenerator ilGenerator = null)
			{
				_context = context;
				_automata = context.Automata;
				_processKind = context.ProcessKind;
				_processResources = ProcessResources.Get(_automata);
				_executionPathBuilder = _processResources.DynamicMemorySpanAllocator.Allocate(65536);

				ILGenerator = ilGenerator ?? _automata.ILGenerator;

				var initNode = _automata.EnsureSubGraph(context.Rule).InitNode.EnsureSafe();
				var executionStream = _processResources.ExecutionStreamPool.Get();
				var instructionStream = _processResources.InstructionStreamPool.Get().Mount(instructionReader, _automata);
				var predicateResultStream = _processResources.PredicateResultStreamPool.Get();

				_stack = _processResources.StackPool.Get().Allocate(StackCapacity).AddReference();

				var precedenceStack = _processResources.PrecedenceStackPool.Get();

				_precedenceContext = _processResources.PrecedenceStackPool.Get().AddReference();
				//var thread = new Thread(initNode, _stack, new PrecedenceContext(_automata, _processResources.DynamicMemorySpanAllocator));

				var thread = new Thread(initNode, _stack, precedenceStack);
				var threadContext = new ThreadContext(this, instructionStream, executionStream, predicateResultStream, context.CreateContextStateInternal());

				_threads = new ThreadCollection(this, thread, threadContext);
			}

			public AutomataContextState ContextState => CurrentThreadContext.ContextState;

			private ref ThreadContext CurrentThreadContext => ref _threads.Context;

			private ProcessILGenerator ILGenerator { get; }

			public TInstruction Instruction => CurrentThreadContext.Instruction;

			public int InstructionOperand => CurrentThreadContext.InstructionOperand;

			public int InstructionPointer => CurrentThreadContext.InstructionStreamPointer;

			public ref TInstruction InstructionReference => ref CurrentThreadContext.Instruction;

			public int InstructionStreamPosition => CurrentThreadContext.InstructionStreamPosition;

			public bool IsMainThread => CurrentThreadContext.Index == 0;

			public int StackCapacity => 0xFFFF;

			public void AddReference()
			{
				_referenceCount.AddReference();
			}

			public void AdvanceInstructionPosition(int position)
			{
				ref var threadContext = ref CurrentThreadContext;
				var instructionQueue = threadContext.InstructionStream;

				threadContext.InstructionStream = instructionQueue.Advance(position, threadContext.InstructionStreamPointer, _automata).AddReference();

				instructionQueue.ReleaseReference();
			}

			[UsedImplicitly]
			private ForkNode BuildForkNode(int nodeIndex, ExecutionPath executionPath, PredicateResult predicateResult)
			{
				return _processResources.ForkNodePool.Get().Mount(nodeIndex, executionPath, predicateResult);
			}

			private ForkNode BuildPredicateForkNode(PredicateNode predicateNode, PredicateResult predicateResult)
			{
				return _processResources.ForkNodePool.Get().Mount(predicateNode, predicateResult);
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

			private static Node ExecuteForkPath(Process process, ref Thread thread, ref ThreadContext threadContext, object[] closure)
			{
				var executionMethod = (ExecutionPathMethod)closure[0];
				var executionPath = executionMethod.ExecutionPath;
				var kind = executionMethod.Kind;
				var predicateNode = (PredicateNode)executionPath.Nodes[0];

				return kind == ExecutionPathMethodKind.Main ? process.ExecuteForkPathMain(predicateNode, executionPath, ref thread, ref threadContext) : process.ExecuteForkPathParallel(predicateNode, executionPath, ref thread, ref threadContext);
			}

			private Node ExecuteForkPathParallel(PredicateNode node, ExecutionPath executionPath, ref Thread thread, ref ThreadContext threadContext)
			{
				try
				{
					threadContext.EnqueueParallelPath(executionPath);

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

			private Node ExecuteForkPathMain(PredicateNode node, ExecutionPath executionPath, ref Thread thread, ref ThreadContext threadContext)
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
				_precedenceContext.ReleaseReference();
				_stack.ReleaseReference();
				_executionPathBuilder.Dispose();
				_threads.Dispose();
			}

			private void EnqueueParallelPath(ExecutionPath executionPath)
			{
				CurrentThreadContext.EnqueueParallelPath(executionPath);
			}

			internal void EnqueuePredicateResult(PredicateResult predicateResult)
			{
				CurrentThreadContext.EnqueuePredicateResult(predicateResult);
			}

			public AutomataResult ForkFinish()
			{
				RunExecutionStream();
				OnFinished();

				return _processResources.SuccessAutomataResultPool.Get().Mount(this);
			}

			public AutomataResult ForkRunNext()
			{
				return Run();
			}

			private ThreadStatusKind ForkThread(ref Thread thread, ref ThreadContext context, ReadOnlySpan<int> executionPaths)
			{
				return _threads.ForkThread(ref thread, ref context, executionPaths);
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			private AutomataStack GetAutomataStack()
			{
				return _threads.Thread.Stack;
			}

			private bool GetExecuteThreadQueue()
			{
				return CurrentThreadContext.IsExecutionStreamRunning;
			}

			public ref TInstruction GetInstructionOperand(out int operand)
			{
				return ref CurrentThreadContext.GetInstructionOperand(out operand);
			}

			private UnexpectedNode GetUnexpectedNode()
			{
				return UnexpectedNode.Instance;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public void MoveInstructionPointer()
			{
				CurrentThreadContext.InstructionStreamPointer++;
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
					while (true)
					{
						ref var threadFork = ref _threads.Pop();

						if (threadFork.IsEmpty)
							break;

						ref var thread = ref threadFork.Thread;
						ref var threadContext = ref threadFork.Context;

						switch (thread.Run(ref threadContext))
						{
							case ThreadStatusKind.Run:
								throw new InvalidOperationException();

							case ThreadStatusKind.Fork:
								continue;

							case ThreadStatusKind.Finished:

								if (_processKind == ProcessKind.SubProcess && _threads.Parallel)
									return _processResources.ForkAutomataResultPool.Get().Mount(this);

								RunExecutionStream();
								OnFinished();

								return _processResources.SuccessAutomataResultPool.Get().Mount(this);

							case ThreadStatusKind.Unexpected:
								continue;
						}
					}
				}
				catch (Exception e)
				{
					return _processResources.ExceptionAutomataResultPool.Get().Mount(e, this);
				}

				return _processResources.ExceptionAutomataResultPool.Get().Mount(new InvalidOperationException(), this);
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

			private sealed class ProcessResources
			{
				private static readonly ThreadLocal<Dictionary<Automata<TInstruction, TOperand>, ProcessResources>> ThreadLocalDictionary = new(() => new Dictionary<Automata<TInstruction, TOperand>, ProcessResources>());

				public readonly Pool<ExceptionAutomataResult> ExceptionAutomataResultPool;
				public readonly Pool<ExecutionStream> ExecutionStreamPool;
				public readonly Pool<PredicateResultStream> PredicateResultStreamPool;
				public readonly Pool<ForkAutomataResult> ForkAutomataResultPool;
				public readonly Pool<ExecutionPath> ForkExecutionPathPool;
				public readonly Pool<ForkNode> ForkNodePool;
				public readonly Pool<PredicateNode> ForkPredicateNodePool;
				public readonly Pool<InstructionStream> InstructionStreamPool;
				public readonly MemorySpanAllocator<int> DynamicMemorySpanAllocator = MemorySpanAllocator.Create(ArrayPool<int>.Shared);
				public readonly Pool<AutomataStack> StackPool;
				public readonly Pool<PrecedenceContextStack> PrecedenceStackPool;
				public readonly Pool<SuccessAutomataResult> SuccessAutomataResultPool;

				private ProcessResources(Automata<TInstruction, TOperand> automata)
				{
					StackPool = new Pool<AutomataStack>(p => new AutomataStack(automata, DynamicMemorySpanAllocator, p));
					PrecedenceStackPool = new Pool<PrecedenceContextStack>(p => new PrecedenceContextStack(automata, DynamicMemorySpanAllocator, p));
					InstructionStreamPool = automata?.CreateInstructionStreamPool() ?? new Pool<InstructionStream>(p => new InstructionStream(p));
					ExecutionStreamPool = new Pool<ExecutionStream>(p => new ExecutionStream(DynamicMemorySpanAllocator, p));
					PredicateResultStreamPool = new Pool<PredicateResultStream>(p => new PredicateResultStream(MemorySpanAllocator<PredicateResult>.Shared, p));
					ForkExecutionPathPool = new Pool<ExecutionPath>(p => new ExecutionPath(automata, p));
					ForkPredicateNodePool = new Pool<PredicateNode>(p => new PredicateNode(automata, p));
					ForkNodePool = new Pool<ForkNode>(p => new ForkNode(automata, p));
					SuccessAutomataResultPool = new Pool<SuccessAutomataResult>(p => new SuccessAutomataResult(p));
					ExceptionAutomataResultPool = new Pool<ExceptionAutomataResult>(p => new ExceptionAutomataResult(p));
					ForkAutomataResultPool = new Pool<ForkAutomataResult>(p => new ForkAutomataResult(p));
				}

				public static ProcessResources Get(Automata<TInstruction, TOperand> automata)
				{
					var threadLocalDictionary = ThreadLocalDictionary.Value;

					if (threadLocalDictionary.TryGetValue(automata, out var resources) == false)
						threadLocalDictionary[automata] = resources = new ProcessResources(automata);

					return resources;
				}
			}
		}

		private protected enum ThreadStatusKind
		{
			Run,
			Fork,
			Finished,
			Unexpected
		}
	}
}