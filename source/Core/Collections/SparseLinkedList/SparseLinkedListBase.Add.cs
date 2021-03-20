// <copyright file="SparseLinkedListBase.Add.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Diagnostics;

namespace Zaaml.Core.Collections
{
	internal partial class SparseLinkedListBase<T>
	{
		private protected void AddVoidRangeImpl(long count)
		{
			InsertVoidRangeImpl(Count, count);
		}

		private protected void AddImpl(T item)
		{
			try
			{
				EnterStructureChange();

				ref var cursor = ref NavigateToAdd();

				var realNode = (RealizedNode) cursor.Node;

				realNode.Span[(int) realNode.Size++] = item;

				LongCount++;

				cursor.UpgradeWithNode(realNode);
			}
			finally
			{
				LeaveStructureChange();
			}
		}

		private protected void AddRangeImpl(IEnumerable<T> collection)
		{
			var enumerator = new InsertEnumerator(collection.GetEnumerator(), this);

			if (enumerator.HasAny == false)
				return;

			try
			{
				EnterStructureChange();

				ref var cursor = ref NavigateToAdd();

				AddRangeImpl(ref enumerator, ref cursor);

				cursor.UpdateStructureVersion();
			}
			finally
			{
				LeaveStructureChange();
			}
		}

		private protected void AddRangeImpl(ref InsertEnumerator enumerator, ref NodeCursor cursor)
		{
			CoerceInsertNodeCursor(ref cursor);

			var count = 1;
			var nextNode = cursor.Node.Next;
			var realizedNode = (RealizedNode) cursor.Node;

			realizedNode.Span[(int) realizedNode.Size++] = enumerator.Current;

			while (true)
			{
				var realizedNodeSpan = realizedNode.Span;
				var realizedNodeIndex = (int) realizedNode.Size;

				while (realizedNodeIndex < NodeCapacity && enumerator.MoveNext())
				{
					realizedNodeSpan[realizedNodeIndex++] = enumerator.Current;
					count++;
				}

				realizedNode.Size = realizedNodeIndex;

				if (realizedNode.Size < NodeCapacity)
					break;

				if (enumerator.MoveNext() == false)
					break;

				count++;

				var next = GetRealizedNode();

				next.Size = 1;
				next.Prev = realizedNode;

				next.Span[0] = enumerator.Current;

				realizedNode.Next = next;
				realizedNode = next;

				cursor.MoveNext();
			}

			enumerator.Dispose();

			realizedNode.Next = nextNode;
			nextNode.Prev = realizedNode;

			LongCount += count;
		}

		private ref NodeCursor NavigateToAdd()
		{
			ref var cursor = ref GetCursor();

			if (cursor.Node is RealizedNode realNode && LongCount == cursor.NodeOffset + cursor.NodeSize && realNode.Size < NodeCapacity)
				return ref cursor;

			if (ReferenceEquals(HeadNode.Next, TailNode))
			{
				Debug.Assert(TailNode.Size == 0);

				realNode = GetRealizedNode();

				HeadNode.Next = realNode;
				realNode.Prev = HeadNode;
				realNode.Next = TailNode;
				TailNode.Prev = realNode;
			}
			else if (TailNode.Size == 0)
			{
				realNode = (RealizedNode) TailNode.Prev;
			}
			else
			{
				realNode = GetRealizedNode();

				TailNode.Next = realNode;
				realNode.Prev = TailNode;

				TailNode = GetGapNode();
				realNode.Next = TailNode;
				TailNode.Prev = realNode;
			}

			if (realNode.Size == NodeCapacity)
			{
				var newRealNode = GetRealizedNode();

				realNode.Next = newRealNode;
				newRealNode.Prev = realNode;
				newRealNode.Next = TailNode;
				TailNode.Prev = newRealNode;

				realNode = newRealNode;
			}

			cursor = ref GetTailCursor();

			cursor.MovePrev();
			cursor.Index = LongCount;

			Debug.Assert(ReferenceEquals(cursor.Node, realNode));

			return ref cursor;
		}
	}
}