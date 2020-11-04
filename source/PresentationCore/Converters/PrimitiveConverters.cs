// <copyright file="PrimitiveConverters.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using Zaaml.Core.Converters;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.Utils;

namespace Zaaml.PresentationCore.Converters
{
  internal sealed class ColorSolidColorBrushConverter : DelegatePrimitiveConverter<Color, SolidColorBrush>
  {
    #region Ctors

    public ColorSolidColorBrushConverter()
      : base(color => color.ToSolidColorBrush(), null)
    {
    }

    #endregion
  }

  internal sealed class ColorBrushConverter : DelegatePrimitiveConverter<Color, Brush>
  {
    #region Ctors

    public ColorBrushConverter()
      : base(color => color.ToSolidColorBrush(), null)
    {
    }

    #endregion
  }

  internal sealed class StringBrushConverter : DelegatePrimitiveConverter<string, Brush>
  {
    #region Ctors

    public StringBrushConverter()
      : base(str => StringColorConverter.ConvertString(str).ToSolidColorBrush(), null)
    {
    }

    #endregion
  }

  internal sealed class StringSolidColorBrushConverter : DelegatePrimitiveConverter<string, SolidColorBrush>
  {
    #region Ctors

    public StringSolidColorBrushConverter()
      : base(ConvertString, ConvertString)
    {
    }

    #endregion

    #region  Methods

    public static SolidColorBrush ConvertString(string colorString)
    {
      return ConvertString(colorString, CultureInfo.CurrentCulture);
    }

    public static SolidColorBrush ConvertString(string colorString, IFormatProvider formatProvider)
    {
      return StringColorConverter.ConvertString(colorString, formatProvider).ToSolidColorBrush();
    }

    #endregion
  }

  internal sealed class SolidColorBrushColorConverter : DelegatePrimitiveConverter<SolidColorBrush, Color>
  {
    #region Ctors

    public SolidColorBrushColorConverter()
      : base(brush => brush.Color, null)
    {
    }

    #endregion
  }

  internal sealed class SolidColorBrushStringConverter : DelegatePrimitiveConverter<SolidColorBrush, string>
  {
    #region Ctors

    public SolidColorBrushStringConverter()
      : base(ConvertBrush, ConvertBrush)
    {
    }

    #endregion

    #region  Methods

    public static string ConvertBrush(SolidColorBrush brush)
    {
      return ConvertBrush(brush, CultureInfo.CurrentCulture);
    }

    public static string ConvertBrush(SolidColorBrush brush, IFormatProvider formatProvider)
    {
      return ColorStringConverter.ConvertColor(brush.Color, formatProvider);
    }

    #endregion
  }

  internal sealed class ColorStringConverter : DelegatePrimitiveConverter<Color, string>
  {
    #region Ctors

    public ColorStringConverter()
      : base(ConvertColor, ConvertColor)
    {
    }

    #endregion

    #region  Methods

    public static string ConvertColor(Color color)
    {
      return ConvertColor(color, CultureInfo.CurrentCulture);
    }

    public static string ConvertColor(Color color, IFormatProvider formatProvider)
    {
	    return KnownColors.TryGetColorName(color, out var colorName) ? colorName : color.ToString(formatProvider);
    }

    #endregion
  }

  internal sealed class StringColorConverter : DelegatePrimitiveConverter<string, Color>
  {
    #region Ctors

    public StringColorConverter()
      : base(ConvertString, ConvertString)
    {
    }

    #endregion

    #region  Methods

    public static Color ConvertString(string colorString)
    {
      return ColorUtils.ConvertFromString(colorString);
    }

    public static Color ConvertString(string colorString, IFormatProvider formatProvider)
    {
      return ColorUtils.ConvertFromString(colorString, formatProvider);
    }

    #endregion
  }

  internal sealed class StringFontFamilyConverter : DelegatePrimitiveConverter<string, FontFamily>
  {
    #region Ctors

    public StringFontFamilyConverter() : base(s => new FontFamily(s), null)
    {
    }

    #endregion
  }


  internal abstract class StringFontPropertyConverter<T> : DelegatePrimitiveConverter<string, T>
  {
    #region Static Fields and Constants

