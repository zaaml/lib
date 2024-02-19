// <copyright file="TabViewTestWindow.xaml.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Interop;
using Zaaml.UI.Test.Utils;

namespace Zaaml.UI.Test.Controls.TabView
{
	public partial class TabViewTestWindow
	{
		public TabViewTestWindow()
		{
			InitializeComponent();
		}

		protected override void OnContentRendered(EventArgs e)
		{
			Image2.Source = ScreenUtils.GetWindowImage((HwndSource)PresentationSource.FromVisual(this));
		}
	}
}