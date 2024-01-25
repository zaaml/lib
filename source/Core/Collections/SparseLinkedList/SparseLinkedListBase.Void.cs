// <copyright file="SparseLinkedListBase.Clean.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Diagnostics;

namespace Zaaml.Core.Collections
{
	internal partial class SparseLinkedListBase<T>
	{
		private protected void VoidAtImpl(long index)
		{
			VoidRangeImpl(index, 1);
		}

		private void VoidNodeRange(ref NodeCursor cursor, long index, long count)
		{
			if (cursor.Node is not RealizedNode realizedNode)
				return;

			var prevNode = realizedNode.Prev;
			var nextNode = realizedNode.Next;

			VoidNode voidNode;

			// Remove whole node
			if (cursor.NodeOffset == index && count >= realizedNode.Size)
			{
				// Between Head and Tail
				var isPrevHead = ReferenceEquals(prevNode, HeadNode);
				var isNextTail = ReferenceEquals(nextNode, TailNode);

				if (isPrevHead && isNextTail)
				{
					ReleaseNode(realizedNode);

					InitHeadTail();

					return;
				}

				var isPrevVoid = prevNode is VoidNode;
				var isNextVoid = nextNode is VoidNode;

				if (isPrevVoid && isNextVoid)
				{
					if (isPrevHead)
					{
						HeadNode.Size += realizedNode.Size + nextNode.Size;

						RemoveNode(nextNode);
						RemoveNode(realizedNode);

						return;
					}

					if (isNextTail)
					{
						TailNode.Size += realizedNode.Size + prevNode.Size;

						RemoveNode(prevNode);
						RemoveNode(realizedNode);

						return;
					}

					prevNode.Size += realizedNode.Size + nextNode.Size;

					RemoveNode(nextNode);
					RemoveNode(realizedNode);

					return;
				}

				// Prev Node Void. Expand
				if (isPrevVoid)
				{
					prevNode.Size += realizedNode.Size;
					prevNode.Next = nextNode;
					nextNode.Prev = prevNode;

					ReleaseNode(realizedNode);

					return;
				}

				// Next Node Void. Expand
				if (isNextVoid)
				{
					nextNode.Size += realizedNode.Size;
					prevNode.Next = nextNode;
					nextNode.Prev = prevNode;

					ReleaseNode(realizedNode);

					return;
				}

				// Create Void Node.
				voidNode = GetVoidNode();

				voidNode.Prev = prevNode;
				voidNode.Next = nextNode;
				voidNode.Size = realizedNode.Size;

				prevNode.Next = voidNode;
				nextNode.Prev = voidNode;

				ReleaseNode(realizedNode);

				return;
			}

			// Clean part of Node
			var items = realizedNode.Span;
			var itemsStart = (int) (index - cursor.NodeOffset);
			var itemsEnd = itemsStart + count;
			var loopEnd = Math.Min(itemsEnd, realizedNode.Size);

			items.Slice(itemsStart, (int) (loopEnd - itemsStart)).Clear();

			if (itemsEnd < realizedNode.Size)
				return;

			voidNode = nextNode as VoidNode;

			if (voidNode == null)
			{
				voidNode = GetVoidNode();
				voidNode.Prev = realizedNode;
				voidNode.Next = nextNode;

				nextNode.Prev = voidNode;
				realizedNode.Next = voidNode;
			}

			voidNode.Size += realizedNode.Size - itemsStart;
			realizedNode.Size = itemsStart;
		}

		private protected void VoidRangeImpl(long index, long count)
		{
			if (count == 0)
				return;

			try
			{
				EnterStructureChange();

				ref var firstNodeCursor = ref NavigateTo(index);
				var firstNode = firstNodeCursor.Node;
				var endIndex = index + count - 1;

				// Single Node
				if (firstNodeCursor.Contains(endIndex))
				{
					VoidNodeRange(ref firstNodeCursor, index, count);

					return;
				}

				var lastNodeCursor = firstNodeCursor.GetNext();
				var lastNode = lastNodeCursor.Node;

				// Contiguous nodes
				if (lastNodeCursor.IsValid && lastNodeCursor.Contains(endIndex))
				{
					// First is Void. Expand to next
					if (firstNode is VoidNode)
					{
						VoidNodeRange(ref lastNodeCursor, lastNodeCursor.NodeOffset, count - (lastNodeCursor.NodeOffset - index));
					}
					// Last is Void. Expand to prev
					else if (lastNode is VoidNode)
					{
						VoidNodeRange(ref firstNodeCursor, index, firstNode.Size - (index - firstNodeCursor.NodeOffset));
					}
					// Contiguous realized nodes. Make void between
					else
					{
						var firstCount = firstNode.Size - (index - firstNodeCursor.NodeOffset);
						var lastCount = count - (lastNodeCursor.NodeOffset - index);

						VoidNodeRange(ref lastNodeCursor, lastNodeCursor.NodeOffset, lastCount);
						VoidNodeRange(ref firstNodeCursor, index, firstCount);
					}

					return;
				}

				VoidNode voidNode = null;

				while (lastNodeCursor.IsValid && lastNodeCursor.Contains(endIndex) == false)
				{
					var nextCursor = lastNodeCursor.GetNext();

					ReleaseNode(lastNodeCursor.Node);

					voidNode ??= lastNodeCursor.Node as VoidNode;

					lastNodeCursor = nextCursor;
					lastNode = lastNodeCursor.Node;
				}

				Debug.Assert(lastNode != null);

				var voidSize = lastNodeCursor.NodeOffset - firstNodeCursor.NodeOffset - firstNode.Size;
				var firstVoid = firstNode is VoidNode;
				var lastVoid = lastNode is VoidNode;

				if (firstVoid && lastVoid)
				{
					if (ReferenceEquals(firstNode, HeadNode))
					{
						if (ReferenceEquals(lastNode, TailNode))
						{
							InitHeadTail();

							return;
						}

						HeadNode.Next = lastNode.Next;
						lastNode.Next.Prev = HeadNode;
						HeadNode.Size += voidSize + lastNode.Size;

						ReleaseNode(lastNode);
					}
					else
					{
						lastNode.Size += voidSize + firstNode.Size;

						lastNode.Prev = firstNode.Prev;
						firstNode.Prev.Next = lastNode;

						ReleaseNode(firstNode);
					}
				}
				else if (firstVoid)
				{
					firstNode.Next = lastNode;
					lastNode.Prev = firstNode;

					firstNode.Size += voidSize;

					VoidNodeRange(ref lastNodeCursor, lastNodeCursor.NodeOffset, count - (lastNodeCursor.NodeOffset - index));
				}
				else if (lastVoid)
				{
					firstNode.Next = lastNode;
					lastNode.Prev = firstNode;

					lastNode.Size += voidSize;

					VoidNodeRange(ref firstNodeCursor, index, firstNode.Size - (index - firstNodeCursor.NodeOffset));
				}
				else
				{
					voidNode ??= GetVoidNode();

					voidNode.Size = voidSize;

					firstNode.Next = voidNode;
					lastNode.Prev = voidNode;
					voidNode.Prev = firstNode;
					voidNode.Next = lastNode;

					VoidNodeRange(ref firstNodeCursor, index, firstNode.Size - (index - firstNodeCursor.NodeOffset));
					VoidNodeRange(ref lastNodeCursor, lastNodeCursor.NodeOffset, count - (lastNodeCursor.NodeOffset - index));
				}
			}
			finally
			{
				LeaveStructureChange();
			}
		}
	}
}