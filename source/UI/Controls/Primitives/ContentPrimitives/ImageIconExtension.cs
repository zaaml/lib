// <copyright file="ImageIconExtension.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows.Controls;
using System.Windows.Media;
using Zaaml.PresentationCore.MarkupExtensions;

namespace Zaaml.UI.Controls.Primitives.ContentPrimitives
{
	public sealed class ImageIconExtension : MarkupExtensionBase
	{
		public ImageSource Source { get; set; }

		public Stretch Stretch { get; set; } = Stretch.None;

		public StretchDirection StretchDirection { get; set; } = StretchDirection.Both;

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			return new ImageIcon
			{
				Source = Source,
				Stretch = Stretch,
				StretchDirection = StretchDirection
			};
		}
	}
}