// <copyright file="DelegateContentItemGenerator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using Zaaml.UI.Controls.Interfaces;
using Zaaml.UI.Controls.Primitives.ContentPrimitives;

namespace Zaaml.UI.Controls.Core
{
	internal sealed class DelegateContentItemGenerator<TSelectionItem, TItem> : DelegateContentItemGeneratorImplementation<TSelectionItem, DefaultSelectionItemItemGenerator<TSelectionItem, TItem>>
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

			if (SelectionItemsControl.IsAttachDetachOverridenInternal)
			{
				SelectionItemsControl.AttachOverrideInternal(item, selection);

				return;
			}

			item.ItemsControl = SelectionItemsControl;
			item.Selection = selection;

			if (ItemContentMemberBinding != null)
			{
				ItemGenerator<TSelectionItem>.InstallBinding(item, IconContentPresenter.ContentProperty, ItemContentMemberBinding);
			}
			else
			{
				switch (SelectionItemsControl.ContentMode)
				{
					case SelectionItemContentMode.None:
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
			}

			item.ContentTemplate = ItemContentTemplate;
			item.ContentTemplateSelector = ItemContentTemplateSelector;
			item.ContentStringFormat = ItemContentStringFormat;
		}

		public override void DetachItem(TSelectionItem item, object source)
		{
			var selection = (Selection<TItem>)source;

			if (SelectionItemsControl.IsAttachDetachOverridenInternal)
			{
				SelectionItemsControl.DetachOverrideInternal(item, selection);

				return;
			}

			item.ItemsControl = null;
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