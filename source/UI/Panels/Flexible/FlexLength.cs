// <copyright file="FlexLength.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using Zaaml.Core.Extensions;
using Zaaml.Core.Utils;

#if !SILVERLIGHT
using System.ComponentModel.Design.Serialization;
#endif

namespace Zaaml.UI.Panels.Flexible
{
  public readonly struct FlexLength : IEquatable<FlexLength>
  {
    private readonly double _unitValue;

    public bool IsAbsolute => UnitType == FlexLengthUnitType.Pixel;

    public bool IsAuto => UnitType == FlexLengthUnitType.Auto;

    public bool IsStar => UnitType == FlexLengthUnitType.Star;

    public double Value => UnitType != FlexLengthUnitType.Auto ? _unitValue : 1.0;

    public FlexLengthUnitType UnitType { get; }

    public static FlexLength Auto { get; } = new FlexLength(1.0, FlexLengthUnitType.Auto);

    public static FlexLength Star { get; } = new FlexLength(1.0, FlexLengthUnitType.Star);

    public FlexLength(double pixels)
    {
      this = new FlexLength(pixels, FlexLengthUnitType.Pixel);
    }

    public FlexLength(double value, FlexLengthUnitType type)
    {
      //if (DoubleUtils.IsNaN(value))
      //  throw new ArgumentException(System.Windows.SR.Get("InvalidCtorParameterNoNaN", (object) "value"));

      //if (double.IsInfinity(value))
      //  throw new ArgumentException(System.Windows.SR.Get("InvalidCtorParameterNoInfinity", (object) "value"));

      //if (type != FlexLengthUnitType.Auto && type != FlexLengthUnitType.Pixel && type != FlexLengthUnitType.Star)
      //  throw new ArgumentException(System.Windows.SR.Get("InvalidCtorParameterUnknownFlexLengthUnitType", (object) "type"));

      _unitValue = type == FlexLengthUnitType.Auto ? 0.0 : value;
      UnitType = type;
    }

    public static bool operator ==(FlexLength flexLength1, FlexLength flexLength2)
    {
      return flexLength1.UnitType == flexLength2.UnitType && Equals(flexLength1.Value, flexLength2.Value);
    }

    public static bool operator !=(FlexLength flexLength1, FlexLength flexLength2)
    {
      return flexLength1.UnitType != flexLength2.UnitType || !Equals(flexLength1.Value, flexLength2.Value);
    }

    public override bool Equals(object other)
    {
      return other is FlexLength length && this == length;
    }

    public bool Equals(FlexLength flexLength)
    {
      return this == flexLength;
    }

    public override int GetHashCode()
    {
      return (int) ((int) _unitValue + UnitType);
    }

    public override string ToString()
    {
      return FlexLengthConverter.ToString(this, CultureInfo.InvariantCulture);
    }
  }

  public class FlexLengthConverter : TypeConverter
  {
    #region Static Fields and Constants

    private static readonly string[] UnitStrings = {"auto", "px", "*"};
    private static readonly string[] PixelUnitStrings = {"in", "cm", "pt"};

    private static readonly double[] PixelUnitFactors =
    {
      96.0, // Pixels per Inch
      96.0 / 2.54, // Pixels per Centimeter
      96.0 / 72.0, // Pixels per Point
    };

    #endregion

    #region  Methods

    public override bool CanConvertFrom(ITypeDescriptorContext typeDescriptorContext, Type sourceType)
    {
      switch (Type.GetTypeCode(sourceType))
      {
        case TypeCode.Int16:
        case TypeCode.UInt16:
        case TypeCode.Int32:
        case TypeCode.UInt32:
        case TypeCode.Int64:
        case TypeCode.UInt64:
        case TypeCode.Single:
        case TypeCode.Double:
        case TypeCode.Decimal:
        case TypeCode.String:
          return true;
        default:
          return false;
      }
    }

    public override bool CanConvertTo(ITypeDescriptorContext typeDescriptorContext, Type destinationType)
    {
#if SILVERLIGHT
      return destinationType == typeof(string);
#else
      return destinationType == typeof(InstanceDescriptor) || destinationType == typeof(string);
#endif
    }

