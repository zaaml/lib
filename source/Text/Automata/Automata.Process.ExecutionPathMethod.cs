// <copyright file="Automata.Process.ExecutionPathMethod.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Runtime.CompilerServices;

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		private protected partial class Process
		{
			private static readonly ExecutionPathMethodDelegate ExecuteForkPathDelegate = ExecuteForkPath;

			internal delegate Node ExecutionPathMethodDelegate(Process process, ref Thread thread, ref ThreadContext threadContext, object[] closure);

			internal sealed class ExecutionPathMethod
			{
				private readonly object[] _closure;
				private readonly ExecutionPathMethodDelegate _delegate;

				public ExecutionPathMethod(ExecutionPathMethodKind kind, ProcessILGenerator builder, ExecutionPath executionPath)
				{
					Kind = kind;
					ExecutionPath = executionPath;
			
					_delegate = ExecutionPath.IsForkExecutionPath ? ExecuteForkPathDelegate : builder.Build(kind, executionPath, out _closure);

					if (ExecutionPath.IsForkExecutionPath)
						_closure = new object[] { this };
				}

				public ExecutionPath ExecutionPath { get; }
				public ExecutionPathMethodKind Kind { get; }

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public Node Execute(Process process, ref Thread thread, ref ThreadContext threadContext)
				{
					return _delegate(process, ref thread, ref threadContext, _closure);
				}
			}
		}
	}
}