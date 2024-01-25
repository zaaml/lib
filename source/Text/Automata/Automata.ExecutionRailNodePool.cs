// <copyright file="Automata.Pool.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Runtime.CompilerServices;

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		private protected sealed class ExecutionRailNodePool
		{
			private ExecutionRailNode _poolHead;

			public ExecutionRailNode Rent()
			{
				if (_poolHead == null)
					return new ExecutionRailNode(this);

				var reference = _poolHead;
				var next = reference.Next;

				reference.Next = null;

				_poolHead = next;

				return reference;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public void Return(ExecutionRailNode executionRailNode)
			{
				executionRailNode.Next = _poolHead;

				_poolHead = executionRailNode;
			}
		}
	}
}