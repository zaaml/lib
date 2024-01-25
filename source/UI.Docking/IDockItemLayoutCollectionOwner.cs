// <copyright file="IDockItemLayoutCollectionOwner.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.UI.Controls.Docking
{
	internal interface IDockItemLayoutCollectionOwner
	{
		DockItemLayoutCollection Items { get; }

		void OnItemAdded(DockItemLayout dockItemLayout);

		void OnItemRemoved(DockItemLayout dockItemLayout);
	}
}