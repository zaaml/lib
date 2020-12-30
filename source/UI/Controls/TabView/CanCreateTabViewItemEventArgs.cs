//  <copyright file="CanCreateTabItemEventArgs.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//    Copyright (c) zaaml. All rights reserved.
//  </copyright>

using System;

namespace Zaaml.UI.Controls.TabView
{
	public class TabViewControlEventArgs : EventArgs
	{
		public TabViewControlEventArgs(TabViewControl tabViewControl)
		{
			TabViewControl = tabViewControl;
		}

		public TabViewControl TabViewControl { get; }
	}

	public class CanCreateTabViewItemEventArgs : TabViewControlEventArgs
	{
		public CanCreateTabViewItemEventArgs(TabViewControl tabViewControl) : base(tabViewControl)
		{
		}

		#region Properties

		public bool CanCreate { get; set; }

		#endregion
	}
}