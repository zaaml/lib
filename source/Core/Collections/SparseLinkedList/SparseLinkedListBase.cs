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
		private const int DefaultCapacity = 16;
#endif

		protected SparseLinkedListBase() : this(0, new SparseLinkedListManager<T>(new SparseMemoryAllocator<T>(DefaultCapacity)))
		{
		}

		protected SparseLinkedListBase(long count, SparseLinkedListManager<T> manager)
		{
			Manager = manager;

			HeadNode = GetVoidNode();
			TailNode = GetVoidNode();

			LongCount = count;

			InitHeadTail();
		}

		public SparseLinkedListNode<T> Head => new SparseLinkedListNode<T>(HeadNode, this);

		public SparseLinkedListNode<T> Tail => new SparseLinkedListNode<T>(TailNode, this);

		internal ulong StructureVersion { get; private set; }

		internal ulong ActualStructureVersion { get; private set; }

		protected VoidNode HeadNode { get; private set; }

		protected int NodeCapacity => Manager.SparseMemoryAllocator.NodeCapacity;

		private SparseLinkedListManager<T> Manager { get; }

		private RealizedNode GetRealizedNode()
		{
			return Manager.GetRealizedNode();
		}

		private VoidNode GetVoidNode()
		{
			return Manager.GetVoidNode();
		}

		private void ReleaseNode(NodeBase node)
		{
			Manager.ReleaseNode(node);
		}

		private ref NodeCursor GetCursor()
		{
			if (_cursor.IsEmpty || _cursor.StructureVersion != StructureVersion)
				_cursor = new NodeCursor(0, this, HeadNode, 0);

			return ref _cursor;
		}

		protected VoidNode TailNode { get; private set; }

		private ref NodeCursor GetHeadCursor()
		{
			_cursor = new NodeCursor(0, this, HeadNode, 0);

			return ref _cursor;
		}

		private ref NodeCursor GetTailCursor()
		{
			_cursor = new(LongCount - TailNode.Size, this, TailNode, LongCount - TailNode.Size);

			return ref _cursor;
		}

		public long LongCount { get; private set; }

		[PublicAPI]
		public int Count => (int) LongCount;

		private void InitHeadTail()
		{
			var count = LongCount;

			LongCount = 0;

			InitImpl(count);
		}

		protected void InitImpl(long count)
		{
			if (LongCount != 0)
				throw new InvalidOperationException();

			EnterStructureChange();

			HeadNode.Next = TailNode;
			TailNode.Prev = HeadNode;

			HeadNode.Size = count;
			TailNode.Size = 0;

			LongCount = count;

			LeaveStructureChange();
		}

		private void VerifyStructureImpl()
		{
			if (HeadNode == null || TailNode == null)
				throw new InvalidOperationException();

			if (ReferenceEquals(HeadNode, TailNode))
				throw new InvalidOperationException();

			if (HeadNode.Prev != null || TailNode.Next != null)
				throw new InvalidOperationException();

			if (HeadNode.Next is VoidNode && ReferenceEquals(HeadNode.Next, TailNode) == false)
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

					if (next is VoidNode && current is VoidNode)
					{
						throw new InvalidOperationException();
					}
				}

				current = next;
			}
		}

		[Conditional("COLLECTION_VERIFY_STRUCTURE")]
		private protected void VerifyStructure()
		{
			VerifyStructureImpl();
		}

		// ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
		private protected void VerifyIndex(long index, bool insert = false)
		{
			if (index == 0 && Count == 0)
				return;

			if (index < 0 || index > Count || index == Count && insert == false)
				throw new IndexOutOfRangeException(nameof(index));
		}

		private protected void VerifyRange(long index, long count)
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

		private void DeallocateItems(Memory<T> sparseMemorySpan)
		{
			sparseMemorySpan.Dispose();
		}

		private protected long FindImpl(T item)
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
			ref var cursor = ref NavigateTo(index);

			offset = cursor.NodeOffset;

			return cursor.Node;
		}

		private ref NodeCursor NavigateTo(long index)
		{
			ref var cursor = ref GetCursor();

			cursor.NavigateTo(index);

			return ref cursor;
		}

		private ref NodeCursor NavigateTo(long index, bool realize)
		{
			ref var cursor = ref GetCursor();

			cursor.NavigateTo(index);

			if (realize)
				EnsureRealizedNode(ref cursor, index, false);

			return ref cursor;
		}

		private ref NodeCursor NavigateToInsert(long index)
		{
			if (Count == 0 || ReferenceEquals(HeadNode.Next, TailNode))
				return ref GetHeadCursor();

			ref var tailCursor = ref GetTailCursor();

			if (tailCursor.Contains(index))
				return ref tailCursor;

			ref var cursor = ref GetCursor();

			cursor.NavigateToInsert(index);

			return ref cursor;
		}

		private protected T GetItemImpl(long index)
		{
			ref var cursor = ref NavigateTo(index);

			return cursor.Node.GetItem(ref cursor);
		}

		private Memory<T> AllocateItems()
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

		private void RemoveEmptyVoidNode(VoidNode voidNode)
		{
			if (voidNode.Size == 0 && ReferenceEquals(voidNode, HeadNode) == false && ReferenceEquals(voidNode, TailNode) == false)
				RemoveNode(voidNode);
		}

		private void RemoveNode(NodeBase node)
		{
			Debug.Assert(ReferenceEquals(node, HeadNode) == false);
			Debug.Assert(ReferenceEquals(node, TailNode) == false);

			var prev = node.Prev;
			var next = node.Next;

			prev.Next = next;
			next.Prev = prev;

			ReleaseNode(node);
		}

		private protected void SetItemImpl(long index, T value)
		{
			ref var cursor = ref NavigateTo(index, true);

			cursor.Node.SetItem(ref cursor, value);
		}

		private protected void CopyToImpl(T[] array, int arrayIndex)
		{
			if (array.Length - arrayIndex < Count)
				throw new InvalidOperationException("Insufficient array length");

			CopyToImpl(array.AsSpan(arrayIndex));
		}

		private protected void CopyToImpl(Span<T> span)
		{
			CopyToImpl(0, LongCount, span);
		}

		private protected void CopyToImplSafe(Span<T> span)
		{
			CopyToImplSafe(0, LongCount, span);
		}

		private protected void CopyToImpl(long offset, long length, Span<T> span)
		{
			if (span.Length < length)
				throw new InvalidOperationException("Insufficient span length");

			if (offset + length > LongCount || offset < 0)
				throw new ArgumentOutOfRangeException();

			CopyToImplSafe(offset, length, span);
		}

		private void CopyToImplSafe(long offset, long length, Span<T> span)
		{
			var index = 0;
			NodeBase current = HeadNode;

			if (offset > 0)
			{
				long currentOffset = 0;

				while (currentOffset + current.Size < offset)
				{
					currentOffset += current.Size;
					current = current.Next;
				}

				var localOffset = (int) (offset - currentOffset);
				var copyLength = (int) Math.Min(current.Size - localOffset, length);
				var targetSpan = span.Slice(index, copyLength);

				if (current is RealizedNode realizedNode)
				{
					var sourceSpan = realizedNode.Span.Slice(localOffset, copyLength);

					sourceSpan.CopyTo(targetSpan);
				}
				else
					targetSpan.Clear();

				length -= copyLength;
				index += copyLength;
				current = current.Next;
			}

			while (current != null && length > 0)
			{
				var copyLength = (int) Math.Min(current.Size, length);
				var targetSpan = span.Slice(index, copyLength);

				if (current is RealizedNode realizedNode)
				{
					var sourceSpan = realizedNode.Span.Slice(0, copyLength);

					sourceSpan.CopyTo(targetSpan);
				}
				else
					targetSpan.Clear();

				index += copyLength;
				length -= copyLength;
				current = current.Next;
			}
		}

		private protected bool IsVoidImpl => ReferenceEquals(HeadNode.Next, TailNode);
	}
}