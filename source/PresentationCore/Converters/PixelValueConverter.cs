// <copyright file="PixelValueConverter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.Core;

namespace Zaaml.PresentationCore.Converters
{
	internal sealed class PixelValueConverter
	{
		private double _pixel2ValueRatio;
		private Range<double> _pixelRange = new Range<double>(0, 0);

		private double _value2PixelRatio;
		private Range<double> _valueRange = new Range<double>(0, 0);

		public Range<double> PixelRange
		{
			get => _pixelRange;
			set
			{
				_pixelRange = value;

				UpdateRatio();
			}
		}

		public Range<double> ValueRange
		{
			get => _valueRange;
			set
			{
				_valueRange = value;

				UpdateRatio();
			}
		}

		public double PixelToValue(double pixel)
		{
			return _valueRange.Minimum + (PixelRange.Clamp(pixel) - PixelRange.Minimum) * _pixel2ValueRatio;
		}

		private void UpdateRatio()
		{
			var pixelDistance = PixelRange.Maximum - PixelRange.Minimum;
			var valueDistance = _valueRange.Maximum - _valueRange.Minimum;

			_pixel2ValueRatio = valueDistance / pixelDistance;
			_value2PixelRatio = pixelDistance / valueDistance;
		}

		public double ValueToPixel(double value)
		{
			return PixelRange.Minimum + (_valueRange.Clamp(value) - _valueRange.Minimum) * _value2PixelRatio;
		}
	}
}