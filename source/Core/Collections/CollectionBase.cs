// <copyright file="CollectionBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Zaaml.Core.Collections
{
	[DebuggerDisplay("Count = {" + nameof(Count) + "}")]
	public abstract class CollectionBase<T> : IList<T>, IList, IReadOnlyList<T>
	{
		private object _syncRoot;

		protected CollectionBase()
		{
			Items = new List<T>();
		}

		internal CollectionBase(int capacity)
		{
			Items = new List<T>(capacity);
		}

		protected CollectionBase(IList<T> list)
		{
			if (list == null)
				Error.ThrowArgumentNullException(ExceptionArgument.list);

			Items = list;
		}

		internal int Capacity
		{
			get
			{
				var listItems = Items as List<T>;

				return listItems?.Capacity ?? Count;
			}
			set
			{
				if (Items is List<T> listItems)
					listItems.Capacity = value;
			}
		}

		protected IList<T> Items { get; }

		protected virtual void ClearItems()
		{
			Items.Clear();
		}

		protected virtual void InsertItem(int index, T item)
		{
			Items.Insert(index, item);
		}

		private static bool IsCompatibleObject(object value)
		{
			if (value is T)
				return true;

			if (value == null)
				return default(T) == null;

			return false;
		}

		protected virtual void RemoveItem(int index)
		{
			Items.RemoveAt(index);
		}

		public void RemoveRange(int index, int count)
		{
			if (Items is List<T> list)
				list.RemoveRange(index, count);
			else
			{
				if (index < 0)
					Error.ThrowArgumentOutOfRangeException(ExceptionArgument.index, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);

				if (count < 0)
					Error.ThrowArgumentOutOfRangeException(ExceptionArgument.count, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);

				if (Count - index < count)
					Error.ThrowArgumentException(ExceptionResource.Argument_InvalidOffLen);

				if (count <= 0)
					return;

				while (count > 0)
				{
					count--;
					RemoveAt(index);
				}
			}
		}

		protected virtual void SetItem(int index, T item)
		{
			Items[index] = item;
		}

		bool ICollection.IsSynchronized => false;

		object ICollection.SyncRoot
		{
			get
			{
				if (_syncRoot != null)
					return _syncRoot;

				if (Items is ICollection items)
					_syncRoot = items.SyncRoot;
				else
					Interlocked.CompareExchange<object>(ref _syncRoot, new object(), null);

				return _syncRoot;
			}
		}

		void ICollection.CopyTo(Array array, int index)
		{
			if (array == null)
				Error.ThrowArgumentNullException(ExceptionArgument.array);

			if (array.Rank != 1)
				Error.ThrowArgumentException(ExceptionResource.Arg_RankMultiDimNotSupported);

			if (array.GetLowerBound(0) != 0)
				Error.ThrowArgumentException(ExceptionResource.Arg_NonZeroLowerBound);

			if (index < 0)
				Error.ThrowArgumentOutOfRangeException(ExceptionArgument.index, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);

			if (array.Length - index < Count)
				Error.ThrowArgumentException(ExceptionResource.Arg_ArrayPlusOffTooSmall);

			if (array is T[] array1)
			{
				Items.CopyTo(array1, index);
			}
			else
			{
				var elementType = array.GetType().GetElementType();
				var c = typeof(T);

				if (!elementType.IsAssignableFrom(c) && !c.IsAssignableFrom(elementType))
					Error.ThrowArgumentException(ExceptionResource.Argument_InvalidArrayType);

				var objArray = array as object[];

				if (objArray == null)
					Error.ThrowArgumentException(ExceptionResource.Argument_InvalidArrayType);

				var count = Items.Count;

				try
				{
					for (var index1 = 0; index1 < count; ++index1)
						objArray[index++] = Items[index1];
				}
				catch (ArrayTypeMismatchException)
				{
					Error.ThrowArgumentException(ExceptionResource.Argument_InvalidArrayType);
				}
			}
		}

		object IList.this[int index]
		{
			get => Items[index];

			set
			{
				Error.IfNullAndNullsAreIllegalThenThrow<T>(value, ExceptionArgument.value);
				try
				{
					this[index] = (T)value;
				}
				catch (InvalidCastException)
				{
					Error.ThrowWrongValueTypeArgumentException(value, typeof(T));
				}
			}
		}

		bool IList.IsReadOnly => Items.IsReadOnly;

		bool IList.IsFixedSize
		{
			get
			{
				var items = Items as IList;

				return items?.IsFixedSize ?? Items.IsReadOnly;
			}
		}

		int IList.Add(object value)
		{
			if (Items.IsReadOnly)
				Error.ThrowNotSupportedException(ExceptionResource.NotSupported_ReadOnlyCollection);

			Error.IfNullAndNullsAreIllegalThenThrow<T>(value, ExceptionArgument.value);

			try
			{
				Add((T)value);
			}
			catch (InvalidCastException)
			{
				Error.ThrowWrongValueTypeArgumentException(value, typeof(T));
			}

			return Count - 1;
		}

		bool IList.Contains(object value)
		{
			return IsCompatibleObject(value) && Contains((T)value);
		}

		int IList.IndexOf(object value)
		{
			return IsCompatibleObject(value) ? IndexOf((T)value) : -1;
		}

		void IList.Insert(int index, object value)
		{
			if (Items.IsReadOnly)
				Error.ThrowNotSupportedException(ExceptionResource.NotSupported_ReadOnlyCollection);

			Error.IfNullAndNullsAreIllegalThenThrow<T>(value, ExceptionArgument.value);

			try
			{
				Insert(index, (T)value);
			}
			catch (InvalidCastException)
			{
				Error.ThrowWrongValueTypeArgumentException(value, typeof(T));
			}
		}

		void IList.Remove(object value)
		{
			if (Items.IsReadOnly)
				Error.ThrowNotSupportedException(ExceptionResource.NotSupported_ReadOnlyCollection);

			if (!IsCompatibleObject(value))
				return;

			Remove((T)value);
		}

		public int Count => Items.Count;

		public void Add(T item)
		{
			if (Items.IsReadOnly)
				Error.ThrowNotSupportedException(ExceptionResource.NotSupported_ReadOnlyCollection);

			InsertItem(Items.Count, item);
		}

		public void Clear()
		{
			if (Items.IsReadOnly)
				Error.ThrowNotSupportedException(ExceptionResource.NotSupported_ReadOnlyCollection);

			ClearItems();
		}

		public void CopyTo(T[] array, int index)
		{
			Items.CopyTo(array, index);
		}

		public bool Contains(T item)
		{
			return Items.Contains(item);
		}

		public bool Remove(T item)
		{
			if (Items.IsReadOnly)
				Error.ThrowNotSupportedException(ExceptionResource.NotSupported_ReadOnlyCollection);

			var index = Items.IndexOf(item);

			if (index < 0)
				return false;

			RemoveItem(index);

			return true;
		}


		bool ICollection<T>.IsReadOnly => Items.IsReadOnly;

		IEnumerator IEnumerable.GetEnumerator()
		{
			return Items.GetEnumerator();
		}

		public IEnumerator<T> GetEnumerator()
		{
			return Items.GetEnumerator();
		}

		public T this[int index]
		{
			get => Items[index];

			set
			{
				if (Items.IsReadOnly)
					Error.ThrowNotSupportedException(ExceptionResource.NotSupported_ReadOnlyCollection);

				if (index < 0 || index >= Items.Count)
					Error.ThrowArgumentOutOfRangeException();

				SetItem(index, value);
			}
		}

		public int IndexOf(T item)
		{
			return Items.IndexOf(item);
		}

		public void Insert(int index, T item)
		{
			if (Items.IsReadOnly)
				Error.ThrowNotSupportedException(ExceptionResource.NotSupported_ReadOnlyCollection);

			if (index < 0 || index > Items.Count)
				Error.ThrowArgumentOutOfRangeException(ExceptionArgument.index, ExceptionResource.ArgumentOutOfRange_ListInsert);

			InsertItem(index, item);
		}

		public void RemoveAt(int index)
		{
			if (Items.IsReadOnly)
				Error.ThrowNotSupportedException(ExceptionResource.NotSupported_ReadOnlyCollection);

			if (index < 0 || index >= Items.Count)
				Error.ThrowArgumentOutOfRangeException();

			RemoveItem(index);
		}
	}
}