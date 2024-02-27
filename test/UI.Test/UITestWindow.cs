// <copyright file="UITestWindow.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.UI.Windows;

namespace Zaaml.UI.Test
{
	public class UITestWindow : AppWindow
	{
		public UITestWindow()
		{
			ShowInTaskbar = false;
			AllowsTransparency = true;
			Opacity = 0.0;
		}
	}
}