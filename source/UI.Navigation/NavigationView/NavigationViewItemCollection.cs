// <copyright file="NavigationViewItemCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.UI.Controls.NavigationView
{
	public sealed class NavigationViewItemCollection : NavigationViewItemCollectionBase<NavigationViewControl, NavigationViewItemBase>
	{
		public NavigationViewItemCollection(NavigationViewControl navigationViewControl) : base(navigationViewControl)
		{
		}

		internal NavigationViewControl NavigationViewControl => Control;
	}
}