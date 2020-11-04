// <copyright file="FlagConverterBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Globalization;
using Zaaml.Core;

namespace Zaaml.PresentationCore.Converters
{
  public abstract class FlagConverterBase : BaseValueConverter
  {
    #region  Methods

    protected override object ConvertBackCore(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotSupportedException("Only forward conversion supported.");
    }

    protected override object ConvertCore(object value, Type targetType, object parameter, CultureInfo culture)
    {
      try
      {
        return GetValue(((int) value & (int) parameter) != 0);
      }
      catch (Exception ex)
      {
        LogService.LogError(ex);
      }

      return GetValue(false);
    }

    protected abstract object GetValue(bool value);

    #endregion
  }
}