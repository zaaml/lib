// <copyright file="Automata.PrimitiveEntry.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		#region Nested Types

		protected abstract class PrimitiveEntry : Entry
		{
			#region Methods

			public static implicit operator PrimitiveEntry(FiniteState state)
			{
				return new StateEntry(state);
			}

			public static implicit operator PrimitiveEntry(TOperand input)
			{
				return new SingleMatchEntry(input);
			}

			#endregion
		}

		#endregion
	}
}