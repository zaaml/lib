// <copyright file="ItemsControlMediator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.UI.Panels.Core;

namespace Zaaml.UI.Controls.Core
{
	internal class ItemsControlMediator<TControl, TItem, TCollection, TPresenter, TPanel>
		where TItem : FrameworkElement
		where TCollection : ItemCollectionBase<TControl, TItem>
		where TPresenter : ItemsPresenterBase<TControl, TItem, TCollection, TPanel>
		where TPanel : ItemsPanel<TItem>
		where TControl : System.Windows.Controls.Control
	{
		public ItemsControlMediator(TControl itemsControl)
		{
			ItemsControl = itemsControl;
		}

		public TControl ItemsControl { get; }

		public TPanel ItemsPanel { get; private set; }

		public TPresenter ItemsPresenter { get; private set; }
	}
}