// <copyright file="LinqExtensions.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using Zaaml.Core.Collections;

namespace Zaaml.Core.Extensions
{
  public static class LinqExtensions
  {
    #region  Methods

    public static IEnumerable<T> Concat<T>(this IEnumerable<T> collection, params T[] values)
    {
      foreach (var element in collection)
        yield return element;

      foreach (var value in values)
        yield return value;
    }

    public static IEnumerable<T> Concat<T>(this IEnumerable<T> collection, params IEnumerable<T>[] collections)
    {
      foreach (var element in collection)
        yield return element;

      foreach (var element in collections.SelectMany(c => c))
        yield return element;
    }

#if false
		public static IEnumerable<T> Prepend<T>(this IEnumerable<T> collection, T value)
    {
      yield return value;

      foreach (var element in collection)
        yield return element;
    }
#endif
		
    public static IEnumerable<T> Enumerate<T>(params T[] values)
    {
      return values;
    }

    internal static IEnumerable<TElement> ExceptElement<TElement>(this IEnumerable<TElement> source, TElement element)
    {
      return ExceptElement(source, element, EqualityComparer<TElement>.Default);
    }

    internal static IEnumerable<TElement> ExceptElement<TElement>(this IEnumerable<TElement> source, TElement element, IEqualityComparer<TElement> comparer)
    {
      return source.Where(e => comparer.Equals(e, element) == false);
    }

    public static int FindIndex<T>(this IEnumerable<T> items, Func<T, bool> predicate)
    {
      if (items == null) throw new ArgumentNullException(nameof(items));
      if (predicate == null) throw new ArgumentNullException(nameof(predicate));

      var retVal = 0;
      foreach (var item in items)
      {
        if (predicate(item))
          return retVal;

        retVal++;
      }
      return -1;
    }

    public static TElement FirstMaxElementOrDefault<TElement, TValue>(this IEnumerable<TElement> source, Func<TElement, TValue> selector,
      IComparer<TValue> comparer)
    {
      return MinMaxElementOrDefault(source, selector, comparer, i => i >= 0);
    }

    public static TElement FirstMaxElementOrDefault<TElement, TValue>(this IEnumerable<TElement> source, Func<TElement, TValue> selector)
    {
      return MinMaxElementOrDefault(source, selector, Comparer<TValue>.Default, i => i >= 0);
    }

    public static TElement FirstMinElementOrDefault<TElement, TValue>(this IEnumerable<TElement> source, Func<TElement, TValue> selector,
      IComparer<TValue> comparer)
    {
      return MinMaxElementOrDefault(source, selector, comparer, i => i <= 0);
    }

    public static TElement FirstMinElementOrDefault<TElement, TValue>(this IEnumerable<TElement> source, Func<TElement, TValue> selector)
    {
      return MinMaxElementOrDefault(source, selector, Comparer<TValue>.Default, i => i <= 0);
    }

    public static TSource FirstOrDefault<TSource>(this IEnumerable<TSource> source, TSource defaultValue)
    {
      if (source == null)
        throw new ArgumentNullException(nameof(source));

      var list = source as IList<TSource>;
      if (list != null)
      {
        if (list.Count > 0)
          return list[0];
      }
      else
      {
        using (var enumerator = source.GetEnumerator())
        {
          if (enumerator.MoveNext())
            return enumerator.Current;
        }
      }
      return defaultValue;
    }

    public static TSource FirstOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate, TSource defaultValue)
    {
      if (source == null)
        throw new ArgumentNullException(nameof(source));
      if (predicate == null)
        throw new ArgumentNullException(nameof(predicate));

      foreach (var element in source)
      {
        if (predicate(element))
          return element;
      }

      return defaultValue;
    }

    internal static IEnumerable<T> FlattenLeaf<T>(this IEnumerable<T> collection, Func<T, IEnumerable<T>> flattener)
    {
      var queue = new Queue<T>();

      foreach (var item in collection)
      {
        queue.Enqueue(item);

        do
        {
          var current = queue.Dequeue();

          var children = flattener(current);

          if (children == null)
          {
            yield return current;
            continue;
          }

          foreach (var child in flattener(current))
            queue.Enqueue(child);
        } while (queue.Count > 0);
      }
    }

    public static int IndexOf<T>(this IEnumerable<T> items, T item)
    {
      return items.FindIndex(i => EqualityComparer<T>.Default.Equals(item, i));
    }

    public static int IndexOf<T>(this IEnumerable<T> items, T item, EqualityComparer<T> comparer)
    {
      return items.FindIndex(i => comparer.Equals(item, i));
    }

    public static int IndexOfReference<T>(this IEnumerable<T> items, T item) where T : class
    {
      return items.FindIndex(i => ReferenceEquals(item, i));
    }

