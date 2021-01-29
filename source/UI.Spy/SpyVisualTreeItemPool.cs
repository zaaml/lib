// <copyright file="SpyVisualTreeItemPool.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Windows;

namespace Zaaml.UI.Controls.Spy
{
	internal sealed class SpyVisualTreeItemPool
	{
		private readonly Stack<SpyVisualTreeItem> _stackPool = new();

		public SpyVisualTreeItem GetItem(UIElement element)
		{
			var spyVisualTreeItem = _stackPool.Count > 0 ? _stackPool.Pop() : new SpyVisualTreeItem(this);

			spyVisualTreeItem.Element = element;

			return spyVisualTreeItem;
		}

		public void ReleaseItem(SpyVisualTreeItem item)
		{
			_stackPool.Push(item);
		}
	}
}