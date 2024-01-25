// <copyright file="Automata.ExecutionPathMethodKind.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		private protected enum ExecutionPathMethodKind
		{
			Main,
			Parallel
		}
	}
}