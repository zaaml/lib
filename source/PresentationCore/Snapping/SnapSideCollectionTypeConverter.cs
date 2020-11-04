// <copyright file="SnapSideCollectionTypeConverter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.ComponentModel;
using System.Globalization;

namespace Zaaml.PresentationCore.Snapping
{
  internal class SnapSideCollectionTypeConverter : TypeConverter
  {
    #region Static Fields and Constants

    private static readonly char[] Separators = {',', ' '};

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
        throw new InvalidOperationException();

      var snapSideCollection = new SnapSideCollection();

      var delimitedValues = strValue.Split(Separators, StringSplitOptions.RemoveEmptyEntries);

      foreach (var snapSideStr in delimitedValues)
      {
        SnapSide snapSide;

        if (Enum.TryParse(snapSideStr, true, out snapSide))
          snapSideCollection.Add(snapSide);
        else
          throw new InvalidOperationException($"Can not convert string value '{snapSideStr}' to value of type SnapSide");
      }

      return snapSideCollection;
    }

    #endregion
  }
}