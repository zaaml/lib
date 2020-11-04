// <copyright file="ChainConverter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Markup;

namespace Zaaml.PresentationCore.Converters
{
  [ContentProperty(nameof(Converters))]
  public sealed class ChainConverter : BaseValueConverter
  {
    #region Properties

    public List<IValueConverter> Converters { get; } = new List<IValueConverter>();

    #endregion

    #region  Methods

    protected override object ConvertBackCore(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return Converters.AsEnumerable().Reverse().Aggregate(value, (current, converter) => converter.Convert(current, targetType, parameter, culture));
    }

    protected override object ConvertCore(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return Converters.Aggregate(value, (current, converter) => converter.Convert(current, targetType, parameter, culture));
    }

    public static Type GetTargetType(IValueConverter converter)
    {
      return typeof(object);
    }

    public static void SetTargetType(IValueConverter converter, Type targetType)
    {
    }

    #endregion
  }
}