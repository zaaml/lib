// <copyright file="TableViewItemGenerator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Markup;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.TableView
{
	[ContentProperty(nameof(ItemTemplate))]
	public sealed class TableViewItemGenerator : TableViewItemGeneratorBase
	{
		public static readonly DependencyProperty ItemTemplateProperty = DPM.Register<TableViewItemTemplate, TableViewItemGenerator>
			("ItemTemplate", g => g.OnItemTemplateChanged);

		public TableViewItemGenerator()
		{
			Implementation = new TemplatedGeneratorImpl<TableViewItem>(this);
		}

		private TemplatedGeneratorImpl<TableViewItem> Implementation { get; }

		public TableViewItemTemplate ItemTemplate
		{
			get => (TableViewItemTemplate) GetValue(ItemTemplateProperty);
			set => SetValue(ItemTemplateProperty, value);
		}

		protected override bool SupportsRecycling => true;

		protected override void AttachItem(TableViewItem item, object itemSource)
		{
			Implementation.AttachItem(item, itemSource);
		}

		protected override TableViewItem CreateItem(object itemSource)
		{
			return Implementation.CreateItem(itemSource);
		}

		protected override void DetachItem(TableViewItem item, object itemSource)
		{
			Implementation.DetachItem(item, itemSource);
		}

		protected override void DisposeItem(TableViewItem item, object itemSource)
		{
			Implementation.DisposeItem(item, itemSource);
		}

		private void OnItemTemplateChanged()
		{
			Implementation.ItemTemplate = ItemTemplate;
		}
	}
}