    public static TElement LastMaxElementOrDefault<TElement, TValue>(this IEnumerable<TElement> source, Func<TElement, TValue> selector,
      IComparer<TValue> comparer)
    {
      return MinMaxElementOrDefault(source, selector, comparer, i => i > 0);
    }

    public static TElement LastMaxElementOrDefault<TElement, TValue>(this IEnumerable<TElement> source, Func<TElement, TValue> selector)
    {
      return MinMaxElementOrDefault(source, selector, Comparer<TValue>.Default, i => i > 0);
    }

    public static TElement LastMinElementOrDefault<TElement, TValue>(this IEnumerable<TElement> source, Func<TElement, TValue> selector,
      IComparer<TValue> comparer)
    {
      return MinMaxElementOrDefault(source, selector, comparer, i => i < 0);
    }

    public static TElement LastMinElementOrDefault<TElement, TValue>(this IEnumerable<TElement> source, Func<TElement, TValue> selector)
    {
      return MinMaxElementOrDefault(source, selector, Comparer<TValue>.Default, i => i < 0);
    }

    public static TSource LastOrDefault<TSource>(this IEnumerable<TSource> source, TSource defaultValue)
    {
      if (source == null)
        throw new ArgumentNullException(nameof(source));

      var list = source as IList<TSource>;
      if (list != null)
      {
        var count = list.Count;
        if (count > 0)
          return list[count - 1];
      }
      else
      {
        using (var enumerator = source.GetEnumerator())
        {
          if (!enumerator.MoveNext()) return defaultValue;

          TSource current;

          do current = enumerator.Current; while (enumerator.MoveNext());

          return current;
        }
      }
      return defaultValue;
    }

    public static TSource LastOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate, TSource defaultValue)
    {
      if (source == null)
        throw new ArgumentNullException(nameof(source));
      if (predicate == null)
        throw new ArgumentNullException(nameof(predicate));

      var result = defaultValue;

      foreach (var element in source)
      {
        if (predicate(element))
          result = element;
      }

      return result;
    }

    public static TElement LeftOfFirstOrDefault<TElement>(this IEnumerable<TElement> items, TElement item)
    {
      return LeftOfFirstOrDefault(items, item, EqualityComparer<TElement>.Default);
    }

    public static TElement LeftOfFirstOrDefault<TElement>(this IEnumerable<TElement> items, TElement item, IEqualityComparer<TElement> comparer)
    {
      var prev = default(TElement);
      foreach (var i in items)
      {
        if (comparer.Equals(item, i))
          return prev;

        prev = i;
      }

      return default(TElement);
    }

    private static TElement MinMaxElementOrDefault<TElement, TValue>(this IEnumerable<TElement> source, Func<TElement, TValue> selector,
      IComparer<TValue> comparer, Func<int, bool> trigger)
    {
      using (var enumerator = source.GetEnumerator())
      {
        if (enumerator.MoveNext() == false)
          return default(TElement);

        var element = enumerator.Current;
        var value = selector(element);

        while (enumerator.MoveNext())
        {
          var current = enumerator.Current;
          var currentValue = selector(current);
          if (trigger(comparer.Compare(value, currentValue))) continue;

          element = current;
          value = currentValue;
        }

        return element;
      }
    }

    public static IEnumerable<int> Range(Range<int> range)
    {
      return Enumerable.Range(range.Minimum, range.Maximum - range.Minimum + 1);
    }

    public static TElement RightOfFirstOrDefault<TElement>(this IEnumerable<TElement> items, TElement item)
    {
      return RightOfFirstOrDefault(items, item, EqualityComparer<TElement>.Default);
    }

    public static TElement RightOfFirstOrDefault<TElement>(this IEnumerable<TElement> items, TElement item, IEqualityComparer<TElement> comparer)
    {
      var found = false;
      foreach (var i in items)
      {
        if (comparer.Equals(item, i))
          found = true;
        else if (found)
          return i;
      }

      return default(TElement);
    }

    public static IEnumerable<T> SkipNull<T>(this IEnumerable<T> collection) where T : class
    {
      return collection.Where(e => e != null);
    }

    internal static MultiMap<TKey, TOutput> ToMultiMap<TInput, TKey, TOutput>(this IEnumerable<TInput> source, Func<TInput, TKey> keySelector,
      Func<TInput, TOutput> elementSelector)
    {
      var multiMap = new MultiMap<TKey, TOutput>();
      foreach (var input in source)
      {
        var key = keySelector(input);
        var value = elementSelector(input);
        multiMap.AddValue(key, value);
      }

      return multiMap;
    }

    public static IEnumerable<TElement> WhereNot<TElement>(this IEnumerable<TElement> source, Func<TElement, bool> filter)
    {
      return source.Where(e => filter(e) == false);
    }

    #endregion
  }
}