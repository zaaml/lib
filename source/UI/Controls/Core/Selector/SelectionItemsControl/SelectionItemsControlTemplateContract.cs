// <copyright file="SelectionItemsControlTemplateContract.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;

namespace Zaaml.UI.Controls.Core
{
	public class SelectionItemsControlTemplateContract<TSelectionItem, TItem> : ItemsControlBaseTemplateContract<SelectionItemsPresenter<TSelectionItem, TItem>>
		where TSelectionItem : SelectionItem<TItem>, new()
		where TItem : FrameworkElement
	{
	}
}