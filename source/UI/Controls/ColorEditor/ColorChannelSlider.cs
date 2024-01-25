// <copyright file="ColorChannelSlider.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.CommandCore;
using Zaaml.PresentationCore.Converters;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.PresentationCore.Utils;
using Zaaml.UI.Controls.Core;
using Zaaml.UI.Controls.Primitives.TrackBar;

namespace Zaaml.UI.Controls.ColorEditor
{
	[TemplateContractType(typeof(ColorChannelSliderTemplateContract))]
	public class ColorChannelSlider : TemplateContractControl
	{
		private static readonly DependencyPropertyKey ActualColorPropertyKey = DPM.RegisterReadOnly<Color, ColorChannelSlider>
			("ActualColor");

		public static readonly DependencyProperty ActualColorProperty = ActualColorPropertyKey.DependencyProperty;

		public static readonly DependencyProperty OrientationProperty = DPM.Register<Orientation, ColorChannelSlider>
			("Orientation", Orientation.Horizontal, d => d.OnOrientationPropertyChangedPrivate);

		public static readonly DependencyProperty ValueProperty = DPM.Register<double, ColorChannelSlider>
			("Value", 1.0, s => s.OnValuePropertyChangedPrivate, s => s.OnValueCoerce);

		public static readonly DependencyProperty ChannelProperty = DPM.Register<ColorChannel, ColorChannelSlider>
			("Channel", ColorChannel.Alpha, c => c.OnChannelChanged);

		private EditorColorStruct _editorColor = EditorColorStruct.Default;

		static ColorChannelSlider()
		{
			ControlUtils.OverrideIsTabStop<ColorChannelSlider>(false);

			DefaultStyleKeyHelper.OverrideStyleKey<ColorChannelSlider>();
		}

		public ColorChannelSlider()
		{
			ChannelCommand = new RelayCommand<ColorChannelCommand?>(OnChannelCommandExecuted);

			this.OverrideStyleKey<ColorChannelSlider>();
		}

		public Color ActualColor
		{
			get => (Color) GetValue(ActualColorProperty);
			private set => this.SetReadOnlyValue(ActualColorPropertyKey, value);
		}

		public ColorChannel Channel
		{
			get => (ColorChannel) GetValue(ChannelProperty);
			set => SetValue(ChannelProperty, value);
		}

		public ICommand ChannelCommand { get; }

		private ColorRectangleRenderer ColorRectangleRenderer => TemplateContract.ColorRectangleRenderer;

		internal EditorColorStruct EditorColor
		{
			get => _editorColor;
			set
			{
				_editorColor = value;

				Sync();
			}
		}

		public Orientation Orientation
		{
			get => (Orientation) GetValue(OrientationProperty);
			set => SetValue(OrientationProperty, value);
		}

		private ColorChannelSliderTemplateContract TemplateContract => (ColorChannelSliderTemplateContract) TemplateContractCore;

		private TrackBarControl TrackBarControl => TemplateContract.TrackBarControl;

		private TrackBarValueItem TrackBarValueItem => TemplateContract.TrackBarValueItem;

		public double Value
		{
			get => (double) GetValue(ValueProperty);
			set => SetValue(ValueProperty, value);
		}

		private void ExecuteChannelCommand(ColorChannelCommand command)
		{
			Value = ColorChannelEditorControl.ExecuteChannelCommand(Channel, Value, command);
		}

		private void OnChannelChanged()
		{
			UpdateTrackBar();

			Sync();
		}

		private void OnChannelCommandExecuted(ColorChannelCommand? command)
		{
			if (command == null)
				return;

			ExecuteChannelCommand(command.Value);
		}

		protected override void OnMouseWheel(MouseWheelEventArgs e)
		{
			e.Handled = true;

			Value = ColorChannelEditorControl.ExecuteChannelCommand(Channel, Value, e.Delta > 0 ? ColorChannelCommand.WheelIncrement : ColorChannelCommand.WheelDecrement);
		}

		private void OnOrientationPropertyChangedPrivate(Orientation oldValue, Orientation newValue)
		{
			UpdateTrackBar();
		}

		protected override void OnPreviewKeyDown(KeyEventArgs e)
		{
			switch (e.Key)
			{
				case Key.Left:
				case Key.Down:

					ExecuteChannelCommand(ColorChannelCommand.SmallDecrement);

					e.Handled = true;

					break;

				case Key.Right:
				case Key.Up:
					ExecuteChannelCommand(ColorChannelCommand.SmallIncrement);

					e.Handled = true;

					break;
			}

			if (e.Handled == false)
				base.OnPreviewKeyDown(e);
		}

