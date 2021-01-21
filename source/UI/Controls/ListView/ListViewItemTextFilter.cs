// <copyright file="DefaultListViewItemTextFilter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.ListView
{
	public class ListViewItemTextFilter : ListViewItemTextFilterBase<object>
	{
		public ListViewItemTextFilter()
		{
			Filter = new DefaultItemTextFilter();
		}

		private DefaultItemTextFilter Filter { get; }

		protected override bool Pass(ListViewControl listViewControl, object item)
		{
			return Filter.Pass(item, FilterTextCache, listViewControl.ItemContentMember);
		}

		private sealed class DefaultItemTextFilter : DefaultItemTextFilter<ListViewItem>
		{
			protected override object GetItemContent(ListViewItem item)
			{
				return item.Content;
			}
		}
	}
}