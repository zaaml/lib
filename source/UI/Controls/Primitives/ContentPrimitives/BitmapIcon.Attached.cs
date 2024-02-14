// <copyright file="BitmapIcon.Attached.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Zaaml.UI.Controls.Primitives.ContentPrimitives
{
	public sealed partial class BitmapIcon
	{
		public static ImageSource GetSource(DependencyObject dependencyObject)
		{
			return (ImageSource)dependencyObject.GetValue(SourceProperty);
		}

		public static Stretch GetStretch(DependencyObject dependencyObject)
		{
			return (Stretch)dependencyObject.GetValue(StretchProperty);
		}

		public static StretchDirection GetStretchDirection(DependencyObject dependencyObject)
		{
			return (StretchDirection)dependencyObject.GetValue(StretchDirectionProperty);
		}

		public static void SetSource(DependencyObject dependencyObject, ImageSource imageSource)
		{
			dependencyObject.SetValue(SourceProperty, imageSource);
		}

		public static void SetStretch(DependencyObject dependencyObject, Stretch stretch)
		{
			dependencyObject.SetValue(StretchProperty, stretch);
		}

		public static void SetStretchDirection(DependencyObject dependencyObject, StretchDirection stretchDirection)
		{
			dependencyObject.SetValue(StretchDirectionProperty, stretchDirection);
		}
	}
}