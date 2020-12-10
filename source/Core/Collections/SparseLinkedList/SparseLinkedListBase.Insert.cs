// <copyright file="SparseLinkedListBase.Insert.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Zaaml.Core.Collections
{
	internal partial class SparseLinkedListBase<T>
	{
		private Node FindInsertNodeImpl(int index)
		{
			if (Count == 0)
				return HeadNode;

			var node = Current;

			if (ReferenceEquals(HeadNode.Next, TailNode))
			{
				node = HeadNode;
			}
			else if (node.Contains(index) || node.Count == 0 && node.Index == index)
			{
			}
			else if (index >= node.Index)
			{
				while (node != null)
				{
					if (node.Contains(index) || node.Count == 0 && node.Index == index)
						break;

					node = node.Next;
				}
			}
			else
			{
				while (node != null)
				{
					if (node.Contains(index) || node.Count == 0 && node.Index == index)
						break;

					node = node.Prev;
				}
			}

			Current = node;

			return node;
		}

		private protected void InsertCleanRangeImpl(int index, int count)
		{
			if (index >= TailNode.Index)
			{
				if (ReferenceEquals(HeadNode.Next, TailNode))
				{
					HeadNode.Count += count;
					TailNode.Index += count;

					Current = HeadNode;
				}
				else
				{
					TailNode.Count += count;

					Current = TailNode;
				}

				Count += count;

				return;
			}

			var node = FindInsertNodeImpl(index);
			var realizedNode = node as RealizedNode;
			var prevNode = node.Prev;
			var nextNode = node.Next;

			if (prevNode is GapNode prevGapNode && prevGapNode.Index == index)
			{
				prevGapNode.Count += count;

				AdvanceIndices(prevGapNode);

				Count += count;
				Current = node;

				return;
			}

			if (realizedNode == null)
			{
				node.Count += count;

				AdvanceIndices(node);

				Count += count;
				Current = node;

				return;
			}

			var splitIndex = index - realizedNode.Index;
			var splitCount = realizedNode.Count - splitIndex;

			if (count <= NodeCapacity - realizedNode.Count)
			{
				//Array.Copy(realizedNode.Items, splitIndex, realizedNode.Items, splitIndex + count, splitCount);

				var sourceSpan = realizedNode.Span.Slice(splitIndex, splitCount);
				var destinationSpan = realizedNode.Span.Slice(splitIndex + count, splitCount);

				sourceSpan.CopyTo(destinationSpan);
			
				//Array.Clear(realizedNode.Items, splitIndex, count);
				
				realizedNode.Span.Slice(splitIndex, count).Clear();
				
				realizedNode.Count += count;

				AdvanceIndices(realizedNode);

				Count += count;
				Current = node;

				return;
			}

			var gapNode = CreateGapNode();

			gapNode.Index = index;
			gapNode.Count = count;

			if (index == realizedNode.Index)
			{
				gapNode.Prev = prevNode;
				gapNode.Next = realizedNode;

				prevNode.Next = gapNode;
				realizedNode.Prev = gapNode;
			}
			else
			{
				var nextRealizedNode = CreateRealizedNode();

				node.Next = gapNode;

				gapNode.Prev = node;
				gapNode.Next = nextRealizedNode;

				nextRealizedNode.Index = gapNode.Index + gapNode.Count;
				nextRealizedNode.Count = splitCount;
				nextRealizedNode.Prev = gapNode;
				nextRealizedNode.Next = nextNode;
				nextNode.Prev = nextRealizedNode;

				realizedNode.Count = splitIndex;

				//Array.Copy(realizedNode.ItemsPrivate, splitIndex, nextRealizedNode.ItemsPrivate, 0, splitCount);
				var sourceSpan = realizedNode.Span.Slice(splitIndex, splitCount);
				var destinationSpan = nextRealizedNode.Span.Slice(0, splitCount);

				sourceSpan.CopyTo(destinationSpan);

				//Array.Clear(realizedNode.ItemsPrivate, splitIndex, splitCount);
				sourceSpan.Clear();
			}

			AdvanceIndices(gapNode);

			Count += count;
			Current = gapNode;
		}

		private protected void InsertImpl(int index, T item)
		{
			InsertRangeImpl(index, new InsertEnumerator(item, this));
		}

		private protected void InsertRangeImpl(int index, InsertEnumerator enumerator)
		{
			if (enumerator.HasAny == false)
				return;

			var count = 1;
			var node = FindInsertNodeImpl(index);
			var nextNode = node.Next;

			if (node is RealizedNode realizedNode)
			{
				var splitIndex = index - realizedNode.Index;
				var splitCount = realizedNode.Count - splitIndex;

				if (splitCount > 0)
				{
					var splitItems = AllocateItems();

					//Array.Copy(realizedNode.ItemsPrivate, splitIndex, splitItems, 0, splitCount);

					var sourceSpan = realizedNode.Span.Slice(splitIndex, splitCount);
					var destinationSpan = splitItems.Span.Slice(0, splitCount);
					
					sourceSpan.CopyTo(destinationSpan);

					//Array.Clear(realizedNode.ItemsPrivate, splitIndex, splitCount);
					realizedNode.Span.Slice(splitIndex, splitCount).Clear();

					realizedNode.Count = splitIndex + 1;
					realizedNode.Span[splitIndex] = enumerator.Current;

					enumerator = enumerator.WithItems(splitItems, splitCount);
					count -= splitCount;
				}
			}
			else
			{
				realizedNode = EnsureRealizedNode(node, index, true);
				nextNode = realizedNode.Next;

				realizedNode[index] = enumerator.Current;
			}

			while (true)
			{
				while (realizedNode.Count < NodeCapacity && enumerator.MoveNext())
				{
					realizedNode.Span[realizedNode.Count++] = enumerator.Current;
					count++;
				}

				if (realizedNode.Count < NodeCapacity)
					break;

				if (enumerator.MoveNext() == false)
					break;

				count++;

				var next = CreateRealizedNode();

				next.Index = realizedNode.Index + NodeCapacity;
				next.Count = 1;
				next.Prev = realizedNode;

				next.Span[0] = enumerator.Current;

				realizedNode.Next = next;
				realizedNode = next;
			}

			enumerator.Dispose();

			realizedNode.Next = nextNode;
			nextNode.Prev = realizedNode;

			AdvanceIndices(realizedNode);

			Count += count;
			Current = realizedNode;
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