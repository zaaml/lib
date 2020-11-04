// <copyright file="ScaleConverter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Globalization;

namespace Zaaml.PresentationCore.Converters
{
  public sealed class ScaleConverter : BaseValueConverter
  {
    #region Static Fields and Constants

    public static readonly ScaleConverter Instance = new ScaleConverter();

    #endregion

    #region  Methods

    protected override object ConvertBackCore(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }


    protected override object ConvertCore(object value, Type targetType, object parameter, CultureInfo culture)
    {
      var scale = 1.0;
      if (parameter is double)
        scale = (double) parameter;
      else if (parameter is int)
        scale = (int) parameter;
      else if (parameter is string)
        double.TryParse((string) parameter, out scale);

      return (double) value * scale;
    }

    #endregion
  }
}