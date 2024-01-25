// <copyright file="EmptyReadOnlyList.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Zaaml.Core.Collections
{
  internal class EmptyReadOnlyList<T> : IList<T>, IList
  {
    #region Static Fields and Constants

    public static readonly IList<T> Instance = new EmptyReadOnlyList<T>();

    #endregion

    #region Ctors

    private EmptyReadOnlyList()
    {
    }

    #endregion

    #region Interface Implementations

    #region ICollection

    public void CopyTo(Array array, int index)
    {
    }

    public object SyncRoot { get; } = new object();

    public bool IsSynchronized => false;

    #endregion

    #region ICollection<T>

    public void Add(T item)
    {
      throw AccessException.ExceptionInstance;
    }

    public void Clear()
    {
      throw AccessException.ExceptionInstance;
    }

    public bool Contains(T item)
    {
      return false;
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
    }

    public bool Remove(T item)
    {
      throw AccessException.ExceptionInstance;
    }

    public int Count => 0;

    public bool IsReadOnly => true;

    #endregion

    #region IEnumerable

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    #endregion

    #region IEnumerable<T>

    public IEnumerator<T> GetEnumerator()
    {
      return Enumerable.Empty<T>().GetEnumerator();
    }

    #endregion

    #region IList

    public int Add(object value)
    {
      throw new NotImplementedException();
    }

    public bool Contains(object value)
    {
      return false;
    }

    public int IndexOf(object value)
    {
      return -1;
    }

    public void Insert(int index, object value)
    {
      throw AccessException.ExceptionInstance;
    }

    public void Remove(object value)
    {
      throw AccessException.ExceptionInstance;
    }

    object IList.this[int index]
    {
      get => throw new ArgumentOutOfRangeException(nameof(index));
      set => throw AccessException.ExceptionInstance;
    }

    public bool IsFixedSize => true;

    #endregion

    #region IList<T>

    public int IndexOf(T item)
    {
      return -1;
    }

    public void Insert(int index, T item)
    {
      throw AccessException.ExceptionInstance;
    }

    public void RemoveAt(int index)
    {
      throw AccessException.ExceptionInstance;
    }

    public T this[int index]
    {
      get => throw new ArgumentOutOfRangeException(nameof(index));
      set => throw AccessException.ExceptionInstance;
    }

    #endregion

    #endregion

    #region  Nested Types

    private class AccessException : Exception
    {
      #region Static Fields and Constants

      public static readonly Exception ExceptionInstance = new AccessException();

      #endregion

      #region Ctors

      private AccessException()
      {
      }

      #endregion

      #region Properties

      public override string Message => "Operation is not valid due to Parent collection state";

      #endregion
    }

    #endregion
  }
}