// <copyright file="NavigationViewPage.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.NavigationView
{
	public class NavigationViewPage : HeaderedContentControl
	{
		private static readonly DependencyPropertyKey NavigationViewItemPropertyKey = DPM.RegisterReadOnly<NavigationViewItem, NavigationViewPage>
			("NavigationViewItem");

		public static readonly DependencyProperty NavigationViewItemProperty = NavigationViewItemPropertyKey.DependencyProperty;

		static NavigationViewPage()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<NavigationViewPage>();
		}

		public NavigationViewPage()
		{
			this.OverrideStyleKey<NavigationViewPage>();
		}

		public NavigationViewItem NavigationViewItem
		{
			get => (NavigationViewItem) GetValue(NavigationViewItemProperty);
			internal set => this.SetReadOnlyValue(NavigationViewItemPropertyKey, value);
		}
	}
}