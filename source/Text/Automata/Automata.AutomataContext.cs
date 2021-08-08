// <copyright file="Automata.AutomataContext.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		private protected abstract class AutomataContext
		{
			protected AutomataContext(Rule rule, Automata<TInstruction, TOperand> automata)
			{
				Rule = rule;
				Automata = automata;
			}

			public Automata<TInstruction, TOperand> Automata { get; }

			internal AutomataContextState ContextStateInternal => Process.ContextState;

			public abstract Process Process { get; }

			public Rule Rule { get; }

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