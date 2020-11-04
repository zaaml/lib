// <copyright file="UIElementUtils.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;

namespace Zaaml.PresentationCore.Utils
{
	internal static class UIElementUtils
	{
		public static void OverrideFocusable<TElement>(bool focusable) where TElement : UIElement
		{
			UIElement.FocusableProperty.OverrideMetadata(typeof(TElement), new FrameworkPropertyMetadata(focusable ? KnownBoxes.BoolTrue : KnownBoxes.BoolFalse));
		}
	}
}