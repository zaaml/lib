// <copyright file="SparseLinkedListBase.Insert.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Zaaml.Core.Collections
{
	internal partial class SparseLinkedListBase<T>
	{
		private NodeCursor FindInsertNodeCursorImpl(long index)
		{
			if (Count == 0 || ReferenceEquals(HeadNode.Next, TailNode))
				return HeadCursor;

			if (TailCursor.Contains(index))
				return TailCursor;

			var cursor = Cursor;

			if (cursor.Contains(index))
				return cursor;

			if (index >= cursor.Index)
			{
				while (cursor.Node != null)
				{
					var next = cursor.GetNext();

					if (cursor.Contains(index, true))
						return (next.Contains(index) ? next : cursor).WithIndex(index);

					cursor = next;
				}
			}
			else
			{
				while (cursor.Node != null)
				{
					var prev = cursor.GetPrev();

					if (cursor.Contains(index, true))
						return (prev.Contains(index) ? prev : cursor).WithIndex(index);

					cursor = prev;
				}
			}

			return NodeCursor.Empty;
		}

		private protected void InsertCleanRangeImpl(long index, long count)
		{
			try
			{
				EnterStructureChange();

				if (ReferenceEquals(HeadNode.Next, TailNode))
				{
					HeadNode.Size += count;
					LongCount += count;

					return;
				}

				if (index == 0 || HeadCursor.Contains(index))
				{
					HeadNode.Size += count;
					LongCount += count;

					return;
				}

				if (index == LongCount || TailCursor.Contains(index))
				{
					TailNode.Size += count;
					LongCount += count;

					return;
				}

				var cursor = FindInsertNodeCursorImpl(index);
				var node = cursor.Node;
				var realizedNode = node as RealizedNode;
				var prevNode = node.Prev;
				var nextNode = node.Next;

				if (prevNode is GapNode prevGapNode && cursor.GetPrev().NodeOffset == index)
				{
					prevGapNode.Size += count;
					LongCount += count;

					return;
				}

				if (realizedNode == null)
				{
					node.Size += count;
					LongCount += count;

					return;
				}

				var splitIndex = (int) (index - cursor.NodeOffset);
				var splitCount = (int) (realizedNode.Size - splitIndex);

				if (count <= NodeCapacity - realizedNode.Size)
				{
					//Array.Copy(realizedNode.Items, splitIndex, realizedNode.Items, splitIndex + count, splitCount);

					var sourceSpan = realizedNode.Span.Slice(splitIndex, splitCount);
					var targetSpan = realizedNode.Span.Slice((int) (splitIndex + count), splitCount);

					sourceSpan.CopyTo(targetSpan);

					//Array.Clear(realizedNode.Items, splitIndex, count);

					var clearSpan = realizedNode.Span.Slice(splitIndex, (int) count);

					clearSpan.Clear();

					realizedNode.Size += count;

					LongCount += count;

					return;
				}

				var gapNode = Manager.GetGapNode();

				gapNode.Size = count;

				if (index == cursor.NodeOffset)
				{
					gapNode.Prev = prevNode;
					gapNode.Next = realizedNode;

					prevNode.Next = gapNode;
					realizedNode.Prev = gapNode;
				}
				else
				{
					var nextRealizedNode = Manager.GetRealizedNode();

					node.Next = gapNode;

					gapNode.Prev = node;
					gapNode.Next = nextRealizedNode;

					nextRealizedNode.Size = splitCount;
					nextRealizedNode.Prev = gapNode;
					nextRealizedNode.Next = nextNode;
					nextNode.Prev = nextRealizedNode;

					realizedNode.Size = splitIndex;

					//Array.Copy(realizedNode.ItemsPrivate, splitIndex, nextRealizedNode.ItemsPrivate, 0, splitCount);
					var sourceSpan = realizedNode.Span.Slice(splitIndex, splitCount);
					var targetSpan = nextRealizedNode.Span.Slice(0, splitCount);

					sourceSpan.CopyTo(targetSpan);

					//Array.Clear(realizedNode.ItemsPrivate, splitIndex, splitCount);
					sourceSpan.Clear();
				}

				LongCount += count;
			}
			finally
			{
				LeaveStructureChange();
			}
		}

		private int StructureChangeCount { get; set; }

		private void EnterStructureChange()
		{
			StructureChangeCount++;
		}

		private void LeaveStructureChange()
		{
			StructureChangeCount--;

			if(StructureChangeCount == 0)
				IncrementStructureVersion();
		}

		private void IncrementStructureVersion()
		{
			StructureVersion++;
		}

		private protected void InsertImpl(long index, T item)
		{
			InsertRangeImpl(index, new InsertEnumerator(item, this));
		}

		private protected void InsertRangeImpl(long index, InsertEnumerator enumerator)
		{
			if (enumerator.HasAny == false)
				return;

			try
			{
				EnterStructureChange();

				var count = 1;
				var cursor = FindInsertNodeCursorImpl(index);

				if (cursor.Node is RealizedNode && cursor.NodeSize == NodeCapacity && index == cursor.NodeOffset + cursor.NodeSize)
					cursor = cursor.GetNext();

				var nextNode = cursor.Node.Next;

				if (cursor.Node is RealizedNode realizedNode)
				{
					var splitIndex = (int) (index - cursor.NodeOffset);
					var splitCount = (int) (realizedNode.Size - splitIndex);

					if (splitCount == 0)
					{
						realizedNode.Span[(int) realizedNode.Size++] = enumerator.Current;
					}
					else
					{
						var splitItems = AllocateItems();
						var sourceSpan = realizedNode.Span.Slice(splitIndex, splitCount);
						var targetSpan = splitItems.Span.Slice(0, splitCount);

						sourceSpan.CopyTo(targetSpan);
						sourceSpan.Clear();

						realizedNode.Size = splitIndex + 1;
						realizedNode.Span[splitIndex] = enumerator.Current;

						enumerator = enumerator.WithItems(splitItems, splitCount);
						count -= splitCount;
					}
				}
				else
				{
					realizedNode = EnsureRealizedNode(ref cursor, index, true);
					nextNode = realizedNode.Next;

					cursor = NavigateTo(index);

					cursor.Node.SetItem(ref cursor, enumerator.Current);
				}

				while (true)
				{
					while (realizedNode.Size < NodeCapacity && enumerator.MoveNext())
					{
						realizedNode.Span[(int) realizedNode.Size++] = enumerator.Current;
						count++;
					}

					if (realizedNode.Size < NodeCapacity)
						break;

					if (enumerator.MoveNext() == false)
						break;

					count++;

					var next = Manager.GetRealizedNode();

					next.Size = 1;
					next.Prev = realizedNode;

					next.Span[0] = enumerator.Current;

					realizedNode.Next = next;
					realizedNode = next;
				}

				enumerator.Dispose();

				realizedNode.Next = nextNode;
				nextNode.Prev = realizedNode;

				LongCount += count;
			}
			finally
			{
				LeaveStructureChange();
			}
		}

		private protected void InsertRangeImpl(int index, IEnumerable<T> collection)
		{
			var enumerator = new InsertEnumerator(collection.GetEnumerator(), this);

			InsertRangeImpl(index, enumerator);
		}

		private protected struct InsertEnumerator
		{
			public InsertEnumerator(T singleItem, SparseLinkedListBase<T> list)
			{
				BaseEnumerator = null;
				List = list;
				SparseMemorySpan = SparseMemorySpan<T>.Empty;
				ItemsCount = 0;
				Current = singleItem;
				CurrentItemIndex = -1;

				HasAny = true;
			}

			public InsertEnumerator(IEnumerator<T> baseEnumerator, SparseLinkedListBase<T> list)
			{
				BaseEnumerator = baseEnumerator;
				List = list;
				SparseMemorySpan = SparseMemorySpan<T>.Empty;
				ItemsCount = 0;

				CurrentItemIndex = 0;

				HasAny = baseEnumerator.MoveNext();
				Current = baseEnumerator.Current;
			}

			private InsertEnumerator(InsertEnumerator enumerator, SparseMemorySpan<T> sparseMemorySpan, int count)
			{
				BaseEnumerator = enumerator.BaseEnumerator;
				SparseMemorySpan = sparseMemorySpan;
				ItemsCount = count;
				List = enumerator.List;
				Current = enumerator.Current;
				CurrentItemIndex = enumerator.CurrentItemIndex;
				HasAny = enumerator.HasAny;
			}

			public InsertEnumerator WithItems(SparseMemorySpan<T> items, int count)
			{
				return new InsertEnumerator(this, items, count);
			}

			public T Current { get; private set; }

			public bool MoveNext()
			{
				if (BaseEnumerator != null)
				{
					var next = BaseEnumerator.MoveNext();

					if (next)
					{
						Current = BaseEnumerator.Current;

						return true;
					}

					BaseEnumerator.Dispose();
					BaseEnumerator = null;

					if (SparseMemorySpan.IsEmpty)
						return false;

					Current = SparseMemorySpan.Span[0];

					return true;
				}

				if (CurrentItemIndex < ItemsCount - 1)
				{
					CurrentItemIndex++;

					Current = SparseMemorySpan.Span[CurrentItemIndex];

					return true;
				}

				Current = default;

				return false;
			}

			public void Dispose()
			{
				if (BaseEnumerator != null)
				{
					BaseEnumerator.Dispose();
					BaseEnumerator = null;
				}

				if (SparseMemorySpan.IsEmpty == false)
				{
					List.DeallocateItems(SparseMemorySpan);
					SparseMemorySpan = SparseMemorySpan<T>.Empty;
				}

				List = null;
			}

			private int CurrentItemIndex { get; set; }

			private IEnumerator<T> BaseEnumerator { get; set; }

			private SparseLinkedListBase<T> List { get; set; }

			private SparseMemorySpan<T> SparseMemorySpan { get; set; }

			private int ItemsCount { get; }

			public bool HasAny { get; }
		}
	}
}