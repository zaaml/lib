// <copyright file="SelectionItemsPresenterTemplateContract.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;

namespace Zaaml.UI.Controls.Core
{
	public class SelectionItemsPresenterTemplateContract<TSelectionItem, TItem> : ItemsPresenterBaseTemplateContract<SelectionItemsPanel<TSelectionItem, TItem>, TSelectionItem>
		where TSelectionItem : SelectionItem<TItem>, new()
		where TItem : FrameworkElement
	{
	}
}