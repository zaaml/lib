// <copyright file="MemorySpanAllocator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Buffers;
using System.Threading;

namespace Zaaml.Core
{
	internal static class MemorySpanAllocator
	{
		public static MemorySpanAllocator<T> Create<T>(ArrayPool<T> arrayPool)
		{
			return new MemorySpanAllocator<T>(arrayPool);
		}
	}

	internal sealed partial class MemorySpanAllocator<T> : IMemorySpanAllocator<T>
	{
		private static readonly ThreadLocal<MemorySpanAllocator<T>> ThreadLocalInstance = new(() => new MemorySpanAllocator<T>(PageManagerCollection.Shared));

		public MemorySpanAllocator(ArrayPool<T> arrayPool)
		{
			ManagerCollection = new PageManagerCollection(arrayPool);
		}

		private MemorySpanAllocator(PageManagerCollection managerCollection)
		{
			ManagerCollection = managerCollection;
		}

		private PageManagerCollection ManagerCollection { get; }

		public static MemorySpanAllocator<T> Shared => ThreadLocalInstance.Value;

		public MemorySpan<T> Allocate(int size)
		{
			return ManagerCollection.Allocate(size);
		}

		public void Deallocate(MemorySpan<T> memorySpan)
		{
			ManagerCollection.Deallocate(memorySpan);
		}
	}
}