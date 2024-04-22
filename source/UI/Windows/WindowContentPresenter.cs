// <copyright file="WindowContentPresenter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Windows;
using System.Windows.Markup;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Windows
{
	[ContentProperty(nameof(Content))]
	public sealed class WindowContentPresenter : Control, IWindowElement
	{
		public static readonly DependencyProperty ContentProperty = DPM.Register<object, WindowContentPresenter>
			("Content");

		public static readonly DependencyProperty ContentTemplateProperty = DPM.Register<DataTemplate, WindowContentPresenter>
			("ContentTemplate");

		static WindowContentPresenter()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<WindowContentPresenter>();
		}

		public WindowContentPresenter()
		{
			Focusable = false;
			IsTabStop = false;

			this.OverrideStyleKey<WindowContentPresenter>();
		}

		public object Content
		{
			get => GetValue(ContentProperty);
			set => SetValue(ContentProperty, value);
		}

		public DataTemplate ContentTemplate
		{
			get => (DataTemplate)GetValue(ContentTemplateProperty);
			set => SetValue(ContentTemplateProperty, value);
		}

		IEnumerable<IWindowElement> IWindowElement.EnumerateWindowElements()
		{
			yield break;
		}

		IWindow IWindowElement.Window { get; set; }
	}
}