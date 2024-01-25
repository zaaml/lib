// <copyright file="ColorChannelTextEditor.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.PresentationCore.Utils;
using Zaaml.UI.Controls.Editors.Text;

namespace Zaaml.UI.Controls.ColorEditor
{
	[TemplateContractType(typeof(ColorChannelTextBoxTemplateContract))]
	public class ColorChannelTextEditor : TextEditorBase<double>
	{
		public static readonly DependencyProperty ChannelProperty = DPM.Register<ColorChannel, ColorChannelTextEditor>
			("Channel", ColorChannel.Alpha, d => d.OnChannelPropertyChangedPrivate);

		static ColorChannelTextEditor()
		{
			ControlUtils.OverrideIsTabStop<ColorChannelTextEditor>(false);

			DefaultStyleKeyHelper.OverrideStyleKey<ColorChannelTextEditor>();
		}

		public ColorChannelTextEditor()
		{
			this.OverrideStyleKey<ColorChannelTextEditor>();
		}

		public ColorChannel Channel
		{
			get => (ColorChannel) GetValue(ChannelProperty);
			set => SetValue(ChannelProperty, value);
		}

		private ColorChannelTextBoxTemplateContract TemplateContract => (ColorChannelTextBoxTemplateContract) TemplateContractCore;

		protected override string CoerceText(string text)
		{
			return TryParseTextValue(text, Channel, true, out _) ? text : Text;
		}

		private static bool CoerceValidateValue(ref double value, ColorChannel channel)
		{
			switch (channel)
			{
				case ColorChannel.Alpha:
					return value.Clamp(0.0, 1.0).IsCloseTo(value);
				case ColorChannel.Red:
					return value.Clamp(0.0, 1.0).IsCloseTo(value);
				case ColorChannel.Green:
					return value.Clamp(0.0, 1.0).IsCloseTo(value);
				case ColorChannel.Blue:
					return value.Clamp(0.0, 1.0).IsCloseTo(value);
				case ColorChannel.Hue:
					return value.Clamp(0.0, 360.0).IsCloseTo(value);
				case ColorChannel.Saturation:

					value /= 100.0;

					return value.Clamp(0.0, 1.0).IsCloseTo(value);
				case ColorChannel.Value:

					value /= 100.0;

					return value.Clamp(0.0, 1.0).IsCloseTo(value);
				default:
					throw new ArgumentOutOfRangeException(nameof(channel), channel, null);
			}
		}

		private static string FormatArgb(double value, bool argbHex)
		{
			var byteValue = (byte) (value * 255.0);

			return argbHex ? byteValue.ToString("X2") : byteValue.ToString();
		}

		private static string FormatStringValue(double value, ColorChannel channel, bool argbHex)
		{
			switch (channel)
			{
				case ColorChannel.Alpha:
					return FormatArgb(value, argbHex);
				case ColorChannel.Red:
					return FormatArgb(value, argbHex);
				case ColorChannel.Green:
					return FormatArgb(value, argbHex);
				case ColorChannel.Blue:
					return FormatArgb(value, argbHex);
				case ColorChannel.Hue:
					return $"{value:###}";
				case ColorChannel.Saturation:
					return $"{(value * 100):###}";
				case ColorChannel.Value:
					return $"{(value * 100):###}";
				default:
					throw new ArgumentOutOfRangeException(nameof(channel), channel, null);
			}
		}

		protected override string FormatValue(double value)
		{
			return FormatStringValue(value, Channel, true);
		}

		private void OnChannelPropertyChangedPrivate(ColorChannel oldValue, ColorChannel newValue)
		{
			SyncText();
		}

		protected override bool StaysEditingOnCommit => true;

		protected override void OnMouseWheel(MouseWheelEventArgs e)
		{
			e.Handled = true;

			Value = ColorChannelEditorControl.ExecuteChannelCommand(Channel, Value, e.Delta > 0 ? ColorChannelCommand.WheelIncrement : ColorChannelCommand.WheelDecrement);
		}

		protected override void OnPreviewKeyDown(KeyEventArgs e)
		{
			if (e.Handled)
				return;

			switch (e.Key)
			{
				case Key.Up:

					Value = ColorChannelEditorControl.ExecuteChannelCommand(Channel, Value, ColorChannelCommand.SmallIncrement);
					e.Handled = true;

					break;
				case Key.Down:

					Value = ColorChannelEditorControl.ExecuteChannelCommand(Channel, Value, ColorChannelCommand.SmallDecrement);
					e.Handled = true;

					break;
				case Key.PageUp:

					Value = ColorChannelEditorControl.ExecuteChannelCommand(Channel, Value, ColorChannelCommand.LargeIncrement);
					e.Handled = true;

					break;
				case Key.PageDown:

					Value = ColorChannelEditorControl.ExecuteChannelCommand(Channel, Value, ColorChannelCommand.LargeDecrement);
					e.Handled = true;

					break;
			}

			if (e.Handled == false)
				base.OnPreviewKeyDown(e);
		}

		protected override bool TryParse(string text, out double value)
		{
			return TryParseTextValue(text, Channel, true, out value);
		}

		private bool TryParseArgbTextValue(string textValue, bool argbHex, out double result)
		{
			if (argbHex)
			{
				if (int.TryParse(textValue, NumberStyles.HexNumber, CultureInfo.CurrentCulture, out var intValue))
				{
					result = intValue / 255.0;

					return true;
				}

				result = double.NaN;

				return false;
			}

			return double.TryParse(textValue, NumberStyles.Float, CultureInfo.CurrentCulture, out result);
		}

		private bool TryParseTextValue(string textValue, ColorChannel channel, bool argbHex, out double result)
		{
			switch (channel)
			{
				case ColorChannel.Alpha:
					return TryParseArgbTextValue(textValue, argbHex, out result) && CoerceValidateValue(ref result, channel);
				case ColorChannel.Red:
					return TryParseArgbTextValue(textValue, argbHex, out result) && CoerceValidateValue(ref result, channel);
				case ColorChannel.Green:
					return TryParseArgbTextValue(textValue, argbHex, out result) && CoerceValidateValue(ref result, channel);
				case ColorChannel.Blue:
					return TryParseArgbTextValue(textValue, argbHex, out result) && CoerceValidateValue(ref result, channel);
				case ColorChannel.Hue:
					return double.TryParse(textValue, out result) && CoerceValidateValue(ref result, channel);
				case ColorChannel.Saturation:
					return double.TryParse(textValue, out result) && CoerceValidateValue(ref result, channel);
				case ColorChannel.Value:
					return double.TryParse(textValue, out result) && CoerceValidateValue(ref result, channel);
				default:
					throw new ArgumentOutOfRangeException(nameof(channel), channel, null);
			}
		}
	}
}