// <copyright file="SparseLinkedList.Node.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.Core.Collections
{
	internal partial class SparseLinkedList<T>
	{
		#region  Nested Types

		internal abstract class Node
		{
			#region Properties

			public int Count { get; set; }

			public int Index { get; set; }

			public abstract T this[int index] { get; set; }

			public SparseLinkedList<T> List { get; set; }

			public Node Next { get; set; }

			public Node Prev { get; set; }

			#endregion

			#region  Methods

			public bool Contains(int index)
			{
				return index >= Index && index < Index + Count;
			}

			public abstract T GetLocalItem(int index);

			public override string ToString()
			{
				var range = Count == 0 ? $"[{Index}]" : $"[{Index}..{Index + Count - 1}]";

				return this is GapNode ? $"gap{range}" : $"real{range}";
			}

			#endregion
		}

		internal sealed class GapNode : Node
		{
			#region Properties

			public override T this[int index]
			{
				get
				{
#if DEBUG
					if (Contains(index) == false)
						throw new IndexOutOfRangeException();
#endif
					return default(T);
				}
				set { throw new InvalidOperationException(); }
			}

			#endregion

			#region  Methods

			public override T GetLocalItem(int index)
			{
#if DEBUG
				if (Contains(Index + index) == false)
					throw new IndexOutOfRangeException();
#endif

				return default(T);
			}

			#endregion
		}

		internal sealed class RealizedNode : Node
		{
			#region Ctors

			public RealizedNode(T[] items)
			{
				Items = items;
			}

			#endregion

			#region Properties

			public override T this[int index]
			{
				get
				{
#if DEBUG
					if (Contains(index) == false)
						throw new IndexOutOfRangeException();
#endif

					return Items[index - Index];
				}
				set
				{
#if DEBUG
					if (Contains(index) == false)
						throw new IndexOutOfRangeException();
#endif
					Items[index - Index] = value;
				}
			}

			public T[] Items { get; }

			#endregion

			#region  Methods

			public override T GetLocalItem(int index)
			{
#if DEBUG
				if (Contains(Index + index) == false)
					throw new IndexOutOfRangeException();
#endif

				return Items[index];
			}

			#endregion
		}

		#endregion
	}

	[PublicAPI]
	internal struct SparseLinkedListNode<T>
	{
		public SparseLinkedListNode(SparseLinkedList<T>.Node node, SparseLinkedList<T> list)
		{
			_node = node;
			_list = list;
			_version = list.Version;
		}

		private SparseLinkedListNode(int version)
		{
			_node = null;
			_list = null;
			_version = version;
		}

		private readonly SparseLinkedList<T> _list;
		private readonly int _version;
		private readonly SparseLinkedList<T>.Node _node;

		private void Verify()
		{
			if (IsEmpty)
				throw new InvalidOperationException("Node is Empty.");

			if (_version != _list.Version)
				throw new InvalidOperationException("List has changed.");
		}

		public static readonly SparseLinkedListNode<T> Empty = new SparseLinkedListNode<T>(-1);

		[PublicAPI]
		public bool IsEmpty => _version == -1;

		[PublicAPI]
		public SparseLinkedListNode<T> Prev
		{
			get
			{
				Verify();

				return _node.Prev != null ? new SparseLinkedListNode<T>(_node.Prev, _list) : Empty;
			}
		}

		[PublicAPI]
		public SparseLinkedListNode<T> Next
		{
			get
			{
				Verify();

				return _node.Next != null ? new SparseLinkedListNode<T>(_node.Next, _list) : Empty;
			}
		}

		[PublicAPI]
		public bool IsGap
		{
			get
			{
				Verify();

				return _node is SparseLinkedList<T>.GapNode;
			}
		}

		[PublicAPI]
		public bool IsRealized
		{
			get
			{
				Verify();

				return _node is SparseLinkedList<T>.RealizedNode;
			}
		}

		[PublicAPI]
		public int Index
		{
			get
			{
				Verify();

				return _node.Index;
			}
		}

		[PublicAPI]
		public int Count
		{
			get
			{
				Verify();

				return _node.Count;
			}
		}

		[PublicAPI]
		public T this[int index]
		{
			get
			{
				Verify();

				return _node.GetLocalItem(index);
			}
		}
	}
}