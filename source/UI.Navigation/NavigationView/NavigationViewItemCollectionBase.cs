// <copyright file="NavigationViewItemCollectionBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.UI.Controls.Core;
using Control = System.Windows.Controls.Control;

namespace Zaaml.UI.Controls.NavigationView
{
	public abstract class NavigationViewItemCollectionBase<TControl, TItem> : ItemCollectionBase<TControl, TItem>
		where TControl : Control
		where TItem : NavigationViewItemBase
	{
		protected NavigationViewItemCollectionBase(TControl control) : base(control)
		{
		}

		protected override ItemGenerator<TItem> DefaultGenerator => null;
	}
}