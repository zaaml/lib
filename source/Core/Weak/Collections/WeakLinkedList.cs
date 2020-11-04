// <copyright file="WeakLinkedList.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;

namespace Zaaml.Core.Weak.Collections
{
  internal class WeakLinkedList<T> : IEnumerable<T> where T : class
  {
    #region Fields

    private int _gcCount;
		private WeakLinkedNode<T> _head;
    private WeakLinkedNode<T> _tail;

    #endregion

    #region Properties

    public bool IsEmpty => _head == null;

    private bool IsGCOccurred => GarbageCleanupCounter.CleanupCount != _gcCount;

    #endregion

    #region  Methods

    public WeakLinkedNode<T> Add(T item)
    {
      EnsureClean();

      var node = WeakLinkedNode.Insert(ref _head, ref _tail, item);

      OnCollectionChanged();

      EnsureGC();

      return node;
    }

    public void Cleanup()
    {
      EnsureClean();
    }

    public void Clear()
    {
      if (_head == null)
        return;

      while (_head != null)
        _head = _head.CleanNext();

      _tail = null;
      _head = null;

      OnCollectionChanged();

      UpdateGCCounter();
    }

    protected virtual void OnCollectionChanged()
    {
    }

    public void Cleanup(Func<T, bool> predicate)
    {
	    if (_head == null)
		    return;

	    WeakLinkedNode.Clean(ref _head, out _tail, predicate);

	    OnCollectionChanged();
		}

    private void EnsureClean()
    {
      if (_head == null || IsGCOccurred == false)
        return;

      WeakLinkedNode.Clean(ref _head, out _tail);

      OnCollectionChanged();

      UpdateGCCounter();
    }

    private void EnsureGC()
    {
      if (IsGCOccurred)
        UpdateGCCounter();
    }

    public void Remove(T item)
    {
      EnsureClean();

      WeakLinkedNode.Remove(ref _head, ref _tail, item);

      OnCollectionChanged();

      EnsureGC();
    }

    public void Remove(WeakLinkedNode<T> node)
    {
      EnsureClean();

      WeakLinkedNode.RemoveNode(ref _head, ref _tail, node);

      OnCollectionChanged();

      EnsureGC();
    }

    private void UpdateGCCounter()
    {
      _gcCount = GarbageCleanupCounter.CleanupCount;
    }

    #endregion

    #region Interface Implementations

    #region IEnumerable

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    #endregion

    #region IEnumerable<T>

    public IEnumerator<T> GetEnumerator()
    {
      if (_head == null)
        yield break;

      EnsureClean();

      if (_head == null)
        yield break;

      foreach (var item in _head.EnumerateAlive(false))
        yield return item;
    }

    #endregion

    #endregion
  }
}