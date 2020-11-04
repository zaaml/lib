// <copyright file="EnumConverter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) zaaml. All rights reserved.
// </copyright>

using System;
using System.ComponentModel;
using System.Globalization;

namespace Zaaml.Core.Converters
{
  internal class EnumTypeConverter : TypeConverter
  {
    #region Fields

    private readonly Type _enumType;

    #endregion

    #region Ctors

    public EnumTypeConverter(Type enumType)
    {
      _enumType = enumType;
    }

    #endregion

    #region Methods

    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
      return sourceType == typeof (string);
    }

    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
    {
      return destinationType == typeof (string);
    }

    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
      var stringValue = value as string;
      return stringValue != null ? Enum.Parse(_enumType, stringValue, true) : base.ConvertFrom(context, culture, value);
    }

    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value,
      Type destinationType)
    {
      if (destinationType == typeof (string) && value.GetType() == destinationType)
        return value.ToString();

      return base.ConvertTo(context, culture, value, destinationType);
    }

    #endregion
  }
}