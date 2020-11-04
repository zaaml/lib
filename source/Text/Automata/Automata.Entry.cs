// <copyright file="Automata.Entry.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Diagnostics;

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		#region Nested Types

		[DebuggerDisplay("{" + nameof(DebuggerDisplay) + "}")]
		protected abstract class Entry
		{
			#region Properties

			protected abstract string DebuggerDisplay { get; }

			#endregion

			#region Methods

			public static implicit operator Entry(TOperand operand)
			{
				return new SingleMatchEntry(operand);
			}

			public static implicit operator Entry(FiniteState state)
			{
				return new StateEntry(state);
			}

			public override string ToString()
			{
				return DebuggerDisplay;
			}

			#endregion
		}

		#endregion
	}
}