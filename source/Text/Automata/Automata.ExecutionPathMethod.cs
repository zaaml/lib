// <copyright file="Automata.ExecutionPathMethod.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Runtime.CompilerServices;

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		#region Nested Types

		private sealed class ExecutionPathMethod
		{
			#region Fields

			private object[] _mainClosure;
			private Func<Process, AutomataStack, object[], Node> _mainDelegate;
			private object[] _parallelClosure;
			private Func<Process, AutomataStack, object[], Node> _parallelDelegate;

			#endregion

			#region Ctors

			public ExecutionPathMethod(ExecutionPath executionPath, IILBuilder builder, bool lazy)
			{
				if (lazy)
				{
					_mainDelegate = (process, threadStack, closure) =>
					{
						_mainDelegate = builder.BuildMain(executionPath, out _mainClosure);

						return _mainDelegate(process, threadStack, _mainClosure);
					};

					_parallelDelegate = (process, threadStack, closure) =>
					{
						_parallelDelegate = builder.BuildParallel(executionPath, out _parallelClosure);

						return _parallelDelegate(process, threadStack, _parallelClosure);
					};
				}
				else
				{
					_mainDelegate = builder.BuildMain(executionPath, out _mainClosure);
					_parallelDelegate = builder.BuildParallel(executionPath, out _parallelClosure);
				}
			}

			public ExecutionPathMethod(IILBuilder builder, Automata<TInstruction, TOperand> automata, int index)
			{
				_mainDelegate = (process, threadStack, closure) =>
				{
					var executionPath = automata._executionPathRegistry[index];

					_mainDelegate = builder.BuildMain(executionPath, out _mainClosure);

					return _mainDelegate(process, threadStack, _mainClosure);
				};

				_parallelDelegate = (process, threadStack, closure) =>
				{
					var executionPath = automata._executionPathRegistry[index];

					_parallelDelegate = builder.BuildParallel(executionPath, out _parallelClosure);

					return _parallelDelegate(process, threadStack, _parallelClosure);
				};
			}

			#endregion

			#region Methods

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public Node ExecuteMain(Process process, AutomataStack threadStack)
			{
				return _mainDelegate(process, threadStack, _mainClosure);
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public Node ExecuteParallel(Process process, AutomataStack threadStack)
			{
				return _parallelDelegate(process, threadStack, _parallelClosure);
			}

			#endregion
		}

		#endregion
	}
}