// <copyright file="PageMemorySpanAllocator.PageManager.Page.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using Zaaml.Core.Weak.Collections;

namespace Zaaml.Core
{
	internal partial class PageMemorySpanAllocator<T>
	{
		private sealed partial class PageManager
		{
			public sealed partial class Page : IDisposable
			{
				private WeakReference<Allocator> _allocator;
				private WeakLinkedList<Allocator> _allocators;
				private int _gcCount;

				public Page(PageManager pageManager)
				{
					PageManager = pageManager;
				}

				private T[] Array { get; set; }

				private bool IsDisposed { get; set; }

				private int Offset { get; set; }

				public PageManager PageManager { get; }

				private MemorySpan<T> Allocate(int length, IMemorySpanAllocator<T> allocator)
				{
					if (IsDisposed)
						throw new InvalidOperationException();

					if (_allocators != null)
						CleanupAllocators(false);

					Array ??= PageManager.RentArray();

					var span = new MemorySpan<T>(Array, Offset, length, allocator);

					Offset += length;

					return span;
				}

				public bool CanAllocate(int length)
				{
					if (IsDisposed)
						return false;

					if (_allocators != null)
						CleanupAllocators(false);

					Array ??= PageManager.RentArray();

					return Offset + length <= Array.Length;
				}

				private void CleanupAllocators(bool releasePage, bool force = false)
				{
					var gcCount = GC.CollectionCount(0);
					var needCleanup = force;

					if (gcCount != _gcCount)
					{
						_gcCount = gcCount;

						needCleanup = true;
					}

					if (needCleanup == false)
						return;

					var enumerator = _allocators.GetEnumerator();

					while (enumerator.MoveNext())
					{
						var pageAllocator = enumerator.Current;

						if (pageAllocator == null || pageAllocator.ReferenceCount == 0)
							continue;

						enumerator.Dispose();

						return;
					}

					enumerator.Dispose();

					ReleaseArray(releasePage);
				}

				public Allocator CreateAllocator(PageMemorySpanAllocator<T> parentAllocator)
				{
					var pageAllocator = new Allocator(this, parentAllocator);

					if (_allocators == null && _allocator == null)
						_allocator = new WeakReference<Allocator>(pageAllocator);
					else
					{
						_allocators ??= new WeakLinkedList<Allocator>();

						if (_allocator != null && _allocator.TryGetTarget(out var allocator))
							_allocators.Add(allocator);

						_allocators.Add(pageAllocator);
						_allocator = null;
					}

					return pageAllocator;
				}

				private void DestroyAllocator(Allocator allocator)
				{
					if (IsDisposed)
						throw new InvalidOperationException();

					if (allocator.ReferenceCount > 0)
						throw new InvalidOperationException();

					if (_allocators != null)
					{
						_allocators.Remove(allocator);

						CleanupAllocators(true, true);
					}
					else
					{
						if (_allocator.TryGetTarget(out var currentAllocator) == false || ReferenceEquals(currentAllocator, allocator) == false)
							throw new InvalidOperationException();

						_allocator = null;

						ReleaseArray(true);
					}
				}

				private void ReleaseArray(bool releasePage)
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

				public void Dispose()
				{
					if (IsDisposed)
						throw new InvalidOperationException();

					IsDisposed = true;
				}
			}
		}
	}
}