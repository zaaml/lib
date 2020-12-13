// <copyright file="PixelValueConverter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.Core;

namespace Zaaml.PresentationCore.Converters
{
  internal class PixelValueConverter
  {
    #region Fields

    private readonly double _pixel2ValueRatio;

    private readonly Range<double> _pixelRange;
    private readonly double _value2PixelRatio;
    private readonly Range<double> _valueRange;

    #endregion

    #region Ctors

    public PixelValueConverter(Range<double> pixelRange, Range<double> valueRange)
    {
      _pixelRange = pixelRange;
      _valueRange = valueRange;

      var pixelDistance = _pixelRange.Maximum - _pixelRange.Minimum;
      var valueDistance = _valueRange.Maximum - _valueRange.Minimum;

      _pixel2ValueRatio = valueDistance / pixelDistance;
      _value2PixelRatio = pixelDistance / valueDistance;
    }

    #endregion

    #region  Methods

    public double PixelToValue(double pixel)
    {
      return _valueRange.Minimum + (_pixelRange.Clamp(pixel) - _pixelRange.Minimum) * _pixel2ValueRatio;
    }

    public double ValueToPixel(double value)
    {
      return _pixelRange.Minimum + (_valueRange.Clamp(value) - _valueRange.Minimum) * _value2PixelRatio;
    }

    #endregion
  }
}