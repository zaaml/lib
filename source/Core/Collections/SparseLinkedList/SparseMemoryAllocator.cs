// <copyright file="SparseMemoryAllocator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Buffers;

namespace Zaaml.Core.Collections
{
	internal sealed class SparseMemoryAllocator<T>
	{
		public SparseMemoryAllocator(int nodeCapacity)
		{
			NodeCapacity = nodeCapacity;

			SpanAllocator = MemorySpanAllocator.Create(ArrayPool<T>.Create(), true);
		}

		private MemorySpanAllocator<T> SpanAllocator { get; }

		public int NodeCapacity { get; }

		public MemorySpan<T> Allocate()
		{
			return SpanAllocator.Allocate(NodeCapacity);
		}
	}
}