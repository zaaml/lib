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
		#region Nested Types

		private protected class Pool<T> : IPool<T>
		{
			#region Fields

			private readonly Func<Pool<T>, T> _factory;
			private readonly Stack<T> _stack = new Stack<T>();

			#endregion

			#region Ctors

			public Pool(Func<Pool<T>, T> factory)
			{
				_factory = factory;
			}

			#endregion

			#region Interface Implementations

			#region IPool<T>

			public T Get()
			{
				return _stack.Count > 0 ? _stack.Pop() : _factory(this);
			}

			public void Release(T item)
			{
				_stack.Push(item);
			}

			#endregion

			#endregion
		}

		#endregion
	}
}