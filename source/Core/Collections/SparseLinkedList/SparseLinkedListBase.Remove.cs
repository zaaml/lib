// <copyright file="SparseLinkedListBase.Remove.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Diagnostics;

namespace Zaaml.Core.Collections
{
	internal partial class SparseLinkedListBase<T>
	{
		private protected void RemoveAtImpl(long index)
		{
			RemoveRangeAtImpl(index, 1);
		}

		private void RemoveNodeRange(ref NodeCursor cursor, long index, long count)
		{
			try
			{
				EnterStructureChange();

				// Remove whole node
				if (index == cursor.NodeOffset && count == cursor.Node.Size)
				{
					if (ReferenceEquals(cursor.Node, HeadNode) || ReferenceEquals(cursor.Node, TailNode))
					{
						cursor.Node.Size -= count;

						LongCount -= count;
					}
					else
					{
						RemoveNode(cursor.Node);

						LongCount -= count;
					}

					return;
				}

				var realizedNode = cursor.Node as RealizedNode;

				if (realizedNode == null)
				{
					cursor.Node.Size -= count;

					LongCount -= count;
				}
				else
				{
					var removeStartIndex = (int) (index - cursor.NodeOffset);

					if (removeStartIndex + count == realizedNode.Size)
					{
						realizedNode.Span.Slice(removeStartIndex, (int) count).Clear();
					}
					else
					{
						var moveIndex = (int) (removeStartIndex + count);
						var moveCount = (int) (realizedNode.Size - moveIndex);
						var clearIndex = removeStartIndex + moveCount;

						var sourceSpan = realizedNode.Span.Slice(moveIndex, moveCount);
						var targetSpan = realizedNode.Span.Slice(removeStartIndex, moveCount);

						sourceSpan.CopyTo(targetSpan);

						var clearSpan = realizedNode.Span.Slice(clearIndex, (int) (realizedNode.Size - clearIndex));

						clearSpan.Clear();
					}

					realizedNode.Size -= count;

					LongCount -= count;
				}
			}
			finally
			{
				LeaveStructureChange();
			}
		}

		private protected void RemoveRangeAtImpl(long index, long count)
		{
			if (count == 0)
				return;

			if (index == 0 && count == LongCount)
			{
				ClearImpl();

				return;
			}

			try
			{
				EnterStructureChange();

				ref var firstNodeCursor = ref NavigateTo(index);
				var firstNode = firstNodeCursor.Node;
				var endIndex = index + count - 1;

				// Single Node
				if (firstNodeCursor.Contains(endIndex))
				{
					RemoveNodeRange(ref firstNodeCursor, index, count);

					return;
				}

				var lastNodeCursor = firstNodeCursor.GetNext();
				var lastNode = lastNodeCursor.Node;

				// Contiguous nodes
				if (lastNodeCursor.IsValid && lastNodeCursor.Contains(endIndex))
				{
					var firstCount = firstNode.Size - (index - firstNodeCursor.NodeOffset);
					var lastCount = count - (lastNodeCursor.NodeOffset - index);

					RemoveNodeRange(ref lastNodeCursor, lastNodeCursor.NodeOffset, lastCount);
					RemoveNodeRange(ref firstNodeCursor, index, firstCount);

					return;
				}

				while (lastNodeCursor.IsValid && lastNodeCursor.Contains(endIndex) == false)
				{
					var next = lastNodeCursor.GetNext();

					ReleaseNode(lastNodeCursor.Node);

					lastNodeCursor = next;
					lastNode = lastNodeCursor.Node;
				}

				Debug.Assert(lastNode != null);

				var voidSize = lastNodeCursor.NodeOffset - firstNodeCursor.NodeOffset - firstNode.Size;
				var firstVoidNode = firstNode is VoidNode;
				var lastVoidNode = lastNode is VoidNode;

				if (firstVoidNode && lastVoidNode)
				{
					if (ReferenceEquals(firstNode, HeadNode))
					{
						if (ReferenceEquals(lastNode, TailNode))
						{
							LongCount -= count;

							InitHeadTail();

							return;
						}

						HeadNode.Next = lastNode.Next;
						lastNode.Next.Prev = HeadNode;
						HeadNode.Size += voidSize + lastNode.Size - count;

						ReleaseNode(lastNode);

						LongCount -= count;
					}
					else
					{
						lastNode.Size += voidSize + firstNode.Size - count;

						lastNode.Prev = firstNode.Prev;
						firstNode.Prev.Next = lastNode;

						ReleaseNode(firstNode);

						LongCount -= count;
					}

					return;
				}

				firstNode.Next = lastNode;
				lastNode.Prev = firstNode;

				{
					var firstCount = firstNode.Size - (index - firstNodeCursor.NodeOffset);
					var lastCount = count - (lastNodeCursor.NodeOffset - index);

					RemoveNodeRange(ref lastNodeCursor, lastNodeCursor.NodeOffset, lastCount);
					RemoveNodeRange(ref firstNodeCursor, index, firstCount);

					LongCount -= count - firstCount - lastCount;
				}
			}
			finally
			{
				if (ReferenceEquals(HeadNode.Next, TailNode) && TailNode.Size > 0)
				{
					HeadNode.Size += TailNode.Size;
					TailNode.Size = 0;
				}

				LeaveStructureChange();
			}
		}
	}
}