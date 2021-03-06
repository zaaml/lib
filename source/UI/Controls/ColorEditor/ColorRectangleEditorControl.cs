// <copyright file="ColorRectangleEditorControl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Converters;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.PresentationCore.Utils;
using Zaaml.UI.Controls.Primitives;

namespace Zaaml.UI.Controls.ColorEditor
{
	[TemplateContractType(typeof(ColorRectangleEditorControlTemplateContract))]
	public class ColorRectangleEditorControl : ColorEditorControlBase
	{
		public static readonly DependencyProperty XChannelProperty = DPM.Register<ColorChannel, ColorRectangleEditorControl>
			("XChannel", ColorChannel.Saturation, d => d.OnXChannelPropertyChangedPrivate);

		public static readonly DependencyProperty YChannelProperty = DPM.Register<ColorChannel, ColorRectangleEditorControl>
			("YChannel", ColorChannel.Value, d => d.OnYChannelPropertyChangedPrivate);

		public static readonly DependencyProperty ZChannelProperty = DPM.Register<ColorChannel, ColorRectangleEditorControl>
			("ZChannel", ColorChannel.Hue, d => d.OnZChannelPropertyChangedPrivate);

		// ReSharper disable once InconsistentNaming
		private static readonly Dictionary<ColorChannel, Tuple<ColorChannel, ColorChannel>> ZXYDictionary = new()
		{
			{ColorChannel.Red, Tuple.Create(ColorChannel.Blue, ColorChannel.Green)},
			{ColorChannel.Green, Tuple.Create(ColorChannel.Blue, ColorChannel.Red)},
			{ColorChannel.Blue, Tuple.Create(ColorChannel.Red, ColorChannel.Green)},
			{ColorChannel.Hue, Tuple.Create(ColorChannel.Saturation, ColorChannel.Value)},
			{ColorChannel.Saturation, Tuple.Create(ColorChannel.Hue, ColorChannel.Value)},
			{ColorChannel.Value, Tuple.Create(ColorChannel.Hue, ColorChannel.Saturation)}
		};

		static ColorRectangleEditorControl()
		{
			ControlUtils.OverrideIsTabStop<ColorRectangleEditorControl>(false);

			DefaultStyleKeyHelper.OverrideStyleKey<ColorRectangleEditorControl>();
		}

		public ColorRectangleEditorControl()
		{
			this.OverrideStyleKey<ColorRectangleEditorControl>();
		}

		private ColorRectangleRenderer ColorRectangleRenderer => TemplateContract.ColorRectangleRenderer;

		private ColorRectangleEditorControlTemplateContract TemplateContract => (ColorRectangleEditorControlTemplateContract) TemplateContractInternal;

		public ColorChannel XChannel
		{
			get => (ColorChannel) GetValue(XChannelProperty);
			set => SetValue(XChannelProperty, value);
		}

		private XYController XYController => TemplateContract.XYController;

		private XYControllerItem XYControllerItem => TemplateContract.XYControllerItem;

		public ColorChannel YChannel
		{
			get => (ColorChannel) GetValue(YChannelProperty);
			set => SetValue(YChannelProperty, value);
		}

		public ColorChannel ZChannel
		{
			get => (ColorChannel) GetValue(ZChannelProperty);
			set => SetValue(ZChannelProperty, value);
		}

		private ColorChannelSlider ZChannelSlider => TemplateContract.ZChannelSlider;

		private void Bind()
		{
			BindXChannel();
			BindYChannel();
			BindZChannel();
		}

		private void BindXChannel()
		{
			if (IsTemplateAttached == false)
				return;

			XYController.MinimumX = ColorUtils.GetChannelMinimumValue(XChannel);
			XYController.MaximumX = ColorUtils.GetChannelMaximumValue(XChannel);

			var xBinding = new Binding {Path = new PropertyPath(GetChannelProperty(XChannel)), Source = this, Mode = BindingMode.TwoWay};

			XYControllerItem.SetBinding(XYControllerItem.XProperty, xBinding);

			ColorRectangleRenderer.XAxis ??= new ColorRectangleRendererAxis();

			ColorRectangleRenderer.XAxis.Channel = XChannel;
		}

		private void BindYChannel()
		{
			if (IsTemplateAttached == false)
				return;

			XYController.MinimumY = ColorUtils.GetChannelMinimumValue(YChannel);
			XYController.MaximumY = ColorUtils.GetChannelMaximumValue(YChannel);

			var yConverter = new InvertedRangeValueConverter(XYController.MinimumY, XYController.MaximumY);
			var yBinding = new Binding {Path = new PropertyPath(GetChannelProperty(YChannel)), Source = this, Mode = BindingMode.TwoWay, Converter = yConverter};

			XYControllerItem.SetBinding(XYControllerItem.YProperty, yBinding);

			ColorRectangleRenderer.YAxis ??= new ColorRectangleRendererAxis();

			ColorRectangleRenderer.YAxis.Channel = YChannel;
		}

		private void BindZChannel()
		{
			if (IsTemplateAttached == false)
				return;

			var zChannel = ZChannel;

			ZChannelSlider.Channel = zChannel;

			ZChannelSlider.SetBinding(ColorChannelSlider.ValueProperty, new Binding {Path = new PropertyPath(GetChannelProperty(zChannel)), Source = this, Mode = BindingMode.TwoWay});

			ColorRectangleRenderer.ZAxis ??= new ColorRectangleRendererAxis();
			ColorRectangleRenderer.ZAxis.SetBinding(ColorRectangleRendererAxis.ValueProperty, new Binding {Path = new PropertyPath(GetChannelProperty(zChannel)), Source = this});

			ColorRectangleRenderer.ZAxis.Channel = ZChannel;
		}

		public static void GetXYChannels(ColorChannel zChannel, out ColorChannel xChannel, out ColorChannel yChannel)
		{
			// ReSharper disable once UseDeconstruction
			var tuple = ZXYDictionary[zChannel];

			xChannel = tuple.Item1;
			yChannel = tuple.Item2;
		}

		protected override void OnChannelValueChanged(ColorChannel channel, double oldValue, double newValue)
		{
			base.OnChannelValueChanged(channel, oldValue, newValue);

			if (ZChannelSlider != null)
				ZChannelSlider.EditorColor = EditorColor;
		}

		protected override void OnTemplateContractAttached()
		{
			base.OnTemplateContractAttached();

			Bind();
		}

		private void OnXChannelPropertyChangedPrivate(ColorChannel oldValue, ColorChannel newValue)
		{
			BindXChannel();
		}

		private void OnYChannelPropertyChangedPrivate(ColorChannel oldValue, ColorChannel newValue)
		{
			BindYChannel();
		}

		private void OnZChannelPropertyChangedPrivate(ColorChannel oldValue, ColorChannel newValue)
		{
			BindZChannel();
		}

		public void SetChannels()
		{
		}

		internal override void SyncColor(EditorColorStruct color)
		{
			base.SyncColor(color);

			if (ZChannelSlider != null)
				ZChannelSlider.EditorColor = EditorColor;
		}

		protected override Size MeasureOverride(Size availableSize)
		{
			var measureOverride = base.MeasureOverride(availableSize);

			return measureOverride;
		}

		protected override Size ArrangeOverride(Size arrangeBounds)
		{
			var arrangeOverride = base.ArrangeOverride(arrangeBounds);

			return arrangeOverride;
		}
	}
}