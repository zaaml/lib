// <copyright file="Automata.ThreadStatusKind.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		private protected enum ThreadStatusKind
		{
			Run,
			Fork,
			Block,
			Finished,
			Unexpected
		}
	}
}