    private static readonly Dictionary<string, T> Dictionary = new Dictionary<string, T>(StringComparer.OrdinalIgnoreCase);

    #endregion

    #region Ctors

    protected StringFontPropertyConverter() : base(ConvertString, null)
    {
    }

    #endregion

    #region  Methods

    protected static void InitDictionary(Type valuesType)
    {
      var type = typeof(T);

      foreach (var fieldInfo in valuesType.GetFields(BindingFlags.Static | BindingFlags.Public).Where(f => f.FieldType == type))
        Dictionary[fieldInfo.Name] = (T) fieldInfo.GetValue(null);

      foreach (var propertyInfo in valuesType.GetProperties(BindingFlags.Static | BindingFlags.Public).Where(p => p.PropertyType == type))
        Dictionary[propertyInfo.Name] = (T)propertyInfo.GetValue(null, null);
    }

    public static T ConvertString(string fontWeightString)
    {
      return Dictionary[fontWeightString];
    }

    #endregion
  }


  internal sealed class StringFontWeightConverter : StringFontPropertyConverter<FontWeight>
  {
    #region Ctors

    static StringFontWeightConverter()
    {
      InitDictionary(typeof(FontWeights));
    }

    #endregion
  }

  internal sealed class StringFontStretchConverter : StringFontPropertyConverter<FontStretch>
  {
    #region Ctors

    static StringFontStretchConverter()
    {
      InitDictionary(typeof(FontStretches));
    }

    #endregion
  }

  internal sealed class StringFontStyleConverter : DelegatePrimitiveConverter<string, FontStyle>
  {
    private static readonly string FontStylesNormal = FontStyles.Normal.ToString();
    private static readonly string FontStylesItalic = FontStyles.Italic.ToString();

    public StringFontStyleConverter() : base(ConvertString, null)
    {
    }

    private static FontStyle ConvertString(string fontStyleString)
    {
      if (string.Equals(fontStyleString, FontStylesNormal, StringComparison.OrdinalIgnoreCase))
        return FontStyles.Normal;

      if (string.Equals(fontStyleString, FontStylesItalic, StringComparison.OrdinalIgnoreCase))
        return FontStyles.Italic;

      throw new InvalidOperationException();
    }
  }

  internal sealed class StringGeometryConverter : DelegatePrimitiveConverter<string, Geometry>
  {
    private static readonly Path PathElement = new Path();

    public StringGeometryConverter() : base(ConvertString, null)
    {
    }

    internal static Geometry ConvertString(string geometryString)
    {
      PathElement.SetBinding(Path.DataProperty, new Binding { Source = geometryString, BindsDirectlyToSource = true, Mode = BindingMode.OneTime });

      var geometry = PathElement.Data;

      PathElement.ClearValue(Path.DataProperty);

      return geometry;
    }
  }

  internal sealed class FontFamilyStringConverter : DelegatePrimitiveConverter<FontFamily, string>
  {
    #region Ctors

    public FontFamilyStringConverter() : base(f => f.Source, null)
    {
    }

    #endregion
  }

  internal sealed class FontStyleStringConverter : DelegatePrimitiveConverter<FontStyle, string>
  {
    #region Ctors

    public FontStyleStringConverter() : base(f => f.ToString(), null)
    {
    }

    #endregion
  }

  internal sealed class FontWeightStringConverter : DelegatePrimitiveConverter<FontWeight, string>
  {
    #region Ctors

    public FontWeightStringConverter() : base(f => f.ToString(), null)
    {
    }

    #endregion
  }

  internal sealed class FontStretchStringConverter : DelegatePrimitiveConverter<FontStretch, string>
  {
    #region Ctors

    public FontStretchStringConverter() : base(f => f.ToString(), null)
    {
    }

    #endregion
  }

	internal sealed class GridLengthDoubleConverter : DelegatePrimitiveConverter<GridLength, double>
	{
		public GridLengthDoubleConverter() : base(g => g.Value, null)
		{
		}
	}

	internal sealed class DoubleGridLengthConverter : DelegatePrimitiveConverter<double, GridLength>
	{
		public DoubleGridLengthConverter() : base(d => new GridLength(d), null)
		{
		}
	}
}