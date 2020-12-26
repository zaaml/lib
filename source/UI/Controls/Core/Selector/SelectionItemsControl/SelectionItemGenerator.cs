// <copyright file="SelectionItemGenerator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Markup;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.UI.Controls.Core
{
	[ContentProperty(nameof(ItemTemplate))]
	public sealed class SelectionItemGenerator<TSelectionItem, TItem> : SelectionItemGeneratorBase<TSelectionItem, TItem>
		where TSelectionItem : SelectionItem<TItem>, new()
		where TItem : FrameworkElement
	{
		public static readonly DependencyProperty ItemTemplateProperty = DPM.Register<SelectionItemTemplate, SelectionItemGenerator<TSelectionItem, TItem>>
			("ItemTemplate", g => g.OnItemTemplateChanged);

		public SelectionItemGenerator()
		{
			Implementation = new TemplatedGeneratorImplementation<TSelectionItem>(this);
		}

		private TemplatedGeneratorImplementation<TSelectionItem> Implementation { get; }

		public SelectionItemTemplate ItemTemplate
		{
			get => (SelectionItemTemplate) GetValue(ItemTemplateProperty);
			set => SetValue(ItemTemplateProperty, value);
		}

		protected override bool SupportsRecycling => true;

		protected override void AttachItem(TSelectionItem item, object source)
		{
			Implementation.AttachItem(item, source);
		}

		protected override TSelectionItem CreateItem(object source)
		{
			return Implementation.CreateItem(source);
		}

		protected override void DetachItem(TSelectionItem item, object source)
		{
			Implementation.DetachItem(item, source);
		}

		protected override void DisposeItem(TSelectionItem item, object source)
		{
			Implementation.DisposeItem(item, source);
		}

		private void OnItemTemplateChanged()
		{
			Implementation.ItemTemplate = ItemTemplate;
		}
	}
}