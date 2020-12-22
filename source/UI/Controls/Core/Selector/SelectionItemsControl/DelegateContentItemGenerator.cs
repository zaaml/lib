// <copyright file="DelegateContentItemGenerator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using Zaaml.UI.Controls.Interfaces;

namespace Zaaml.UI.Controls.Core
{
	internal sealed class DelegateContentItemGenerator<TSelectionItem, TItem> : DelegateContentItemGeneratorImpl<TSelectionItem, DefaultSelectionItemItemGenerator<TSelectionItem, TItem>>
		where TSelectionItem : SelectionItem<TItem>, IContentControl, new()
		where TItem : FrameworkElement
	{
		public DelegateContentItemGenerator(SelectionItemsControl<TSelectionItem, TItem> selectionItemsControl) : base(selectionItemsControl)
		{
			SelectionItemsControl = selectionItemsControl;
		}

		public SelectionItemsControl<TSelectionItem, TItem> SelectionItemsControl { get; }

		public override void AttachItem(TSelectionItem item, object source)
		{
			var selection = (Selection<TItem>) source;

			item.Selection = selection;

			switch (SelectionItemsControl.ContentMode)
			{
				case SelectionItemContentMode.Auto:
					item.Content = selection.Source;
					break;
				case SelectionItemContentMode.Selection:
					item.Content = selection;
					break;
				case SelectionItemContentMode.Item:
					item.Content = selection.Item;
					break;
				case SelectionItemContentMode.Source:
					item.Content = selection.Source;
					break;
				case SelectionItemContentMode.Value:
					item.Content = selection.Value;
					break;
				case SelectionItemContentMode.Index:
					item.Content = selection.Index;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			item.ContentTemplate = ItemContentTemplate;
			item.ContentTemplateSelector = ItemContentTemplateSelector;
			item.ContentStringFormat = ItemContentStringFormat;
		}

		public override void DetachItem(TSelectionItem item, object source)
		{
			item.Selection = Selection<TItem>.Empty;

			var contentItem = (IContentControl) item;

			item.ClearValue(contentItem.ContentProperty);
			item.ClearValue(contentItem.ContentTemplateProperty);
			item.ClearValue(contentItem.ContentTemplateSelectorProperty);
			item.ClearValue(contentItem.ContentStringFormatProperty);
		}

		public void OnContentModeChanged()
		{
			Generator.OnGeneratorChangingInt();
			Generator.OnGeneratorChangedInt();
		}
	}
}