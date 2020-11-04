// <copyright file="PropertyTreeViewItem.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.UI.Controls.TreeView;

namespace Zaaml.UI.Controls.PropertyView
{
	public abstract class PropertyTreeViewItem : TreeViewItem
	{
		protected PropertyTreeViewItem(PropertyViewControl propertyView)
		{
			PropertyView = propertyView;
		}

		public PropertyViewControl PropertyView { get; }
	}
}