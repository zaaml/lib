// <copyright file="SparseLinkedListBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Zaaml.Core.Collections
{
	internal partial class SparseLinkedListBase<T>
	{
		private NodeCursor _cursor;
#if TEST
		private const int DefaultCapacity = 4;
#else
		protected const int DefaultCapacity = 16;
#endif

		public SparseLinkedListBase() : this(0, new SparseLinkedListManager<T>(new SparseMemoryManager<T>(DefaultCapacity)))
		{
		}

		public SparseLinkedListBase(int count) : this(count,
			new SparseLinkedListManager<T>(new SparseMemoryManager<T>(DefaultCapacity)))
		{
		}

		protected SparseLinkedListBase(int count, SparseLinkedListManager<T> manager)
		{
			Manager = manager;

			HeadNode = Manager.GetGapNode();
			TailNode = Manager.GetGapNode();

			LongCount = count;

			InitHeadTail();
		}

		public SparseLinkedListNode<T> Head => new SparseLinkedListNode<T>(HeadNode, this);

		public SparseLinkedListNode<T> Tail => new SparseLinkedListNode<T>(TailNode, this);

		internal int StructureVersion { get; private set; }

		protected GapNode HeadNode { get; private set; }

		protected int NodeCapacity => Manager.SparseMemoryManager.NodeCapacity;

		private SparseLinkedListManager<T> Manager { get; }

		protected GapNode TailNode { get; private set; }

		private NodeCursor Cursor
		{
			get
			{
				if (_cursor.IsEmpty || _cursor.StructureVersion != StructureVersion || StructureChangeCount > 0)
					_cursor = new NodeCursor(0, this, HeadNode, 0);

				return _cursor;
			}
			set => _cursor = value;
		}

		private NodeCursor HeadCursor => new NodeCursor(0, this, HeadNode, 0);

		private NodeCursor TailCursor =>
			new NodeCursor(LongCount > 0 ? LongCount - 1 : 0, this, TailNode, LongCount - TailNode.Size);

		public long LongCount { get; private set; }

		[PublicAPI] public int Count => (int) LongCount;

		private void InitHeadTail()
		{
			EnterStructureChange();

			HeadNode.Next = TailNode;
			TailNode.Prev = HeadNode;

			HeadNode.Size = Count;
			TailNode.Size = 0;

			LeaveStructureChange();
		}

		private protected void VerifyStructureImpl()
		{
			if (HeadNode == null || TailNode == null)
				throw new InvalidOperationException();

			if (ReferenceEquals(HeadNode, TailNode))
				throw new InvalidOperationException();

			if (HeadNode.Prev != null || TailNode.Next != null)
				throw new InvalidOperationException();

			if (ReferenceEquals(HeadNode.Next, TailNode))
			{
				if (TailNode.Size > 0)
					throw new InvalidOperationException();

				if (ReferenceEquals(TailNode.Prev, HeadNode) == false)
					throw new InvalidOperationException();

				if (HeadNode.Size != LongCount)
					throw new InvalidOperationException();

				return;
			}

			NodeBase current = HeadNode;

			while (current != null)
			{
				var next = current.Next;

				if (next != null)
				{
					if (ReferenceEquals(next.Prev, current) == false)
						throw new InvalidOperationException();

					if (next is GapNode && current is GapNode)
					{
						throw new InvalidOperationException();
					}
				}

				current = next;
			}
		}

		[Conditional("TEST")]
		private protected void VerifyStructure()
		{
			VerifyStructureImpl();
		}

		// ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
		private protected void VerifyIndex(int index, bool insert = false)
		{
			if (index == 0 && Count == 0)
				return;

			if (index < 0 || index > Count || index == Count && insert == false)
				throw new IndexOutOfRangeException(nameof(index));
		}

		private protected void VerifyRange(int index, int count)
		{
			if (index == 0 && Count == 0 && count == 0)
				return;

			if (index < 0)
				throw new IndexOutOfRangeException(nameof(index));

			if (count < 0)
				throw new IndexOutOfRangeException(nameof(count));

			if (index >= Count)
				throw new IndexOutOfRangeException(nameof(index));

			if (index + count > Count)
				throw new IndexOutOfRangeException(nameof(count));
		}

		private void DeallocateItems(SparseMemorySpan<T> sparseMemorySpan)
		{
			sparseMemorySpan.Release();
		}

		private protected int FindImpl(T item)
		{
			var equalityComparer = EqualityComparer<T>.Default;
			var enumerator = new Enumerator(this);
			var index = 0;

			while (enumerator.MoveNext())
			{
				var current = enumerator.Current;

				if (equalityComparer.Equals(current, item))
					return index;

				index++;
			}

			return -1;
		}

		public SparseLinkedListNode<T> FindNode(long index, out long offset)
		{
			var node = FindNodeImpl(index, out offset);

			return node != null ? new SparseLinkedListNode<T>(node, this) : SparseLinkedListNode<T>.Empty;
		}

		private NodeBase FindNodeImpl(long index, out long offset)
		{
			var cursor = NavigateTo(index);

			offset = cursor.NodeOffset;

			return cursor.Node;
		}

		private NodeBase FindNodeImpl(long index)
		{
			return NavigateTo(index).Node;
		}

		private NodeCursor NavigateTo(long index)
		{
			var cursor = Cursor.NavigateTo(index);

			Cursor = cursor;

			return cursor;
		}

		private NodeCursor NavigateTo(long index, bool realize)
		{
			var cursor = Cursor.NavigateTo(index);

			if (realize) 
				EnsureRealizedNode(ref cursor, index, false);

			Cursor = cursor;

			return cursor;
		}

		private protected T GetItemImpl(long index)
		{
			var cursor = NavigateTo(index);

			return cursor.Node.GetItem(ref cursor);
		}

		private SparseMemorySpan<T> AllocateItems()
		{
			return Manager.Allocate();
		}

		private protected void ClearImpl()
		{
			EnterStructureChange();

			var current = HeadNode.Next;

			while (current != null && ReferenceEquals(current, TailNode) == false)
			{
				var next = current.Next;

				RemoveNode(current);

				current = next;
			}

			LongCount = 0;

			InitHeadTail();

			LeaveStructureChange();
		}

		private void RemoveEmptyGapNode(GapNode gapNode)
		{
			if (gapNode.Size == 0 && ReferenceEquals(gapNode, HeadNode) == false &&  ReferenceEquals(gapNode, TailNode) == false)
				RemoveNode(gapNode);
		}

		private void RemoveNode(NodeBase node)
		{
			Debug.Assert(ReferenceEquals(node, HeadNode) == false);
			Debug.Assert(ReferenceEquals(node, TailNode) == false);

			var prev = node.Prev;
			var next = node.Next;

			if (prev != null)
				prev.Next = next;

			if (next != null)
				next.Prev = prev;

			ReleaseNode(node);
		}

		private protected void SetItemImpl(long index, T value)
		{
			var cursor = NavigateTo(index, true);

			cursor.Node.SetItem(ref cursor, value);
		}
	}
}