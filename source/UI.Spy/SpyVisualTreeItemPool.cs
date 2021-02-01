// <copyright file="SpyVisualTreeItemPool.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Windows;

namespace Zaaml.UI.Controls.Spy
{
	internal sealed class SpyVisualTreeDataItemPool
	{
		private readonly Stack<SpyVisualTreeDataItem> _stackPool = new();

		public SpyVisualTreeDataItem GetItem(UIElement element)
		{
			var spyVisualTreeItem = _stackPool.Count > 0 ? _stackPool.Pop() : new SpyVisualTreeDataItem(this);

			spyVisualTreeItem.Element = element;

			return spyVisualTreeItem;
		}

		public void ReleaseItem(SpyVisualTreeDataItem dataItem)
		{
			_stackPool.Push(dataItem);
		}
	}
}