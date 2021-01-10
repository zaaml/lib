// <copyright file="NavigationViewDisplayModeChangedEventArgs.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.UI.Controls.NavigationView
{
	public sealed class NavigationViewDisplayModeChangedEventArgs : EventArgs
	{
		public NavigationViewDisplayModeChangedEventArgs(NavigationViewDisplayMode oldDisplayMode, NavigationViewDisplayMode newDisplayMode)
		{
			OldDisplayMode = oldDisplayMode;
			NewDisplayMode = newDisplayMode;
		}

		public NavigationViewDisplayMode NewDisplayMode { get; }

		public NavigationViewDisplayMode OldDisplayMode { get; }
	}
}