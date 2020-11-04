// <copyright file="SafeImageSourceConverter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Media;
using Zaaml.Core;

namespace Zaaml.PresentationCore.TypeConverters
{
  internal class SafeImageSourceConverter : TypeConverter
  {
    #region Static Fields and Constants

    private static readonly ImageSourceConverter BaseConverter = new ImageSourceConverter();
    internal static readonly SafeImageSourceConverter Converter = new SafeImageSourceConverter();

    #endregion

    #region  Methods

    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
      try
      {
        return BaseConverter.ConvertFrom(context, culture, value);
      }
      catch (Exception e)
      {
        LogService.LogError(e);
        return null;
      }
    }

    #endregion
  }
}