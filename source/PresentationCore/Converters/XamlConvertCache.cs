// <copyright file="XamlConvertCache.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.PresentationCore.Converters
{
  internal sealed class XamlConvertCache
  {
    #region Fields

    private ConvertCacheStore _cacheStore;

    #endregion

    #region Ctors

    public XamlConvertCache(object value)
    {
      _cacheStore = new ConvertCacheStore(value);
    }

    public XamlConvertCache(object value, Type targetType)
    {
      _cacheStore = new ConvertCacheStore(value);
      XamlStaticConverter.ConvertCache(ref _cacheStore, targetType);
    }

    #endregion

    #region Properties

    public object OriginalValue => _cacheStore.Value;

    public object Value => _cacheStore.CachedValue;

    #endregion

    #region  Methods

    public object Convert(Type targetType)
    {
      return XamlStaticConverter.ConvertCache(ref _cacheStore, targetType);
    }

    #endregion
  }
}