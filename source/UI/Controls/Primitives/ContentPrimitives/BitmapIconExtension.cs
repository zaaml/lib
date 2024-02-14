// <copyright file="BitmapIconExtension.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows.Controls;
using System.Windows.Media;
using Zaaml.PresentationCore.MarkupExtensions;

namespace Zaaml.UI.Controls.Primitives.ContentPrimitives
{
	public sealed class BitmapIconExtension : MarkupExtensionBase
	{
		public ImageSource Source { get; set; }

		public Stretch Stretch { get; set; } = Stretch.Uniform;

		public StretchDirection StretchDirection { get; set; } = StretchDirection.Both;

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			return new BitmapIcon
			{
				Source = Source,
				Stretch = Stretch,
				StretchDirection = StretchDirection
			};
		}
	}
}