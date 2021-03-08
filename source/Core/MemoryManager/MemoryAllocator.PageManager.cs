// <copyright file="MemoryAllocator.PageManager.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Buffers;
using System.Collections.Generic;
using System.Threading;

namespace Zaaml.Core
{
	internal sealed partial class MemoryAllocator<T>
	{
		private sealed partial class PageManager
		{
			private const int PageSize = 65536;

			private static readonly ThreadLocal<PageManager> ThreadLocalInstance = new(() => new PageManager());
			private readonly ArrayPool<T> _arrayPool = ArrayPool<T>.Shared;
			private readonly Stack<Page> _pagesPool = new();
			private Page _currentPage;

			private PageManager()
			{
			}

			public static PageManager Instance => ThreadLocalInstance.Value;

			public IMemoryAllocator<T> CreateAllocator(int capacity, MemoryAllocator<T> memoryAllocator)
			{
				return GetPage(capacity).CreateAllocator(memoryAllocator);
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
				return _arrayPool.Rent(PageSize);
			}

			private void ReturnArray(T[] array)
			{
				_arrayPool.Return(array, true);
			}
		}
	}
}