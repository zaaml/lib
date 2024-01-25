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
			var voidNode = (VoidNode) cursor.Node;
			var prevNode = voidNode.Prev;
			var nextNode = voidNode.Next;
			
			Debug.Assert(index < LongCount);

			if (prevCursor.IsEmpty == false && prevCursor.Node is RealizedNode prevRealizedNode && index < prevCursor.NodeOffset + NodeCapacity)
			{
				var extraSize = index - (prevCursor.NodeOffset + prevRealizedNode.Size) + 1;

				prevRealizedNode.Size += extraSize;

				if (insert == false)
					voidNode.Size -= extraSize;

				RemoveEmptyVoidNode(voidNode);

				prevCursor.NavigateTo(index);
				cursor = prevCursor;

				return prevRealizedNode;
			}

			try
			{
				EnterStructureChange();

				var realizedNode = GetRealizedNode();
				var alignedIndex = index / NodeCapacity * NodeCapacity;

				if (ReferenceEquals(HeadNode.Next, TailNode) == false && cursor.Contains(alignedIndex) == false)
				{
					Debug.Assert(prevNode is RealizedNode);

					alignedIndex = cursor.NodeOffset;
				}

				realizedNode.Size = index - alignedIndex + 1;

				if (ReferenceEquals(HeadNode.Next, TailNode))
				{
					Debug.Assert(TailNode.Size == 0);

					cursor = ref GetHeadCursor();

					HeadNode.Next = realizedNode;
					TailNode.Prev = realizedNode;

					realizedNode.Prev = HeadNode;
					realizedNode.Next = TailNode;

					var extraCount = HeadNode.Size - alignedIndex;

					HeadNode.Size = alignedIndex;

					if (index < LongCount)
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

					if (realizedNode.Size == voidNode.Size && ReferenceEquals(voidNode, TailNode) == false)
					{
						realizedNode.Next = nextNode;
						nextNode.Prev = realizedNode;

						ReleaseNode(voidNode);
					}
					else
					{
						realizedNode.Next = voidNode;
						voidNode.Prev = realizedNode;

						if (insert == false)
							voidNode.Size -= realizedNode.Size;
					}

					cursor = prevCursor;
				}
				else if (ReferenceEquals(voidNode, HeadNode))
				{
					var nextVoidSize = HeadNode.Size - realizedNode.Size - alignedIndex;

					if (nextVoidSize == 0)
					{
						realizedNode.Next = HeadNode.Next;
						HeadNode.Next.Prev = realizedNode;

						realizedNode.Prev = HeadNode;
					}
					else
					{
						var nextVoidNode = GetVoidNode();

						nextVoidNode.Size = nextVoidSize;

						realizedNode.Next = nextVoidNode;
						nextVoidNode.Prev = realizedNode;

						nextVoidNode.Next = nextNode;
						nextNode.Prev = nextVoidNode;

						realizedNode.Prev = HeadNode;
					}

					HeadNode.Next = realizedNode;
					HeadNode.Size = alignedIndex;

					cursor = ref GetHeadCursor();
				}
				else
				{
					// ReSharper disable once PossibleNullReferenceException
					var prevVoidSize = alignedIndex - cursor.NodeOffset;
					var prevVoidNode = GetVoidNode();

					prevVoidNode.Size = prevVoidSize;
					prevVoidNode.Prev = prevNode;
					prevVoidNode.Next = realizedNode;

					prevNode.Next = prevVoidNode;
					realizedNode.Prev = prevVoidNode;
					realizedNode.Next = voidNode;
					voidNode.Prev = realizedNode;

					if (insert == false)
						voidNode.Size -= prevVoidNode.Size + realizedNode.Size;

					cursor = prevCursor.GetPrev();
				}

				RemoveEmptyVoidNode(voidNode);

				return realizedNode;
			}
			finally
			{
				LeaveStructureChange();

				cursor = ref GetCursor();
				cursor.NavigateTo(index);
			}
		}
	}
}