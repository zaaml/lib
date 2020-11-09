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
#if !SILVERLIGHT
  [Serializable]
#endif
  public class CollectionBase<T> : IList<T>, IList, IReadOnlyList<T>
  {
    #region Fields

    private readonly IList<T> _items;

#if !SILVERLIGHT
    [NonSerialized]
#endif
    private object _syncRoot;

    #endregion

    #region Ctors

    public CollectionBase()
    {
      _items = new List<T>();
    }

    internal CollectionBase(int capacity)
    {
      _items = new List<T>(capacity);
    }

    public CollectionBase(IList<T> list)
    {
      if (list == null)
        Error.ThrowArgumentNullException(ExceptionArgument.list);

      _items = list;
    }

    #endregion

    #region Properties

    internal int Capacity
    {
      get
      {
        var listItems = _items as List<T>;

        return listItems?.Capacity ?? Count;
      }
      set
      {
	      if (_items is List<T> listItems)
          listItems.Capacity = value;
      }
    }

    public void RemoveRange(int index, int count)
    {
	    if (_items is List<T> list)
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

    protected IList<T> Items => _items;

    #endregion

    #region  Methods

    protected virtual void ClearItems()
    {
      _items.Clear();
    }

    protected virtual void InsertItem(int index, T item)
    {
      _items.Insert(index, item);
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
      _items.RemoveAt(index);
    }

    protected virtual void SetItem(int index, T item)
    {
      _items[index] = item;
    }

    #endregion

    #region Interface Implementations

    #region ICollection

    bool ICollection.IsSynchronized => false;

    object ICollection.SyncRoot
    {
      get
      {
        if (_syncRoot != null)
          return _syncRoot;

        if (_items is ICollection items)
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
        _items.CopyTo(array1, index);
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

        var count = _items.Count;

        try
        {
          for (var index1 = 0; index1 < count; ++index1)
            objArray[index++] = _items[index1];
        }
        catch (ArrayTypeMismatchException)
        {
          Error.ThrowArgumentException(ExceptionResource.Argument_InvalidArrayType);
        }
      }
    }

    #endregion

    #region ICollection<T>

    public int Count => _items.Count;

    public void Add(T item)
    {
      if (_items.IsReadOnly)
        Error.ThrowNotSupportedException(ExceptionResource.NotSupported_ReadOnlyCollection);

      InsertItem(_items.Count, item);
    }

    public void Clear()
    {
      if (_items.IsReadOnly)
        Error.ThrowNotSupportedException(ExceptionResource.NotSupported_ReadOnlyCollection);

      ClearItems();
    }

    public void CopyTo(T[] array, int index)
    {
      _items.CopyTo(array, index);
    }

    public bool Contains(T item)
    {
      return _items.Contains(item);
    }

    public bool Remove(T item)
    {
      if (_items.IsReadOnly)
        Error.ThrowNotSupportedException(ExceptionResource.NotSupported_ReadOnlyCollection);

      var index = _items.IndexOf(item);

      if (index < 0)
        return false;

      RemoveItem(index);

      return true;
    }


    bool ICollection<T>.IsReadOnly => _items.IsReadOnly;

    #endregion

    #region IEnumerable

    IEnumerator IEnumerable.GetEnumerator()
    {
      return _items.GetEnumerator();
    }

    #endregion

    #region IEnumerable<T>

    public IEnumerator<T> GetEnumerator()
    {
      return _items.GetEnumerator();
    }

    #endregion

    #region IList

    object IList.this[int index]
    {
      get => _items[index];

      set
      {
        Error.IfNullAndNullsAreIllegalThenThrow<T>(value, ExceptionArgument.value);
        try
        {
          this[index] = (T) value;
        }
        catch (InvalidCastException)
        {
          Error.ThrowWrongValueTypeArgumentException(value, typeof(T));
        }
      }
    }


    bool IList.IsReadOnly => _items.IsReadOnly;


    bool IList.IsFixedSize
    {
      get
      {
        var items = _items as IList;

        return items?.IsFixedSize ?? _items.IsReadOnly;
      }
    }


    int IList.Add(object value)
    {
      if (_items.IsReadOnly)
        Error.ThrowNotSupportedException(ExceptionResource.NotSupported_ReadOnlyCollection);

      Error.IfNullAndNullsAreIllegalThenThrow<T>(value, ExceptionArgument.value);

      try
      {
        Add((T) value);
      }
      catch (InvalidCastException)
      {
        Error.ThrowWrongValueTypeArgumentException(value, typeof(T));
      }

      return Count - 1;
    }
		
    bool IList.Contains(object value)
    {
      return IsCompatibleObject(value) && Contains((T) value);
    }
		
    int IList.IndexOf(object value)
    {
      return IsCompatibleObject(value) ? IndexOf((T) value) : -1;
    }
		
    void IList.Insert(int index, object value)
    {
      if (_items.IsReadOnly)
        Error.ThrowNotSupportedException(ExceptionResource.NotSupported_ReadOnlyCollection);

      Error.IfNullAndNullsAreIllegalThenThrow<T>(value, ExceptionArgument.value);

      try
      {
        Insert(index, (T) value);
      }
      catch (InvalidCastException)
      {
        Error.ThrowWrongValueTypeArgumentException(value, typeof(T));
      }
    }
		
    void IList.Remove(object value)
    {
      if (_items.IsReadOnly)
        Error.ThrowNotSupportedException(ExceptionResource.NotSupported_ReadOnlyCollection);

      if (!IsCompatibleObject(value))
        return;

      Remove((T) value);
    }

    #endregion

    #region IList<T>

    public T this[int index]
    {
      get => _items[index];

      set
      {
        if (_items.IsReadOnly)
          Error.ThrowNotSupportedException(ExceptionResource.NotSupported_ReadOnlyCollection);

        if (index < 0 || index >= _items.Count)
          Error.ThrowArgumentOutOfRangeException();

        SetItem(index, value);
      }
    }

    public int IndexOf(T item)
    {
      return _items.IndexOf(item);
    }

    public void Insert(int index, T item)
    {
      if (_items.IsReadOnly)
        Error.ThrowNotSupportedException(ExceptionResource.NotSupported_ReadOnlyCollection);

      if (index < 0 || index > _items.Count)
        Error.ThrowArgumentOutOfRangeException(ExceptionArgument.index, ExceptionResource.ArgumentOutOfRange_ListInsert);

      InsertItem(index, item);
    }

    public void RemoveAt(int index)
    {
      if (_items.IsReadOnly)
        Error.ThrowNotSupportedException(ExceptionResource.NotSupported_ReadOnlyCollection);

      if (index < 0 || index >= _items.Count)
        Error.ThrowArgumentOutOfRangeException();

      RemoveItem(index);
    }

    #endregion

    #endregion
  }
}