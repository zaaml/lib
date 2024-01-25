// <copyright file="UnexpectedNode.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		private protected sealed class DataNode<TData> : Node
		{
			public DataNode(TData data) : base(ThreadStatusKind.Unexpected)
			{
				Data = data;
			}

			public TData Data { get; }

			protected override string KindString => "Data";
		}
	}
}