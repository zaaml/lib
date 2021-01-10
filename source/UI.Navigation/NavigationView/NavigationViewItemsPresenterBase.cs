// <copyright file="NavigationViewItemsPresenterBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.PresentationCore.TemplateCore;
using Zaaml.UI.Controls.Core;
using Control = System.Windows.Controls.Control;

namespace Zaaml.UI.Controls.NavigationView
{
	[TemplateContractType(typeof(NavigationViewItemsPresenterTemplateContract))]
	public abstract class NavigationViewItemsPresenterBase<TControl, TItem, TItemCollection, TPanel> : ItemsPresenterBase<TControl, TItem, TItemCollection, TPanel>
		where TItem : NavigationViewItemBase
		where TItemCollection : NavigationViewItemCollectionBase<TControl, TItem>
		where TControl : Control
		where TPanel : NavigationViewPanelBase<TItem>
	{
	}
}