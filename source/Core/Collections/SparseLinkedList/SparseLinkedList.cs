// <copyright file="SparseLinkedList.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace Zaaml.Core.Collections
{
	internal partial class SparseLinkedList<T>
	{
		#region Static Fields and Constants

#if TEST
		private const int DefaultCapacity = 4;
#else
		private const int DefaultCapacity = 16;
#endif

		#endregion

		#region Fields

		private bool _locked;

		#endregion

		#region Ctors

		public SparseLinkedList() : this(0, DefaultCapacity)
		{
		}

		private SparseLinkedList(int count, int nodeCapacity)
		{
			NodeCapacity = nodeCapacity;

			HeadNode = CreateGapNode();
			TailNode = CreateGapNode();

			Count = count;

			InitHeadTail();
		}

		#endregion

		#region Properties

		private Node Current { get; set; }

		[UsedImplicitly]
		internal string Dump
		{
			get
			{
				var sb = new StringBuilder();
				var node = HeadNode;

				while (node != null)
				{
					if (ReferenceEquals(node, HeadNode) == false)
						sb.Append("  ");

					sb.Append(node);

					node = node.Next;
				}

				return sb.ToString();
			}
		}

		public SparseLinkedListNode<T> Head => new SparseLinkedListNode<T>(HeadNode, this);

		private Node HeadNode { get; }

		protected int NodeCapacity { get; }

		public SparseLinkedListNode<T> Tail => new SparseLinkedListNode<T>(TailNode, this);

		private Node TailNode { get; }

		private T[] TempItems { get; set; }

		internal int Version { get; private set; }

		#endregion

		#region  Methods

		private static void AdvanceIndices(Node node)
		{
			while (true)
			{
				var next = node.Next;

				if (next == null)
					return;

				next.Index = node.Index + node.Count;

				node = next;
			}
		}

		private T[] AllocateItems()
		{
			if (TempItems == null)
				return AllocateItemsCore();

			var items = TempItems;

			TempItems = null;

			return items;
		}

		protected virtual T[] AllocateItemsCore()
		{
			return new T[NodeCapacity];
		}

		private void ClearImpl()
		{
			var current = HeadNode.Next;

			while (current != null && ReferenceEquals(current, TailNode) == false)
			{
				var next = current.Next;

				RemoveNode(current);

				current = next;
			}

			Count = 0;

			InitHeadTail();
		}

		private void CopyToImpl(T[] array, int arrayIndex)
		{
			if (array.Length - arrayIndex < Count)
				throw new InvalidOperationException("Insufficient array length");

			var index = arrayIndex;
			var current = HeadNode;

			while (current != null)
			{
				if (current is RealizedNode realizedNode)
					Array.Copy(realizedNode.Items, 0, array, index, realizedNode.Count);

				index += current.Count;

				current = current.Next;
			}
		}

		private void CopyToImpl(Array array, int arrayIndex)
		{
			if (array.Length - arrayIndex < Count)
				throw new InvalidOperationException("Insufficient array length");

			var index = arrayIndex;
			var current = HeadNode;

			while (current != null)
			{
				if (current is RealizedNode realizedNode)
					Array.Copy(realizedNode.Items, 0, array, index, realizedNode.Count);

				index += current.Count;

				current = current.Next;
			}
		}

		private GapNode CreateGapNode()
		{
			var node = CreateGapNodeCore();

			MountNode(node);

			node.Next = null;
			node.Prev = null;

			return node;
		}

		internal virtual GapNode CreateGapNodeCore()
		{
			return new GapNode();
		}

		private RealizedNode CreateRealizedNode()
		{
			var node = CreateRealizedNodeCore();

			MountNode(node);

			if (node.Items.Length != NodeCapacity)
				throw new InvalidOperationException("Node capacity");

			node.Next = null;
			node.Prev = null;

			return node;
		}

		internal virtual RealizedNode CreateRealizedNodeCore()
		{
			return new RealizedNode(AllocateItems());
		}

		private void DeallocateItems(T[] items)
		{
			Array.Clear(items, 0, items.Length);

			if (TempItems != null)
				return;

			TempItems = items;
		}

		private RealizedNode EnsureRealizedNode(Node node, int index, bool insert)
		{
			if (node is RealizedNode realizedNode)
			{
				Current = realizedNode;

				return realizedNode;
			}

			if (node.Prev is RealizedNode prevRealizedNode && index < prevRealizedNode.Index + NodeCapacity)
			{
				var gapNode = (GapNode) node;
				var extraSize = index - (prevRealizedNode.Index + prevRealizedNode.Count) + 1;

				prevRealizedNode.Count += extraSize;

				if (insert == false)
					gapNode.Count -= extraSize;

				gapNode.Index += extraSize;

				RemoveEmptyGapNode(gapNode);

				Current = prevRealizedNode;

				return prevRealizedNode;
			}

			realizedNode = RealizeNode((GapNode) node, index, insert);

			Current = realizedNode;

			return realizedNode;
		}

		private int FindImpl(T item)
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

		public SparseLinkedListNode<T> FindNode(int index)
		{
			var node = FindNodeImpl(index);

			return node != null ? new SparseLinkedListNode<T>(node, this) : SparseLinkedListNode<T>.Empty;
		}

		private Node FindNodeImpl(int index)
		{
			if (Count == 0)
				return HeadNode;

			if (Current.Contains(index))
				return Current;

			var node = Current;

			if (index >= node.Index)
			{
				while (node != null && node.Contains(index) == false)
					node = node.Next;
			}
			else
			{
				while (node != null && node.Contains(index) == false)
					node = node.Prev;
			}

			return node == null ? null : Current = node;
		}

		private T GetItem(int index)
		{
			var node = FindNodeImpl(index);

			return node[index];
		}

		private void InitHeadTail()
		{
			HeadNode.Next = TailNode;
			TailNode.Prev = HeadNode;

			HeadNode.Count = Count;
			TailNode.Index = Count;
			TailNode.Count = 0;

			Current = HeadNode;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void Lock()
		{
			if (_locked)
				throw new InvalidOperationException();

			_locked = true;
		}

		private void MountNode(Node node)
		{
			if (node.List != null)
				throw new InvalidOperationException();

			node.List = this;
		}

		private RealizedNode RealizeNode(GapNode gapNode, int index, bool insert)
		{
			var alignedIndex = index / NodeCapacity * NodeCapacity;
			var prevNode = gapNode.Prev;
			var nextNode = gapNode.Next;
			var realizedNode = CreateRealizedNode();

			if (ReferenceEquals(HeadNode.Next, TailNode) == false && gapNode.Contains(alignedIndex) == false)
			{
				Debug.Assert(prevNode is RealizedNode);

				alignedIndex = prevNode.Next.Index;
			}

			realizedNode.Index = alignedIndex;
			realizedNode.Count = index - alignedIndex + 1;

			if (ReferenceEquals(HeadNode.Next, TailNode))
			{
				Debug.Assert(TailNode.Count == 0);

				HeadNode.Next = realizedNode;
				TailNode.Prev = realizedNode;

				realizedNode.Prev = HeadNode;
				realizedNode.Next = TailNode;

				var extraCount = HeadNode.Count - realizedNode.Index;

				HeadNode.Count = realizedNode.Index;
				TailNode.Index = realizedNode.Index + realizedNode.Count;

				if (index < Count)
				{
					if (insert == false) 
						extraCount -= realizedNode.Count;

					TailNode.Count += extraCount;
				}
				else
					TailNode.Count = 0;

				return realizedNode;
			}

			if (prevNode != null && prevNode.Next.Index == alignedIndex)
			{
				prevNode.Next = realizedNode;
				realizedNode.Prev = prevNode;

				if (realizedNode.Count == gapNode.Count && ReferenceEquals(gapNode, TailNode) == false)
				{
					realizedNode.Next = nextNode;
					nextNode.Prev = realizedNode;

					ReleaseNode(gapNode);
				}
				else
				{
					realizedNode.Next = gapNode;
					gapNode.Prev = realizedNode;

					gapNode.Index += realizedNode.Count;

					if (insert == false)
						gapNode.Count -= realizedNode.Count;
				}
			}
			else if (ReferenceEquals(gapNode, HeadNode))
			{
				var nextGapCount = HeadNode.Count - realizedNode.Count - alignedIndex;

				if (nextGapCount == 0)
				{
					realizedNode.Next = HeadNode.Next;
					HeadNode.Next.Prev = realizedNode;

					realizedNode.Prev = HeadNode;
				}
				else
				{
					var nextGapNode = CreateGapNode();

					nextGapNode.Index = realizedNode.Index + realizedNode.Count;
					nextGapNode.Count = nextGapCount;

					realizedNode.Next = nextGapNode;
					nextGapNode.Prev = realizedNode;

					nextGapNode.Next = nextNode;
					nextNode.Prev = nextGapNode;

					realizedNode.Prev = HeadNode;
				}

				HeadNode.Next = realizedNode;
				HeadNode.Count = alignedIndex;
			}
			else
			{
				// ReSharper disable once PossibleNullReferenceException
				var prevGapCount = alignedIndex - prevNode.Next.Index;
				var prevGapNode = CreateGapNode();

				prevGapNode.Index = gapNode.Index;
				prevGapNode.Count = prevGapCount;
				prevGapNode.Prev = prevNode;
				prevGapNode.Next = realizedNode;

				prevNode.Next = prevGapNode;
				realizedNode.Prev = prevGapNode;
				realizedNode.Next = gapNode;
				gapNode.Prev = realizedNode;

				gapNode.Index = realizedNode.Index + realizedNode.Count;

				if (insert == false)
					gapNode.Count -= prevGapNode.Count + realizedNode.Count;
			}

			RemoveEmptyGapNode(gapNode);

			return realizedNode;
		}

		private void ReleaseNode(Node node)
		{
			if (ReferenceEquals(Current, node))
				Current = node.Prev ?? node.Next ?? HeadNode;

			node.Next = null;
			node.Prev = null;
			node.List = null;

			ReleaseNodeCore(node);
		}

		internal virtual void ReleaseNodeCore(Node node)
		{
			if (node is RealizedNode realizedNode)
				DeallocateItems(realizedNode.Items);
		}

		private void RemoveEmptyGapNode(GapNode gapNode)
		{
			if (gapNode.Count == 0 && ReferenceEquals(gapNode, HeadNode) == false && ReferenceEquals(gapNode, TailNode) == false)
				RemoveNode(gapNode);
		}

		private void RemoveNode(Node node)
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

		private void SetItem(int index, T value)
		{
			var realizedNode = EnsureRealizedNode(FindNodeImpl(index), index, false);

			realizedNode[index] = value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void Unlock()
		{
			if (_locked == false)
				throw new InvalidOperationException();

			_locked = false;
		}

		[Conditional("TEST")]
		private void VerifyStructure()
		{
			var current = HeadNode;

			while (current != null)
			{
				var next = current.Next;

				if (next != null)
				{
					if (current.Index + current.Count != next.Index)
						throw new InvalidOperationException();

					if (ReferenceEquals(next.Prev, current) == false)
						throw new InvalidOperationException();

					if (current is GapNode && current.Prev is GapNode)
					{
						if (ReferenceEquals(current, TailNode) == false)
							throw new InvalidOperationException();
					}
				}

				current = next;
			}

			if (TailNode.Index + TailNode.Count != Count)
				throw new InvalidOperationException();
		}

		// ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
		private void VerifyIndex(int index, bool insert = false)
		{
			if (index == 0 && Count == 0)
				return;

			if (index < 0 || index > Count || index == Count && insert == false)
				throw new IndexOutOfRangeException(nameof(index));
		}

		private void VerifyRange(int index, int count)
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

		#endregion

		#region Interface Implementations

		#region ICollection<T>

		[PublicAPI]
		public int Count { get; private set; }

		#endregion

		#endregion
	}
}