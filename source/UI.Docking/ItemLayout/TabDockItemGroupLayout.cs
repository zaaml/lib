// <copyright file="TabDockItemGroupLayout.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.UI.Controls.Docking
{
	public sealed class TabDockItemGroupLayout : DockItemGroupLayout
	{
		public TabDockItemGroupLayout(TabDockItemGroup groupItem)
			: base(groupItem)
		{
		}

		public TabDockItemGroupLayout()
		{
		}

		internal TabDockItemGroupLayout(TabDockItemGroupLayout groupLayout, DockItemLayoutCloneMode mode) : base(groupLayout, mode)
		{
		}

		internal override DockItemGroupKind GroupKind => DockItemGroupKind.Tab;

		internal override DockItemLayout CloneCore(DockItemLayoutCloneMode mode)
		{
			return new TabDockItemGroupLayout(this, mode);
		}
	}
}