// <copyright file="LinkedListStack.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using Zaaml.Core.Pools;

namespace Zaaml.Core.Collections
{
	internal sealed class LinkedListStack<T> : PoolSharedObject<LinkedListStack<T>>
	{
		private readonly LinkedListStackNodePool<T> _nodePool;

		private LinkedListStackNode<T> _tail;

		public LinkedListStack(LinkedListStackNodePool<T> nodePool, IPool<LinkedListStack<T>> listPool) : base(listPool)
		{
			_nodePool = nodePool;
			NodeSize = nodePool.NodeSize;
		}

		public int Count { get; private set; }

		public int NodeSize { get; }

		public ref T PeekRef()
		{
			return ref _tail.Array[_tail.Count - 1];
		}
		
		public T Pop()
		{
			if (Count == 0)
				throw new InvalidOperationException();

			if (_tail.Count == 0)
			{
				var prev = _tail.Prev;

				_tail.Prev = null;

				var releaseNode = _tail;

				Array.Clear(releaseNode.Array, 0, releaseNode.Array.Length);

				_nodePool.Release(releaseNode);

				_tail = prev;
			}

			Count--;

			return _tail.Array[--_tail.Count];
		}

		public void Push(T thread)
		{
			if (_tail == null || _tail.Count == NodeSize)
			{
				var node = _nodePool.Get();

				node.Prev = _tail;
				_tail = node;
			}

			_tail.Array[_tail.Count++] = thread;

			Count++;
		}

		public void PushRef(ref T thread)
		{
			if (_tail == null || _tail.Count == NodeSize)
			{
				var node = _nodePool.Get();

				node.Prev = _tail;
				_tail = node;
			}

			_tail.Array[_tail.Count++] = thread;

			Count++;
		}
	}

	internal sealed class LinkedListStackNode<T>
	{
		public readonly T[] Array;
		public int Count;
		public LinkedListStackNode<T> Prev;

		public LinkedListStackNode(int nodeSize)
		{
			Array = new T[nodeSize];
		}
	}

	internal sealed class LinkedListStackNodePool<T> : IPool<LinkedListStackNode<T>>
	{
		private readonly Stack<LinkedListStackNode<T>> _stackPool = new Stack<LinkedListStackNode<T>>();

		public LinkedListStackNodePool(int nodeSize)
		{
			NodeSize = nodeSize;
		}

		public int NodeSize { get; }

		public LinkedListStackNode<T> Get()
		{
			return _stackPool.Count > 0 ? _stackPool.Pop() : new LinkedListStackNode<T>(NodeSize);
		}

		public void Release(LinkedListStackNode<T> item)
		{
			if (NodeSize != item.Array.Length)
				throw new InvalidOperationException();

			_stackPool.Push(item);
		}
	}
}