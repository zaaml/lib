// <copyright file="DropDownSelectionPresenterBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.DropDown
{
	public abstract class DropDownSelectionPresenterBase<TSelectionItem, TItem> : SelectionItemsControl<TSelectionItem, TItem>
		where TSelectionItem : SelectionItem<TItem>, new()
		where TItem : FrameworkElement
	{
	}
}