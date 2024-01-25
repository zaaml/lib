// <copyright file="DockController.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.UI.Controls.Docking
{
	internal sealed class DockController : DockControllerBase
	{
		public DockController(DockControlView controlView) : base(controlView)
		{
		}

		protected override bool IsPreview => false;

		protected override void OnItemDockStateChanged(DockItem dockItem, DockItemState oldState, DockItemState newState)
		{
			DockControl?.OnItemDockStateChangedInternal(dockItem, oldState, newState);
		}
	}
}