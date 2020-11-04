// <copyright file="StringCollectionTypeConverter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;

namespace Zaaml.Core.Converters
{
  public class StringCollectionTypeConverter : TypeConverter
  {
    #region Static Fields and Constants

    private static readonly char[] Delimiters = {',', ' '};

    #endregion

    #region  Methods

    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
      return sourceType == typeof(string);
    }

    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
      var strValue = value as string;
      if (strValue == null)
        throw new InvalidOperationException("Expected string value");

      var result = new StringCollection();
      result.AddRange(strValue.Split(Delimiters, StringSplitOptions.RemoveEmptyEntries));
      return result;
    }

    #endregion
  }
}