// <copyright file="FrameworkElementUtils.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;

namespace Zaaml.PresentationCore.Utils
{
	internal static class FrameworkElementUtils
	{
		public static void OverrideVisualStyle<TElement>(Style style) where TElement : FrameworkElement
		{
			FrameworkElement.FocusVisualStyleProperty.OverrideMetadata(typeof(TElement), new FrameworkPropertyMetadata(style));
		}
	}
}