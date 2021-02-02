// <copyright file="ArtboardItem.Migrate.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Media;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.UI.Controls.Artboard
{
	public partial class ArtboardItem
	{
		public static readonly DependencyProperty DesignWidthProperty = DPM.Register<double, ArtboardItem>
			("DesignWidth", 640.0, d => d.OnDesignWidthPropertyChangedPrivate);

		public static readonly DependencyProperty DesignHeightProperty = DPM.Register<double, ArtboardItem>
			("DesignHeight", 480.0, d => d.OnDesignHeightPropertyChangedPrivate);

		public static readonly DependencyProperty DesignTopContentProperty = DPM.Register<object, ArtboardItem>
			("DesignTopContent");

		public static readonly DependencyProperty DesignBottomContentProperty = DPM.Register<object, ArtboardItem>
			("DesignBottomContent");

		public static readonly DependencyProperty DesignTopContentTemplateProperty = DPM.Register<DataTemplate, ArtboardItem>
			("DesignTopContentTemplate");

		public static readonly DependencyProperty DesignBottomContentTemplateProperty = DPM.Register<DataTemplate, ArtboardItem>
			("DesignBottomContentTemplate");

		public static readonly DependencyProperty DesignBackgroundProperty = DPM.Register<Brush, ArtboardItem>
			("DesignBackground");

		public static readonly DependencyProperty DesignBorderBrushProperty = DPM.Register<Brush, ArtboardItem>
			("DesignBorderBrush");

		public static readonly DependencyProperty DesignBorderThicknessProperty = DPM.Register<Thickness, ArtboardItem>
			("DesignBorderThickness");

		public Brush DesignBackground
		{
			get => (Brush)GetValue(DesignBackgroundProperty);
			set => SetValue(DesignBackgroundProperty, value);
		}

		public Brush DesignBorderBrush
		{
			get => (Brush)GetValue(DesignBorderBrushProperty);
			set => SetValue(DesignBorderBrushProperty, value);
		}

		public Thickness DesignBorderThickness
		{
			get => (Thickness)GetValue(DesignBorderThicknessProperty);
			set => SetValue(DesignBorderThicknessProperty, value);
		}

		public object DesignBottomContent
		{
			get => GetValue(DesignBottomContentProperty);
			set => SetValue(DesignBottomContentProperty, value);
		}

		private ArtboardDesignContentControl DesignBottomContentControl => TemplateContract.DesignBottomContentControl;

		public DataTemplate DesignBottomContentTemplate
		{
			get => (DataTemplate)GetValue(DesignBottomContentTemplateProperty);
			set => SetValue(DesignBottomContentTemplateProperty, value);
		}

		public double DesignHeight
		{
			get => (double)GetValue(DesignHeightProperty);
			set => SetValue(DesignHeightProperty, value);
		}

		public object DesignTopContent
		{
			get => GetValue(DesignTopContentProperty);
			set => SetValue(DesignTopContentProperty, value);
		}

		private ArtboardDesignContentControl DesignTopContentControl => TemplateContract.DesignTopContentControl;

		public DataTemplate DesignTopContentTemplate
		{
			get => (DataTemplate)GetValue(DesignTopContentTemplateProperty);
			set => SetValue(DesignTopContentTemplateProperty, value);
		}

		public double DesignWidth
		{
			get => (double)GetValue(DesignWidthProperty);
			set => SetValue(DesignWidthProperty, value);
		}

		private void OnDesignHeightPropertyChangedPrivate()
		{
			OnDesignSizeChanged();
		}

		private void OnDesignSizeChanged()
		{
			UpdateDesignSize();
		}

		private void OnDesignWidthPropertyChangedPrivate()
		{
			OnDesignSizeChanged();
		}

		private void UpdateDesignSize()
		{
			//if (IsTemplateAttached == false)
			//	return;

			//var designWidth = DesignWidth;
			//var designHeight = DesignHeight;

			//foreach (var component in Components)
			//{
			//	component.DesignWidth = designWidth;
			//	component.DesignHeight = designHeight;
			//}

			//ScrollView.OnDesignSizeChangedInternal();
		}
	}
}