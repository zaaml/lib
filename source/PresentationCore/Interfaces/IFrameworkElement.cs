// <copyright file="IFrameworkElement.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;

namespace Zaaml.PresentationCore.Interfaces
{
	internal interface IUIElement : IDependencyObject
	{
		void InvalidateArrange();

		void InvalidateMeasure();
	}

	internal interface IFrameworkElement : IUIElement
	{
		double Height { get; }

		HorizontalAlignment HorizontalAlignment { get; }

		double MaxHeight { get; }

		double MaxWidth { get; }

		double MinHeight { get; }

		double MinWidth { get; }

		VerticalAlignment VerticalAlignment { get; }

		double Width { get; }
	}
}