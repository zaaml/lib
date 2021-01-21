// <copyright file="DefaultTreeViewItemTextFilter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.TreeView
{
	public sealed class TreeViewItemTextFilter : TreeViewItemTextFilterBase<object>
	{
		public TreeViewItemTextFilter()
		{
			Filter = new DefaultItemTextFilter();
		}

		private DefaultItemTextFilter Filter { get; }

		protected override bool Pass(TreeViewControl treeViewControl, object item)
		{
			return Filter.Pass(item, FilterTextCache, treeViewControl.ItemContentMember);
		}

		private sealed class DefaultItemTextFilter : DefaultItemTextFilter<TreeViewItem>
		{
			protected override object GetItemContent(TreeViewItem item)
			{
				return item.Content;
			}
		}
	}
}