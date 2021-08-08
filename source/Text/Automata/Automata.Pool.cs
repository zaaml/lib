// <copyright file="Automata.Pool.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using Zaaml.Core.Pools;

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		private protected class Pool<T> : IPool<T>
		{
			private readonly Func<Pool<T>, T> _factory;
			private readonly Stack<T> _stack = new();

			public Pool(Func<Pool<T>, T> factory)
			{
				_factory = factory;
			}

			public T Get()
			{
				return _stack.Count > 0 ? _stack.Pop() : _factory(this);
			}

			public void Release(T item)
			{
				_stack.Push(item);
			}
		}
	}
}