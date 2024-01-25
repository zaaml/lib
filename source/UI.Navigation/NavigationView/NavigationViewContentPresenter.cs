// <copyright file="NavigationViewContentPresenter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.NavigationView
{
	public class NavigationViewContentPresenter : Control
	{
		public static readonly DependencyProperty ContentProperty = DPM.Register<object, NavigationViewContentPresenter>
			("Content", p => p.LogicalChildMentor.OnLogicalChildPropertyChanged);

		static NavigationViewContentPresenter()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<NavigationViewContentPresenter>();
		}

		public NavigationViewContentPresenter()
		{
			this.OverrideStyleKey<NavigationViewContentPresenter>();
		}

		public object Content
		{
			get => GetValue(ContentProperty);
			set => SetValue(ContentProperty, value);
		}
	}
}