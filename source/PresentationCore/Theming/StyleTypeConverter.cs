// <copyright file="StyleTypeConverter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.ComponentModel;
using System.Globalization;

namespace Zaaml.PresentationCore.Theming
{
  public sealed class StyleTypeConverter : TypeConverter
  {
    #region  Methods

    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
    {
      return typeof(System.Windows.Style).IsAssignableFrom(destinationType);
    }

    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
    {
      return ((StyleBase) value).StyleService.NativeStyle;
    }

    #endregion
  }
}