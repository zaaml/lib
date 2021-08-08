// <copyright file="Automata.InternalState.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		private class InternalState : Rule
		{
			public InternalState(string name) : base(name)
			{
			}
		}
	}
}