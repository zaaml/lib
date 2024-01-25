// <copyright file="Automata.AutomataContext.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		private protected abstract class AutomataContext
		{
			protected AutomataContext(Syntax rule, Automata<TInstruction, TOperand> automata, IServiceProvider serviceProvider)
			{
				Rule = rule;
				Automata = automata;
				ServiceProvider = serviceProvider;
			}

			public Automata<TInstruction, TOperand> Automata { get; }

			public IServiceProvider ServiceProvider { get; }

			internal AutomataContextState ContextStateInternal => Process.ContextState;

			public abstract Process Process { get; }

			public Syntax Rule { get; }

			protected virtual AutomataContextState CloneContextState(AutomataContextState contextState)
			{
				return null;
			}

			internal AutomataContextState CloneContextStateInternal(AutomataContextState contextState)
			{
				return CloneContextState(contextState);
			}

			protected virtual AutomataContextState CreateContextState()
			{
				return null;
			}

			public abstract ProcessKind ProcessKind { get; }

			internal AutomataContextState CreateContextStateInternal()
			{
				return CreateContextState();
			}

			protected virtual void DisposeContextState(AutomataContextState contextState)
			{
			}

			internal void DisposeContextStateInternal(AutomataContextState contextState)
			{
				DisposeContextState(contextState);
			}
		}

		protected abstract class AutomataContextState
		{
		}
	}
}