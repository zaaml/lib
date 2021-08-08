// <copyright file="MemoryAllocator.PageManager.Page.Allocator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Core
{
	internal partial class MemorySpanAllocator<T>
	{
		private sealed partial class PageManager
		{
			public sealed partial class Page
			{
				public sealed class Allocator : IMemorySpanAllocator<T>
				{
					public Allocator(Page page, PageManagerCollection parentAllocator)
					{
						Page = page;
						ParentAllocator = parentAllocator;
					}

					public Page Page { get; }

					public PageManagerCollection ParentAllocator { get; }

					public int ReferenceCount { get; private set; }

					public bool CanAllocate(int length)
					{
						return Page.CanAllocate(length);
					}

					public MemorySpan<T> Allocate(int size)
					{
						if (CanAllocate(size) == false)
							return ParentAllocator.AllocateWithNewPage(size);

						ReferenceCount++;

						return Page.Allocate(size, this);
					}

					public void Deallocate(MemorySpan<T> memorySpan)
					{
						ReferenceCount--;

						Page.Deallocate(memorySpan, this);
					}
				}
			}
		}
	}
}