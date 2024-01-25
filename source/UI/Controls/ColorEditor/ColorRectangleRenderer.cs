// <copyright file="ColorRectangleRenderer.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Zaaml.Core.ColorModel;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Utils;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.ColorEditor
{
	public sealed class ColorRectangleRenderer : FixedTemplateControl<Image>
	{
		private const int AlphaIndex = 0;
		private const int HueIndex = 1;
		private const int SaturationIndex = 2;
		private const int ValueIndex = 3;
		private const int RedIndex = 1;
		private const int GreenIndex = 2;
		private const int BlueIndex = 3;
		private const int SkipIndex = 4;

		public static readonly DependencyProperty XAxisProperty = DPM.Register<ColorRectangleRendererAxis, ColorRectangleRenderer>
			("XAxis", d => d.OnXAxisPropertyChangedPrivate);

		public static readonly DependencyProperty YAxisProperty = DPM.Register<ColorRectangleRendererAxis, ColorRectangleRenderer>
			("YAxis", d => d.OnYAxisPropertyChangedPrivate);

		public static readonly DependencyProperty ZAxisProperty = DPM.Register<ColorRectangleRendererAxis, ColorRectangleRenderer>
			("ZAxis", d => d.OnZAxisPropertyChangedPrivate);

		public static readonly DependencyProperty BaseColorProperty = DPM.Register<Color, ColorRectangleRenderer>
			("BaseColor", Colors.Black, d => d.OnBaseColorPropertyChangedPrivate);

		private WriteableBitmap _surfaceBitmap;

		static ColorRectangleRenderer()
		{
			ControlUtils.OverrideIsTabStop<ColorRectangleRenderer>(false);
		}

		public Color BaseColor
		{
			get => (Color)GetValue(BaseColorProperty);
			set => SetValue(BaseColorProperty, value);
		}

		public ColorRectangleRendererAxis XAxis
		{
			get => (ColorRectangleRendererAxis)GetValue(XAxisProperty);
			set => SetValue(XAxisProperty, value);
		}

		public ColorRectangleRendererAxis YAxis
		{
			get => (ColorRectangleRendererAxis)GetValue(YAxisProperty);
			set => SetValue(YAxisProperty, value);
		}

		public ColorRectangleRendererAxis ZAxis
		{
			get => (ColorRectangleRendererAxis)GetValue(ZAxisProperty);
			set => SetValue(ZAxisProperty, value);
		}

		protected override void ApplyTemplateOverride()
		{
			base.ApplyTemplateOverride();

			TemplateRoot.Stretch = Stretch.None;

			if (_surfaceBitmap != null)
				TemplateRoot.Source = _surfaceBitmap;
		}

		protected override Size ArrangeOverride(Size arrangeBounds)
		{
			if (_surfaceBitmap == null || arrangeBounds.Width.IsCloseTo(_surfaceBitmap.PixelWidth) == false || arrangeBounds.Height.IsCloseTo(_surfaceBitmap.PixelHeight) == false)
				BuildSurfaceBitmap(arrangeBounds);

			return base.ArrangeOverride(arrangeBounds);
		}

		private void BuildSurfaceBitmap(Size arrangeBounds)
		{
			var width = (int)arrangeBounds.Width;
			var height = (int)arrangeBounds.Height;

			if (width == 0 || height == 0)
				return;

			_surfaceBitmap = new WriteableBitmap(width, height, DpiUtils.DpiX, DpiUtils.DpiY, PixelFormats.Bgra32, null);

			int[] pixelBuffer = null;

			try
			{
				pixelBuffer = ArrayPool<int>.Shared.Rent(width * height);

				FillSurfaceBitmap(_surfaceBitmap, pixelBuffer);

				_surfaceBitmap.WritePixels(new Int32Rect(0, 0, width, height), pixelBuffer, _surfaceBitmap.PixelWidth * 4, 0, 0);
			}
			finally
			{
				// ReSharper disable once AssignNullToNotNullAttribute
				ArrayPool<int>.Shared.Return(pixelBuffer);
			}

			if (TemplateRoot != null)
				TemplateRoot.Source = _surfaceBitmap;
		}

		private void FillHsv(WriteableBitmap surfaceBitmap, int[] pixelBuffer)
		{
			var width = surfaceBitmap.PixelWidth;
			var height = surfaceBitmap.PixelHeight;

			var xAxis = XAxis;
			var yAxis = YAxis;
			var zAxis = ZAxis;

			var zChannelValue = zAxis?.Value ?? 0.0;
			var xChannelValue = xAxis?.Value;
			var yChannelValue = yAxis?.Value;

			var actualZChannelValue = zChannelValue;

			var xScale = xAxis?.Channel == ColorChannel.Hue ? 360.0 : 1;
			var yScale = yAxis?.Channel == ColorChannel.Hue ? 360.0 : 1;

			var hsvSetter = new ChannelSetter(BaseColor.ToHsvColor(), xAxis, yAxis, zAxis);
			var hsvValues = hsvSetter.CreateValues();

			for (var x = 0; x < width; x++)
			{
				var relX = xScale * x / width;
				var actualXChannelValue = xChannelValue ?? relX;

				for (var y = 0; y < height; y++)
				{
					var relY = yScale * (1.0 - (double)y / height);
					var actualYChannelValue = yChannelValue ?? relY;

					hsvSetter.SetValues(actualXChannelValue, actualYChannelValue, actualZChannelValue, hsvValues);

					pixelBuffer[y * width + x] = ChannelSetter.GetHsvPixel(hsvValues);
				}
			}
		}

		private void FillMixed(WriteableBitmap surfaceBitmap, int[] pixelBuffer)
		{
			var width = surfaceBitmap.PixelWidth;
			var height = surfaceBitmap.PixelHeight;

			var xAxis = XAxis;
			var yAxis = YAxis;
			var zAxis = ZAxis;

			var xChannel = xAxis?.Channel;
			var yChannel = yAxis?.Channel;
			var zChannelValue = zAxis?.Value ?? 0.0;
			var xChannelValue = xAxis?.Value;
			var yChannelValue = yAxis?.Value;

			var actualZChannelValue = zChannelValue;

			var xScale = xChannel == ColorChannel.Hue ? 360.0 : 1;
			var yScale = yChannel == ColorChannel.Hue ? 360.0 : 1;

			var baseRgbColor = BaseColor.ToRgbColor();
			var baseHsvColor = BaseColor.ToHsvColor();
			var rgbSetter = new ChannelSetter(baseRgbColor, xAxis, yAxis, zAxis);
			var hsvSetter = new ChannelSetter(baseHsvColor, xAxis, yAxis, zAxis);

			var hsvValues = hsvSetter.CreateValues();
			var rgbValues = rgbSetter.CreateValues();

			for (var x = 0; x < width; x++)
			{
				var relX = xScale * x / width;
				var actualXChannelValue = xChannelValue ?? relX;

				for (var y = 0; y < height; y++)
				{
					var relY = yScale * (1.0 - (double)y / height);
					var actualYChannelValue = yChannelValue ?? relY;

					hsvSetter.SetValues(actualXChannelValue, actualYChannelValue, actualZChannelValue, hsvValues);

					ChannelSetter.ConvertHsvValuesToRgbValues(hsvValues, rgbValues);

					rgbSetter.SetValues(actualXChannelValue, actualYChannelValue, actualZChannelValue, rgbValues);

					pixelBuffer[y * width + x] = ChannelSetter.GetRgbPixel(rgbValues);
				}
			}
		}

		private void FillRgb(WriteableBitmap surfaceBitmap, int[] pixelBuffer)
		{
			var width = surfaceBitmap.PixelWidth;
			var height = surfaceBitmap.PixelHeight;

			var xAxis = XAxis;
			var yAxis = YAxis;
			var zAxis = ZAxis;

			var zChannelValue = zAxis?.Value ?? 0.0;
			var xChannelValue = xAxis?.Value;
			var yChannelValue = yAxis?.Value;

			var actualZChannelValue = zChannelValue;
			var rgbSetter = new ChannelSetter(BaseColor.ToRgbColor(), xAxis, yAxis, zAxis);
			var rgbValues = rgbSetter.CreateValues();

			for (var x = 0; x < width; x++)
			{
				var relX = (double)x / width;
				var actualXChannelValue = xChannelValue ?? relX;

				for (var y = 0; y < height; y++)
				{
					var relY = 1.0 - (double)y / height;
					var actualYChannelValue = yChannelValue ?? relY;

					rgbSetter.SetValues(actualXChannelValue, actualYChannelValue, actualZChannelValue, rgbValues);

					pixelBuffer[y * width + x] = ChannelSetter.GetRgbPixel(rgbValues);
				}
			}
		}

		private void FillSurfaceBitmap(WriteableBitmap surfaceBitmap, int[] pixelBuffer)
		{
			var xAxis = XAxis;
			var yAxis = YAxis;
			var zAxis = ZAxis;

			if (IsHsv(xAxis) && IsHsv(yAxis) && IsHsv(zAxis))
				FillHsv(surfaceBitmap, pixelBuffer);
			else if (IsRgb(xAxis) && IsRgb(yAxis) && IsRgb(zAxis))
				FillRgb(surfaceBitmap, pixelBuffer);
			else
				FillMixed(surfaceBitmap, pixelBuffer);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static int GetPixelColor(double a, double r, double g, double b)
		{
			var alpha = (byte)(a * 255.0);
			var red = (byte)(r * 255.0);
			var green = (byte)(g * 255.0);
			var blue = (byte)(b * 255.0);

			return alpha << 24 | red << 16 | green << 8 | blue << 0;
		}

		private void InvalidateSurfaceBitmap()
		{
			_surfaceBitmap = null;

			InvalidateArrange();
		}

		private static bool IsHsv(ColorRectangleRendererAxis axis)
		{
			return axis == null || ColorUtils.GetChannelSpace(axis.Channel, ColorSpace.Hsv) == ColorSpace.Hsv;
		}

		private static bool IsRgb(ColorRectangleRendererAxis axis)
		{
			return axis == null || ColorUtils.GetChannelSpace(axis.Channel, ColorSpace.Rgb) == ColorSpace.Rgb;
		}

		protected override Size MeasureOverride(Size availableSize)
		{
			base.MeasureOverride(availableSize);

			return new Size(0, 0);
		}

		private void OnAxisChannelChanged(object sender, EventArgs e)
		{
			InvalidateSurfaceBitmap();
		}

		private void OnAxisPropertyChanged(ColorRectangleRendererAxis oldValue, ColorRectangleRendererAxis newValue)
		{
			if (ReferenceEquals(oldValue, newValue))
				return;

			if (oldValue != null)
			{
				oldValue.ValueChanged -= OnAxisValueChanged;
				oldValue.ChannelChanged -= OnAxisChannelChanged;
			}

			if (newValue != null)
			{
				newValue.ValueChanged += OnAxisValueChanged;
				newValue.ChannelChanged += OnAxisChannelChanged;
			}
		}

		private void OnAxisValueChanged(object sender, EventArgs e)
		{
			var axis = (ColorRectangleRendererAxis)sender;

			if (axis.Value != null)
				InvalidateSurfaceBitmap();
		}

		private void OnBaseColorPropertyChangedPrivate(Color oldValue, Color newValue)
		{
			InvalidateSurfaceBitmap();
		}

		private void OnXAxisPropertyChangedPrivate(ColorRectangleRendererAxis oldValue, ColorRectangleRendererAxis newValue)
		{
			OnAxisPropertyChanged(oldValue, newValue);
		}

		private void OnYAxisPropertyChangedPrivate(ColorRectangleRendererAxis oldValue, ColorRectangleRendererAxis newValue)
		{
			OnAxisPropertyChanged(oldValue, newValue);
		}

		private void OnZAxisPropertyChangedPrivate(ColorRectangleRendererAxis oldValue, ColorRectangleRendererAxis newValue)
		{
			OnAxisPropertyChanged(oldValue, newValue);
		}

		protected override void UndoTemplateOverride()
		{
			TemplateRoot.Source = null;

			base.UndoTemplateOverride();
		}

		private sealed class ChannelSetter
		{
			private readonly double[] _baseValues = new double[5];
			private readonly int[] _indices = new int[3];

			public ChannelSetter(HsvColor baseColor, ColorRectangleRendererAxis xAxis, ColorRectangleRendererAxis yAxis, ColorRectangleRendererAxis zAxis)
			{
				_baseValues[AlphaIndex] = baseColor.Alpha;
				_baseValues[HueIndex] = baseColor.Hue;
				_baseValues[SaturationIndex] = baseColor.Saturation;
				_baseValues[ValueIndex] = baseColor.Value;

				_indices[0] = GetHsvAxisIndex(xAxis);
				_indices[1] = GetHsvAxisIndex(yAxis);
				_indices[2] = GetHsvAxisIndex(zAxis);
			}

			public ChannelSetter(RgbColor baseColor, ColorRectangleRendererAxis xAxis, ColorRectangleRendererAxis yAxis, ColorRectangleRendererAxis zAxis)
			{
				_baseValues[AlphaIndex] = baseColor.A;
				_baseValues[RedIndex] = baseColor.R;
				_baseValues[GreenIndex] = baseColor.G;
				_baseValues[BlueIndex] = baseColor.B;

				_indices[0] = GetRgbAxisIndex(xAxis);
				_indices[1] = GetRgbAxisIndex(yAxis);
				_indices[2] = GetRgbAxisIndex(zAxis);
			}

			public static void ConvertHsvValuesToRgbValues(double[] hsvValues, double[] rgbValues)
			{
				var a = hsvValues[AlphaIndex];
				var h = hsvValues[HueIndex];
				var s = hsvValues[SaturationIndex];
				var v = hsvValues[ValueIndex];

				var hn = Math.Floor(h / 60.0);

				var hi = (int)hn % 6;
				var f = h / 60.0 - hn;

				var p = v * (1.0 - s);
				var q = v * (1.0 - f * s);
				var t = v * (1.0 - (1.0 - f) * s);

				switch (hi)
				{
					case 0:
						FillRgbValues(a, v, t, p, rgbValues);
						break;
					case 1:
						FillRgbValues(a, q, v, p, rgbValues);
						break;
					case 2:
						FillRgbValues(a, p, v, t, rgbValues);
						break;
					case 3:
						FillRgbValues(a, p, q, v, rgbValues);
						break;
					case 4:
						FillRgbValues(a, t, p, v, rgbValues);
						break;
					case 5:
						FillRgbValues(a, v, p, q, rgbValues);
						break;
					default:
						FillRgbValues(a, 0.0, 0.0, 0.0, rgbValues);
						break;
				}
			}

			public double[] CreateValues()
			{
				var values = new double[5];

				InitValues(values);

				return values;
			}

			private static void FillRgbValues(double a, double r, double g, double b, double[] values)
			{
				values[AlphaIndex] = a;
				values[RedIndex] = r;
				values[GreenIndex] = g;
				values[BlueIndex] = b;
			}

			private static int GetHsvAxisIndex(ColorRectangleRendererAxis axis)
			{
				if (axis == null)
					return SkipIndex;

				return axis.Channel switch
				{
					ColorChannel.Alpha => AlphaIndex,
					ColorChannel.Hue => HueIndex,
					ColorChannel.Saturation => SaturationIndex,
					ColorChannel.Value => ValueIndex,

					_ => SkipIndex
				};
			}

			public static int GetHsvPixel(double[] hsvValues)
			{
				var a = hsvValues[AlphaIndex];
				var h = hsvValues[HueIndex];
				var s = hsvValues[SaturationIndex];
				var v = hsvValues[ValueIndex];

				var hn = Math.Floor(h / 60.0);

				var hi = (int)hn % 6;
				var f = (h / 60.0) - hn;

				var p = v * (1.0 - s);
				var q = v * (1.0 - f * s);
				var t = v * (1.0 - (1.0 - f) * s);

				return hi switch
				{
					0 => GetPixelColor(a, v, t, p),
					1 => GetPixelColor(a, q, v, p),
					2 => GetPixelColor(a, p, v, t),
					3 => GetPixelColor(a, p, q, v),
					4 => GetPixelColor(a, t, p, v),
					5 => GetPixelColor(a, v, p, q),
					_ => GetPixelColor(a, 0.0, 0.0, 0.0)
				};
			}

			private static int GetRgbAxisIndex(ColorRectangleRendererAxis axis)
			{
				if (axis == null)
					return SkipIndex;

				return axis.Channel switch
				{
					ColorChannel.Alpha => AlphaIndex,
					ColorChannel.Red => RedIndex,
					ColorChannel.Green => GreenIndex,
					ColorChannel.Blue => BlueIndex,

					_ => SkipIndex
				};
			}

			public static int GetRgbPixel(double[] rgbValues)
			{
				return GetPixelColor(rgbValues[AlphaIndex], rgbValues[RedIndex], rgbValues[GreenIndex], rgbValues[BlueIndex]);
			}

			private void InitValues(double[] values)
			{
				values[0] = _baseValues[0];
				values[1] = _baseValues[1];
				values[2] = _baseValues[2];
				values[3] = _baseValues[3];
			}

			public void SetValues(double xValue, double yValue, double zValue, double[] values)
			{
				values[_indices[0]] = xValue;
				values[_indices[1]] = yValue;
				values[_indices[2]] = zValue;
			}
		}
	}

	public sealed class ColorRectangleRendererAxis : InheritanceContextObject
	{
		public static readonly DependencyProperty ChannelProperty = DPM.Register<ColorChannel, ColorRectangleRendererAxis>
			("Channel", ColorChannel.Alpha, d => d.OnChannelPropertyChangedPrivate);

		public static readonly DependencyProperty ValueProperty = DPM.Register<double?, ColorRectangleRendererAxis>
			("Value", null, d => d.OnValuePropertyChangedPrivate);

		public event EventHandler ValueChanged;

		public event EventHandler ChannelChanged;

		public ColorChannel Channel
		{
			get => (ColorChannel)GetValue(ChannelProperty);
			set => SetValue(ChannelProperty, value);
		}

		public double? Value
		{
			get => (double?)GetValue(ValueProperty);
			set => SetValue(ValueProperty, value);
		}

		private void OnChannelPropertyChangedPrivate(ColorChannel oldValue, ColorChannel newValue)
		{
			ChannelChanged?.Invoke(this, EventArgs.Empty);
		}

		private void OnValuePropertyChangedPrivate(double? oldValue, double? newValue)
		{
			ValueChanged?.Invoke(this, EventArgs.Empty);
		}

		internal void SetChannelValue(ColorChannel channel, Color color, EditorColorStruct? editorColor = null)
		{
			Channel = channel;

			var channelValue = color.GetChannelValue(channel);

			if (editorColor != null)
				channelValue = editorColor.Value.GetChannelValue(channel);

			Value = channelValue;
		}
	}
}