// <copyright file="PanelHostCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;

namespace Zaaml.UI.Panels.Core
{
	internal sealed class PanelHostCollection<TItem> : PanelHostCollectionBase<TItem, ItemsPanel<TItem>>
		where TItem : FrameworkElement
	{
		public PanelHostCollection(ItemsPanel<TItem> panel) : base(panel)
		{
		}
	}
}