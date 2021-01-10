// <copyright file="NavigationViewCommandItemCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.NavigationView
{
	public sealed class NavigationViewCommandItemCollection : NavigationViewItemCollectionBase<NavigationViewCommandBar, NavigationViewCommandItem>
	{
		internal NavigationViewCommandItemCollection(NavigationViewCommandBar control) : base(control)
		{
		}

		private NavigationViewCommandBar CommandBar => Control;

		protected override ItemGenerator<NavigationViewCommandItem> DefaultGenerator => null;

		protected override void OnItemAttached(NavigationViewCommandItem item)
		{
			base.OnItemAttached(item);

			CommandBar.OnItemAttached(item);
		}

		protected override void OnItemDetached(NavigationViewCommandItem item)
		{
			CommandBar.OnItemDetached(item);

			base.OnItemDetached(item);
		}
	}
}