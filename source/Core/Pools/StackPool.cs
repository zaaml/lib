// <copyright file="StackPool.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Core.Pools
{
	internal sealed class StackPool<T> : DelegateStackPool<T> where T : new()
	{
		public StackPool() : base(() => new T())
		{
		}
	}

	internal static class StackPool
	{
		public static IPool<T> GetShared<T>() where T : new()
		{
			return ThreadLocalPool<T, StackPool<T>>.Shared;
		}
	}
}