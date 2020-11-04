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
			Implementation = new TemplatedGeneratorImpl<ListViewItem>(this);
		}

		private TemplatedGeneratorImpl<ListViewItem> Implementation { get; }

		public ListViewItemTemplate ItemTemplate
		{
			get => (ListViewItemTemplate) GetValue(ItemTemplateProperty);
			set => SetValue(ItemTemplateProperty, value);
		}

		protected override bool SupportsRecycling => true;

		protected override void AttachItem(ListViewItem item, object itemSource)
		{
			Implementation.AttachItem(item, itemSource);
		}

		protected override ListViewItem CreateItem(object itemSource)
		{
			return Implementation.CreateItem(itemSource);
		}

		protected override void DetachItem(ListViewItem item, object itemSource)
		{
			Implementation.DetachItem(item, itemSource);
		}

		protected override void DisposeItem(ListViewItem item, object itemSource)
		{
			Implementation.DisposeItem(item, itemSource);
		}

		private void OnItemTemplateChanged()
		{
			Implementation.ItemTemplate = ItemTemplate;
		}
	}
}