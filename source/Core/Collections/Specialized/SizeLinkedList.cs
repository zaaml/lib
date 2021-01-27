// <copyright file="SizeLinkedList.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Diagnostics;

namespace Zaaml.Core.Collections.Specialized
{
	internal partial class SizeLinkedList
	{
		private NodeCursor _cursor;

		public SizeLinkedList()
		{
			Init();
		}

		private NodeCursor Cursor
		{
			get
			{
				if (_cursor.IsValid)
					return _cursor;

				return _cursor = new NodeCursor(this);
			}
			set => _cursor = value;
		}

		private Node Head { get; set; }

		private long StructureVersion { get; set; }

		private Node Tail { get; set; }

		private void AddSizeImpl(double size)
		{
			AddSizeRange(1, size);
		}

		private void AddSizeRangeImpl(long count, double size)
		{
			var cursor = Cursor.Prev;

			if (ReferenceEquals(Head.Next, Tail) && Tail.Count == 0 && (Head.Count == 0 || Head.EqualSize(size)))
			{
				Head.Size = size;
				Head.Count += count;
			}
			else if (Tail.Count == 0 || Tail.EqualSize(size))
			{
				var prev = Tail.Prev;

				if (prev.EqualSize(size)) 
					prev.Count += count;
				else
				{
					Tail.Size = size;
					Tail.Count += count;
				}
			}
			else
			{
				var node = GetNode();

				node.Prev = Tail;
				Tail.Next = node;
				Tail = node;

				node.Size = size;
				node.Count = count;
			}

			Size += size * count;
			Count += count;

			StructureVersion++;

			Cursor = cursor.UpdateStructureVersion();
		}

		private void ClearImpl()
		{
			Init();
		}

		private Node GetNode()
		{
			return new Node();
		}

		private void ReleaseNode(Node node)
		{
		}

		private double GetSizeImpl(long index)
		{
			return NavigateToIndex(index).Node.Size;
		}

		private void Init()
		{
			Head = new Node();
			Tail = new Node();

			Head.Next = Tail;
			Tail.Prev = Head;

			Count = 0;
			Size = 0;

			StructureVersion++;

			Cursor = new NodeCursor(this);
		}

		private void InsertLeft(Node node, long count, double size)
		{
			var prev = node.Prev;
			var left = GetNode();

			left.Next = node;
			node.Prev = left;

			if (prev != null)
			{
				left.Prev = prev;
				prev.Next = left;
			}
			else
			{
				Debug.Assert(ReferenceEquals(Head, node));

				left.Prev = null;

				Head = left;
			}

			left.Count = count;
			left.Size = size;
		}

		private void InsertMid(ref NodeCursor cursor, long count, double size)
		{
			var node = cursor.Node;
			var localIndex = cursor.LocalIndex;
			var next = node.Next;

			var left = node;
			var mid = GetNode();
			var right = GetNode();

			right.Size = node.Size;
			right.Count = node.Count - localIndex;
			left.Count = localIndex;

			mid.Count = count;
			mid.Size = size;

			left.Next = mid;

			mid.Prev = left;
			mid.Next = right;

			right.Prev = mid;

			if (ReferenceEquals(node, Tail))
				Tail = right;
			else
			{
				right.Next = next;
				next.Prev = right;
			}
		}

		private void InsertRight(Node node, long count, double size)
		{
			var next = node.Next;
			var right = GetNode();

			right.Prev = node;
			node.Next = right;

			if (next != null)
			{
				next.Prev = right;
				right.Next = next;
			}
			else
			{
				Debug.Assert(ReferenceEquals(Tail, node));

				right.Next = null;

				Tail = right;
			}

			right.Count = count;
			right.Size = size;
		}

		private void InsertSizeImpl(long index, double size)
		{
			InsertSizeRangeImpl(index, 1, size);
		}

		private void InsertSizeRangeImpl(long index, int count, double size)
		{
			if (index == Count)
			{
				AddSizeRangeImpl(count, size);

				return;
			}

			var cursor = NavigateToIndex(index);
			var prevCursor = cursor.Prev;
			var node = cursor.Node;

			if (node.EqualSize(size))
				node.Count += count;
			else
			{
				var localIndex = cursor.LocalIndex;

				if (localIndex == 0)
					InsertLeft(node, count, size);
				else if (localIndex + 1 == node.Count)
					InsertRight(node, count, size);
				else
					InsertMid(ref cursor, count, size);
			}

			Count += count;
			Size += count * size;

			StructureVersion++;

			Cursor = prevCursor.UpdateStructureVersion();
		}

		private NodeCursor NavigateToIndex(long index)
		{
			return Cursor = Cursor.NavigateToIndex(index);
		}

		private NodeCursor NavigateToOffset(double offset)
		{
			return Cursor = Cursor.NavigateToOffset(offset);
		}

		private void RemoveSizeAtImpl(long index)
		{
			RemoveSizeRangeImpl(index, 1);
		}

