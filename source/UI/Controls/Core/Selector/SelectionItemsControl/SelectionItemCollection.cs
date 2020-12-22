// <copyright file="SelectionItemCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;

namespace Zaaml.UI.Controls.Core
{
	public class SelectionItemCollection<TSelectionItem, TItem> : ItemCollectionBase<SelectionItemsControl<TSelectionItem, TItem>, TSelectionItem>
		where TSelectionItem : SelectionItem<TItem>, new()
		where TItem : FrameworkElement
	{
		public SelectionItemCollection(SelectionItemsControl<TSelectionItem, TItem> control) : base(control)
		{
		}

		protected override ItemGenerator<TSelectionItem> DefaultGenerator { get; } = new SelectionItemGenerator<TSelectionItem, TItem>();

		internal SelectionItemGeneratorBase<TSelectionItem, TItem> Generator
		{
			get => (SelectionItemGeneratorBase<TSelectionItem, TItem>) GeneratorCore;
			set => GeneratorCore = value;
		}
	}
}