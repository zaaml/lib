// <copyright file="ValueConverters.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Zaaml.Core.Converters;
using Zaaml.Core.Utils;
using Zaaml.PresentationCore.Extensions;
#if SILVERLIGHT
using System.ComponentModel.DataAnnotations;
#else
using System.Windows.Controls;

#endif

namespace Zaaml.PresentationCore.Converters
{
  public static class ValueConverters
  {
    #region Static Fields and Constants

    public static readonly IValueConverter ColorBrushConverter;
    public static readonly IValueConverter ColorStringConverter;
    public static readonly IValueConverter StringDoubleConverter;
    public static readonly IValueConverter StringIntConverter;
    public static readonly IValueConverter StringShortConverter;
    public static readonly IValueConverter StringDecimalConverter;

    public static readonly IValueConverter FalseVisibilityCollapsedConverter = VisibilityConverter.FalseToCollapsedVisibility;
    public static readonly IValueConverter TrueVisibilityCollapsedConverter = VisibilityConverter.TrueToCollapsedVisibility;

    public static readonly IValueConverter MathRoundConverter = new DelegateConverter(v => Math.Round((double) v), v => Math.Round((double) v));
    public static readonly IValueConverter SolidColorBrushCloneConverter = new DelegateConverter(v => (v as SolidColorBrush)?.CloneBrush()?.AsFrozen());

    public static readonly IValueConverter TextTrimConverter = new DelegateConverter(v => (v as string)?.Trim() ?? v);

    public static readonly IValueConverter BoolConverter = Converters.BoolConverter.Instance;

    #endregion

    #region Ctors

    static ValueConverters()
    {
      PrimitiveConverterFactory.RegisterConverter(new ColorSolidColorBrushConverter());
      PrimitiveConverterFactory.RegisterConverter(new ColorBrushConverter());
      PrimitiveConverterFactory.RegisterConverter(new StringBrushConverter());
      PrimitiveConverterFactory.RegisterConverter(new StringSolidColorBrushConverter());
      PrimitiveConverterFactory.RegisterConverter(new SolidColorBrushColorConverter());
      PrimitiveConverterFactory.RegisterConverter(new SolidColorBrushStringConverter());
      PrimitiveConverterFactory.RegisterConverter(new ColorStringConverter());
      PrimitiveConverterFactory.RegisterConverter(new StringColorConverter());

      PrimitiveConverterFactory.RegisterConverter(new StringGeometryConverter());

      PrimitiveConverterFactory.RegisterConverter(new StringFontFamilyConverter());
      PrimitiveConverterFactory.RegisterConverter(new StringFontStyleConverter());
      PrimitiveConverterFactory.RegisterConverter(new StringFontWeightConverter());
      PrimitiveConverterFactory.RegisterConverter(new StringFontStretchConverter());
      
      PrimitiveConverterFactory.RegisterConverter(new FontFamilyStringConverter());
      PrimitiveConverterFactory.RegisterConverter(new FontStyleStringConverter());
      PrimitiveConverterFactory.RegisterConverter(new FontWeightStringConverter());
      PrimitiveConverterFactory.RegisterConverter(new GridLengthDoubleConverter());
      PrimitiveConverterFactory.RegisterConverter(new DoubleGridLengthConverter());

      ColorBrushConverter = CreateXamlIValueConverter<Color, SolidColorBrush>();
      ColorStringConverter = CreateXamlIValueConverter<Color, string>();
      StringDoubleConverter = CreateXamlIValueConverter<string, double>();
      StringIntConverter = CreateXamlIValueConverter<string, int>();
      StringShortConverter = CreateXamlIValueConverter<string, short>();
      StringDecimalConverter = CreateXamlIValueConverter<string, decimal>();
    }

    #endregion

    #region  Methods

    private static DelegateConverter CreateXamlIValueConverter<TInput, TOutput>()
    {
      var directConverter = PrimitiveConverterFactory.GetConverter<TInput, TOutput>();
      var reverseConverter = PrimitiveConverterFactory.GetConverter<TOutput, TInput>();

      if (directConverter == null || reverseConverter == null)
        throw new Exception("Could not find appropriate converter");

      return new DelegateConverter((value, type) => SafeConverter(value, type, directConverter),
        (value, type) => SafeConverter(value, type, reverseConverter));
    }

    private static object SafeConverter(object value, Type targetType, IPrimitiveConverter converter)
    {
      try
      {
        if (value == null)
	        return TargetNullValueConverter.Instance.Convert(null, targetType, null, CultureInfo.InvariantCulture);

        if (value.GetType() != converter.FromType || targetType.IsAssignableFrom(converter.ToType) == false)
          throw new Exception("Invalid value converter");

        return converter.Convert(value);
      }
      catch (Exception e)
      {
#if SILVERLIGHT
        return new ValidationResult(e.Message);
#else
        return new ValidationResult(false, e.Message);
#endif
      }
    }

    #endregion
  }

  internal class GenericTypeConverter<TTo> : TypeConverter
  {
    #region  Methods

    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
      var primitiveConverter = PrimitiveConverterFactory.GetConverter(sourceType, typeof(TTo));

      return primitiveConverter != null;
    }

    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
    {
      return false;
    }

    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
      var targetType = typeof(TTo);

      if (value == null)
        return RuntimeUtils.CreateDefaultValue(targetType);

      var sourceType = value.GetType();

      return PrimitiveConverterFactory.GetConverter(sourceType, targetType).Convert(value);
    }

    #endregion
  }
}