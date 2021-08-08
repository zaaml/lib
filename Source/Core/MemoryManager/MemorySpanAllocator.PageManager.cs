// <copyright file="MemorySpanAllocator.PageManager.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Buffers;
using System.Collections.Generic;
using System.Threading;

namespace Zaaml.Core
{
	internal sealed partial class MemorySpanAllocator<T>
	{
		private sealed class PageManagerCollection : IMemorySpanAllocator<T>
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
			private static readonly ThreadLocal<PageManagerCollection> ThreadLocalInstance = new(() => new PageManagerCollection(ArrayPool<T>.Shared));

			private readonly IMemorySpanAllocator<T>[] _allocators;
			private readonly PageManager[] _pageManagers;

			public PageManagerCollection(ArrayPool<T> arrayPool)
			{
				_pageManagers = new[]
				{
					new PageManager(TinyPageSize, arrayPool),
					new PageManager(SmallPageSize, arrayPool),
					new PageManager(MediumPageSize, arrayPool),
					new PageManager(LargePageSize, arrayPool),
					new PageManager(HugePageSize, arrayPool)
				};

				_allocators = new IMemorySpanAllocator<T>[] { null, null, null, null, null };
			}

			public static PageManagerCollection Shared => ThreadLocalInstance.Value;

			internal MemorySpan<T> AllocateWithNewPage(int size)
			{
				var allocatorIndex = GetAllocatorIndex(size);

				return (_allocators[allocatorIndex] = CreatePageAllocator(size, allocatorIndex)).Allocate(size);
			}

			private IMemorySpanAllocator<T> CreatePageAllocator(int capacity, int allocatorIndex)
			{
				return _pageManagers[allocatorIndex].CreateAllocator(capacity, this);
			}

			private IMemorySpanAllocator<T> GetAllocator(int size)
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
				if (memorySpan.Allocator is not PageManager.Page.Allocator pageAllocator)
					throw new InvalidOperationException("Allocator");

				if (ReferenceEquals(pageAllocator.ParentAllocator, this) == false)
					throw new InvalidOperationException("Allocator");

				pageAllocator.Deallocate(memorySpan);
			}
		}

		private sealed partial class PageManager
		{
			private readonly ArrayPool<T> _arrayPool;
			private readonly int _pageSize;
			private readonly Stack<Page> _pagesPool = new();
			private Page _currentPage;

			public PageManager(int pageSize, ArrayPool<T> arrayPool)
			{
				_pageSize = pageSize;
				_arrayPool = arrayPool;
			}

			public IMemorySpanAllocator<T> CreateAllocator(int capacity, PageManagerCollection pageManagerCollection)
			{
				return GetPage(capacity).CreateAllocator(pageManagerCollection);
			}

			private Page GetPage(int capacity)
			{
				if (_currentPage?.CanAllocate(capacity) == true)
					return _currentPage;

				return _currentPage = _pagesPool.Count > 0 ? _pagesPool.Pop() : new Page(this);
			}

			private void ReleasePage(Page page)
			{
				_pagesPool.Push(page);
			}

			private T[] RentArray()
			{
				return _arrayPool.Rent(_pageSize);
			}

			private void ReturnArray(T[] array)
			{
				_arrayPool.Return(array, true);
			}
		}
	}
}