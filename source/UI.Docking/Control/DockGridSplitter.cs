// <copyright file="DockGridSplitter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows.Controls;
using Zaaml.PresentationCore.Theming;
using Zaaml.PresentationCore.Utils;

namespace Zaaml.UI.Controls.Docking
{
	public class DockGridSplitter : GridSplitter
	{
		static DockGridSplitter()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<DockGridSplitter>();
			UIElementUtils.OverrideFocusable<DockGridSplitter>(false);
		}

		public DockGridSplitter()
		{
			this.OverrideStyleKey<DockGridSplitter>();
		}
	}
}