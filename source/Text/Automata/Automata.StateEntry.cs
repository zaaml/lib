// <copyright file="Automata.StateEntry.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		#region Nested Types

		protected class StateEntry : PrimitiveEntry
		{
			#region Ctors

			public StateEntry(FiniteState state)
			{
				State = state;
			}

			#endregion

			#region Properties

			protected override string DebuggerDisplay => State.Name;

			internal bool SkipStack { get; set; }

			public FiniteState State { get; }

			internal StateEntryContext StateEntryContext { get; set; }

			#endregion
		}

		protected abstract class StateEntryContext
		{
		}

		#endregion
	}
}