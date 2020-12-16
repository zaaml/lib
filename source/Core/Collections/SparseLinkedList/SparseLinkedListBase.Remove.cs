// <copyright file="SparseLinkedListBase.Remove.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Diagnostics;

namespace Zaaml.Core.Collections
{
	internal partial class SparseLinkedListBase<T>
	{
		private protected void RemoveAtImpl(int index)
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
					var removeStartIndex = (int)(index - cursor.NodeOffset);

					if (removeStartIndex + count == realizedNode.Size)
					{
						//Array.Clear(realizedNode.ItemsPrivate, removeStartIndex, count);

						realizedNode.Span.Slice(removeStartIndex, (int)count).Clear();
					}
					else
					{
						var moveIndex = (int)(removeStartIndex + count);
						var moveCount = (int)(realizedNode.Size - moveIndex);
						var clearIndex = (int)(removeStartIndex + moveCount);

						//Array.Copy(realizedNode.ItemsPrivate, moveIndex, realizedNode.ItemsPrivate, removeStartIndex, moveCount);

						var sourceSpan = realizedNode.Span.Slice(moveIndex, moveCount);
						var targetSpan = realizedNode.Span.Slice(removeStartIndex, moveCount);

						sourceSpan.CopyTo(targetSpan);

						//Array.Clear(realizedNode.ItemsPrivate, clearIndex, realizedNode.Count - clearIndex);

						var clearSpan = realizedNode.Span.Slice(clearIndex, (int)(realizedNode.Size - clearIndex));

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

		private protected void RemoveRangeAtImpl(int index, int count)
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

				var endIndex = index + count - 1;
				var firstNode = FindNodeImpl(index);
				var firstNodeCursor = Cursor.NavigateTo(index);

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

				var gapSize = lastNodeCursor.NodeOffset - firstNodeCursor.NodeOffset - firstNode.Size;
				var firstGap = firstNode is GapNode;
				var lastGap = lastNode is GapNode;

				if (firstGap && lastGap)
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
						HeadNode.Size += gapSize + lastNode.Size - count;

						ReleaseNode(lastNode);

						LongCount -= count;
					}
					else
					{
						lastNode.Size += gapSize + firstNode.Size - count;

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