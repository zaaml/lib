// <copyright file="SkinTypeConverter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.ComponentModel;
using System.Globalization;

namespace Zaaml.PresentationCore.Theming
{
  public sealed class SkinTypeConverter : TypeConverter
  {
    #region  Methods

    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
      return sourceType == typeof(string);
    }

    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
	    return value switch
	    {
		    string strValue => new DeferSkin { Key = strValue },
		    _ => null
	    };
    }

    #endregion
  }
}