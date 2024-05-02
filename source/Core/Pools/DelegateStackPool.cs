// <copyright file="DelegateStackPool.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Zaaml.Core.Pools
{
	internal class DelegateStackPool<T> : IPool<T>
	{
		private readonly Func<T> _factory;
		private readonly Stack<T> _stack = new();

		public DelegateStackPool(Func<T> factory)
		{
			_factory = factory;
		}

		public T Rent()
		{
			return _stack.Count > 0 ? _stack.Pop() : _factory();
		}

		public void Return(T item)
		{
			_stack.Push(item);
		}
	}
}