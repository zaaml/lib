// <copyright file="UIElementUtils.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.Core.Runtime;

namespace Zaaml.PresentationCore.Utils
{
	internal static class UIElementUtils
	{
		public static void OverrideFocusable<TElement>(bool focusable) where TElement : UIElement
		{
			UIElement.FocusableProperty.OverrideMetadata(typeof(TElement), new FrameworkPropertyMetadata(focusable ? BooleanBoxes.True : BooleanBoxes.False));
		}
	}
}