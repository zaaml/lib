// <copyright file="EditorColorStruct.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows.Media;
using Zaaml.Core.ColorModel;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Extensions;

namespace Zaaml.UI.Controls.ColorEditor
{
	internal struct EditorColorStruct
	{
		private double _hue;
		private double _saturation;
		private double _value;
		private double _red;
		private double _green;
		private double _blue;
		private double _alpha;

		public static EditorColorStruct Default => new(1.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0);

		public EditorColorStruct(double alpha, double red, double green, double blue, double hue, double saturation, double value)
		{
			_alpha = alpha;
			_red = red;
			_green = green;
			_blue = blue;
			_hue = hue;
			_saturation = saturation;
			_value = value;
		}

		public double Hue
		{
			get => _hue;
			set
			{
				if (_hue.IsCloseTo(value))
					return;

				_hue = value;

				UpdateRgb();
			}
		}

		private void UpdateRgb()
		{
			var hsvColor = new HsvColor(_alpha, _hue, _saturation, _value);
			var rgbColor = hsvColor.ToRgbColor();

			_red = rgbColor.R;
			_green = rgbColor.G;
			_blue = rgbColor.B;
		}

		private void UpdateHsv()
		{
			var rgbColor = new RgbColor(_alpha, _red, _green, _blue);
			var hsvColor = rgbColor.ToHsvColor();

			if (hsvColor.Saturation.IsCloseTo(0) == false)
				_hue = hsvColor.Hue;

			_saturation = hsvColor.Saturation;
			_value = hsvColor.Value;
		}

		public double Saturation
		{
			get => _saturation;
			set
			{
				if (_saturation.IsCloseTo(value))
					return;

				_saturation = value;

				UpdateRgb();
			}
		}

		public double Value
		{
			get => _value;
			set
			{
				if (_value.IsCloseTo(value))
					return;

				_value = value;

				UpdateRgb();
			}
		}

		public double Red
		{
			get => _red;
			set
			{
				if (_red.IsCloseTo(value))
					return;

				_red = value;

				UpdateHsv();
			}
		}

		public double Green
		{
			get => _green;
			set
			{
				if (_green.IsCloseTo(value))
					return;

				_green = value;

				UpdateHsv();
			}
		}

		public double Blue
		{
			get => _blue;
			set
			{
				if (_blue.IsCloseTo(value))
					return;

				_blue = value;

				UpdateHsv();
			}
		}

		public double Alpha
		{
			get => _alpha;
			set
			{
				if (_alpha.IsCloseTo(value))
					return;

				_alpha = value;
			}
		}

		public double GetChannelValue(ColorChannel channel)
		{
			return channel switch
			{
				ColorChannel.Alpha => Alpha,
				ColorChannel.Red => Red,
				ColorChannel.Green => Green,
				ColorChannel.Blue => Blue,
				ColorChannel.Hue => Hue,
				ColorChannel.Saturation => Saturation,
				ColorChannel.Value => Value,
				_ => throw new ArgumentOutOfRangeException(nameof(channel), channel, null)
			};
		}

		public void SetChannelValue(ColorChannel channel, double value)
		{
			switch (channel)
			{
				case ColorChannel.Alpha:

					Alpha = value;

					break;
				case ColorChannel.Red:

					Red = value;

					break;
				case ColorChannel.Green:

					Green = value;

					break;
				case ColorChannel.Blue:

					Blue = value;

					break;
				case ColorChannel.Hue:

					Hue = value;

					break;
				case ColorChannel.Saturation:

					Saturation = value;

					break;
				case ColorChannel.Value:

					Value = value;

					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(channel), channel, null);
			}
		}

		public Color Color
		{
			get => RgbColor.ToXamlColor();
			set
			{
				var hsvColor = value.ToHsvColor();
				var rgbColor = value.ToRgbColor();

				_alpha = rgbColor.A;
				_red = rgbColor.R;
				_green = rgbColor.G;
				_blue = rgbColor.B;

				_hue = hsvColor.Saturation.IsCloseTo(0) ? Hue : hsvColor.Hue;
				_saturation = hsvColor.Saturation;
				_value = hsvColor.Value;
			}
		}

		internal RgbColor RgbColor => new RgbColor(Alpha, Red, Green, Blue);

		internal HsvColor HsvColor => new HsvColor(Alpha, Hue, Saturation, Value);
	}
}