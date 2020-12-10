// <copyright file="SparseLinkedListBase.Clean.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
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

		private void CleanNodeRange(Node node, int index, int count)
		{
			if (!(node is RealizedNode realizedNode))
				return;

			var prevNode = realizedNode.Prev;
			var nextNode = realizedNode.Next;

			Current = nextNode;

			GapNode gapNode;

			// Remove whole node
			if (realizedNode.Index == index && count >= realizedNode.Count)
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
						HeadNode.Count += realizedNode.Count + nextNode.Count;

						RemoveNode(nextNode);
						RemoveNode(realizedNode);

						Current = HeadNode;

						return;
					}

					if (isNextTail)
					{
						TailNode.Count += realizedNode.Count + prevNode.Count;
						TailNode.Index -= realizedNode.Count + prevNode.Count;

						RemoveNode(prevNode);
						RemoveNode(realizedNode);

						Current = TailNode;

						return;
					}

					prevNode.Count += realizedNode.Count + nextNode.Count;

					RemoveNode(nextNode);
					RemoveNode(realizedNode);

					Current = prevNode;

					return;
				}

				// Prev Node Gap. Expand
				if (isPrevGap)
				{
					prevNode.Count += realizedNode.Count;
					prevNode.Next = nextNode;
					nextNode.Prev = prevNode;

					ReleaseNode(realizedNode);

					return;
				}

				// Next Node Gap. Expand
				if (isNextGap)
				{
					nextNode.Count += realizedNode.Count;
					nextNode.Index -= realizedNode.Count;
					prevNode.Next = nextNode;
					nextNode.Prev = prevNode;

					ReleaseNode(realizedNode);

					return;
				}

				// Create Gap Node.
				gapNode = CreateGapNode();

				gapNode.Prev = prevNode;
				gapNode.Next = nextNode;
				gapNode.Count = realizedNode.Count;
				gapNode.Index = realizedNode.Index;

				prevNode.Next = gapNode;
				nextNode.Prev = gapNode;

				ReleaseNode(realizedNode);

				return;
			}

			// Clean part of Node
			var items = realizedNode.Span;
			var itemsStart = index - realizedNode.Index;
			var itemsEnd = itemsStart + count;
			var loopEnd = Math.Min(itemsEnd, realizedNode.Count);

			//Array.Clear(items, itemsStart, loopEnd - itemsStart);
			items.Slice(itemsStart, loopEnd - itemsStart).Clear();

			if (itemsEnd < realizedNode.Count)
				return;

			gapNode = nextNode as GapNode;

			if (gapNode == null)
			{
				gapNode = CreateGapNode();
				gapNode.Prev = realizedNode;
				gapNode.Next = nextNode;

				nextNode.Prev = gapNode;
				realizedNode.Next = gapNode;
			}

			gapNode.Index = index;
			gapNode.Count += realizedNode.Count - itemsStart;
			realizedNode.Count = itemsStart;
		}

		private protected void CleanRangeImpl(int index, int count)
		{
			if (count == 0)
				return;

			var endIndex = index + count - 1;
			var firstNode = FindNodeImpl(index);

			// Single Node
			if (firstNode.Contains(endIndex))
			{
				CleanNodeRange(firstNode, index, count);

				return;
			}

			var lastNode = firstNode.Next;

			// Contiguous nodes
			if (lastNode != null && lastNode.Contains(endIndex))
			{
				// First is Gap. Expand to next
				if (firstNode is GapNode)
				{
					CleanNodeRange(lastNode, lastNode.Index, count - (lastNode.Index - index));
				}
				// Last is Gap. Expand to prev
				else if (lastNode is GapNode)
				{
					CleanNodeRange(firstNode, index, firstNode.Count - (index - firstNode.Index));
				}
				// Contiguous realized nodes. Make gap between
				else
				{
					var firstCount = firstNode.Count - (index - firstNode.Index);
					var lastCount = count - (lastNode.Index - index);

					CleanNodeRange(lastNode, lastNode.Index, lastCount);
					CleanNodeRange(firstNode, index, firstCount);
				}

				return;
			}

			GapNode gapNode = null;

			while (lastNode != null && lastNode.Contains(endIndex) == false)
			{
				var next = lastNode.Next;

				ReleaseNode(lastNode);

				gapNode ??= lastNode as GapNode;

				lastNode = next;
			}

			Debug.Assert(lastNode != null);

			var gapSize = lastNode.Index - firstNode.Index - firstNode.Count;
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
					HeadNode.Count += gapSize + lastNode.Count;

					ReleaseNode(lastNode);

					Current = HeadNode;
				}
				else
				{
					lastNode.Index -= gapSize + firstNode.Count;
					lastNode.Count += gapSize + firstNode.Count;

					lastNode.Prev = firstNode.Prev;
					firstNode.Prev.Next = lastNode;

					ReleaseNode(firstNode);

					Current = lastNode;
				}
			}
			else if (firstGap)
			{
				firstNode.Next = lastNode;
				lastNode.Prev = firstNode;

				firstNode.Count += gapSize;

				CleanNodeRange(lastNode, lastNode.Index, count - (lastNode.Index - index));
			}
			else if (lastGap)
			{
				firstNode.Next = lastNode;
				lastNode.Prev = firstNode;

				lastNode.Index -= gapSize;
				lastNode.Count += gapSize;

				CleanNodeRange(firstNode, index, firstNode.Count - (index - firstNode.Index));
			}
			else
			{
				gapNode ??= CreateGapNode();

				gapNode.Index = firstNode.Index + firstNode.Count;
				gapNode.Count = gapSize;

				firstNode.Next = gapNode;
				lastNode.Prev = gapNode;
				gapNode.Prev = firstNode;
				gapNode.Next = lastNode;

				CleanNodeRange(firstNode, index, firstNode.Count - (index - firstNode.Index));
				CleanNodeRange(lastNode, lastNode.Index, count - (lastNode.Index - index));
			}
		}
	}
}