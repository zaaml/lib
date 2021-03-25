// <copyright file="SparseLinkedList.Interface.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Zaaml.Core.Collections
{
	internal partial class SparseLinkedList<T>
	{
		[PublicAPI]
		public SparseLinkedList(IEnumerable<T> collection)
		{
			InsertRange(0, collection);

			VerifyStructure();
		}

		[PublicAPI]
		internal SparseLinkedList(IEnumerable<T> collection, SparseLinkedListManager<T> listManager) : base(0, listManager)
		{
			InsertRange(0, collection);

			VerifyStructure();
		}

		[PublicAPI]
		public void AddRange(IEnumerable<T> collection)
		{
			Lock();

			Version++;

			AddRangeImpl(collection);

			VerifyStructure();

			Unlock();
		}

		[PublicAPI]
		public bool Contains(T item)
		{
			Lock();

			var index = FindImpl(item);

			Unlock();

			return index != -1;
		}

		[PublicAPI]
		public void CopyTo(T[] array, int arrayIndex)
		{
			Lock();

			CopyToImpl(array, arrayIndex);

			Unlock();
		}

		[PublicAPI]
		public long IndexOf(T item)
		{
			Lock();

			var findImpl = FindImpl(item);

			Unlock();

			return findImpl;
		}

		[PublicAPI]
		public void InsertRange(long index, IEnumerable<T> collection)
		{
			Lock();

			VerifyIndex(index, true);

			Version++;

			InsertRangeImpl(index, collection);

			VerifyStructure();

			Unlock();
		}

		[PublicAPI]
		internal void Merge(SparseLinkedList<T> list)
		{
			Lock();
			list.Lock();

			Version++;
			list.Version++;

			MergeImpl(list);

			VerifyStructure();
			list.VerifyStructure();

			Unlock();
			list.Unlock();
		}

		[PublicAPI]
		public bool Remove(T item)
		{
			Lock();

			var index = FindImpl(item);

			if (index == -1)
			{
				Unlock();

				return false;
			}

			RemoveAtImpl(index);

			VerifyStructure();

			Unlock();

			return true;
		}

		[PublicAPI]
		public void RemoveRange(long index, long count)
		{
			Lock();

			VerifyRange(index, count);

			Version++;

			RemoveRangeAtImpl(index, count);

			VerifyStructure();

			Unlock();
		}

		[PublicAPI]
		internal void SplitAt(long index, SparseLinkedList<T> targetList)
		{
			Lock();
			targetList.Lock();

			VerifyIndex(index, true);

			Version++;
			targetList.Version++;

			SplitAtImpl(index, targetList);

			VerifyStructure();
			targetList.VerifyStructure();

			Unlock();
			targetList.Unlock();
		}

		[PublicAPI]
		internal void Swap(long index, SparseLinkedList<T> list)
		{
			Lock();
			list.Lock();

			VerifyIndex(index);

			Version++;
			list.Version++;

			SwapImpl(list);

			VerifyStructure();
			list.VerifyStructure();

			Unlock();
			list.Unlock();
		}

		[PublicAPI]
		public void Clear()
		{
			Lock();

			ClearImpl();

			VerifyStructure();

			Unlock();
		}

		[PublicAPI]
		public void AddVoidRange(long count)
		{
			Lock();

			Version++;

			AddVoidRangeImpl(count);

			VerifyStructure();

			Unlock();
		}

		[PublicAPI]
		public void AddVoid()
		{
			Lock();

			Version++;

			AddVoidRangeImpl(1);

			VerifyStructure();

			Unlock();
		}

		[PublicAPI]
		public void VoidAt(long index)
		{
			Lock();

			VerifyIndex(index);

			Version++;

			VoidAtImpl(index);

			VerifyStructure();

			Unlock();
		}

		[PublicAPI]
		public void VoidRange(long index, long count)
		{
			Lock();

			VerifyRange(index, count);

			Version++;

			VoidRangeImpl(index, count);

			VerifyStructure();

			Unlock();
		}

		[PublicAPI]
		public void Void()
		{
			Lock();

			Version++;

			VoidRangeImpl(0, LongCount);

			VerifyStructure();

			Unlock();
		}

		[PublicAPI]
		public void InsertVoidRange(long index, long count)
		{
			Lock();

			VerifyIndex(index, true);

			Version++;

			InsertVoidRangeImpl(index, count);

			VerifyStructure();

			Unlock();
		}

		[PublicAPI]
		public void InsertVoid(long index)
		{
			Lock();

			VerifyIndex(index, true);

			Version++;

			InsertVoidRangeImpl(index, 1);

			VerifyStructure();

			Unlock();
		}

		[PublicAPI]
		public void Add(T item)
		{
			Lock();

			Version++;

			AddImpl(item);

			VerifyStructure();

			Unlock();
		}

		[PublicAPI]
		public T this[long index]
		{
			get
			{
				Lock();

				VerifyIndex(index);

				var item = GetItemImpl(index);

				Unlock();

				return item;
			}
			set
			{
				Lock();

				VerifyIndex(index);

				Version++;

				SetItemImpl(index, value);

				VerifyStructure();

				Unlock();
			}
		}

		[PublicAPI]
		public void Insert(long index, T item)
		{
			Lock();

			VerifyIndex(index, true);

			Version++;

			InsertImpl(index, item);

			VerifyStructure();

			Unlock();
		}

		[PublicAPI]
		public void RemoveAt(long index)
		{
			Lock();

			VerifyIndex(index);

			Version++;

			RemoveAtImpl(index);

			VerifyStructure();

			Unlock();
		}

		[PublicAPI]
		public bool IsVoid => IsVoidImpl;
	}
}