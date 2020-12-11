// <copyright file="SparseLinkedListBase.Realize.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Diagnostics;

namespace Zaaml.Core.Collections
{
	internal partial class SparseLinkedListBase<T>
	{
		private RealizedNode EnsureRealizedNode(ref NodeCursor cursor, long index, bool insert)
		{
			if (cursor.Node is RealizedNode realizedNode)
				return realizedNode;

			realizedNode = RealizeNode(ref cursor, index, insert);

			return realizedNode;
		}

		private RealizedNode RealizeNode(ref NodeCursor cursor, long index, bool insert)
		{
			var prevCursor = cursor.GetPrev();
			var gapNode = (GapNode)cursor.Node;

			if (prevCursor.Node is RealizedNode prevRealizedNode && index < prevCursor.NodeOffset + NodeCapacity)
			{
				var extraSize = index - (prevCursor.NodeOffset + prevRealizedNode.Size) + 1;

				prevRealizedNode.Size += extraSize;

				if (insert == false)
					gapNode.Size -= extraSize;

				RemoveEmptyGapNode(gapNode);

				cursor = prevCursor.NavigateTo(index);

				return prevRealizedNode;
			}

			try
			{
				EnterStructureChange();

				var alignedIndex = index / NodeCapacity * NodeCapacity;
				var prevNode = gapNode.Prev;
				var nextNode = gapNode.Next;
				var realizedNode = CreateRealizedNode();

				if (ReferenceEquals(HeadNode.Next, TailNode) == false && cursor.Contains(alignedIndex) == false)
				{
					Debug.Assert(prevNode is RealizedNode);

					alignedIndex = cursor.NodeOffset;
				}

				realizedNode.Size = index - alignedIndex + 1;

				if (ReferenceEquals(HeadNode.Next, TailNode))
				{
					Debug.Assert(TailNode.Size == 0);

					cursor = HeadCursor;

					HeadNode.Next = realizedNode;
					TailNode.Prev = realizedNode;

					realizedNode.Prev = HeadNode;
					realizedNode.Next = TailNode;

					var extraCount = HeadNode.Size - alignedIndex;

					HeadNode.Size = alignedIndex;

					if (index < Count)
					{
						if (insert == false)
							extraCount -= realizedNode.Size;

						TailNode.Size += extraCount;
					}
					else
						TailNode.Size = 0;

					return realizedNode;
				}

				if (prevNode != null && cursor.NodeOffset == alignedIndex)
				{
					prevNode.Next = realizedNode;
					realizedNode.Prev = prevNode;

					if (realizedNode.Size == gapNode.Size && ReferenceEquals(gapNode, TailNode) == false)
					{
						realizedNode.Next = nextNode;
						nextNode.Prev = realizedNode;

						ReleaseNode(gapNode);
					}
					else
					{
						realizedNode.Next = gapNode;
						gapNode.Prev = realizedNode;

						if (insert == false)
							gapNode.Size -= realizedNode.Size;
					}

					cursor = prevCursor;
				}
				else if (ReferenceEquals(gapNode, HeadNode))
				{
					var nextGapCount = HeadNode.Size - realizedNode.Size - alignedIndex;

					if (nextGapCount == 0)
					{
						realizedNode.Next = HeadNode.Next;
						HeadNode.Next.Prev = realizedNode;

						realizedNode.Prev = HeadNode;
					}
					else
					{
						var nextGapNode = CreateGapNode();

						nextGapNode.Size = nextGapCount;

						realizedNode.Next = nextGapNode;
						nextGapNode.Prev = realizedNode;

						nextGapNode.Next = nextNode;
						nextNode.Prev = nextGapNode;

						realizedNode.Prev = HeadNode;
					}

					HeadNode.Next = realizedNode;
					HeadNode.Size = alignedIndex;

					cursor = HeadCursor;
				}
				else
				{
					// ReSharper disable once PossibleNullReferenceException
					var prevGapCount = alignedIndex - cursor.NodeOffset;
					var prevGapNode = CreateGapNode();

					prevGapNode.Size = prevGapCount;
					prevGapNode.Prev = prevNode;
					prevGapNode.Next = realizedNode;

					prevNode.Next = prevGapNode;
					realizedNode.Prev = prevGapNode;
					realizedNode.Next = gapNode;
					gapNode.Prev = realizedNode;

					if (insert == false)
						gapNode.Size -= prevGapNode.Size + realizedNode.Size;

					cursor = prevCursor.GetPrev();
				}

				RemoveEmptyGapNode(gapNode);

				return realizedNode;
			}
			finally
			{
				LeaveStructureChange();

				cursor = Cursor.NavigateTo(index);
			}
		}

		private void ReleaseNode(NodeBase node)
		{
			Manager.ReleaseNode(node);
		}
	}
}