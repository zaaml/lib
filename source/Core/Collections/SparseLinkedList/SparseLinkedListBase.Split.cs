// <copyright file="SparseLinkedListBase.Split.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Diagnostics;

namespace Zaaml.Core.Collections
{
	internal partial class SparseLinkedListBase<T>
	{
		private protected void SplitAtImpl(long index, SparseLinkedListBase<T> targetList)
		{
			Debug.Assert(targetList.Count == 0);

			if (ReferenceEquals(Manager, targetList.Manager) == false)
				throw new InvalidOperationException("Manager of source list and target list must be the same");

			if (index == LongCount)
				return;

			try
			{
				EnterStructureChange();
				targetList.EnterStructureChange();

				if (index == 0)
				{
					SwapImpl(targetList);

					return;
				}

				var cursor = Cursor.NavigateTo(index);
				var nodeSplitIndex = (int) (index - cursor.NodeOffset);
				var nodeSplitCount = (int) (cursor.Node.Size - nodeSplitIndex);
				var targetListTail = targetList.TailNode;

				if (cursor.Node is GapNode gapNode)
				{
					if (ReferenceEquals(HeadNode, gapNode))
					{
						// Target
						targetList.HeadNode.Size = nodeSplitCount;
						targetList.HeadNode.Next = gapNode.Next;
						gapNode.Next.Prev = targetList.HeadNode;
						targetList.TailNode = TailNode;

						// Source
						TailNode = targetListTail;
						TailNode.Prev = HeadNode;
						HeadNode.Next = TailNode;

						HeadNode.Size = index;
						TailNode.Size = 0;
					}
					else if (ReferenceEquals(TailNode, gapNode))
					{
						TailNode.Size -= nodeSplitCount;
						targetList.HeadNode.Size += nodeSplitCount;
					}
					else
					{
						var prevNode = gapNode.Prev;

						// Target
						targetList.HeadNode.Size = nodeSplitCount;
						targetList.HeadNode.Next = gapNode.Next;
						gapNode.Next.Prev = targetList.HeadNode;
						targetList.TailNode = TailNode;

						// Source
						TailNode = targetListTail;
						TailNode.Prev = prevNode;

						prevNode.Next = TailNode;
						TailNode.Size = nodeSplitIndex;

						ReleaseNode(gapNode);
					}
				}
				else if (nodeSplitIndex == 0)
				{
					var node = cursor.Node;
					var prev = node.Prev;

					// Target
					targetList.HeadNode.Next = node;
					node.Prev = targetList.HeadNode;
					targetList.TailNode = TailNode;

					TailNode = targetListTail;
					prev.Next = targetListTail;
					targetListTail.Prev = prev;
				}
				else
				{
					var sourceRealizedNode = (RealizedNode) cursor.Node;
					var targetRealizedNode = CreateRealizedNode();

					var sourceSpan = sourceRealizedNode.Span.Slice(nodeSplitIndex, nodeSplitCount);
					var targetSpan = targetRealizedNode.Span.Slice(0, nodeSplitCount);

					sourceSpan.CopyTo(targetSpan);

					// Target
					targetRealizedNode.Size = nodeSplitCount;
					targetList.HeadNode.Next = targetRealizedNode;
					targetRealizedNode.Prev = targetList.HeadNode;
					targetList.TailNode = TailNode;

					// Source
					sourceRealizedNode.Size = nodeSplitIndex;
					TailNode = targetListTail;
					TailNode.Prev = sourceRealizedNode;
					sourceRealizedNode.Next = TailNode;
				}

				targetList.LongCount = LongCount - index;

				LongCount = index;
			}
			finally
			{
				LeaveStructureChange();
				targetList.LeaveStructureChange();
			}
		}

		private protected void SwapImpl(SparseLinkedListBase<T> targetList)
		{
			try
			{
				EnterStructureChange();
				targetList.EnterStructureChange();

				var head = HeadNode;
				var tail = TailNode;
				var count = Count;

				HeadNode = targetList.HeadNode;
				TailNode = targetList.TailNode;
				LongCount = targetList.Count;

				targetList.HeadNode = head;
				targetList.TailNode = tail;
				targetList.LongCount = count;
			}
			finally
			{
				LeaveStructureChange();
				targetList.LeaveStructureChange();
			}
		}
	}
}