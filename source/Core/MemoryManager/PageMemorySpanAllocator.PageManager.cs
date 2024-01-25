// <copyright file="PageMemorySpanAllocator.PageManager.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Buffers;
using System.Threading;

namespace Zaaml.Core
{
	internal sealed partial class PageMemorySpanAllocator<T> : IMemorySpanAllocator<T>
	{
		public const int TinyPageSize = 0x1000;
		public const int SmallPageSize = 0x10000;
		public const int MediumPageSize = 0x100000;
		public const int LargePageSize = 0x1000000;
		public const int HugePageSize = 0x10000000;

		public const int TinyAllocator = 0;
		public const int SmallAllocator = 1;
		public const int MediumAllocator = 2;
		public const int LargeAllocator = 3;
		public const int HugeAllocator = 4;

		// ReSharper disable once MemberHidesStaticFromOuterClass
		private static readonly ThreadLocal<PageMemorySpanAllocator<T>> ThreadLocalInstance = new(() => new PageMemorySpanAllocator<T>(ArrayPool<T>.Shared, true));

		private readonly PageManager.Page.Allocator[] _allocators;
		private readonly PageManager[] _pageManagers;

		public PageMemorySpanAllocator(ArrayPool<T> arrayPool, bool clearArray)
		{
			_pageManagers = new[]
			{
				new PageManager(this, TinyPageSize, arrayPool, clearArray),
				new PageManager(this, SmallPageSize, arrayPool, clearArray),
				new PageManager(this, MediumPageSize, arrayPool, clearArray),
				new PageManager(this, LargePageSize, arrayPool, clearArray),
				new PageManager(this, HugePageSize, arrayPool, clearArray)
			};

			_allocators = new PageManager.Page.Allocator[] { null, null, null, null, null };
		}

		public static PageMemorySpanAllocator<T> Shared => ThreadLocalInstance.Value;

		internal MemorySpan<T> AllocateWithNewPage(int size)
		{
			var allocatorIndex = GetAllocatorIndex(size);
			var pageAllocator = CreatePageAllocator(size, allocatorIndex);

			_allocators[allocatorIndex] = pageAllocator;

			return (_allocators[allocatorIndex] = pageAllocator).Allocate(size);
		}

		private PageManager.Page.Allocator CreatePageAllocator(int capacity, int allocatorIndex)
		{
			return _pageManagers[allocatorIndex].CreateAllocator(capacity);
		}

		private PageManager.Page.Allocator GetAllocator(int size)
		{
			var allocatorIndex = GetAllocatorIndex(size);

			return _allocators[allocatorIndex] ??= CreatePageAllocator(size, allocatorIndex);
		}

		private static int GetAllocatorIndex(int size)
		{
			return (size << 4) switch
			{
				> LargePageSize => HugeAllocator,
				> MediumPageSize => LargeAllocator,
				> SmallPageSize => MediumAllocator,
				> TinyPageSize => SmallAllocator,
				_ => TinyAllocator
			};
		}

		public MemorySpan<T> Allocate(int size)
		{
			return GetAllocator(size).Allocate(size);
		}

		public void Deallocate(MemorySpan<T> memorySpan)
		{
#if true
			if (memorySpan.Allocator is not PageManager.Page.Allocator pageAllocator)
				throw new InvalidOperationException("Allocator");

			if (ReferenceEquals(pageAllocator.ParentAllocator, this) == false)
				throw new InvalidOperationException("Allocator");
#endif

			pageAllocator.Deallocate(memorySpan);
		}

		private sealed partial class PageManager
		{
			private readonly ArrayPool<T> _arrayPool;
			private readonly bool _clearArray;
			private readonly PageMemorySpanAllocator<T> _pageMemorySpanAllocator;
			private readonly int _pageSize;
			private Page _currentPage;

			public PageManager(PageMemorySpanAllocator<T> pageMemorySpanAllocator, int pageSize, ArrayPool<T> arrayPool, bool clearArray)
			{
				_pageMemorySpanAllocator = pageMemorySpanAllocator;
				_pageSize = pageSize;
				_arrayPool = arrayPool;
				_clearArray = clearArray;
			}

			public Page.Allocator CreateAllocator(int capacity)
			{
				return GetPage(capacity).CreateAllocator(_pageMemorySpanAllocator);
			}

			private Page GetPage(int capacity)
			{
				if (_currentPage?.CanAllocate(capacity) == true)
					return _currentPage;

				return _currentPage = new Page(this);
			}

			private void ReleasePage(Page page)
			{
				if (ReferenceEquals(_currentPage, page))
					_currentPage = null;

				page.Dispose();
			}

			private T[] RentArray()
			{
				return _arrayPool.Rent(_pageSize);
			}

			private void ReturnArray(T[] array)
			{
				_arrayPool.Return(array, _clearArray);
			}
		}
	}
}