// <copyright file="PooledStringBuilder.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Text;

namespace Zaaml.Text
{
	internal sealed class PooledStringBuilder
	{
		private static readonly Pool StaticPoolInstance = CreatePool();
		private readonly Pool _pool;
		public readonly StringBuilder Builder = new();

		private PooledStringBuilder(Pool pool)
		{
			_pool = pool;
		}

		public int Length => Builder.Length;

		public static Pool CreatePool(int size = 32)
		{
			var pool = new Pool(size);

			return pool;
		}

		public void Free()
		{
			var builder = Builder;

			if (builder.Capacity > 1024)
				return;

			builder.Clear();

			_pool.Return(this);
		}

		public static PooledStringBuilder GetInstance()
		{
			return StaticPoolInstance.Rent();
		}

		public static implicit operator StringBuilder(PooledStringBuilder obj)
		{
			return obj.Builder;
		}

		public string ToStringAndFree()
		{
			var result = Builder.ToString();

			Free();

			return result;
		}

		public string ToStringAndFree(int startIndex, int length)
		{
			var result = Builder.ToString(startIndex, length);

			Free();

			return result;
		}

		public sealed class Pool
		{
			private readonly int _size;
			private readonly Stack<PooledStringBuilder> _stackPool = new();

			public Pool(int size)
			{
				_size = size;
			}

			public PooledStringBuilder Rent()
			{
				return _stackPool.Count > 0 ? _stackPool.Pop() : new PooledStringBuilder(this);
			}

			public void Return(PooledStringBuilder pooledStringBuilder)
			{
				if (_stackPool.Count < _size)
				{
					_stackPool.Push(pooledStringBuilder);
				}
			}
		}
	}
}