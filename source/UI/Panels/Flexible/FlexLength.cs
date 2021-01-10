// <copyright file="FlexLength.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Globalization;

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
}