		private void RemoveSizeRangeImpl(long index, long count)
		{
			if (index == 0 && count == Count)
			{
				ClearImpl();

				return;
			}

			var cursor = NavigateToIndex(index);
			var localIndex = cursor.LocalIndex;
			var node = cursor.Node;
			var head = node;

			if (localIndex > 0)
			{
				if (localIndex + count < node.Count)
				{
					node.Count -= count;
					Count -= count;
					Size -= node.Size * count;

					StructureVersion++;

					return;
				}

				var restCount = node.Count - localIndex;

				Count -= restCount;
				Size -= restCount * node.Size;

				node.Count = localIndex;

				count -= restCount;

				node = node.Next;
			}

			while (count > 0)
			{
				if (count > node.Count)
				{
					Count -= node.Count;
					Size -= node.Size;

					count -= node.Count;

					node = node.Next;
				}
				else
				{
					node.Count -= count;
					Count -= count;
					Size -= node.Size * count;

					count = 0;
				}
			}

			if (node == null)
			{
				Tail = GetNode();

				head.Next = Tail;
				Tail.Prev = head;
			}
			else
				RemoveIfEmpty(node);

			RemoveIfEmpty(head);
			TryMergeWithNext(head);

			StructureVersion++;
		}

		private double FindOffsetImpl(long index)
		{
			VerifyIndex(index);

			var cursor = NavigateToIndex(index);

			return cursor.NodeSizeOffset + cursor.Offset;
		}

		private void SetSizeImpl(long index, double size)
		{
			var cursor = NavigateToIndex(index);
			var node = cursor.Node;

			if (node.EqualSize(size))
				return;

			var prev = node.Prev;
			var next = node.Next;
			var prevCursor = cursor.Prev;
			var localIndex = cursor.LocalIndex;
			var nodeSize = node.Size;

			if (localIndex == 0)
			{
				node.Count--;

				if (prev?.EqualSize(size) == true)
					prev.Count++;
				else if (node.Count == 0)
				{
					if (next?.EqualSize(size) == true)
						next.Count++;
					else
					{
						node.Count = 1;
						node.Size = size;
					}
				}
				else
					InsertLeft(node, 1, size);
			}
			else if (localIndex == node.Count - 1)
			{
				node.Count--;

				if (next?.EqualSize(size) == true)
					next.Count++;
				else if (node.Count == 0)
				{
					if (prev?.EqualSize(size) == true)
						prev.Count++;
					else
					{
						node.Count = 1;
						node.Size = size;
					}
				}
				else
					InsertRight(node, 1, size);
			}
			else
			{
				node.Count--;

				InsertMid(ref cursor, 1, size);
			}

			Size -= nodeSize;
			Size += size;

			RemoveIfEmpty(node);

			StructureVersion++;

			Cursor = prevCursor.UpdateStructureVersion();
		}

		private void RemoveIfEmpty(Node node)
		{
			if (node.Count != 0) 
				return;

			if (ReferenceEquals(node, Tail))
				node.Size = 0;
			else
				RemoveNode(node);
		}

		private void TryMergeWithNext(Node node)
		{
			if (node.Next != null && EqualSize(node.Size, node.Next.Size))
				MergeNodes(node, node.Next);
		}

		private void RemoveNode(Node node)
		{
			var prev = node.Prev;
			var next = node.Next;

			if (prev != null)
				prev.Next = next;
			else
			{
				Debug.Assert(ReferenceEquals(node, Head));

				Head = next;
			}

			next.Prev = prev;

			ReleaseNode(node);

			if (prev != null)
				TryMergeWithNext(prev);
		}

		private void MergeNodes(Node prev, Node next)
		{
			prev.Count += next.Count;
			next.Count = 0;
			next.Size = 0;

			if (ReferenceEquals(next, Tail) == false)
				RemoveNode(next);
		}

		// ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
		private void VerifyIndex(long index, bool insert = false)
		{
			if (index == 0 && Count == 0)
				return;

			if (index < 0 || index > Count || index == Count && insert == false)
				throw new IndexOutOfRangeException(nameof(index));
		}

		private void VerifyRange(long index, long count)
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

		[Conditional("DEBUG")]
		private void VerifyStructure()
		{
			if (Head == null || Tail == null)
				throw new InvalidOperationException();

			if (Tail.Count == 0 && Tail.Size.Equals(0) == false)
				throw new InvalidOperationException();

			if (ReferenceEquals(Head, Tail))
				throw new InvalidOperationException();

			if (Head.Prev != null || Tail.Next != null)
				throw new InvalidOperationException();

			if (ReferenceEquals(Head.Next, Tail))
			{
				if (ReferenceEquals(Tail.Prev, Head) == false)
					throw new InvalidOperationException();

				if (EqualSize(Head.Size, Tail.Size) && Tail.Count > 0 && Head.Count > 0)
					throw new InvalidOperationException();

				return;
			}

			var current = Head;
			var currentSize = current.Size;

			while (current != null)
			{
				if (current.Count == 0 && ReferenceEquals(current, Tail) == false)
					throw new InvalidOperationException();

				var next = current.Next;

				if (next != null)
				{
					if (EqualSize(currentSize, next.Size))
					{
						if (!ReferenceEquals(next, Tail) || next.Count != 0)
							throw new InvalidOperationException();
					}

					currentSize = next.Size;

					if (ReferenceEquals(next.Prev, current) == false)
						throw new InvalidOperationException();
				}

				current = next;
			}
		}
	}
}