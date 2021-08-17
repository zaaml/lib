// <copyright file="Automata.Process.ThreadCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

// ReSharper disable ForCanBeConvertedToForeach

using System;
using System.Buffers;
using System.Diagnostics;
using Zaaml.Core;
using Zaaml.Core.Utils;

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		partial class Process
		{
			private struct ThreadCollection
			{
				private readonly Process _process;
				private ThreadFork[] _threads;
				private int _threadsHead;

				public ThreadCollection(Process process, Thread thread, ThreadContext threadContext)
				{
					_process = process;
					_threads = ArrayPool<ThreadFork>.Shared.Rent(64);
					_threadsHead = 0;

					_threads[_threadsHead] = new ThreadFork(thread, threadContext);
				}

				public ref ThreadContext Context => ref _threads[_threadsHead].Context;

				public ref Thread Thread => ref _threads[_threadsHead].Thread;

				public void Dispose()
				{
					while (_threadsHead >= 0)
						_threads[_threadsHead--].Dispose();

					ArrayPool<ThreadFork>.Shared.Return(_threads, true);
				}

				public void RunExecutionStream()
				{
					if (_threadsHead > 0)
					{
						PreRunExecutionStream();

						while (_threadsHead > 0)
							_threads[_threadsHead--].Dispose();
					}

					RunExecutionStreamPrivate();
				}

				private void PreRunExecutionStream()
				{
					ref var tailThreadFork = ref _threads[0];
					ref var headThreadFork = ref _threads[_threadsHead];

					tailThreadFork.Thread.StackExchange(ref headThreadFork.Thread);

					(tailThreadFork.Context.InstructionStream, headThreadFork.Context.InstructionStream) = (headThreadFork.Context.InstructionStream, tailThreadFork.Context.InstructionStream);

					tailThreadFork.Context.ExecutionStreamPointer = headThreadFork.Context.ExecutionStreamPointer;
					tailThreadFork.Context.PredicateResultStreamPointer = headThreadFork.Context.PredicateResultStreamPointer;
				}

				private void RunExecutionStreamPrivate()
				{
					if (_threadsHead > 0)
						return;

					ref var tailThreadFork = ref _threads[0];

					if (tailThreadFork.Context.ExecutionStreamPointer == 0) 
						return;

					tailThreadFork.Context.RunExecutionStream(ref tailThreadFork.Thread);
					tailThreadFork.Count = 1;
				}

				public ref ThreadFork Pop()
				{
					while (true)
					{
						ref var forkThread = ref _threads[_threadsHead];

						if (forkThread.Count > 0)
						{
							forkThread.Count--;

							if (_threadsHead == 0)
								return ref forkThread;

							ref var prevForkThread = ref _threads[_threadsHead - 1];

							while (forkThread.Count == 0 && prevForkThread.Count == 0)
							{
								var prevExecutionPaths = prevForkThread.ExecutionPaths;

								prevForkThread.Thread.StackExchange(ref forkThread.Thread);
								prevForkThread.ExecutionPaths = forkThread.ExecutionPaths;
								prevForkThread.ExecutionPathsHead = forkThread.ExecutionPathsHead;

								forkThread.ExecutionPaths = prevExecutionPaths;
								forkThread.Dispose();

								forkThread = ref _threads[--_threadsHead];

								if (_threadsHead == 0 || _threads[_threadsHead - 1].Count > 0)
								{
									forkThread.Thread.EntryExecution = forkThread.ExecutionPaths.Slice(forkThread.ExecutionPathsHead, forkThread.ExecutionPaths.Length - forkThread.ExecutionPathsHead - 1);

									return ref forkThread;
								}

								prevForkThread = ref _threads[_threadsHead - 1];
							}

							var head = forkThread.ExecutionPathsHead;
							var forkThreadExecutionPaths = forkThread.ExecutionPaths.Span;

							while (forkThreadExecutionPaths[forkThread.ExecutionPathsHead++] != 0)
							{
							}

							var forkPath = forkThread.ExecutionPaths.Slice(head, forkThread.ExecutionPathsHead - head - 1);

							if (forkThread.Thread.Stack != null)
							{
								prevForkThread.Thread.StackExchange(ref forkThread.Thread);

								forkThread.Thread.Dispose();
								forkThread.Context.Dispose();

								forkThread.Thread = prevForkThread.Thread.Fork(forkPath);
								forkThread.Context = prevForkThread.Context.Fork();
							}
							else
							{
								forkThread.Thread = prevForkThread.Thread.Fork(forkPath);
								forkThread.Context = prevForkThread.Context.Fork();
							}

							return ref forkThread;
						}

						Debug.Assert(ReferenceEquals(forkThread.Thread.Stack, _process._stack));

						if (_threadsHead > 0)
							_threads[_threadsHead - 1].Thread.StackExchange(ref forkThread.Thread);

						forkThread.Dispose();

						_threadsHead--;

						if (_threadsHead >= 0)
							continue;

						return ref ThreadFork.Empty;
					}
				}

				public bool Parallel => _threadsHead > 0;

				public ThreadStatusKind ForkThread(ref Thread thread, ref ThreadContext threadContext, ReadOnlySpan<int> executionPaths)
				{
					Debug.Assert(_threadsHead == threadContext.Index);
					Debug.Assert(ReferenceEquals(_threads[_threadsHead].Thread.Stack, thread.Stack));
					Debug.Assert(ReferenceEquals(thread.Stack, _process._stack));

					var forkCount = ForkExecutionPaths(executionPaths, out var forkExecutionPaths);

					if (_threadsHead + 1 == _threads.Length)
						ArrayUtils.ExpandArrayLength(ref _threads, ArrayPool<ThreadFork>.Shared, true, true);

					_threads[++_threadsHead] = new ThreadFork(forkCount, default, default, forkExecutionPaths);

					return ThreadStatusKind.Fork;
				}

				private int ForkExecutionPaths(ReadOnlySpan<int> executionPaths, out MemorySpan<int> forkExecutionPaths)
				{
					forkExecutionPaths = _process._processResources.DynamicMemorySpanAllocator.Allocate(executionPaths.Length);

					var forkCount = 0;
					var forkExecutionPathsSpan = forkExecutionPaths.Span;

					for (var i = 0; i < executionPaths.Length; i++)
					{
						var executionPathId = executionPaths[i];

						forkExecutionPathsSpan[i] = executionPathId;

						if (executionPathId == 0) 
							forkCount++;
					}

					return forkCount;
				}
			}
		}
	}
}