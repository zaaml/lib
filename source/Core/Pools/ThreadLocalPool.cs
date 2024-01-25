// <copyright file="ThreadLocalPool.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Threading;

namespace Zaaml.Core.Pools
{
	internal static class ThreadLocalPool
	{
		public static IPool<T> GetShared<T, TPool>() where TPool : IPool<T>, new()
		{
			return ThreadLocalPool<T, TPool>.Shared;
		}
		
	}

	internal sealed class ThreadLocalPool<T, TPool> : IPool<T> where TPool : IPool<T>, new()
	{
		private static readonly ThreadLocal<ThreadLocalPool<T, TPool>> ThreadLocalInstance = new(() => new ThreadLocalPool<T, TPool>(new TPool()));
		private readonly TPool _pool;

		private ThreadLocalPool(TPool pool)
		{
			_pool = pool;
		}

		public static ThreadLocalPool<T, TPool> Shared => ThreadLocalInstance.Value;

		public T Rent()
		{
			return _pool.Rent();
		}

		public void Return(T item)
		{
			_pool.Return(item);
		}
	}
}