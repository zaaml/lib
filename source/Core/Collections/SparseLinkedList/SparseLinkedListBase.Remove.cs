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

		private NodeBase RemoveNodeRange(ref NodeCursor cursor, long index, long count, bool returnNext)
		{
			var cursorNode = cursor.Node;
			var cursorNodeOffset = cursor.NodeOffset;

			// Remove whole node
			if (index == cursorNodeOffset && count == cursorNode.Size)
			{
				if (ReferenceEquals(cursorNode, HeadNode) || ReferenceEquals(cursorNode, TailNode))
				{
					cursorNode.Size -= count;

					LongCount -= count;

					return returnNext ? TailNode : HeadNode;
				}

				var returnNode = returnNext ? cursorNode.Next : cursorNode.Prev;

				RemoveNode(cursorNode);

				LongCount -= count;

				return returnNode;
			}

			if (cursorNode is RealizedNode realizedNode)
			{
				var removeStartIndex = (int)(index - cursorNodeOffset);

				if (removeStartIndex + count == realizedNode.Size)
				{
					realizedNode.Span.Slice(removeStartIndex, (int)count).Clear();
				}
				else
				{
					var moveIndex = (int)(removeStartIndex + count);
					var moveCount = (int)(realizedNode.Size - moveIndex);
					var clearIndex = removeStartIndex + moveCount;

					var sourceSpan = realizedNode.Span.Slice(moveIndex, moveCount);
					var targetSpan = realizedNode.Span.Slice(removeStartIndex, moveCount);

					sourceSpan.CopyTo(targetSpan);

					var clearSpan = realizedNode.Span.Slice(clearIndex, (int)(realizedNode.Size - clearIndex));

					clearSpan.Clear();
				}
			}

			cursorNode.Size -= count;
			LongCount -= count;

			return cursorNode;
		}

		private void MergeVoidNodes(NodeBase first, NodeBase last)
		{
			while (first != null && ReferenceEquals(first, last) == false)
			{
				var next = first.Next;

				if (first is VoidNode && next is VoidNode)
				{
					if (ReferenceEquals(next, TailNode))
					{
						if (ReferenceEquals(first, HeadNode))
						{
							HeadNode.Size += TailNode.Size;
							TailNode.Size = 0;

							return;
						}

						TailNode.Size += first.Size;
						first.Size = 0;

						RemoveNode(first);

						return;
					}

					first.Size += next.Size;
					next.Size = 0;

					RemoveNode(next);

					continue;
				}

				first = next;
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
					var mergeNode = RemoveNodeRange(ref firstNodeCursor, index, count, false);

					MergeVoidNodes(mergeNode, mergeNode.Next);

					return;
				}

				var lastNodeCursor = firstNodeCursor.GetNext();
				var lastNode = lastNodeCursor.Node;

				// Contiguous nodes
				if (lastNodeCursor.IsValid && lastNodeCursor.Contains(endIndex))
				{
					var firstCount = firstNode.Size - (index - firstNodeCursor.NodeOffset);
					var lastCount = count - (lastNodeCursor.NodeOffset - index);

					lastNode = RemoveNodeRange(ref lastNodeCursor, lastNodeCursor.NodeOffset, lastCount, true);
					firstNode = RemoveNodeRange(ref firstNodeCursor, index, firstCount, false);

					MergeVoidNodes(firstNode, lastNode);

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
				
				var firstVoidNode = firstNode is VoidNode;
				var lastVoidNode = lastNode is VoidNode;

				if (firstVoidNode && lastVoidNode)
				{
					var voidSize = lastNodeCursor.NodeOffset - firstNodeCursor.NodeOffset - firstNode.Size;

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

					lastNode = RemoveNodeRange(ref lastNodeCursor, lastNodeCursor.NodeOffset, lastCount, true);
					firstNode = RemoveNodeRange(ref firstNodeCursor, index, firstCount, false);

					MergeVoidNodes(firstNode, lastNode);

					LongCount -= count - firstCount - lastCount;
				}
			}
			finally
			{
				LeaveStructureChange();
			}
		}
	}
}