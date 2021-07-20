// <copyright file="LinkedListUtils.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;

namespace Zaaml.Core.Utils
{
	internal static class LinkedListUtils
	{
		public static void AddFirst<TNode>(ILinkedList<TNode> linkedList, TNode node)
			where TNode : class, ILinkedListNode<TNode>
		{
			if (node.Prev != null || node.Next != null)
				throw new InvalidOperationException();

			try
			{
				if (linkedList.Head == null && linkedList.Tail == null)
				{
					linkedList.Head = linkedList.Tail = node;

					return;
				}

				if (linkedList.Head == null || linkedList.Tail == null)
					throw new InvalidOperationException();

				node.Next = linkedList.Head;
				linkedList.Head.Prev = node;

				linkedList.Head = node;
			}
			finally
			{
				linkedList.Version++;
			}
		}

		public static void AddLast<TNode>(ILinkedList<TNode> linkedList, TNode node)
			where TNode : class, ILinkedListNode<TNode>
		{
			if (node.Prev != null || node.Next != null)
				throw new InvalidOperationException();

			try
			{
				if (linkedList.Head == null && linkedList.Tail == null)
				{
					linkedList.Head = linkedList.Tail = node;

					return;
				}

				if (linkedList.Head == null || linkedList.Tail == null)
					throw new InvalidOperationException();

				node.Prev = linkedList.Tail;
				linkedList.Tail.Next = node;

				linkedList.Tail = node;
			}
			finally
			{
				linkedList.Version++;
			}
		}

		public static void Clear<TNode>(ILinkedList<TNode> linkedList)
			where TNode : class, ILinkedListNode<TNode>
		{
			var current = linkedList.Head;

			while (current != null)
			{
				var next = current.Next;

				current.Next = null;
				current.Prev = null;

				current = next;
			}

			linkedList.Head = null;
			linkedList.Tail = null;
			linkedList.Version++;
		}

		public static void Clear<TNode>(ILinkedList<TNode> linkedList, Action<TNode> clearAction)
			where TNode : class, ILinkedListNode<TNode>
		{
			var current = linkedList.Head;

			while (current != null)
			{
				var next = current.Next;

				current.Next = null;
				current.Prev = null;

				clearAction(current);

				current = next;
			}

			linkedList.Head = null;
			linkedList.Tail = null;
			linkedList.Version++;
		}

		public static TNode Find<TNode>(ILinkedList<TNode> linkedList, Func<TNode, bool> predicate)
			where TNode : class, ILinkedListNode<TNode>
		{
			var current = linkedList.Head;

			while (current != null)
			{
				var next = current.Next;

				if (predicate(current))
					return current;

				current = next;
			}

			return null;
		}

		public static LinkedListEnumerator<TNode> GetEnumerator<TNode>(ILinkedList<TNode> linkedList)
			where TNode : class, ILinkedListNode<TNode>
		{
			return new(linkedList);
		}

		public static void Remove<TNode>(ILinkedList<TNode> linkedList, TNode node)
			where TNode : class, ILinkedListNode<TNode>
		{
			var prev = node.Prev;
			var next = node.Next;

			if (ReferenceEquals(linkedList.Head, node))
			{
				linkedList.Head = next;

				if (next == null)
					linkedList.Tail = linkedList.Head;
				else
					next.Prev = null;
			}
			else if (ReferenceEquals(linkedList.Tail, node))
			{
				linkedList.Tail = prev;

				if (prev == null)
					linkedList.Head = linkedList.Tail;
				else
					prev.Next = null;
			}
			else
			{
				prev.Next = next;
				next.Prev = prev;
			}

			node.Next = null;
			node.Prev = null;
			linkedList.Version++;
		}
	}

	internal struct LinkedListEnumerator<TNode> : IEnumerator<TNode>
		where TNode : class, ILinkedListNode<TNode>

	{
		private const int Initial = 0;
		private const int Started = 1;
		private const int Finished = 2;
		private const int Disposed = 3;

		private ILinkedList<TNode> _linkedList;
		private TNode _current;
		private int _status;
		private int _version;

		public LinkedListEnumerator(ILinkedList<TNode> linkedList)
		{
			_linkedList = linkedList;
			_version = linkedList.Version;
			_current = default;
			_status = Initial;
		}

		private void Verify(bool starting)
		{
			if (starting == false && _status == Initial)
				throw new InvalidOperationException("Enumeration not started.");

			if (_status == Finished)
				throw new InvalidOperationException("Enumeration finished.");

			if (_status == Disposed)
				throw new InvalidOperationException("Enumerator disposed");

			if (_version != _linkedList.Version)
				throw new InvalidOperationException("List changed");
		}

		public bool MoveNext()
		{
			Verify(true);

			if (_status == Initial)
			{
				_status = Started;

				_current = _linkedList.Head;

				if (_current == null)
				{
					_status = Finished;

					return false;
				}

				return true;
			}

			_current = _current.Next;

			if (_current == null)
			{
				_status = Finished;

				return false;
			}

			return true;
		}

		public void Reset()
		{
			if (_status == Disposed)
				throw new InvalidOperationException("Enumerator disposed");

			_status = Initial;
			_version = _linkedList.Version;
			_current = null;
		}

		public TNode Current
		{
			get
			{
				Verify(false);

				return _current;
			}
		}

		object IEnumerator.Current => Current;

		public void Dispose()
		{
			_status = Disposed;
			_current = null;
			_linkedList = null;
		}
	}

	internal interface ILinkedListNode<TNode>
		where TNode : class, ILinkedListNode<TNode>
	{
		TNode Next { get; set; }

		TNode Prev { get; set; }
	}

	internal interface ILinkedList<TNode>
		where TNode : class, ILinkedListNode<TNode>
	{
		TNode Head { get; set; }

		TNode Tail { get; set; }

		int Version { get; set; }
	}
}