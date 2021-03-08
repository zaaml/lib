// <copyright file="MemoryAllocator.PageManager.Page.Allocator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Core
{
	internal partial class MemoryAllocator<T>
	{
		private sealed partial class PageManager
		{
			private sealed partial class Page
			{
				private sealed class Allocator : IMemoryAllocator<T>
				{
					public Allocator(Page page, MemoryAllocator<T> parentAllocator)
					{
						Page = page;
						ParentAllocator = parentAllocator;
					}

					private Page Page { get; }

					private MemoryAllocator<T> ParentAllocator { get; }

					public int ReferenceCount { get; private set; }

					public bool CanAllocate(int length)
					{
						return Page.CanAllocate(length);
					}

					public Memory<T> Allocate(int length)
					{
						if (CanAllocate(length) == false)
							return ParentAllocator.AllocateWithNewPage(length);

						ReferenceCount++;

						return Page.Allocate(length, this);
					}

					public void Deallocate(Memory<T> memory)
					{
						ReferenceCount--;

						Page.Deallocate(memory, this);
					}
				}
			}
		}
	}
}