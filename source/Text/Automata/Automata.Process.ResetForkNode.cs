// <copyright file="Automata.Process.ResetForkNode.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Runtime.CompilerServices;

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		partial class Process
		{
			private sealed class ResetForkNode
			{
				public static readonly ResetForkNode Empty = new(null);
				private readonly ResetForkNodePool _pool;

				public ResetForkNode(ResetForkNodePool pool)
				{
					_pool = pool;
				}

				public ResetForkNode FrameHead { get; set; }

				public ResetForkNode Next { get; set; }

				public int ThreadHead { get; set; }

				public void Dispose()
				{
					if (FrameHead != null)
					{
						var current = FrameHead;

						while (current != null)
						{
							var next = current.Next;

							if (ReferenceEquals(this, next))
							{
								current.Next = next.Next;
								
								break;
							}

							current = next;
						}

						FrameHead = null;
					}

					Next = null;

					_pool?.Release(this);
				}
			}

			private sealed class ResetForkNodePool
			{
				private ResetForkNode _poolHead;

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public void Release(ResetForkNode node)
				{
					node.Next = _poolHead;

					_poolHead = node;
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public ResetForkNode Rent()
				{
					if (_poolHead == null)
						return new ResetForkNode(this);

					var reference = _poolHead;
					var next = reference.Next;

					reference.Next = null;

					_poolHead = next;

					return reference;
				}
			}
		}
	}
}