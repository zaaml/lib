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
			Implementation = new TemplatedGeneratorImplementation<TableViewItem>(this);
		}

		private TemplatedGeneratorImplementation<TableViewItem> Implementation { get; }

		public TableViewItemTemplate ItemTemplate
		{
			get => (TableViewItemTemplate) GetValue(ItemTemplateProperty);
			set => SetValue(ItemTemplateProperty, value);
		}

		protected override bool SupportsRecycling => true;

		protected override void AttachItem(TableViewItem item, object source)
		{
			Implementation.AttachItem(item, source);
		}

		protected override TableViewItem CreateItem(object source)
		{
			return Implementation.CreateItem(source);
		}

		protected override void DetachItem(TableViewItem item, object source)
		{
			Implementation.DetachItem(item, source);
		}

		protected override void DisposeItem(TableViewItem item, object source)
		{
			Implementation.DisposeItem(item, source);
		}

		private void OnItemTemplateChanged()
		{
			Implementation.ItemTemplate = ItemTemplate;
		}
	}
}