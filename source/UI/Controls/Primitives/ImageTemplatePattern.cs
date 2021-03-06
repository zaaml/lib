// <copyright file="ImageTemplatePattern.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.UI.Controls.Primitives
{
	internal sealed class ImageTemplatePattern : BackgroundPatternBase
	{
		public static readonly DependencyProperty ImageProperty = DPM.Register<ImageSource, ImageTemplatePattern>
			("Image", i => i.OnImageChanged);

		public ImageSource Image
		{
			get => (ImageSource) GetValue(ImageProperty);
			set => SetValue(ImageProperty, value);
		}

		protected override UIElement CreatePatternElementCore()
		{
			return new Image {Source = Image, Stretch = Stretch.None};
		}

		private void OnImageChanged()
		{
			OnPatternChanged();
		}
	}
}