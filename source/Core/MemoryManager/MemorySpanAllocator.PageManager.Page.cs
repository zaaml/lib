// <copyright file="MemorySpanAllocator.PageManager.Page.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.Core.Weak.Collections;

namespace Zaaml.Core
{
	internal partial class MemorySpanAllocator<T>
	{
		private sealed partial class PageManager
		{
			public sealed partial class Page
			{
				private readonly WeakLinkedList<Allocator> _allocators = new();

				public Page(PageManager pageManager)
				{
					PageManager = pageManager;
				}

				private T[] Array { get; set; }

				private int Offset { get; set; }

				private PageManager PageManager { get; }

				private MemorySpan<T> Allocate(int length, IMemorySpanAllocator<T> allocator)
				{
					Cleanup(false);

					Array ??= PageManager.RentArray();

					var span = new MemorySpan<T>(Array, Offset, length, allocator);

					Offset += length;

					return span;
				}

				public bool CanAllocate(int length)
				{
					Cleanup(false);

					Array ??= PageManager.RentArray();

					return Offset + length <= Array.Length;
				}

				private void Cleanup(bool releasePage)
				{
					if (_allocators.Cleanup() == false)
						return;

					foreach (var pageAllocator in _allocators)
						if (pageAllocator.ReferenceCount > 0)
							return;

					if (Array != null)
					{
						PageManager.ReturnArray(Array);

						Array = null;
					}

					Offset = 0;

					if (releasePage)
						PageManager.ReleasePage(this);
				}

				public IMemorySpanAllocator<T> CreateAllocator(PageManagerCollection parentAllocator)
				{
					var pageAllocator = new Allocator(this, parentAllocator);

					_allocators.Add(pageAllocator);

					return pageAllocator;
				}

				private void Deallocate(MemorySpan<T> span, IMemorySpanAllocator<T> allocator)
				{
					Cleanup(true);
				}
			}
		}
	}
}