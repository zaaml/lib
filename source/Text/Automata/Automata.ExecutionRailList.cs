// <copyright file="Automata.Pool.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Runtime.CompilerServices;

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		private protected readonly struct ExecutionRailList
		{
			public static readonly ExecutionRailList Empty = new(0, null);
			public static readonly ExecutionRailList Start = new(1, ExecutionRailNode.Empty);
			public static readonly ExecutionRailList Next = new(-1, ExecutionRailNode.Empty);

			public ExecutionRailList(int count, ExecutionRailNode root)
			{
				Count = count;
				Root = root;
			}

			public readonly int Count;
			public readonly ExecutionRailNode Root;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public ExecutionRailList MoveNext(out ExecutionRailNode railNode)
			{
				railNode = Root;

				return new ExecutionRailList(Count - 1, Root.Next);
			}

			public void Dispose()
			{
				var self = this;

				while (self.Count > 0)
				{
					self = self.MoveNext(out var node);

					node.Dispose();
				}
			}
		}
	}
}