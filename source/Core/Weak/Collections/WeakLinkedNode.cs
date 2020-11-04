// <copyright file="WeakLinkedNode.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;

namespace Zaaml.Core.Weak.Collections
{
  internal class WeakLinkedNode<T> : WeakReference<T> where T : class
  {
    #region Ctors

    public WeakLinkedNode(T target) : base(target)
    {
    }

    public WeakLinkedNode(T target, bool trackResurrection) : base(target, trackResurrection)
    {
    }

    #endregion

    #region Properties

    public WeakLinkedNode<T> Next { get; private set; }

    #endregion

    #region  Methods

    internal static WeakLinkedNode<T> CleanImpl(WeakLinkedNode<T> head, out WeakLinkedNode<T> tail, Func<T, bool> predicate = null)
    {
      var aliveHead = head;

      while (aliveHead != null && (aliveHead.IsAlive == false || predicate != null && predicate(aliveHead.Target)))
        aliveHead = aliveHead.Next;

      if (aliveHead == null)
      {
        tail = null;

        return null;
      }

      var currentAlive = aliveHead;
      var aliveTail = aliveHead;

      // Find tail removing dead items
      while (currentAlive != null)
      {
        var current = currentAlive.Next;

        while (current != null && (current.IsAlive == false || predicate != null && predicate(current.Target)))
          current = current.Next;

        currentAlive.Next = current;
        aliveTail = currentAlive;
        currentAlive = current;
      }

      tail = aliveTail;

      return aliveHead;
    }

    internal static WeakLinkedNode<T> CleanImpl(WeakLinkedNode<T> head)
    {
	    return CleanImpl(head, out _);
    }

    public IEnumerable<T> EnumerateAlive(bool clean)
    {
      var aliveHead = this;

      if (clean)
        aliveHead = CleanImpl(aliveHead);

      var current = aliveHead;

      if (aliveHead == null)
        yield break;

      while (current != null)
      {
        var currentTarget = current.Target;

        if (currentTarget != null)
          yield return currentTarget;

        current = current.Next;
      }
    }

    public void InsertAfter(WeakLinkedNode<T> node)
    {
      Next = node;
    }

    public void RemoveAfter()
    {
      var remove = Next;

      if (remove != null)
      {
        Next = remove.Next;
        remove.Next = null;
      }
      else
        Next = null;
    }

    public override string ToString()
    {
      var target = Target;

      return target == null ? "Dead" : Target.ToString();
    }

    public WeakLinkedNode<T> CleanNext()
    {
      var next = Next;

      Next = null;

      return next;
    }

    #endregion
  }

  internal static class WeakLinkedNode
  {
    #region  Methods

    public static void Clean<T>(ref WeakLinkedNode<T> head, out WeakLinkedNode<T> tail) where T : class
    {
      head = WeakLinkedNode<T>.CleanImpl(head, out tail);
    }

    public static void Clean<T>(ref WeakLinkedNode<T> head, out WeakLinkedNode<T> tail, Func<T, bool> predicate) where T : class
    {
      head = WeakLinkedNode<T>.CleanImpl(head, out tail, predicate);
    }

    public static WeakLinkedNode<T> Create<T>(T item) where T : class
    {
      return new WeakLinkedNode<T>(item);
    }

    public static WeakLinkedNode<T> Insert<T>(ref WeakLinkedNode<T> head, ref WeakLinkedNode<T> tail, T item) where T : class
    {
      var node = Create(item);

      if (head == null)
        head = tail = node;
      else
      {
        tail.InsertAfter(node);
        tail = node;
      }

      return node;
    }

    public static void RemoveNode<T>(ref WeakLinkedNode<T> head, ref WeakLinkedNode<T> tail, WeakLinkedNode<T> node) where T : class
    {
      //node.EnsureNotNull(nameof(node));

      //if (head == null)
      //{
      //  tail = null;
      //  return;
      //}

      //if (ReferenceEquals(head, node))
      //{
      //  if (ReferenceEquals(head, tail))
      //    head = tail = head.Next;
      //  else
      //    head = head.Next;

      //  return;
      //}

      //if (ReferenceEquals(node.Next, tail))
      //  tail = node;

      //node.RemoveAfter();

      var target = node.Target;

      if (target != null)
        Remove(ref head, ref tail, target);
    }

    public static void Remove<T>(ref WeakLinkedNode<T> head, ref WeakLinkedNode<T> tail, T item) where T : class
    {
      item.EnsureNotNull(nameof(item));

      if (head == null)
      {
        tail = null;

        return;
      }

      if (ReferenceEquals(head.Target, item))
      {
        if (ReferenceEquals(head, tail))
          head = tail = head.Next;
        else
          head = head.Next;

        return;
      }

      var current = head;

      while (current.Next != null && ReferenceEquals(current.Next.Target, item) == false)
        current = current.Next;

      if (current.Next == null || !ReferenceEquals(current.Next.Target, item)) 
	      return;

      if (ReferenceEquals(current.Next, tail))
        tail = current;

      current.RemoveAfter();
    }

    #endregion
  }
}