// <copyright file="ColorChannelEditorControl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Media;
using Zaaml.Core;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.PresentationCore.Utils;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.ColorEditor
{
	[TemplateContractType(typeof(ColorChannelEditorControlTemplateContract))]
	public class ColorChannelEditorControl : TemplateContractControl
	{
		private static readonly DependencyPropertyKey ActualChannelNamePropertyKey = DPM.RegisterReadOnly<string, ColorChannelEditorControl>
			("ActualChannelName");

		public static readonly DependencyProperty ActualChannelNameProperty = ActualChannelNamePropertyKey.DependencyProperty;

		public static readonly DependencyProperty ChannelProperty = DPM.Register<ColorChannel, ColorChannelEditorControl>
			("Channel", ColorChannel.Alpha, d => d.OnChannelPropertyChangedPrivate);

		public static readonly DependencyProperty ValueProperty = DPM.Register<double, ColorChannelEditorControl>
			("Value", 1.0, d => d.OnValuePropertyChangedPrivate);

		private EditorColorStruct _editorColor = EditorColorStruct.Default;

		static ColorChannelEditorControl()
		{
			ControlUtils.OverrideIsTabStop<ColorChannelEditorControl>(false);

			DefaultStyleKeyHelper.OverrideStyleKey<ColorChannelEditorControl>();
		}

		public ColorChannelEditorControl()
		{
			this.OverrideStyleKey<ColorChannelSlider>();

			UpdateActualChannelName();
		}

		public string ActualChannelName
		{
			get => (string) GetValue(ActualChannelNameProperty);
			private set => this.SetReadOnlyValue(ActualChannelNamePropertyKey, value);
		}

		public ColorChannel Channel
		{
			get => (ColorChannel) GetValue(ChannelProperty);
			set => SetValue(ChannelProperty, value);
		}

		internal EditorColorStruct EditorColor
		{
			get => _editorColor;
			set
			{
				_editorColor = value;

				if (Slider != null)
					Slider.EditorColor = value;
			}
		}

		private ColorChannelSlider Slider => TemplateContract.Slider;

		private ColorChannelEditorControlTemplateContract TemplateContract => (ColorChannelEditorControlTemplateContract) TemplateContractInternal;

		private ColorChannelTextEditor TextEditor => TemplateContract.TextEditor;

		public double Value
		{
			get => (double) GetValue(ValueProperty);
			set => SetValue(ValueProperty, value);
		}

		internal static double ExecuteChannelCommand(ColorChannel channel, double value, ColorChannelCommand command)
		{
			switch (command)
			{
				case ColorChannelCommand.SmallIncrement:
					return value + GetChannelSmallChange(channel);
				case ColorChannelCommand.SmallDecrement:
					return value - GetChannelSmallChange(channel);
				case ColorChannelCommand.LargeIncrement:
					return value + GetChannelLargeChange(channel);
				case ColorChannelCommand.LargeDecrement:
					return value - GetChannelLargeChange(channel);
				case ColorChannelCommand.WheelIncrement:
					return value + GetChannelWheelChange(channel);
				case ColorChannelCommand.WheelDecrement:
					return value - GetChannelWheelChange(channel);
				default:
					return value;
			}
		}

		private static double GetChannelLargeChange(ColorChannel channel)
		{
			return channel switch
			{
				ColorChannel.Hue => 30,
				_ => 0.1
			};
		}

		private static double GetChannelSmallChange(ColorChannel channel)
		{
			return channel switch
			{
				ColorChannel.Hue => 1,
				_ => 0.01
			};
		}

		private static double GetChannelWheelChange(ColorChannel channel)
		{
			return channel switch
			{
				ColorChannel.Hue => 10,
				_ => 0.05
			};
		}

		private void OnChannelPropertyChangedPrivate(ColorChannel oldValue, ColorChannel newValue)
		{
			UpdateActualChannelName();
		}

		private void OnColorPropertyChangedPrivate(Color oldValue, Color newValue)
		{
		}

		protected override void OnTemplateContractAttached()
		{
			base.OnTemplateContractAttached();

			Slider.EditorColor = EditorColor;
		}

		private void OnValuePropertyChangedPrivate(double oldValue, double newValue)
		{
		}

		internal static double RoundChannelValue(ColorChannel channel, double value)
		{
			return channel switch
			{
				ColorChannel.Hue => value.RoundMidPointFromZero(0),
				_ => value.RoundMidPointFromZero(3)
			};
		}

		private void UpdateActualChannelName()
		{
			ActualChannelName = Channel switch
			{
				ColorChannel.Alpha => "A",
				ColorChannel.Red => "R",
				ColorChannel.Green => "G",
				ColorChannel.Blue => "B",
				ColorChannel.Hue => "H",
				ColorChannel.Saturation => "S",
				ColorChannel.Value => "B",
				_ => throw new ArgumentOutOfRangeException()
			};
		}
	}

	public class ColorChannelEditorControlTemplateContract : TemplateContract
	{
		[TemplateContractPart(Required = true)]
		public ColorChannelSlider Slider { get; [UsedImplicitly] private set; }

		[TemplateContractPart(Required = true)]
		public ColorChannelTextEditor TextEditor { get; [UsedImplicitly] private set; }
	}
}