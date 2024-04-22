// <copyright file="Automata.Process.ThreadCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

// ReSharper disable ForCanBeConvertedToForeach

using System;
using System.Buffers;
using System.Diagnostics;
using Zaaml.Core.Utils;

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		partial class Process
		{
			private bool Parallel => _threadsHead > 0;

			private void EnterForkFrame(int nodeId)
			{
				if (_executing)
					return;

				//var beginSyntaxNode = (BeginSyntaxNode)_automata._nodeRegistry[nodeId];

				_threads[_threadsHead].Frame++;
			}
			
			private void DisposeFork()
			{
				while (_threadsHead >= 0 && _threads[_threadsHead].IsEmpty)
					_threadsHead--;

				while (_threadsHead >= 0)
					_threads[_threadsHead--].Dispose();

				ArrayPool<ThreadFork>.Shared.Return(_threads, true);
			}
			
			private long TotalForkCount => _threads[_threadsHead].TotalForkCount;

			private ThreadStatusKind ForkThread(ref Thread thread, ref ThreadContext threadContext, ExecutionRailList executionRailList)
			{
				Debug.Assert(_threadsHead == threadContext.Index);
				Debug.Assert(ReferenceEquals(_threads[_threadsHead].Thread.Stack, thread.Stack));

				CollapseThreads();

				if (_threadsHead + 1 == _threads.Length)
					ArrayUtils.ExpandArrayLength(ref _threads, ArrayPool<ThreadFork>.Shared, true, true);

				var forkCount = _threadsHead == 0 ? 1 : TotalForkCount;

				_threads[++_threadsHead] = new ThreadFork(default, default, executionRailList, forkCount);

				return ThreadStatusKind.Fork;
			}

			private void CollapseThreads()
			{
				//var instructionPointer = _threads[0].Context.ExecutionStreamPointer;

				while (_threadsHead > 1)
				{
					ref var currThread = ref _threads[_threadsHead];
					ref var prevThread = ref _threads[_threadsHead - 1];

					if (currThread.ExecutionRailList.Count == 0 &&
					    currThread.Frame == 0 &&
					    prevThread.ExecutionRailList.Count == 0 &&
					    prevThread.Frame == 0)
					{
						prevThread.Thread.Stack.Unfork(currThread.Thread.Stack);
						prevThread.Thread.Precedence.Unfork(currThread.Thread.Precedence);

						currThread.Frame = prevThread.Frame;
						currThread.ParentForkCount = prevThread.ParentForkCount;

						(currThread, prevThread) = (prevThread, currThread);

						prevThread.Context.Index = --_threadsHead;
						currThread.Dispose();
					}
					else
					{
						break;
					}
				}

				//if (_threadsHead == 1 && _threads[1].ExecutionRailList.Count == 0)
				//{
				//	RunExecutionStream();
				//}
			}

			private void LeaveForkFrame(int nodeId)
			{
				if (_executing)
					return;

				//var returnSyntaxNode = (ReturnSyntaxNode)_automata._nodeRegistry[nodeId];

				for (var threadHead = _threadsHead; threadHead >= 0; threadHead--)
				{
					ref var headThread = ref _threads[threadHead];

					if (headThread.Frame == 0)
					{
						headThread.ParentForkCount = 0;
						headThread.ExecutionRailList.Dispose();
						headThread.ExecutionRailList = ExecutionRailList.Empty;
					}
					else
					{
						headThread.Frame--;

						return;
					}
				}

				throw new InvalidOperationException("Invalid fork frame.");
			}

			private ref ThreadFork PeekThreadFork()
			{
				return ref _threads[_threadsHead];
			}

			private ref ThreadContext PeekThreadForkContext()
			{
				return ref _threads[_threadsHead].Context;
			}

			private ref ThreadFork PopThreadFork()
			{
				while (true)
				{
					ref var forkThread = ref _threads[_threadsHead];

					_threads[_threadsHead].Frame = 0;

					if (forkThread.ExecutionRailList.Count > 0)
					{
						forkThread.ExecutionRailList = forkThread.ExecutionRailList.MoveNext(out var executionRailNode);

						if (_threadsHead == 0)
							return ref forkThread;

						ref var prevForkThread = ref _threads[_threadsHead - 1];

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

			private ref ThreadContext PrevThreadContext()
			{
				if (_threadsHead == 0)
					throw new InvalidOperationException();

				return ref _threads[_threadsHead - 1].Context;
			}

			private ref Thread PrevThread()
			{
				if (_threadsHead == 0)
					throw new InvalidOperationException();

				return ref _threads[_threadsHead - 1].Thread;
			}
		}
	}
}