// <copyright file="DockItemStateChangedEventArgs.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.UI.Controls.Docking
{
	public class DockItemStateChangedEventArgs : EventArgs
	{
		public DockItemStateChangedEventArgs(DockItem item, DockItemState oldDockState, DockItemState newDockState)
		{
			Item = item;
			NewDockState = newDockState;
			OldDockState = oldDockState;
		}

		public DockItem Item { get; }

		public DockItemState NewDockState { get; }

		public DockItemState OldDockState { get; }
	}
}