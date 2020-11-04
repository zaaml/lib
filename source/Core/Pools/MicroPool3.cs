// <copyright file="MicroPool3.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Core.Pools
{
  internal class MicroPool3<T> where T : class
  {
    #region Fields

    private T _first;
    private T _second;
    private T _third;

    #endregion

    #region  Methods

    public T Mount()
    {
      var result = _first;

      if (result != null)
      {
        _first = null;

        return result;
      }

      result = _second;

      if (result != null)
      {
        _second = null;

        return result;
      }

      result = _third;

      if (result != null)
      {
        _third = null;

        return result;
      }

      return null;
    }

    public void Release(T effectiveValue)
    {
      if (_first == null)
        _first = effectiveValue;
      else if (_second == null)
        _second = effectiveValue;
      else
        _third = effectiveValue;
    }

    #endregion
  }
}