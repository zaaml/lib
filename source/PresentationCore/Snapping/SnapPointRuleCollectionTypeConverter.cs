// <copyright file="SnapPointRuleCollectionTypeConverter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.ComponentModel;
using System.Globalization;

namespace Zaaml.PresentationCore.Snapping
{
  internal class SnapPointRuleCollectionTypeConverter : TypeConverter
  {
    #region  Methods

    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
      return sourceType == typeof(string);
    }

    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
      var strValue = value as string;
      if (strValue == null)
        throw new InvalidOperationException();

      return SnapPointRuleCollection.Parse(strValue);
    }

    #endregion
  }
}