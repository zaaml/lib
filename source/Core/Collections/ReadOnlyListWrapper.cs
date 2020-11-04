// <copyright file="ReadOnlyListWrapper.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections;
using System.Collections.Generic;

namespace Zaaml.Core.Collections
{
  internal class ReadOnlyListWrapper<T> : IReadOnlyList<T>
  {
    #region Fields

    private readonly IList<T> _list;

    #endregion

    #region Ctors

    public ReadOnlyListWrapper(IList<T> list)
    {
      _list = list;
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
      return _list.GetEnumerator();
    }

    #endregion

    #region IReadOnlyCollection<T>

    public int Count => _list.Count;

    #endregion

    #region IReadOnlyList<T>

    public T this[int index] => _list[index];

    #endregion

    #endregion
  }
}