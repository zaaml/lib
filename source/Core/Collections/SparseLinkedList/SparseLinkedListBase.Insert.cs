// <copyright file="SparseLinkedListBase.Insert.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Zaaml.Core.Collections
{
	internal partial class SparseLinkedListBase<T>
	{
		private int StructureChangeCount { get; set; }

		private void CoerceInsertNodeCursor(ref NodeCursor cursor)
		{
			if (cursor.Node is RealizedNode && cursor.NodeSize == NodeCapacity && cursor.Index == cursor.NodeOffset + cursor.NodeSize)
				cursor = cursor.GetNext();
		}

		private void EnterStructureChange()
		{
			StructureChangeCount++;

			if (StructureChangeCount == 1)
				ActualStructureVersion = StructureVersion + 1;
		}

		private protected void InsertVoidRangeImpl(long index, long count)
		{
			if (index == LongCount)
			{
				AddVoidRangeImpl(count);

				return;
			}

			try
			{
				EnterStructureChange();

				if (GetHeadCursor().Contains(index))
				{
					HeadNode.Size += count;
					LongCount += count;

					return;
				}

				if (GetTailCursor().Contains(index))
				{
					TailNode.Size += count;
					LongCount += count;

					return;
				}

				var cursor = NavigateToInsert(index);
				var node = cursor.Node;
				var realizedNode = node as RealizedNode;
				var prevNode = node.Prev;
				var nextNode = node.Next;

				if (prevNode is VoidNode prevVoidNode && cursor.GetPrev().NodeOffset == index)
				{
					prevVoidNode.Size += count;
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
					var sourceSpan = realizedNode.Span.Slice(splitIndex, splitCount);
					var targetSpan = realizedNode.Span.Slice((int) (splitIndex + count), splitCount);

					sourceSpan.CopyTo(targetSpan);

					var clearSpan = realizedNode.Span.Slice(splitIndex, (int) count);

					clearSpan.Clear();

					realizedNode.Size += count;

					LongCount += count;

					return;
				}

				var voidNode = GetVoidNode();

				voidNode.Size = count;

				if (index == cursor.NodeOffset)
				{
					voidNode.Prev = prevNode;
					voidNode.Next = realizedNode;

					prevNode.Next = voidNode;
					realizedNode.Prev = voidNode;
				}
				else
				{
					var nextRealizedNode = GetRealizedNode();

					node.Next = voidNode;

					voidNode.Prev = node;
					voidNode.Next = nextRealizedNode;

					nextRealizedNode.Size = splitCount;
					nextRealizedNode.Prev = voidNode;
					nextRealizedNode.Next = nextNode;
					nextNode.Prev = nextRealizedNode;

					realizedNode.Size = splitIndex;

					var sourceSpan = realizedNode.Span.Slice(splitIndex, splitCount);
					var targetSpan = nextRealizedNode.Span.Slice(0, splitCount);

					sourceSpan.CopyTo(targetSpan);

					sourceSpan.Clear();
				}

				LongCount += count;
			}
			finally
			{
				LeaveStructureChange();
			}
		}

		private protected void InsertImpl(long index, T item)
		{
			if (index == LongCount)
			{
				AddImpl(item);

				return;
			}

			ref var cursor = ref NavigateToInsert(index);

			InsertImpl(item, ref cursor);
		}

		private void InsertImpl(T item, ref NodeCursor cursor)
		{
			try
			{
				EnterStructureChange();

				CoerceInsertNodeCursor(ref cursor);

				var index = cursor.Index;
				var node = cursor.Node;

				if (node is RealizedNode realizedNode)
				{
					var splitIndex = (int) (index - cursor.NodeOffset);
					var splitCount = (int) (realizedNode.Size - splitIndex);
					var span = realizedNode.Span;

					if (realizedNode.Size < NodeCapacity)
					{
						if (splitCount == 0)
							span[(int) realizedNode.Size++] = item;
						else
						{
							var sourceSpan = span.Slice(splitIndex, splitCount);
							var targetSpan = span.Slice(splitIndex + 1, splitCount);

							sourceSpan.CopyTo(targetSpan);

							realizedNode.Size++;
							span[splitIndex] = item;
						}
					}
					else
					{
						splitCount--;

						var lastItem = span[(int) realizedNode.Size - 1];
						var sourceSpan = span.Slice(splitIndex, splitCount);
						var targetSpan = span.Slice(splitIndex + 1, splitCount);

						sourceSpan.CopyTo(targetSpan);

						span[splitIndex] = item;

						var newNode = GetRealizedNode();

						newNode.Span[0] = lastItem;
						newNode.Size = 1;

						InsertNode(newNode, node);
					}
				}
				else
				{
					realizedNode = EnsureRealizedNode(ref cursor, index, true);
					realizedNode.SetItem(ref cursor, item);
				}

				LongCount++;
			}
			finally
			{
				LeaveStructureChange();
			}
		}

		private void InsertNode(RealizedNode node, NodeBase afterNode)
		{
			var nextNode = afterNode.Next;

			node.Prev = afterNode;

			afterNode.Next = node;

			node.Next = nextNode;
			nextNode.Prev = node;
		}

		private protected void InsertRangeImpl(ref InsertEnumerator enumerator, ref NodeCursor cursor)
		{
			if (enumerator.HasAny == false)
				return;

			try
			{
				EnterStructureChange();

				CoerceInsertNodeCursor(ref cursor);

				var count = 1;
				var index = cursor.Index;
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

					cursor = ref NavigateTo(index);

					cursor.Node.SetItem(ref cursor, enumerator.Current);
				}

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

		private protected void InsertRangeImpl(long index, IEnumerable<T> collection)
		{
			if (index == LongCount)
			{
				AddRangeImpl(collection);

				return;
			}

			var enumerator = new InsertEnumerator(collection.GetEnumerator(), this);
			ref var cursor = ref NavigateToInsert(index);

			InsertRangeImpl(ref enumerator, ref cursor);
		}

		private void LeaveStructureChange()
		{
			StructureChangeCount--;

			if (StructureChangeCount == 0)
			{
				StructureVersion = ActualStructureVersion;

				VerifyStructure();
			}
		}

		private protected struct InsertEnumerator
		{
			public InsertEnumerator(T singleItem, SparseLinkedListBase<T> list)
			{
				BaseEnumerator = null;
				List = list;
				SparseMemorySpan = Memory<T>.Empty;
				ItemsCount = 0;
				Current = singleItem;
				CurrentItemIndex = -1;

				HasAny = true;
			}

			public InsertEnumerator(IEnumerator<T> baseEnumerator, SparseLinkedListBase<T> list)
			{
				BaseEnumerator = baseEnumerator;
				List = list;
				SparseMemorySpan = Memory<T>.Empty;
				ItemsCount = 0;

				CurrentItemIndex = 0;

				HasAny = baseEnumerator.MoveNext();
				Current = HasAny ? baseEnumerator.Current : default;
			}

			private InsertEnumerator(InsertEnumerator enumerator, Memory<T> sparseMemorySpan, int count)
			{
				BaseEnumerator = enumerator.BaseEnumerator;
				SparseMemorySpan = sparseMemorySpan;
				ItemsCount = count;
				List = enumerator.List;
				Current = enumerator.Current;
				CurrentItemIndex = enumerator.CurrentItemIndex;
				HasAny = enumerator.HasAny;
			}

			public InsertEnumerator WithItems(Memory<T> items, int count)
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
					SparseMemorySpan = Memory<T>.Empty;
				}

				List = null;
			}

			private int CurrentItemIndex { get; set; }

			private IEnumerator<T> BaseEnumerator { get; set; }

			private SparseLinkedListBase<T> List { get; set; }

			private Memory<T> SparseMemorySpan { get; set; }

			private int ItemsCount { get; }

			public bool HasAny { get; }
		}
	}
}