﻿// <copyright file="SparseLinkedListBase.Clean.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Diagnostics;

namespace Zaaml.Core.Collections
{
	internal partial class SparseLinkedListBase<T>
	{
		private protected void CleanAtImpl(int index)
		{
			CleanRangeImpl(index, 1);
		}

		private void CleanNodeRange(ref NodeCursor cursor, long index, long count)
		{
			if (!(cursor.Node is RealizedNode realizedNode))
				return;

			var prevNode = realizedNode.Prev;
			var nextNode = realizedNode.Next;

			GapNode gapNode;

			// Remove whole node
			if (cursor.NodeOffset == index && count >= realizedNode.Size)
			{
				// Between Head and Tail
				var isPrevHead = ReferenceEquals(prevNode, HeadNode);
				var isNextTail = ReferenceEquals(nextNode, TailNode);

				if (isPrevHead && isNextTail)
				{
					InitHeadTail();

					ReleaseNode(realizedNode);

					return;
				}

				var isPrevGap = prevNode is GapNode;
				var isNextGap = nextNode is GapNode;

				if (isPrevGap && isNextGap)
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

				// Prev Node Gap. Expand
				if (isPrevGap)
				{
					prevNode.Size += realizedNode.Size;
					prevNode.Next = nextNode;
					nextNode.Prev = prevNode;

					ReleaseNode(realizedNode);

					return;
				}

				// Next Node Gap. Expand
				if (isNextGap)
				{
					nextNode.Size += realizedNode.Size;
					prevNode.Next = nextNode;
					nextNode.Prev = prevNode;

					ReleaseNode(realizedNode);

					return;
				}

				// Create Gap Node.
				gapNode = Manager.GetGapNode();

				gapNode.Prev = prevNode;
				gapNode.Next = nextNode;
				gapNode.Size = realizedNode.Size;

				prevNode.Next = gapNode;
				nextNode.Prev = gapNode;

				ReleaseNode(realizedNode);

				return;
			}

			// Clean part of Node
			var items = realizedNode.Span;
			var itemsStart = (int)(index - cursor.NodeOffset);
			var itemsEnd = itemsStart + count;
			var loopEnd = Math.Min(itemsEnd, realizedNode.Size);

			//Array.Clear(items, itemsStart, loopEnd - itemsStart);
			items.Slice(itemsStart, (int)(loopEnd - itemsStart)).Clear();

			if (itemsEnd < realizedNode.Size)
				return;

			gapNode = nextNode as GapNode;

			if (gapNode == null)
			{
				gapNode = Manager.GetGapNode();
				gapNode.Prev = realizedNode;
				gapNode.Next = nextNode;

				nextNode.Prev = gapNode;
				realizedNode.Next = gapNode;
			}

			gapNode.Size += realizedNode.Size - itemsStart;
			realizedNode.Size = itemsStart;
		}

		private protected void CleanRangeImpl(int index, int count)
		{
			if (count == 0)
				return;
			
			try
			{
				EnterStructureChange();

				var endIndex = index + count - 1;
				var firstNodeCursor = Cursor.NavigateTo(index);
				var firstNode = firstNodeCursor.Node;

				// Single Node
				if (firstNodeCursor.Contains(endIndex))
				{
					CleanNodeRange(ref firstNodeCursor, index, count);

					return;
				}

				var lastNodeCursor = firstNodeCursor.GetNext();
				var lastNode = lastNodeCursor.Node;

				// Contiguous nodes
				if (lastNodeCursor.IsValid && lastNodeCursor.Contains(endIndex))
				{
					// First is Gap. Expand to next
					if (firstNode is GapNode)
					{
						CleanNodeRange(ref lastNodeCursor, lastNodeCursor.NodeOffset, count - (lastNodeCursor.NodeOffset - index));
					}
					// Last is Gap. Expand to prev
					else if (lastNode is GapNode)
					{
						CleanNodeRange(ref firstNodeCursor, index, firstNode.Size - (index - firstNodeCursor.NodeOffset));
					}
					// Contiguous realized nodes. Make gap between
					else
					{
						var firstCount = firstNode.Size - (index - firstNodeCursor.NodeOffset);
						var lastCount = count - (lastNodeCursor.NodeOffset - index);

						CleanNodeRange(ref lastNodeCursor, lastNodeCursor.NodeOffset, lastCount);
						CleanNodeRange(ref firstNodeCursor, index, firstCount);
					}

					return;
				}

				GapNode gapNode = null;

				while (lastNodeCursor.IsValid && lastNodeCursor.Contains(endIndex) == false)
				{
					var nextCursor = lastNodeCursor.GetNext();

					ReleaseNode(lastNodeCursor.Node);

					gapNode ??= lastNodeCursor.Node as GapNode;

					lastNodeCursor = nextCursor;
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
							InitHeadTail();

							return;
						}

						HeadNode.Next = lastNode.Next;
						lastNode.Next.Prev = HeadNode;
						HeadNode.Size += gapSize + lastNode.Size;

						ReleaseNode(lastNode);
					}
					else
					{
						lastNode.Size += gapSize + firstNode.Size;

						lastNode.Prev = firstNode.Prev;
						firstNode.Prev.Next = lastNode;

						ReleaseNode(firstNode);
					}
				}
				else if (firstGap)
				{
					firstNode.Next = lastNode;
					lastNode.Prev = firstNode;

					firstNode.Size += gapSize;

					CleanNodeRange(ref lastNodeCursor, lastNodeCursor.NodeOffset, count - (lastNodeCursor.NodeOffset - index));
				}
				else if (lastGap)
				{
					firstNode.Next = lastNode;
					lastNode.Prev = firstNode;

					lastNode.Size += gapSize;

					CleanNodeRange(ref firstNodeCursor, index, firstNode.Size - (index - firstNodeCursor.NodeOffset));
				}
				else
				{
					gapNode ??= Manager.GetGapNode();

					gapNode.Size = gapSize;

					firstNode.Next = gapNode;
					lastNode.Prev = gapNode;
					gapNode.Prev = firstNode;
					gapNode.Next = lastNode;

					CleanNodeRange(ref firstNodeCursor, index, firstNode.Size - (index - firstNodeCursor.NodeOffset));
					CleanNodeRange(ref lastNodeCursor, lastNodeCursor.NodeOffset, count - (lastNodeCursor.NodeOffset - index));
				}
			}
			finally
			{
				LeaveStructureChange();
			}
		}
	}
}