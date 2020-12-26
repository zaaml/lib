// <copyright file="ListViewItemGenerator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Markup;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.ListView
{
	[ContentProperty(nameof(ItemTemplate))]
	public sealed class ListViewItemGenerator : ListViewItemGeneratorBase
	{
		public static readonly DependencyProperty ItemTemplateProperty = DPM.Register<ListViewItemTemplate, ListViewItemGenerator>
			("ItemTemplate", g => g.OnItemTemplateChanged);

		public ListViewItemGenerator()
		{
			Implementation = new TemplatedGeneratorImplementation<ListViewItem>(this);
		}

		private TemplatedGeneratorImplementation<ListViewItem> Implementation { get; }

		public ListViewItemTemplate ItemTemplate
		{
			get => (ListViewItemTemplate) GetValue(ItemTemplateProperty);
			set => SetValue(ItemTemplateProperty, value);
		}

		protected override bool SupportsRecycling => true;

		protected override void AttachItem(ListViewItem item, object source)
		{
			Implementation.AttachItem(item, source);
		}

		protected override ListViewItem CreateItem(object source)
		{
			return Implementation.CreateItem(source);
		}

		protected override void DetachItem(ListViewItem item, object source)
		{
			Implementation.DetachItem(item, source);
		}

		protected override void DisposeItem(ListViewItem item, object source)
		{
			Implementation.DisposeItem(item, source);
		}

		private void OnItemTemplateChanged()
		{
			Implementation.ItemTemplate = ItemTemplate;
		}
	}
}