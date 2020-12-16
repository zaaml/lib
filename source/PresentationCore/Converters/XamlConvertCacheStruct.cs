// <copyright file="XamlConvertCacheStruct.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using Zaaml.PresentationCore.Extensions;

namespace Zaaml.PresentationCore.Converters
{
  internal struct XamlConvertCacheStruct
  {
    #region Fields

    private Type _cacheType;
    private object _convertedValueCache;
    private object _value;

    #endregion

    #region Ctors

    #endregion

    #region Properties

    public object Value
    {
      get => _value;
      set
      {
        if (ReferenceEquals(_value, value))
          return;

        _value = value;
        _cacheType = null;
      }
    }

    #endregion

    #region  Methods

    public object XamlConvert(Type targetType)
    {
      if (targetType == null)
        throw new ArgumentNullException(nameof(targetType));

      if (_cacheType == targetType)
        return _convertedValueCache;

      _convertedValueCache = _value.XamlConvert(targetType);
      _cacheType = targetType;

      return _convertedValueCache;
    }

    #endregion
  }

  internal struct XamlConvertCacheStruct<TValue>
  {
	  #region Fields

	  private Type _cacheType;
	  private TValue _convertedValueCache;
	  private TValue _value;

	  #endregion

	  #region Ctors

	  #endregion

	  #region Properties

	  public TValue Value
	  {
		  get => _value;
		  set
		  {
			  if (ReferenceEquals(_value, value))
				  return;

			  _value = value;
			  _cacheType = null;
		  }
	  }

	  #endregion

	  #region  Methods

	  public TValue XamlConvert(Type targetType)
	  {
		  if (targetType == null)
			  throw new ArgumentNullException(nameof(targetType));

		  if (_cacheType == targetType)
			  return _convertedValueCache;

		  _convertedValueCache = (TValue)_value.XamlConvert(targetType);
		  _cacheType = targetType;

		  return _convertedValueCache;
	  }

	  #endregion
  }
}