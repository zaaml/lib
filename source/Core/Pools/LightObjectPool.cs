// <copyright file="LightObjectPool.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;

namespace Zaaml.Core.Pools
{
  internal sealed class LightObjectPool<T> where T : class
  {
    #region Fields

    private readonly Action<T> _cleaner;
    private readonly Func<T> _creator;
    private readonly Action<T> _initializer;
    private readonly List<T> _pool = new List<T>();

    #endregion

    #region Ctors

    public LightObjectPool(Func<T> creator) : this(creator, null, null)
    {
    }

    public LightObjectPool(Func<T> creator, Action<T> initializer, Action<T> cleaner)
    {
      _creator = creator;
      _initializer = initializer;
      _cleaner = cleaner;
    }

    #endregion

    #region Properties

    public int InstanceCount { get; private set; }

    #endregion

    #region  Methods

    private T CreateObject()
    {
      var newObject = _creator();

      InstanceCount++;

      return newObject;
    }

    public T GetObject()
    {
      lock (_pool)
      {
        var obj = RemoveObject() ?? CreateObject();

        _initializer?.Invoke(obj);

        return obj;
      }
    }

    public void Release(T obj)
    {
      if (obj == null)
        throw new NullReferenceException();

      lock (_pool)
      {
        _cleaner?.Invoke(obj);

        _pool.Add(obj);
      }
    }

    private T RemoveObject()
    {
      if (_pool.Count <= 0) 
	      return null;

      var refThis = _pool.Last();

      _pool.RemoveAt(_pool.Count - 1);

      return refThis;
    }

    #endregion
  }
}