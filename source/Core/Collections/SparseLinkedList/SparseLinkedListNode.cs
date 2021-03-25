// <copyright file="SparseLinkedListNode.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.Core.Collections
{
	[PublicAPI]
	internal readonly struct SparseLinkedListNode<T>
	{
		public SparseLinkedListNode(SparseLinkedListBase<T>.NodeBase node, SparseLinkedListBase<T> list)
		{
			_node = node;
			_list = list;
			_version = list.StructureVersion;
		}

		private SparseLinkedListNode(ulong version)
		{
			_node = null;
			_list = null;
			_version = version;
		}

		private readonly SparseLinkedListBase<T> _list;
		private readonly ulong _version;
		private readonly SparseLinkedListBase<T>.NodeBase _node;

		private void Verify()
		{
			if (IsEmpty)
				throw new InvalidOperationException("Node is Empty.");

			if (_version != _list.StructureVersion)
				throw new InvalidOperationException("List has changed.");
		}

		public static SparseLinkedListNode<T> Empty => new SparseLinkedListNode<T>(0);

		[PublicAPI] public bool IsEmpty => _version == ulong.MaxValue;

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
		public bool IsVoid
		{
			get
			{
				Verify();

				return _node is SparseLinkedListBase<T>.VoidNode;
			}
		}

		[PublicAPI]
		public bool IsRealized
		{
			get
			{
				Verify();

				return _node is SparseLinkedListBase<T>.RealizedNode;
			}
		}

		[PublicAPI]
		public long Count
		{
			get
			{
				Verify();

				return _node.Size;
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