    public override object ConvertFrom(ITypeDescriptorContext typeDescriptorContext, CultureInfo cultureInfo, object source)
    {
      if (source == null)
        throw GetConvertFromExceptionInt(null);

      if (source is string stringValue)
        return FromString(stringValue, cultureInfo);

      var num = Convert.ToDouble(source, cultureInfo);

      FlexLengthUnitType type;

      if (DoubleUtils.IsNaN(num))
      {
        num = 1.0;
        type = FlexLengthUnitType.Auto;
      }
      else
        type = FlexLengthUnitType.Pixel;

      return new FlexLength(num, type);
    }

    public override object ConvertTo(ITypeDescriptorContext typeDescriptorContext, CultureInfo cultureInfo, object value, Type destinationType)
    {
      if (destinationType == null)
        throw new ArgumentNullException(nameof(destinationType));

      if (value is FlexLength == false)
        throw GetConvertToExceptionInt(value, destinationType);

      var flexLength = (FlexLength) value;

      if (destinationType == typeof(string))
        return ToString(flexLength, cultureInfo);

#if !SILVERLIGHT
      if (destinationType == typeof(InstanceDescriptor))
        return new InstanceDescriptor(typeof(FlexLength).GetConstructor(new[] { typeof(double), typeof(FlexLengthUnitType) }), new object[] { flexLength.Value, flexLength.UnitType });
#endif

      throw GetConvertToExceptionInt(value, destinationType);
    }

    internal static FlexLength FromString(string s, CultureInfo cultureInfo)
    {
	    FromString(s, cultureInfo, out var num, out var unit);

      return new FlexLength(num, unit);
    }

    private static void FromString(string s, CultureInfo cultureInfo, out double value, out FlexLengthUnitType unit)
    {
      var normalized = s.Trim().ToLowerInvariant();

      value = 0.0;
      unit = FlexLengthUnitType.Pixel;

      var strLen = normalized.Length;
      var strLenUnit = 0;
      var unitFactor = 1.0;

      var i = 0;

      if (normalized == UnitStrings[i])
      {
        strLenUnit = UnitStrings[i].Length;
        unit = (FlexLengthUnitType) i;
      }
      else
      {
        for (i = 1; i < UnitStrings.Length; ++i)
        {
          if (normalized.EndsWith(UnitStrings[i], StringComparison.Ordinal) == false)
            continue;

          strLenUnit = UnitStrings[i].Length;
          unit = (FlexLengthUnitType) i;

          break;
        }
      }

      if (i >= UnitStrings.Length)
      {
        for (i = 0; i < PixelUnitStrings.Length; ++i)
        {
          if (normalized.EndsWith(PixelUnitStrings[i], StringComparison.Ordinal) == false)
            continue;

          strLenUnit = PixelUnitStrings[i].Length;
          unitFactor = PixelUnitFactors[i];

          break;
        }
      }

      if (strLen == strLenUnit && (unit == FlexLengthUnitType.Auto || unit == FlexLengthUnitType.Star))
        value = 1;
      else
      {
        Debug.Assert(unit == FlexLengthUnitType.Pixel || DoubleUtils.AreClose(unitFactor, 1.0));

        var valueString = normalized.Substring(0, strLen - strLenUnit);

        value = Convert.ToDouble(valueString, cultureInfo) * unitFactor;
      }
    }

    private Exception GetConvertFromExceptionInt(object value)
    {
      throw new NotSupportedException($"Can not convert from {(value == null ? "null" : value.GetType().FullName)}");
    }

    private Exception GetConvertToExceptionInt(object value, Type destinationType)
    {
      throw new NotSupportedException($"Can not convert to {destinationType.FullName}");
    }

    internal static string ToString(FlexLength gl, CultureInfo cultureInfo)
    {
      switch (gl.UnitType)
      {
        case FlexLengthUnitType.Auto:
          return "Auto";
        case FlexLengthUnitType.Star:
          return gl.Value.IsCloseTo(1.0) ? "*" : Convert.ToString(gl.Value, cultureInfo) + "*";
        default:
          return Convert.ToString(gl.Value, cultureInfo);
      }
    }

    #endregion
  }
}