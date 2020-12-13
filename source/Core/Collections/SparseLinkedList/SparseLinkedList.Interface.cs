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
		internal SparseLinkedList(int count, SparseLinkedListManager<T> listManager) : base(count, listManager)
		{
			VerifyStructure();
		}

		[PublicAPI]
		public SparseLinkedList(int count) : base(count)
		{
			VerifyStructure();
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
		public void Add(T item)
		{
			Lock();

			Version++;

			AddImpl(item);

			VerifyStructure();

			Unlock();
		}

		[PublicAPI]
		public T this[int index]
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
		public void Insert(int index, T item)
		{
			Lock();

			VerifyIndex(index, true);

			Version++;

			InsertImpl(index, item);

			VerifyStructure();

			Unlock();
		}

		[PublicAPI]
		public void RemoveAt(int index)
		{
			Lock();

			VerifyIndex(index);

			Version++;

			RemoveAtImpl(index);

			VerifyStructure();

			Unlock();
		}

		[PublicAPI]
		public void AddCleanRange(int count)
		{
			Lock();

			Version++;

			AddCleanRangeImpl(count);

			VerifyStructure();

			Unlock();
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
		public void CleanAt(int index)
		{
			Lock();

			VerifyIndex(index);

			Version++;

			CleanAtImpl(index);

			VerifyStructure();

			Unlock();
		}

		[PublicAPI]
		public void CleanRange(int index, int count)
		{
			Lock();

			VerifyRange(index, count);

			Version++;

			CleanRangeImpl(index, count);

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
		public int IndexOf(T item)
		{
			Lock();

			var findImpl = FindImpl(item);

			Unlock();

			return findImpl;
		}

		[PublicAPI]
		public void InsertCleanRange(int index, int count)
		{
			Lock();

			VerifyIndex(index, true);

			Version++;

			InsertCleanRangeImpl(index, count);

			VerifyStructure();

			Unlock();
		}

		[PublicAPI]
		public void InsertRange(int index, IEnumerable<T> collection)
		{
			Lock();

			VerifyIndex(index, true);

			Version++;

			InsertRangeImpl(index, collection);

			VerifyStructure();

			Unlock();
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
		public void RemoveRange(int index, int count)
		{
			Lock();

			VerifyRange(index, count);

			Version++;

			RemoveRangeAtImpl(index, count);

			VerifyStructure();

			Unlock();
		}

		[PublicAPI]
		internal void SplitAt(int index, SparseLinkedList<T> targetList)
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
		internal void Swap(int index, SparseLinkedList<T> list)
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
	}
}