// <copyright file="ScrollBarThumb.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.Theming;
using Zaaml.PresentationCore.Utils;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.ScrollView
{
	public sealed class ScrollBarThumb : Control
	{
		private bool _isPressed;

		static ScrollBarThumb()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<ScrollBarThumb>();
			UIElementUtils.OverrideFocusable<ScrollBarThumb>(false);
		}

		public ScrollBarThumb()
		{
			this.OverrideStyleKey<ScrollBarThumb>();
		}

		internal bool IsPressed
		{
			get => _isPressed;
			set
			{
				if (_isPressed == value)
					return;

				_isPressed = value;

				UpdateVisualState(true);
			}
		}

		internal Size ThumbSize { get; set; }

		protected override Size MeasureOverride(Size availableSize)
		{
			var measureOverride = base.MeasureOverride(availableSize);

			measureOverride = measureOverride.Clamp(ThumbSize, XamlConstants.InfiniteSize);

			return measureOverride;
		}

		protected override void UpdateVisualState(bool useTransitions)
		{
			if (IsEnabled == false)
				GotoVisualState("Disabled", useTransitions);
			else if (IsPressed)
				GotoVisualState("Pressed", useTransitions);
			else if (IsMouseOver)
				GotoVisualState("MouseOver", useTransitions);
			else
				GotoVisualState("Normal", useTransitions);
		}
	}
}