// <copyright file="WeakLinkedList.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;

namespace Zaaml.Core.Weak.Collections
{
	internal class WeakLinkedList<T> : IEnumerable<T> where T : class
	{
		private readonly WeakLinkedListNodePool<T> _nodePool;
		private int _gcCount;
		private WeakLinkedNode<T> _head;
		private WeakLinkedNode<T> _tail;

		public WeakLinkedList()
		{
		}

		internal WeakLinkedList(WeakLinkedListNodePool<T> nodePool)
		{
			_nodePool = nodePool;
		}

		public bool IsEmpty => _head == null;

		private bool IsGCOccurred => GC.CollectionCount(0) != _gcCount;

		public void Add(T item)
		{
			EnsureClean();

			WeakLinkedNode.Insert(GetNode(item), ref _head, ref _tail);

			OnCollectionChanged();

			EnsureGC();
		}

		internal void AddNode(WeakLinkedNode<T> node)
		{
			EnsureClean();

			WeakLinkedNode.Insert(node, ref _head, ref _tail);

			OnCollectionChanged();

			EnsureGC();
		}

		public bool Cleanup()
		{
			return EnsureClean();
		}

		public void Cleanup(Func<T, bool> predicate)
		{
			if (_head == null)
				return;

			WeakLinkedNode.Clean(ref _head, out _tail, predicate);

			OnCollectionChanged();
		}

		public void Clear()
		{
			if (_head == null)
				return;

			while (_head != null)
			{
				var head = _head;

				_head = _head.Next;

				head.Dispose();
			}

			_tail = null;
			_head = null;

			OnCollectionChanged();

			UpdateGCCounter();
		}

		internal WeakLinkedNode<T> DetachNodes()
		{
			EnsureClean();

			var head = _head;

			_head = null;
			_tail = null;

			return head;
		}

		private bool EnsureClean()
		{
			if (_head == null || IsGCOccurred == false)
				return false;

			WeakLinkedNode.Clean(ref _head, out _tail);

			OnCollectionChanged();

			UpdateGCCounter();

			return true;
		}

		private void EnsureGC()
		{
			if (IsGCOccurred)
				UpdateGCCounter();
		}

		private WeakLinkedNode<T> GetNode(T item)
		{
			if (_nodePool == null)
				return WeakLinkedNode.Create(item);

			var node = _nodePool.RentNode();

			node.Mount(item);

			return node;
		}

		protected virtual void OnCollectionChanged()
		{
		}

		public void Remove(T item)
		{
			EnsureClean();

			WeakLinkedNode.Remove(ref _head, ref _tail, item);

			OnCollectionChanged();

			EnsureGC();
		}

		public void Remove(WeakLinkedNode<T> node)
		{
			EnsureClean();

			WeakLinkedNode.RemoveNode(ref _head, ref _tail, node);

			OnCollectionChanged();

			EnsureGC();
		}

		private void UpdateGCCounter()
		{
			_gcCount = GC.CollectionCount(0);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public IEnumerator<T> GetEnumerator()
		{
			if (_head == null)
				yield break;

			EnsureClean();

			if (_head == null)
				yield break;

			var current = _head;

			while (current != null)
			{
				var currentTarget = current.Target;

				if (currentTarget != null)
					yield return currentTarget;

				current = current.Next;
			}
		}
	}
}