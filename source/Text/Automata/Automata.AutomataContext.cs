// <copyright file="Automata.AutomataContext.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.Core;

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		#region Nested Types

		protected abstract partial class AutomataContext
		{
			#region Fields

			private Process _process = Process.DummyProcess;

			#endregion

			#region Ctors

			protected AutomataContext(Rule rule)
			{
				EntryPoint = rule;
			}

			#endregion

			#region Properties

			protected AutomataContextState ContextState => _process.ContextState;

			internal AutomataContextState ContextStateInternal => _process.ContextState;

			public EntryPoint EntryPoint { get; }

			[UsedImplicitly] public TInstruction Instruction => _process.Instruction;

			[UsedImplicitly] public int InstructionPointer => _process.InstructionPointer;

			// ReSharper disable once MemberCanBeProtected.Global
			public ref TInstruction InstructionReference => ref _process.Instruction;

			public int InstructionOperand => _process.InstructionOperand;

			protected int InstructionStreamPosition => _process.InstructionStreamPosition;

			internal bool IsMainThread => _process.IsMainThread;

			internal object ProcessField
			{
				get => _process;
				set => _process = (Process) value;
			}

			protected Rule Rule => (Rule) EntryPoint;

			#endregion

			#region Methods

			public ref TInstruction GetInstructionOperand(out int operand)
			{
				return ref _process.GetInstructionOperand(out operand);
			}

			protected void AdvanceInstructionPosition()
			{
				_process.AdvanceInstructionPosition();
			}

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

			internal RuleEntryContext GetTopRuleEntryContext(Rule state)
			{
				return _process.GetTopRuleEntryContext(state);
			}

			#endregion
		}

		protected abstract class AutomataContextState
		{
		}

		#endregion
	}
}