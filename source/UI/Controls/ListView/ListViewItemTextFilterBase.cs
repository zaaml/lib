// <copyright file="ListViewItemFilter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.ListView
{
	public abstract class ListViewItemTextFilterBase<TItem> : ItemTextFilter<ListViewControl, TItem>, IListViewItemFilter
	{
	}
}