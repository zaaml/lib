// <copyright file="TableViewItemElementsCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore;

namespace Zaaml.UI.Controls.TableView
{
	public sealed class TableViewItemElementsCollection : UIElementCollectionBase<UIElement>
	{
		internal TableViewItemElementsCollection(TableViewItem tableViewItem)
		{
			TableViewItem = tableViewItem;
			LogicalHost = tableViewItem;
		}

		internal TableViewItemPanel ItemPanel
		{
			get => (TableViewItemPanel) ElementsHost;
			set => ElementsHost = value;
		}

		public TableViewItem TableViewItem { get; }
	}
}