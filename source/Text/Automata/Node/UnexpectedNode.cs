// <copyright file="UnexpectedNode.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		private protected sealed class UnexpectedNode : Node
		{
			public static readonly UnexpectedNode Instance = new();

			private UnexpectedNode() : base(ThreadStatusKind.Unexpected)
			{
			}

			protected override string KindString => "Unexpected";
		}
	}
}