		protected override void OnTemplateContractAttached()
		{
			base.OnTemplateContractAttached();

			UpdateTrackBar();
			UpdateColorRenderer();
		}

		private object OnValueCoerce(object o)
		{
			var channel = Channel;

			return ((double) o).Clamp(ColorUtils.GetChannelMinimumValue(channel), ColorUtils.GetChannelMaximumValue(channel));
		}

		private void OnValuePropertyChangedPrivate()
		{
		}

		private void Sync()
		{
			if (IsInitialized == false)
				return;

			var channel = Channel;

			Value = _editorColor.GetChannelValue(channel);
			ActualColor = _editorColor.Color;

			UpdateColorRenderer();
		}

		private void UpdateColorRenderer()
		{
			if (ColorRectangleRenderer == null)
				return;

			ColorRectangleRenderer.XAxis ??= new ColorRectangleRendererAxis();
			ColorRectangleRenderer.YAxis ??= new ColorRectangleRendererAxis();
			ColorRectangleRenderer.ZAxis ??= new ColorRectangleRendererAxis();

			var channel = Channel;
			var editorColor = EditorColor;
			var color = editorColor.Color;

			var orientation = Orientation;
			var mainAxis = orientation == Orientation.Horizontal ? ColorRectangleRenderer.XAxis : ColorRectangleRenderer.YAxis;
			var crossAxis = orientation == Orientation.Vertical ? ColorRectangleRenderer.XAxis : ColorRectangleRenderer.YAxis;

			mainAxis.Channel = channel;

			editorColor.SetChannelValue(ColorChannel.Alpha, 1.0);

			ColorRectangleRenderer.BaseColor = editorColor.Color;

			switch (channel)
			{
				case ColorChannel.Alpha:

					ColorRectangleRenderer.YAxis = null;
					ColorRectangleRenderer.ZAxis = null;

					break;

				case ColorChannel.Red:

					crossAxis.SetChannelValue(ColorChannel.Green, color, editorColor);
					ColorRectangleRenderer.ZAxis.SetChannelValue(ColorChannel.Blue, color, editorColor);

					break;
				case ColorChannel.Green:

					crossAxis.SetChannelValue(ColorChannel.Red, color, editorColor);
					ColorRectangleRenderer.ZAxis.SetChannelValue(ColorChannel.Blue, color, editorColor);

					break;
				case ColorChannel.Blue:

					crossAxis.SetChannelValue(ColorChannel.Red, color, editorColor);
					ColorRectangleRenderer.ZAxis.SetChannelValue(ColorChannel.Green, color, editorColor);

					break;
				case ColorChannel.Hue:

					crossAxis.SetChannelValue(ColorChannel.Saturation, color, editorColor);
					ColorRectangleRenderer.ZAxis.SetChannelValue(ColorChannel.Value, color, editorColor);

					break;
				case ColorChannel.Saturation:

					crossAxis.SetChannelValue(ColorChannel.Hue, color, editorColor);
					ColorRectangleRenderer.ZAxis.SetChannelValue(ColorChannel.Value, color, editorColor);

					break;
				case ColorChannel.Value:

					crossAxis.SetChannelValue(ColorChannel.Hue, color, editorColor);
					ColorRectangleRenderer.ZAxis.SetChannelValue(ColorChannel.Saturation, color, editorColor);

					break;
			}
		}

		private void UpdateTrackBar()
		{
			if (IsTemplateAttached == false)
				return;

			var channel = Channel;

			TrackBarControl.Minimum = ColorUtils.GetChannelMinimumValue(channel);
			TrackBarControl.Maximum = ColorUtils.GetChannelMaximumValue(channel);

			var valueConverter = Orientation == Orientation.Horizontal ? DummyConverter.Instance : new InvertedRangeValueConverter(TrackBarControl.Minimum, TrackBarControl.Maximum);
			var valueBinding = new Binding {Path = new PropertyPath(ValueProperty), Source = this, Mode = BindingMode.TwoWay, Converter = valueConverter};

			TrackBarValueItem.SetBinding(TrackBarValueItem.ValueProperty, valueBinding);
		}
	}
}