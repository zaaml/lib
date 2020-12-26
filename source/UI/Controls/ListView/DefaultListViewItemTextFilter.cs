// <copyright file="DefaultListViewItemTextFilter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.ListView
{
	internal class DefaultListViewItemTextFilter : ListViewItemTextFilter<object>
	{
		public DefaultListViewItemTextFilter(ListViewControl listViewControl)
		{
			Filter = new DefaultItemTextFilter(listViewControl);
		}

		private DefaultItemTextFilter Filter { get; }

		protected override bool Pass(object item)
		{
			return Filter.Pass(item, FilterTextCache);
		}

		private sealed class DefaultItemTextFilter : DefaultItemTextFilter<ListViewItem>
		{
			public DefaultItemTextFilter(ListViewControl listViewControl)
			{
				ListViewControl = listViewControl;
			}

			protected override string ContentMember => ListViewControl.ItemContentMember;

			public ListViewControl ListViewControl { get; }

			protected override object GetItemContent(ListViewItem item)
			{
				return item.Content;
			}
		}
	}
}