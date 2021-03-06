// <copyright file="ColorEditorControlBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.ColorEditor
{
	public abstract class ColorEditorControlBase : TemplateContractControl
	{
		public static readonly DependencyProperty HueProperty = DPM.Register<double, ColorEditorControlBase>
			("Hue", 0.0, t => t.OnHuePropertyChangedPrivate, t => t.CoerceHueChannelValue);

		public static readonly DependencyProperty SaturationProperty = DPM.Register<double, ColorEditorControlBase>
			("Saturation", 1.0, t => t.OnSaturationPropertyChangedPrivate, t => t.CoerceSaturationChannelValue);

		public static readonly DependencyProperty ValueProperty = DPM.Register<double, ColorEditorControlBase>
			("Value", 0.0, t => t.OnValuePropertyChangedPrivate, t => t.CoerceValueChannelValue);

		public static readonly DependencyProperty AlphaProperty = DPM.Register<double, ColorEditorControlBase>
			("Alpha", 1.0, t => t.OnAlphaPropertyChangedPrivate, t => t.CoerceAlphaChannelValue);

		public static readonly DependencyProperty ColorProperty = DPM.Register<Color, ColorEditorControlBase>
			("Color", Colors.Black, t => t.OnColorPropertyChangedPrivate, t => t.CoerceColor);

		public static readonly DependencyProperty RedProperty = DPM.Register<double, ColorEditorControlBase>
			("Red", 0.0, t => t.OnRedPropertyChangedPrivate, t => t.CoerceRedChannelValue);

		public static readonly DependencyProperty GreenProperty = DPM.Register<double, ColorEditorControlBase>
			("Green", 0.0, t => t.OnGreenPropertyChangedPrivate, t => t.CoerceGreenChannelValue);

		public static readonly DependencyProperty BlueProperty = DPM.Register<double, ColorEditorControlBase>
			("Blue", 0.0, t => t.OnBluePropertyChangedPrivate, t => t.CoerceBlueChannelValue);

		private EditorColorStruct _editorColor = EditorColorStruct.Default;

		private int _suspendChangeHandlerCount;

		public event EventHandler<ColorChannelValueChangedEventArgs> ChannelValueChanged;

		public double Alpha
		{
			get => (double) GetValue(AlphaProperty);
			set => SetValue(AlphaProperty, value);
		}

		public double Blue
		{
			get => (double) GetValue(BlueProperty);
			set => SetValue(BlueProperty, value);
		}

		public Color Color
		{
			get => (Color) GetValue(ColorProperty);
			set => SetValue(ColorProperty, value);
		}

		internal IEnumerable<DependencyProperty> ColorEditorChannelProperties
		{
			get
			{
				yield return AlphaProperty;
				yield return RedProperty;
				yield return GreenProperty;
				yield return BlueProperty;
				yield return HueProperty;
				yield return SaturationProperty;
				yield return ValueProperty;
			}
		}

		private protected EditorColorStruct EditorColor => _editorColor;

		public double Green
		{
			get => (double) GetValue(GreenProperty);
			set => SetValue(GreenProperty, value);
		}

		private bool HandleChannelValueChange { get; set; }

		public double Hue
		{
			get => (double) GetValue(HueProperty);
			set => SetValue(HueProperty, value);
		}

		protected bool IsChangeHandlerSuspended => _suspendChangeHandlerCount > 0;

		internal bool IsChangeHandlerSuspendedInternal => IsChangeHandlerSuspended;

		public double Red
		{
			get => (double) GetValue(RedProperty);
			set => SetValue(RedProperty, value);
		}

		public double Saturation
		{
			get => (double) GetValue(SaturationProperty);
			set => SetValue(SaturationProperty, value);
		}

		public double Value
		{
			get => (double) GetValue(ValueProperty);
			set => SetValue(ValueProperty, value);
		}

		private double CoerceAlphaChannelValue(double alpha)
		{
			return HandleChannelValueChange ? _editorColor.Alpha : alpha.Clamp(0.0, 1.0);
		}

		private double CoerceBlueChannelValue(double blue)
		{
			return HandleChannelValueChange ? _editorColor.Blue : blue.Clamp(0.0, 1.0);
		}

		private Color CoerceColor(Color color)
		{
			return HandleChannelValueChange ? _editorColor.Color : color;
		}

		private double CoerceGreenChannelValue(double green)
		{
			return HandleChannelValueChange ? _editorColor.Green : green.Clamp(0.0, 1.0);
		}

		private double CoerceHueChannelValue(double hue)
		{
			return HandleChannelValueChange ? _editorColor.Hue : hue.Clamp(0.0, 360.0);
		}

		private double CoerceRedChannelValue(double red)
		{
			return HandleChannelValueChange ? _editorColor.Red : red.Clamp(0.0, 1.0);
		}

		private double CoerceSaturationChannelValue(double saturation)
		{
			return HandleChannelValueChange ? _editorColor.Saturation : saturation.Clamp(0.0, 1.0);
		}

		private double CoerceValueChannelValue(double value)
		{
			return HandleChannelValueChange ? _editorColor.Value : value.Clamp(0.0, 1.0);
		}

		protected static DependencyProperty GetChannelProperty(ColorChannel channel)
		{
			return channel switch
			{
				ColorChannel.Alpha => AlphaProperty,
				ColorChannel.Red => RedProperty,
				ColorChannel.Green => GreenProperty,
				ColorChannel.Blue => BlueProperty,
				ColorChannel.Hue => HueProperty,
				ColorChannel.Saturation => SaturationProperty,
				ColorChannel.Value => ValueProperty,
				_ => throw new ArgumentOutOfRangeException(nameof(channel), channel, null)
			};
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

		private void HandleChannelChange(ColorChannel channel, double value)
		{
			if (IsChangeHandlerSuspended)
				return;

			_editorColor.SetChannelValue(channel, value);

			SyncChannels();
		}

		protected virtual void OnAlphaChanged(double oldValue, double newValue)
		{
		}

		private void OnAlphaPropertyChangedPrivate(double oldValue, double newValue)
		{
			HandleChannelChange(ColorChannel.Alpha, newValue);

			OnChannelValueChanged(ColorChannel.Alpha, oldValue, newValue);

			OnAlphaChanged(oldValue, newValue);
		}

		protected virtual void OnBlueChanged(double oldValue, double newValue)
		{
		}

		private void OnBluePropertyChangedPrivate(double oldValue, double newValue)
		{
			HandleChannelChange(ColorChannel.Blue, newValue);

			OnChannelValueChanged(ColorChannel.Blue, oldValue, newValue);

			OnBlueChanged(oldValue, newValue);
		}

		protected virtual void OnChangeHandlerResumed()
		{
		}

		protected virtual void OnChangeHandlerSuspended()
		{
		}

		protected virtual void OnChannelValueChanged(ColorChannel channel, double oldValue, double newValue)
		{
			ChannelValueChanged?.Invoke(this, new ColorChannelValueChangedEventArgs(channel, oldValue, newValue));
		}

		protected virtual void OnColorChanged(Color oldColor, Color newColor)
		{
		}

		private void OnColorPropertyChangedPrivate(Color oldColor, Color newColor)
		{
			try
			{
				if (IsChangeHandlerSuspended)
					return;

				_editorColor.Color = Color;

				SyncChannels();
			}
			finally
			{
				OnColorChanged(oldColor, newColor);
			}
		}

		protected virtual void OnGreenChanged(double oldValue, double newValue)
		{
		}

		private void OnGreenPropertyChangedPrivate(double oldValue, double newValue)
		{
			HandleChannelChange(ColorChannel.Green, newValue);

			OnChannelValueChanged(ColorChannel.Green, oldValue, newValue);

			OnGreenChanged(oldValue, newValue);
		}

		protected virtual void OnHueChanged(double oldValue, double newValue)
		{
		}

		private void OnHuePropertyChangedPrivate(double oldValue, double newValue)
		{
			HandleChannelChange(ColorChannel.Hue, newValue);

			OnChannelValueChanged(ColorChannel.Hue, oldValue, newValue);

			OnHueChanged(oldValue, newValue);
		}

		protected virtual void OnRedChanged(double oldValue, double newValue)
		{
		}

		private void OnRedPropertyChangedPrivate(double oldValue, double newValue)
		{
			HandleChannelChange(ColorChannel.Red, newValue);

			OnChannelValueChanged(ColorChannel.Red, oldValue, newValue);

			OnRedChanged(oldValue, newValue);
		}

		protected virtual void OnSaturationChanged(double oldValue, double newValue)
		{
		}

		private void OnSaturationPropertyChangedPrivate(double oldValue, double newValue)
		{
			HandleChannelChange(ColorChannel.Saturation, newValue);

			OnChannelValueChanged(ColorChannel.Saturation, oldValue, newValue);

			OnSaturationChanged(oldValue, newValue);
		}

		protected virtual void OnValueChanged(double oldValue, double newValue)
		{
		}

		private void OnValuePropertyChangedPrivate(double oldValue, double newValue)
		{
			HandleChannelChange(ColorChannel.Value, newValue);

			OnChannelValueChanged(ColorChannel.Value, oldValue, newValue);

			OnValueChanged(oldValue, newValue);
		}

		private protected void ResumeChangeHandler()
		{
			if (_suspendChangeHandlerCount == 0)
				throw new InvalidOperationException();

			_suspendChangeHandlerCount--;

			if (_suspendChangeHandlerCount == 0)
				OnChangeHandlerResumed();
		}

		internal void ResumeChangeHandlerInternal()
		{
			ResumeChangeHandler();
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

		private protected void SuspendChangeHandler()
		{
			_suspendChangeHandlerCount++;

			if (_suspendChangeHandlerCount == 1)
				OnChangeHandlerSuspended();
		}

		internal void SuspendChangeHandlerInternal()
		{
			SuspendChangeHandler();
		}

		private void SyncChannels()
		{
			SuspendChangeHandler();

			HandleChannelValueChange = true;

			Alpha = _editorColor.Alpha;
			Red = _editorColor.Red;
			Green = _editorColor.Green;
			Blue = _editorColor.Blue;

			Hue = _editorColor.Hue;
			Saturation = _editorColor.Saturation;
			Value = _editorColor.Value;

			Color = _editorColor.Color;

			HandleChannelValueChange = false;

			ResumeChangeHandler();
		}

		internal void SyncColor(Color color)
		{
			Color = color;
		}

		internal virtual void SyncColor(EditorColorStruct color)
		{
			SuspendChangeHandler();

			HandleChannelValueChange = true;

			_editorColor = color;

			Red = _editorColor.Red;
			Green = _editorColor.Green;
			Blue = _editorColor.Blue;

			Color = _editorColor.Color;

			HandleChannelValueChange = false;

			ResumeChangeHandler();
		}
	}
}