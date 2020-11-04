// <copyright file="SparseLinkedList.Remove.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Diagnostics;

namespace Zaaml.Core.Collections
{
	internal partial class SparseLinkedList<T>
	{
		#region  Methods

		private void RemoveAtImpl(int index)
		{
			RemoveRangeAtImpl(index, 1);
		}

		private void RemoveNodeRange(Node node, int index, int count, bool advanceIndices)
		{
			// Remove whole node
			if (index == node.Index && count == node.Count)
			{
				if (ReferenceEquals(node, HeadNode) || ReferenceEquals(node, TailNode))
				{
					node.Count -= count;

					AdvanceIndices(node);

					Count -= count;
					Current = node;
				}
				else
				{
					var prevNode = node.Prev;

					RemoveNode(node);

					if (advanceIndices)
						AdvanceIndices(prevNode);

					Count -= count;
					Current = prevNode;
				}

				return;
			}

			var realizedNode = node as RealizedNode;

			if (realizedNode == null)
			{
				node.Count -= count;

				if (advanceIndices)
					AdvanceIndices(node);

				Count -= count;
				Current = node;
			}
			else
			{
				var removeStartIndex = index - realizedNode.Index;

				if (removeStartIndex + count == realizedNode.Count)
					Array.Clear(realizedNode.Items, removeStartIndex, count);
				else
				{
					var moveIndex = removeStartIndex + count;
					var moveCount = realizedNode.Count - moveIndex;
					var clearIndex = removeStartIndex + moveCount;

					Array.Copy(realizedNode.Items, moveIndex, realizedNode.Items, removeStartIndex, moveCount);
					Array.Clear(realizedNode.Items, clearIndex, realizedNode.Count - clearIndex);
				}

				realizedNode.Count -= count;

				if (advanceIndices)
					AdvanceIndices(realizedNode);

				Count -= count;
				Current = realizedNode;
			}
		}

		private void RemoveRangeAtImpl(int index, int count)
		{
			if (count == 0)
				return;

			var endIndex = index + count - 1;
			var firstNode = FindNodeImpl(index);

			// Single Node
			if (firstNode.Contains(endIndex))
			{
				RemoveNodeRange(firstNode, index, count, true);

				return;
			}

			var lastNode = firstNode.Next;

			// Contiguous nodes
			if (lastNode != null && lastNode.Contains(endIndex))
			{
				var firstCount = firstNode.Count - (index - firstNode.Index);
				var lastCount = count - (lastNode.Index - index);

				RemoveNodeRange(lastNode, lastNode.Index, lastCount, false);
				RemoveNodeRange(firstNode, index, firstCount, true);

				return;
			}

			while (lastNode != null && lastNode.Contains(endIndex) == false)
			{
				var next = lastNode.Next;

				ReleaseNode(lastNode);

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
						Count -= count;

						InitHeadTail();

						return;
					}

					HeadNode.Next = lastNode.Next;
					lastNode.Next.Prev = HeadNode;
					HeadNode.Count += gapSize + lastNode.Count - count;

					ReleaseNode(lastNode);

					AdvanceIndices(HeadNode);

					Count -= count;

					Current = HeadNode;
				}
				else
				{
					lastNode.Index = firstNode.Index;
					lastNode.Count += gapSize + firstNode.Count - count;

					lastNode.Prev = firstNode.Prev;
					firstNode.Prev.Next = lastNode;

					ReleaseNode(firstNode);

					AdvanceIndices(lastNode);

					Count -= count;

					Current = lastNode;
				}

				return;
			}

			firstNode.Next = lastNode;
			lastNode.Prev = firstNode;

			{
				var firstCount = firstNode.Count - (index - firstNode.Index);
				var lastCount = count - (lastNode.Index - index);

				RemoveNodeRange(lastNode, lastNode.Index, lastCount, false);
				RemoveNodeRange(firstNode, index, firstCount, true);

				Count -= count - firstCount - lastCount;
			}
		}

		#endregion
	}
}