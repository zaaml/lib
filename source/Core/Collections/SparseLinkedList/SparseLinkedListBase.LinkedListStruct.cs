// <copyright file="SparseLinkedListBase.LinkedListStruct.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Diagnostics;

namespace Zaaml.Core.Collections
{
	internal partial class SparseLinkedListBase<T>
	{
		private ref struct LinkedListStruct
		{
			public SparseLinkedListManager<T> ListManager { get; }

			public LinkedListStruct(SparseLinkedListBase<T> list)
			{
				ListManager = list.Manager;
				HeadNode = list.HeadNode;
				TailNode = list.TailNode;
				LongCount = list.LongCount;
			}

			public LinkedListStruct(SparseLinkedListManager<T> listManager)
			{
				ListManager = listManager;
				HeadNode = ListManager.GetGapNode();
				TailNode = ListManager.GetGapNode();
				HeadNode.Next = TailNode;
				TailNode.Prev = HeadNode;
				LongCount = 0;
			}

			public GapNode TailNode { get; set; }

			public GapNode HeadNode { get; set; }

			public long LongCount { get; set; }

			public void Store(SparseLinkedListBase<T> list)
			{
				list.HeadNode = HeadNode;
				list.TailNode = TailNode;
				list.LongCount = LongCount;
			}

			public void Release()
			{
				Debug.Assert(ReferenceEquals(HeadNode.Next, TailNode));
				Debug.Assert(ReferenceEquals(TailNode.Prev, HeadNode));
				Debug.Assert(ReferenceEquals(HeadNode.Prev, null));
				Debug.Assert(ReferenceEquals(TailNode.Next, null));
				Debug.Assert(LongCount == 0);

				ListManager.ReleaseNode(HeadNode);
				ListManager.ReleaseNode(TailNode);
			}

			public void Split(ref NodeCursor cursor, ref LinkedListStruct targetList)
			{
				var index = cursor.Index;
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

						ListManager.ReleaseNode(gapNode);
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

					if (prev is GapNode prevGapNode)
					{
						if (ReferenceEquals(HeadNode, prev))
						{
							TailNode = targetListTail;
							TailNode.Prev = HeadNode;
							HeadNode.Next = TailNode;
						}
						else
						{
							TailNode = prevGapNode;
							TailNode.Next = null;

							ListManager.ReleaseNode(targetListTail);
						}
					}
					else
					{
						TailNode = targetListTail;
						prev.Next = TailNode;
						TailNode.Prev = prev;
					}
				}
				else
				{
					var sourceRealizedNode = (RealizedNode) cursor.Node;
					var targetRealizedNode = ListManager.GetRealizedNode();

					var sourceSpan = sourceRealizedNode.Span.Slice(nodeSplitIndex, nodeSplitCount);
					var targetSpan = targetRealizedNode.Span.Slice(0, nodeSplitCount);

					sourceSpan.CopyTo(targetSpan);

					// Target
					targetRealizedNode.Size = nodeSplitCount;
					targetList.HeadNode.Next = targetRealizedNode;
					targetRealizedNode.Prev = targetList.HeadNode;
					targetRealizedNode.Next = sourceRealizedNode.Next;
					sourceRealizedNode.Next.Prev = targetRealizedNode;
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

			public void Merge(ref LinkedListStruct sourceList)
			{
				if (ReferenceEquals(HeadNode.Next, TailNode))
				{
					sourceList.HeadNode.Size += HeadNode.Size;

					var head = HeadNode;
					var tail = TailNode;

					HeadNode = sourceList.HeadNode;
					TailNode = sourceList.TailNode;

					sourceList.HeadNode = head;
					sourceList.TailNode = tail;
				}
				else if (ReferenceEquals(sourceList.HeadNode.Next, sourceList.TailNode))
				{
					TailNode.Size += sourceList.HeadNode.Size;
				}
				else
				{
					if (TailNode.Size > 0 || sourceList.HeadNode.Size > 0)
					{
						TailNode.Size += sourceList.HeadNode.Size;
						TailNode.Next = sourceList.HeadNode.Next;
						sourceList.HeadNode.Next.Prev = TailNode;
						TailNode = sourceList.TailNode;

						sourceList.TailNode = ListManager.GetGapNode();
						sourceList.HeadNode.Next = sourceList.TailNode;
						sourceList.TailNode.Prev = sourceList.HeadNode;
					}
					else
					{
						var realizedSource = (RealizedNode) TailNode.Prev;
						var realizedTarget = (RealizedNode) sourceList.HeadNode.Next;
						var tailNode = TailNode;

						realizedSource.Next = realizedTarget;
						realizedTarget.Prev = realizedSource;
						TailNode = sourceList.TailNode;

						sourceList.TailNode = tailNode;
						sourceList.HeadNode.Next = sourceList.TailNode;
						sourceList.TailNode.Prev = sourceList.HeadNode;
					}
				}

				LongCount += sourceList.LongCount;

				sourceList.LongCount = 0;
				sourceList.HeadNode.Size = 0;
				sourceList.TailNode.Size = 0;
			}
		}
	}
}