// <copyright file="DummyPool.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Core.Pools
{
	internal sealed class DummyPool<T> : IPool<T>
	{
		public static readonly IPool<T> Instance = new DummyPool<T>();

		private DummyPool()
		{
		}

		public T Rent()
		{
			return default;
		}

		public void Return(T item)
		{
		}
	}
}