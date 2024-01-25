// <copyright file="PageMemorySpanAllocator.PageManager.Page.Allocator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Core
{
	internal partial class PageMemorySpanAllocator<T>
	{
		private sealed partial class PageManager
		{
			public sealed partial class Page
			{
				public sealed class Allocator : IMemorySpanAllocator<T>
				{
					public Allocator(Page page, PageMemorySpanAllocator<T> parentAllocator)
					{
						Page = page;
						ParentAllocator = parentAllocator;
					}

					public Page Page { get; }

					public PageMemorySpanAllocator<T> ParentAllocator { get; }

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

						if (ReferenceCount == 0)
							Page.DestroyAllocator(this);
					}
				}
			}
		}
	}
}