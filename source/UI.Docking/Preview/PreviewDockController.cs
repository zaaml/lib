// <copyright file="PreviewDockController.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;

namespace Zaaml.UI.Controls.Docking
{
	internal sealed class PreviewDockController : DockControllerBase
	{
		public PreviewDockController(PreviewDockControlView controlView) : base(controlView)
		{
		}

		public bool IsEnabled { get; set; }

		protected override bool IsPreview => true;

		private PreviewDockControlView PreviewControlView => (PreviewDockControlView)ControlView;

		public override BaseLayout GetTargetLayout(DockItem item)
		{
			if (IsPreview == false || item.DockState != DockItemState.Float)
				return GetLayout(item.DockState);

			var dragMoveItem = DockControl?.DragMoveItem;

			if (dragMoveItem == null)
				return null;

			var currentDropGuide = PreviewControlView.CurrentDropGuide;

			if (currentDropGuide == null)
				return null;

			var dragMoveItems = new HashSet<string>(dragMoveItem.EnumerateItems().Select(w => w.Name), StringComparer.OrdinalIgnoreCase);

			var currentTarget = (currentDropGuide.Compass.PlacementTarget as DockItem)?.Root;

			if (currentTarget?.DockState != DockItemState.Float)
				return null;

			if (item.IsRoot && item.EnumerateDescendants(true).Any(w => dragMoveItems.Contains(w.Name)))
				return FloatLayout;

			return null;
		}

		internal override void OnItemArranged(DockItem dockItem)
		{
			base.OnItemArranged(dockItem);

			PreviewControlView.OnItemArranged(dockItem);
		}

		internal override void OnItemMeasured(DockItem dockItem)
		{
			base.OnItemMeasured(dockItem);

			PreviewControlView.OnItemMeasured(dockItem);
		}
	}
}