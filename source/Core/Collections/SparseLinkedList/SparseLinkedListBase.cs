// <copyright file="SparseLinkedListBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Zaaml.Core.Collections
{
	internal partial class SparseLinkedListBase<T>
	{
#if TEST
		private const int DefaultCapacity = 4;
#else
		protected const int DefaultCapacity = 16;
#endif

		public SparseLinkedListBase() : this(0, new SparseLinkedListManager<T>(new SparseMemoryManager<T>(DefaultCapacity)))
		{
		}

		public SparseLinkedListBase(int count) : this(count, new SparseLinkedListManager<T>(new SparseMemoryManager<T>(DefaultCapacity)))
		{
		}

		protected SparseLinkedListBase(int count, SparseLinkedListManager<T> manager)
		{
			Manager = manager;

			HeadNode = CreateGapNode();
			TailNode = CreateGapNode();

			Count = count;

			InitHeadTail();
		}

		internal int Version { get; private protected set; }

		protected Node HeadNode { get; }

		protected int NodeCapacity => Manager.SparseMemoryManager.NodeCapacity;

		private SparseLinkedListManager<T> Manager { get; }

		protected Node TailNode { get; }

		private Node Current { get; set; }

		[PublicAPI] public int Count { get; private set; }

		private void InitHeadTail()
		{
			HeadNode.Next = TailNode;
			TailNode.Prev = HeadNode;

			HeadNode.Count = Count;
			TailNode.Index = Count;
			TailNode.Count = 0;

			Current = HeadNode;
		}

		private RealizedNode CreateRealizedNode()
		{
			var node = Manager.GetRealizedNode();

			MountNode(node);

			return node;
		}

		[Conditional("TEST")]
		private protected void VerifyStructure()
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

		private void DeallocateItems(SparseMemorySpan<T> sparseMemorySpan)
		{
			sparseMemorySpan.Release();
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

		private protected T GetItemImpl(int index)
		{
			var node = FindNodeImpl(index);

			return node[index];
		}

		private SparseMemorySpan<T> AllocateItems()
		{
			return Manager.Allocate();
		}

		private protected void ClearImpl()
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

		private GapNode CreateGapNode()
		{
			var node = Manager.GetGapNode();

			MountNode(node);

			return node;
		}

		private void MountNode(Node node)
		{
			if (node.List != null)
				throw new InvalidOperationException();

			node.List = this;
			node.Next = null;
			node.Prev = null;
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

			Manager.ReleaseNode(node);
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

		private protected void SetItemImpl(int index, T value)
		{
			var realizedNode = EnsureRealizedNode(FindNodeImpl(index), index, false);

			realizedNode[index] = value;
		}

		#region Nested Types

		internal abstract class Node
		{
			public int Count { get; set; }

			public int Index { get; set; }

			public abstract T this[int index] { get; set; }

			public SparseLinkedListBase<T> List { get; set; }

			public Node Next { get; set; }

			public Node Prev { get; set; }

			public bool Contains(int index)
			{
				return index >= Index && index < Index + Count;
			}

			public abstract T GetLocalItem(int index);

			public virtual void Release()
			{
				Next = null;
				Prev = null;
				List = null;
				Count = 0;
				Index = 0;
			}

			public override string ToString()
			{
				var range = Count == 0 ? $"[{Index}]" : $"[{Index}..{Index + Count - 1}]";

				return this is GapNode ? $"gap{range}" : $"real{range}";
			}
		}

		internal sealed class GapNode : Node
		{
			public override T this[int index]
			{
				get
				{
#if DEBUG
					if (Contains(index) == false)
						throw new IndexOutOfRangeException();
#endif
					return default;
				}
				set { throw new InvalidOperationException(); }
			}

			public override T GetLocalItem(int index)
			{
#if DEBUG
				if (Contains(Index + index) == false)
					throw new IndexOutOfRangeException();
#endif

				return default;
			}
		}

		internal sealed class RealizedNode : Node
		{
			public override T this[int index]
			{
				get
				{
#if DEBUG
					if (Contains(index) == false)
						throw new IndexOutOfRangeException();
#endif

					return Span[index - Index];
				}
				set
				{
#if DEBUG
					if (Contains(index) == false)
						throw new IndexOutOfRangeException();
#endif
					Span[index - Index] = value;
				}
			}

			public Span<T> Span => SparseMemorySpan.Span;

			private SparseMemorySpan<T> SparseMemorySpan { get; set; } = SparseMemorySpan<T>.Empty;

			public override T GetLocalItem(int index)
			{
#if DEBUG
				if (Contains(Index + index) == false)
					throw new IndexOutOfRangeException();
#endif

				return Span[index];
			}

			public void Mount(SparseMemorySpan<T> sparseMemorySpan)
			{
				SparseMemorySpan = sparseMemorySpan;
			}

			public override void Release()
			{
				base.Release();

				SparseMemorySpan.Span.Clear();
				SparseMemorySpan.Release();
				SparseMemorySpan = SparseMemorySpan<T>.Empty;
			}
		}

		#endregion
	}

	internal class SparseLinkedListManager<T>
	{
		public SparseLinkedListManager(SparseMemoryManager<T> sparseMemoryManager)
		{
			SparseMemoryManager = sparseMemoryManager;
		}

		private Stack<SparseLinkedListBase<T>.GapNode> GapNodePool { get; } = new Stack<SparseLinkedListBase<T>.GapNode>();

		private Stack<SparseLinkedListBase<T>.RealizedNode> RealizedNodePool { get; } = new Stack<SparseLinkedListBase<T>.RealizedNode>();

		public SparseMemoryManager<T> SparseMemoryManager { get; }

		public SparseMemorySpan<T> Allocate()
		{
			return SparseMemoryManager.Allocate();
		}

		public SparseLinkedListBase<T>.GapNode GetGapNode()
		{
			return GapNodePool.Count > 0 ? GapNodePool.Pop() : new SparseLinkedListBase<T>.GapNode();
		}

		public SparseLinkedListBase<T>.RealizedNode GetRealizedNode()
		{
			var realizedNode = RealizedNodePool.Count > 0 ? RealizedNodePool.Pop() : new SparseLinkedListBase<T>.RealizedNode();

			realizedNode.Mount(SparseMemoryManager.Allocate());

			return realizedNode;
		}

		protected virtual void OnNodeReleased(SparseLinkedListBase<T>.Node node)
		{
		}

		protected virtual void OnNodeReleasing(SparseLinkedListBase<T>.Node node)
		{
		}

		public void ReleaseNode(SparseLinkedListBase<T>.Node node)
		{
			OnNodeReleasing(node);

			node.Release();

			if (node is SparseLinkedListBase<T>.GapNode gapNode)
				GapNodePool.Push(gapNode);
			else if (node is SparseLinkedListBase<T>.RealizedNode realizedNode)
				RealizedNodePool.Push(realizedNode);

			OnNodeReleased(node);
		}

		public void ReleaseRealizedNode(SparseLinkedListBase<T>.RealizedNode realizedNode)
		{
			RealizedNodePool.Push(realizedNode);
		}
	}
}