// <copyright file="SparseMemoryManager.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Zaaml.Core.Collections
{
	internal sealed class SparseMemoryManager<T>
	{
		public int NodeCapacity { get; }

		private readonly Stack<SparseMemoryPage<T>> _pagesPool = new Stack<SparseMemoryPage<T>>();

		private SparseMemoryPage<T> _currentPage;

		public SparseMemoryManager(int nodeCapacity, int nodeCount = 64)
		{
			NodeCapacity = nodeCapacity;
			PageSize = nodeCapacity * nodeCount;
		}

		public int PageSize { get; }

		public SparseMemorySpan<T> Allocate()
		{
			return GetPage(NodeCapacity).Allocate(NodeCapacity);
		}

		private SparseMemoryPage<T> Get()
		{
			return _pagesPool.Count > 0 ? _pagesPool.Pop() : new SparseMemoryPage<T>(this);
		}

		public SparseMemoryPage<T> GetPage(int capacity)
		{
			if (_currentPage == null)
				return _currentPage = Get().AddReference();

			if (_currentPage.CanAllocate(capacity))
				return _currentPage;

			_currentPage.ReleaseReference();
			_currentPage = Get().AddReference();

			return _currentPage;
		}

		public void ReleasePage(SparseMemoryPage<T> sparseMemoryPage)
		{
			_pagesPool.Push(sparseMemoryPage);
		}
	}
}