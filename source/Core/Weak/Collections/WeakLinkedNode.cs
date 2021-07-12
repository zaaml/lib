// <copyright file="WeakLinkedNode.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Zaaml.Core.Weak.Collections
{
	internal class WeakLinkedNode<T> where T : class
	{
		public WeakLinkedNode(T target)
		{
			Mount(target);
		}

		public WeakLinkedNode()
		{
		}

		internal WeakLinkedNode(WeakLinkedListNodePool<T> pool)
		{
			Pool = pool;
		}

		internal bool IsAlive => WeakReference.IsAlive;

		internal bool IsInPool { get; set; }

		public WeakLinkedNode<T> Next { get; internal set; }

		private WeakLinkedListNodePool<T> Pool { get; }

		public T Target => WeakReference.Target;

		private WeakReference<T> WeakReference { get; set; }

		internal static WeakLinkedNode<T> CleanImpl(WeakLinkedNode<T> head, out WeakLinkedNode<T> tail, Func<T, bool> predicate = null)
		{
			var aliveHead = head;

			while (aliveHead != null && (aliveHead.IsAlive == false || predicate != null && predicate(aliveHead.Target)))
			{
				var next = aliveHead.Next;

				aliveHead.Dispose();

				aliveHead = next;
			}

			if (aliveHead == null)
			{
				tail = null;

				return null;
			}

			var currentAlive = aliveHead;
			var aliveTail = aliveHead;

			// Find tail removing dead items
			while (currentAlive != null)
			{
				var current = currentAlive.Next;

				while (current != null && (current.IsAlive == false || predicate != null && predicate(current.Target)))
				{
					var next = current.Next;

					current.Dispose();

					current = next;
				}

				currentAlive.Next = current;
				aliveTail = currentAlive;
				currentAlive = current;
			}

			tail = aliveTail;

			return aliveHead;
		}

		public void Dispose()
		{
			WeakReference = null;
			Next = null;
			Pool?.ReturnNode(this);
		}

		internal void Mount(T target)
		{
			WeakReference = new WeakReference<T>(target);
		}

		internal static WeakLinkedNode<T> CleanImpl(WeakLinkedNode<T> head)
		{
			return CleanImpl(head, out _);
		}

		internal IEnumerable<T> EnumerateAlive(bool clean)
		{
			var aliveHead = this;

			if (clean)
				aliveHead = CleanImpl(aliveHead);

			var current = aliveHead;

			if (aliveHead == null)
				yield break;

			while (current != null)
			{
				var currentTarget = current.Target;

				if (currentTarget != null)
					yield return currentTarget;

				current = current.Next;
			}
		}

		public void RemoveAfter()
		{
			var remove = Next;

			if (remove == null)
				return;

			Next = remove.Next;
			remove.Next = null;
		}

		public override string ToString()
		{
			var target = Target;

			return target == null ? "Dead" : Target.ToString();
		}
	}

	internal static class WeakLinkedNode
	{
		public static void Clean<T>(ref WeakLinkedNode<T> head, out WeakLinkedNode<T> tail) where T : class
		{
			head = WeakLinkedNode<T>.CleanImpl(head, out tail);
		}

		public static void Clean<T>(ref WeakLinkedNode<T> head, out WeakLinkedNode<T> tail, Func<T, bool> predicate) where T : class
		{
			head = WeakLinkedNode<T>.CleanImpl(head, out tail, predicate);
		}

		public static WeakLinkedNode<T> Create<T>(T item) where T : class
		{
			return new WeakLinkedNode<T>(item);
		}

		public static void Insert<T>(WeakLinkedNode<T> node, ref WeakLinkedNode<T> head, ref WeakLinkedNode<T> tail) where T : class
		{
			if (head == null)
				head = tail = node;
			else
				tail.Next = node;

			while (tail.Next != null) 
				tail = tail.Next;
		}

		public static void Remove<T>(ref WeakLinkedNode<T> head, ref WeakLinkedNode<T> tail, T item) where T : class
		{
			item.EnsureNotNull(nameof(item));

			if (head == null)
			{
				tail = null;

				return;
			}

			if (ReferenceEquals(head.Target, item))
			{
				var result = head;
				var next = head.Next;

				if (ReferenceEquals(head, tail))
					head = tail = null;
				else
					head = next;

				result.Dispose();

				return;
			}

			var current = head;

			while (current.Next != null && ReferenceEquals(current.Next.Target, item) == false)
				current = current.Next;

			if (current.Next == null)
				return;

			if (ReferenceEquals(current.Next, tail))
				tail = current;

			{
				var result = current.Next;

				current.RemoveAfter();

				result.Dispose();
			}
		}

		public static void RemoveNode<T>(ref WeakLinkedNode<T> head, ref WeakLinkedNode<T> tail, WeakLinkedNode<T> node) where T : class
		{
			var target = node.Target;

			if (target != null)
				Remove(ref head, ref tail, target);
		}
	}
}