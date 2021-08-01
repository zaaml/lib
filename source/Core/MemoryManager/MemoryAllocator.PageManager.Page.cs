// <copyright file="MemoryAllocator.PageManager.Page.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.Core.Weak.Collections;

namespace Zaaml.Core
{
	internal partial class MemoryAllocator<T>
	{
		private sealed partial class PageManager
		{
			private sealed partial class Page
			{
				private readonly WeakLinkedList<Allocator> _allocators = new();

				public Page(PageManager pageManager)
				{
					PageManager = pageManager;
				}

				private T[] Array { get; set; }

				private int Offset { get; set; }

				private PageManager PageManager { get; }

				private Memory<T> Allocate(int length, IMemoryAllocator<T> allocator)
				{
					Cleanup(false);

					Array ??= PageManager.RentArray();

					var span = new Memory<T>(Array, Offset, length, allocator);

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

					var referenceCount = 0;

					foreach (var pageAllocator in _allocators)
						referenceCount += pageAllocator.ReferenceCount;

					if (referenceCount == 0)
					{
						if (Array != null)
						{
							PageManager.ReturnArray(Array);
							
							Array = null;
						}

						Offset = 0;

						if (releasePage)
							PageManager.ReleasePage(this);
					}
				}

				public IMemoryAllocator<T> CreateAllocator(MemoryAllocator<T> parentAllocator)
				{
					var pageAllocator = new Allocator(this, parentAllocator);

					_allocators.Add(pageAllocator);

					return pageAllocator;
				}

				private void Deallocate(Memory<T> span, IMemoryAllocator<T> allocator)
				{
					Cleanup(true);
				}
			}
		}
	}
}