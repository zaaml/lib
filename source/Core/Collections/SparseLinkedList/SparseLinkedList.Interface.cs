// <copyright file="SparseLinkedList.Interface.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Zaaml.Core.Collections
{
	internal partial class SparseLinkedList<T>
	{
		#region Ctors

		[PublicAPI]
		public SparseLinkedList(IEnumerable<T> collection) : this(0, DefaultCapacity)
		{
			InsertRange(0, collection);

			VerifyStructure();
		}

		[PublicAPI]
		public SparseLinkedList(int count) : this(count, DefaultCapacity)
		{
			VerifyStructure();
		}

		#endregion

		#region  Methods

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

		#endregion

		#region Interface Implementations

		#region ICollection<T>

		[PublicAPI]
		public void Add(T item)
		{
			Lock();

			Version++;

			AddImpl(item);

			VerifyStructure();

			Unlock();
		}

		#endregion

		#region IList

		[PublicAPI]
		public void Clear()
		{
			Lock();

			ClearImpl();

			VerifyStructure();

			Unlock();
		}

		#endregion

		#region IList<T>

		[PublicAPI]
		public T this[int index]
		{
			get
			{
				Lock();

				VerifyIndex(index);

				var item = GetItem(index);

				Unlock();

				return item;
			}
			set
			{
				Lock();

				VerifyIndex(index);

				Version++;

				SetItem(index, value);

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

		#endregion

		#endregion
	}
}