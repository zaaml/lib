// <copyright file="Automata.InternalState.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		#region Nested Types

		private class InternalState : FiniteState
		{
			#region Ctors

			public InternalState(string name) : base(name)
			{
			}

			#endregion
		}

		#endregion
	}
}