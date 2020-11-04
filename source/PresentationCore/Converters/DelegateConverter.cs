// <copyright file="DelegateConverter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Globalization;

namespace Zaaml.PresentationCore.Converters
{
  public sealed class DelegateConverter : BaseValueConverter
  {
    #region Fields

    private readonly Func<object, Type, object, CultureInfo, object> _directConverter;
    private readonly Func<object, Type, object, CultureInfo, object> _reverseConverter;

    #endregion

    #region Ctors

    public DelegateConverter(Func<object, Type, object, CultureInfo, object> directConverter, Func<object, Type, object, CultureInfo, object> reverseConverter = null)
    {
      _directConverter = directConverter;
      _reverseConverter = reverseConverter;
    }

    public DelegateConverter(Func<object, Type, object> directConverter, Func<object, Type, object> reverseConverter = null)
    {
      if (directConverter != null)
        _directConverter = (value, type, parameter, culture) => directConverter(value, type);

      if (reverseConverter != null)
        _reverseConverter = (value, type, parameter, culture) => reverseConverter(value, type);
    }

    public DelegateConverter(Func<object, object> directConverter, Func<object, object> reverseConverter = null)
    {
      if (directConverter != null)
        _directConverter = (value, type, parameter, culture) => directConverter(value);

      if (reverseConverter != null)
        _reverseConverter = (value, type, parameter, culture) => reverseConverter(value);
    }

    #endregion

    #region  Methods

    protected override object ConvertBackCore(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (_reverseConverter == null)
        throw new NotSupportedException();

      return _reverseConverter(value, targetType, parameter, culture);
    }

    protected override object ConvertCore(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (_directConverter == null)
        throw new NotSupportedException();

      return _directConverter(value, targetType, parameter, culture);
    }

    #endregion
  }
}