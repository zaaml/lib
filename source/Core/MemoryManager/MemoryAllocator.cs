// <copyright file="MemoryAllocator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Core
{
	internal static class MemoryAllocator
	{
		public static MemoryAllocator<T> Create<T>()
		{
			return new();
		}
	}

	internal sealed partial class MemoryAllocator<T> : IMemoryAllocator<T>
	{
		internal MemoryAllocator()
		{
			Manager = PageManager.Instance;
		}

		private PageManager Manager { get; }

		private IMemoryAllocator<T> UnderlyingAllocator { get; set; }

		internal Memory<T> AllocateWithNewPage(int length)
		{
			return (UnderlyingAllocator = CreatePageAllocator(length)).Allocate(length);
		}

		private IMemoryAllocator<T> CreatePageAllocator(int capacity)
		{
			return Manager.CreateAllocator(capacity, this);
		}

		private IMemoryAllocator<T> GetAllocator(int capacity)
		{
			return UnderlyingAllocator ??= CreatePageAllocator(capacity);
		}

		public Memory<T> Allocate(int capacity)
		{
			return GetAllocator(capacity).Allocate(capacity);
		}

		public void Deallocate(Memory<T> memory)
		{
			memory.Dispose();
		}
	}
}