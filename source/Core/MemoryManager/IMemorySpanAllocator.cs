// <copyright file="IMemorySpanAllocator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Core
{
	internal interface IMemorySpanAllocator<T>
	{
		MemorySpan<T> Allocate(int size);

		void Deallocate(MemorySpan<T> memorySpan);
	}
}