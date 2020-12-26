// <copyright file="DefaultTreeViewItemTextFilter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.TreeView
{
	internal class DefaultTreeViewItemTextFilter : TreeViewItemTextFilter<object>
	{
		public DefaultTreeViewItemTextFilter(TreeViewControl listViewControl)
		{
			Filter = new DefaultItemTextFilter(listViewControl);
		}

		private DefaultItemTextFilter Filter { get; }

		protected override bool Pass(object item)
		{
			return Filter.Pass(item, FilterTextCache);
		}

		private sealed class DefaultItemTextFilter : DefaultItemTextFilter<TreeViewItem>
		{
			public DefaultItemTextFilter(TreeViewControl listViewControl)
			{
				TreeViewControl = listViewControl;
			}

			protected override string ContentMember => TreeViewControl.ItemContentMember;

			public TreeViewControl TreeViewControl { get; }

			protected override object GetItemContent(TreeViewItem item)
			{
				return item.Content;
			}
		}
	}
}