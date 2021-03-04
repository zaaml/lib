// <copyright file="ImageTemplatePattern.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.UI.Controls.Primitives
{
	internal sealed class ImageTemplatePattern : DependencyObject, IPattern
	{
		#region Static Fields and Constants

		public static readonly DependencyProperty ImageProperty = DPM.Register<ImageSource, ImageTemplatePattern>
			("Image", i => i.OnChanged);

		#endregion

		#region Fields

		public event EventHandler TemplateChanged;

		#endregion

		#region Properties

		public ImageSource Image
		{
			get => (ImageSource) GetValue(ImageProperty);
			set => SetValue(ImageProperty, value);
		}

		#endregion

		#region  Methods

		public UIElement CreateElement()
		{
			return new Image {Source = Image, Stretch = Stretch.None};
		}

		private void OnChanged()
		{
			TemplateChanged?.Invoke(this, EventArgs.Empty);
		}

		#endregion
	}
}