// <copyright file="SelectionItemGeneratorBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;

namespace Zaaml.UI.Controls.Core
{
	public abstract class SelectionItemGeneratorBase<TSelectionItem, TItem> : ItemGenerator<TSelectionItem>
		where TSelectionItem : SelectionItem<TItem>, new()
		where TItem : FrameworkElement
	{
	}
}