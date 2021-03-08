// <copyright file="IMemoryAllocator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Core
{
	internal interface IMemoryAllocator<T>
	{
		Memory<T> Allocate(int length);

		void Deallocate(Memory<T> memory);
	}
}