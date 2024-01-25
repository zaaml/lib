// <copyright file="TreeViewFilter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.TreeView
{
	public abstract class TreeViewItemTextFilterBase<TItem> : ItemTextFilter<TreeViewControl, TItem>, ITreeViewItemFilter
	{
	}
}