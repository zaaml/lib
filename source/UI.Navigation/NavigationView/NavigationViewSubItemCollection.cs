// <copyright file="NavigationViewSubItemCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.UI.Controls.NavigationView
{
	public sealed class NavigationViewSubItemCollection : NavigationViewItemCollectionBase<NavigationViewMenuItem, NavigationViewItemBase>
	{
		internal NavigationViewSubItemCollection(NavigationViewMenuItem navigationViewMenuItem) : base(navigationViewMenuItem)
		{
		}

		private NavigationViewMenuItem NavigationViewMenuItem => Control;

		protected override void OnItemAttached(NavigationViewItemBase item)
		{
			base.OnItemAttached(item);

			NavigationViewMenuItem.OnItemAttached(item);
		}

		protected override void OnItemDetached(NavigationViewItemBase item)
		{
			NavigationViewMenuItem.OnItemDetached(item);

			base.OnItemDetached(item);
		}
	}
}