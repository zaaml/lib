// <copyright file="SparseMemoryAllocator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Core.Collections
{
	internal sealed class SparseMemoryAllocator<T>
	{
		public SparseMemoryAllocator(int nodeCapacity)
		{
			NodeCapacity = nodeCapacity;

			Allocator = MemoryAllocator.Create<T>();
		}

		private MemoryAllocator<T> Allocator { get; }

		public int NodeCapacity { get; }

		public Memory<T> Allocate()
		{
			return Allocator.Allocate(NodeCapacity);
		}
	}
}