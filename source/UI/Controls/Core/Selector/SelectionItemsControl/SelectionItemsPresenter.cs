// <copyright file="SelectionItemsPresenter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore.TemplateCore;

namespace Zaaml.UI.Controls.Core
{
	public class SelectionItemsPresenter<TSelectionItem, TItem> : ItemsPresenterBase<SelectionItemsControl<TSelectionItem, TItem>, TSelectionItem, SelectionItemCollection<TSelectionItem, TItem>, SelectionItemsPanel<TSelectionItem, TItem>>
		where TSelectionItem : SelectionItem<TItem>, new()
		where TItem : FrameworkElement
	{
		protected override TemplateContract CreateTemplateContract()
		{
			return new SelectionItemsPresenterTemplateContract<TSelectionItem, TItem>();
		}
	}
}