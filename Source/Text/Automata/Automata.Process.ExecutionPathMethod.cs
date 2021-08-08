// <copyright file="Automata.Process.ExecutionPathMethod.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Runtime.CompilerServices;

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		private protected sealed class ExecutionPathMethod
		{
			private readonly object[] _closure;
			private readonly Func<Process, object[], Node> _delegate;

			public ExecutionPathMethod(ExecutionPathMethodKind kind, Process.ProcessILGenerator builder, ExecutionPath executionPath)
			{
				_delegate = builder.Build(kind, executionPath, out _closure);
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public Node Execute(Process process)
			{
				return _delegate(process, _closure);
			}
		}
	}
}