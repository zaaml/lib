// <copyright file="UITestBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using NUnit.Framework;
using Zaaml.PresentationCore.Theming;

namespace Zaaml.UI.Test
{
	public abstract class UITestBase<TApp> where TApp : UITestApp, new()
	{
		private static readonly Lazy<TApp> LazyApp = new(() => new TApp());
		protected virtual Size DefaultRenderSize => new(1280, 720);

		[SetUp]
		protected void Init()
		{
			_ = LazyApp.Value;
		}

		protected void RenderElement<TElement>(TElement element) where TElement : FrameworkElement
		{
			ThemeManager.SetTheme(element, ThemeManager.ApplicationTheme);

			element.Measure(DefaultRenderSize);
			element.Arrange(new Rect(DefaultRenderSize));
		}
	}
}