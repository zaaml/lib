// <copyright file="IconPresenter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Markup;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Extensions;

namespace Zaaml.UI.Controls.Primitives.ContentPrimitives
{
	[ContentProperty(nameof(Icon))]
	public sealed class IconPresenter : IconPresenterBase
	{
		protected override Size ArrangeOverrideCore(Size finalSize)
		{
			var icon = ActualIcon;

			if (icon == null)
				return finalSize;

			icon.Arrange(finalSize.Rect());

			return finalSize;
		}

		protected override Size MeasureOverrideCore(Size availableSize)
		{
			var icon = ActualIcon;

			if (icon == null)
				return XamlConstants.ZeroSize;

			icon.Measure(availableSize);

			return icon.DesiredSize;
		}
	}

	internal interface IIconPresenter
	{
		IconBase Icon { get; }
	}
}