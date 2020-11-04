// <copyright file="EqualConverter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Globalization;

namespace Zaaml.PresentationCore.Converters
{
  public sealed class EqualConverter : BaseValueConverter
  {
    #region Properties

    public object FalseResult { get; set; }

    public object TrueResult { get; set; }

    #endregion

    #region  Methods

    protected override object ConvertBackCore(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotSupportedException();
    }

    protected override object ConvertCore(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return Equals(value, parameter) ? TrueResult : FalseResult;
    }

    #endregion
  }
}