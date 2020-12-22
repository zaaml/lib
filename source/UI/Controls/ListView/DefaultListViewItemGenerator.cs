// <copyright file="DefaultListViewItemGenerator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.ListView
{
	internal class DefaultListViewItemGenerator : ListViewItemGeneratorBase, IDelegatedGenerator<ListViewItem>
	{
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

		public IItemGenerator<ListViewItem> Implementation { get; set; }
	}

	internal class DefaultItemTemplateListViewItemGenerator : DelegateIconContentSelectableItemGeneratorImpl<ListViewItem, DefaultListViewItemGenerator>
	{
		public DefaultItemTemplateListViewItemGenerator(ListViewControl listViewControl) : base(listViewControl)
		{
		}
	}
}