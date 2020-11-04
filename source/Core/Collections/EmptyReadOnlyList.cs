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
      throw AcessException.ExceptionInstance;
    }

    public void Clear()
    {
      throw AcessException.ExceptionInstance;
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
      throw AcessException.ExceptionInstance;
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
      throw AcessException.ExceptionInstance;
    }

    public void Remove(object value)
    {
      throw AcessException.ExceptionInstance;
    }

    object IList.this[int index]
    {
      get => throw new ArgumentOutOfRangeException(nameof(index));
      set => throw AcessException.ExceptionInstance;
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
      throw AcessException.ExceptionInstance;
    }

    public void RemoveAt(int index)
    {
      throw AcessException.ExceptionInstance;
    }

    public T this[int index]
    {
      get => throw new ArgumentOutOfRangeException(nameof(index));
      set => throw AcessException.ExceptionInstance;
    }

    #endregion

    #endregion

    #region  Nested Types

    private class AcessException : Exception
    {
      #region Static Fields and Constants

      public static readonly Exception ExceptionInstance = new AcessException();

      #endregion

      #region Ctors

      private AcessException()
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