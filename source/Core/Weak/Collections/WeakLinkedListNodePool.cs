// <copyright file="WeakLinkedListNodePool.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Zaaml.Core.Weak.Collections
{
	internal sealed class WeakLinkedListNodePool<T> where T : class
	{
		private WeakLinkedNode<T> PoolHead { get; set; }

		private Stack<WeakLinkedNode<T>> PoolStack{ get; } = new Stack<WeakLinkedNode<T>>();

		public WeakLinkedNode<T> RentNode()
		{
			if (PoolHead == null)
				return new WeakLinkedNode<T>(this);

			var reference = PoolHead;
			var next = reference.Next;

			reference.Next = null;

			PoolHead = next;

			reference.IsInPool = false;

			return reference;
		}

		public void ReturnNode(WeakLinkedNode<T> node)
		{
			if (node.IsInPool)
			{

			}
			
			node.IsInPool = true;

			node.Next = PoolHead;

			PoolHead = node;
		}

		//public WeakLinkedNode<T> RentNode()
		//{
		//	return PoolStack.Count == 0 ? new WeakLinkedNode<T>(this) : PoolStack.Pop();
		//}

		//public void ReturnNode(WeakLinkedNode<T> node)
		//{
		//	node.Next = PoolHead;

		//	PoolStack.Push(node);
		//}
	}
}