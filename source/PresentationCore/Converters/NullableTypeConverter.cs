// <copyright file="NullableTypeConverter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.ComponentModel;
using System.Globalization;

namespace Zaaml.PresentationCore.Converters
{
  public class NullableTypeConverter<T> : TypeConverter
    where T : struct
  {
    #region  Methods

    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
      return sourceType == typeof(string);
    }

    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
      var stringValue = value as string;
      if (string.IsNullOrEmpty(stringValue)) return null;

      var converted = (T) Convert.ChangeType(value, typeof(T), culture);

      return (T?) converted;
    }

    #endregion
  }
}