// <copyright file="Automata.Process.ThreadCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

// ReSharper disable ForCanBeConvertedToForeach

using System;
using System.Buffers;
using System.Diagnostics;
using System.Reflection;
using Zaaml.Core.Reflection;
using Zaaml.Core.Utils;

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		partial class Process
		{
			private struct ThreadCollection
			{
				public static readonly Type Type = typeof(ThreadCollection);
				public static FieldInfo ThreadsField = Type.GetField(nameof(_threads), BF.IPNP);
				public static FieldInfo ThreadsHeadField = Type.GetField(nameof(_threadsHead), BF.IPNP);

				private readonly Process _process;
				private ThreadFork[] _threads;
				private int _threadsHead;

				public ThreadCollection(Process process, Thread thread, ThreadContext threadContext, ResetForkNode resetForkNode)
				{
					_process = process;
					_threads = ArrayPool<ThreadFork>.Shared.Rent(64);
					_threadsHead = 0;
					_threads[_threadsHead] = new ThreadFork(thread, threadContext, resetForkNode);
				}

				public ref ThreadContext Context => ref _threads[_threadsHead].Context;

				public ref Thread Thread => ref _threads[_threadsHead].Thread;

				public void Dispose()
				{
					while (_threadsHead >= 0 && _threads[_threadsHead].IsEmpty)
						_threadsHead--;

					while (_threadsHead >= 0)
						_threads[_threadsHead--].Dispose();

					ArrayPool<ThreadFork>.Shared.Return(_threads, true);
				}

				public void RunExecutionStream()
				{
					if (_threadsHead == 0)
						return;

					ref var tailThreadFork = ref _threads[0];
					ref var headThreadFork = ref _threads[_threadsHead];

					for (var i = _threadsHead; i > 0; i--)
					{
						ref var prevThread = ref _threads[i - 1].Thread;
						ref var currThread = ref _threads[i].Thread;

						prevThread.StackForkExchange(ref currThread);
					}

					(tailThreadFork.Context.InstructionStream, headThreadFork.Context.InstructionStream) = (headThreadFork.Context.InstructionStream, tailThreadFork.Context.InstructionStream);

					tailThreadFork.Context.ExecutionStreamPointer = headThreadFork.Context.ExecutionStreamPointer;
					tailThreadFork.Context.PredicateResultStreamPointer = headThreadFork.Context.PredicateResultStreamPointer;

					while (_threadsHead > 0)
						_threads[_threadsHead--].Dispose();

					if (tailThreadFork.Context.ExecutionStreamPointer == 0)
						return;

					tailThreadFork.Context.RunExecutionStream(ref tailThreadFork.Thread);
				}

				public ref Thread PrevThread()
				{
					if (_threadsHead == 0)
						throw new InvalidOperationException();

					return ref _threads[_threadsHead - 1].Thread;
				}

				public ref ThreadContext PrevContext()
				{
					if (_threadsHead == 0)
						throw new InvalidOperationException();

					return ref _threads[_threadsHead - 1].Context;
				}

				public ref ThreadContext PeekContext()
				{
					return ref _threads[_threadsHead].Context;
				}

				public ref ThreadFork Peek()
				{
					return ref _threads[_threadsHead];
				}

				public ref ThreadFork Pop()
				{
					while (true)
					{
						ref var forkThread = ref _threads[_threadsHead];

						if (forkThread.ExecutionRailList.Count > 0)
						{
							forkThread.ExecutionRailList = forkThread.ExecutionRailList.MoveNext(out var executionRailNode);

							if (_threadsHead == 0)
								return ref forkThread;

							ref var prevForkThread = ref _threads[_threadsHead - 1];

							while (forkThread.ExecutionRailList.Count == 0 && prevForkThread.ExecutionRailList.Count == 0)
							{
								prevForkThread.Thread.StackForkExchange(ref forkThread.Thread);

								forkThread.Dispose();

								forkThread = ref _threads[--_threadsHead];

								if (_threadsHead == 0 || _threads[_threadsHead - 1].ExecutionRailList.Count > 0)
								{
									forkThread.Thread.ExecutionRailNode = executionRailNode;

									return ref forkThread;
								}

								prevForkThread = ref _threads[_threadsHead - 1];
							}

							if (forkThread.Thread.Stack == null)
							{
								forkThread.Thread = prevForkThread.Thread.Fork(executionRailNode);
								forkThread.Context = prevForkThread.Context.Fork();
							}
							else
							{
								prevForkThread.Thread.StackForkExchange(ref forkThread.Thread);

								forkThread.Thread.Dispose();
								forkThread.Context.Dispose();

								forkThread.Thread = prevForkThread.Thread.Fork(executionRailNode);
								forkThread.Context = prevForkThread.Context.Fork();
							}

							return ref forkThread;
						}

						if (_threadsHead > 0)
							_threads[_threadsHead - 1].Thread.StackForkExchange(ref forkThread.Thread);

						forkThread.Dispose();

						_threadsHead--;

						if (_threadsHead >= 0)
							continue;

						return ref ThreadFork.Empty;
					}
				}

				public bool Parallel => _threadsHead > 0;

				public ThreadStatusKind ForkThread(ref Thread thread, ref ThreadContext threadContext, ExecutionRailList executionRailList, ResetForkNode resetForkNode)
				{
					Debug.Assert(_threadsHead == threadContext.Index);
					Debug.Assert(ReferenceEquals(_threads[_threadsHead].Thread.Stack, thread.Stack));

					if (_threadsHead + 1 == _threads.Length)
						ArrayUtils.ExpandArrayLength(ref _threads, ArrayPool<ThreadFork>.Shared, true, true);

					_threads[++_threadsHead] = new ThreadFork(default, default, executionRailList, resetForkNode);

					resetForkNode.ThreadHead = _threadsHead;

					return ThreadStatusKind.Fork;
				}

				public void Init()
				{
					_threads[0].Context.AutomataContextState = _threads[0].Context.AutomataContext.CreateContextStateInternal();
				}

				public void ResetFork(int threadIndex)
				{
					for (var i = threadIndex; i <= _threadsHead; i++)
					{
						var executionRailList = _threads[i].ExecutionRailList;

						while (executionRailList.Count > 0)
							executionRailList = executionRailList.MoveNext(out _);

						_threads[i].ExecutionRailList = executionRailList;
						_threads[i].ResetForkNode = ResetForkNode.Empty;
					}
				}
			}
		}
	}
}