// <copyright file="StringConverter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Globalization;
using Zaaml.Core.Extensions;

namespace Zaaml.PresentationCore.Converters
{
  public sealed class StringConverter : BaseValueConverter
  {
    #region Static Fields and Constants

    public static readonly StringConverter IsNullOrEmpty = new StringConverter(Kind.IsNullOrEmpty);
    public static readonly StringConverter IsNullOrWhiteSpace = new StringConverter(Kind.IsNullOrWhiteSpace);

    #endregion

    #region Fields

    private readonly Kind _kind;

    #endregion

    #region Ctors

    private StringConverter(Kind kind)
    {
      _kind = kind;
    }

    #endregion

    #region  Methods

    protected override object ConvertBackCore(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new InvalidOperationException("StringConverter can only be used OneWay.");
    }


    protected override object ConvertCore(object value, Type targetType, object parameter, CultureInfo culture)
    {
      var stringValue = (string) value;

      switch (_kind)
      {
        case Kind.IsNullOrEmpty:
          return stringValue.IsNullOrEmpty() ? KnownBoxes.BoolTrue : KnownBoxes.BoolFalse;
        case Kind.IsNullOrWhiteSpace:
          return stringValue.IsNullOrWhiteSpace() ? KnownBoxes.BoolTrue : KnownBoxes.BoolFalse;
        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    #endregion

    #region  Nested Types

    private enum Kind
    {
      IsNullOrEmpty,
      IsNullOrWhiteSpace
    }

    #endregion
  }
}