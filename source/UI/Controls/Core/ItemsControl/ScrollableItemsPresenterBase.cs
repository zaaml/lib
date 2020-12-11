// <copyright file="ScrollableItemsPresenterBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.UI.Controls.ScrollView;
using Zaaml.UI.Panels.Core;

namespace Zaaml.UI.Controls.Core
{
	public abstract class ScrollableItemsPresenterBase<TControl, TItem, TCollection, TPanel> : ItemsPresenterBase<TControl, TItem, TCollection, TPanel>, IDelegateScrollViewPanel
		where TItem : System.Windows.Controls.Control
		where TCollection : ItemCollectionBase<TControl, TItem>
		where TPanel : ItemsPanel<TItem>
		where TControl : System.Windows.Controls.Control
	{
		private ScrollViewControl _scrollView;

		internal ScrollViewControl ScrollView
		{
			get => _scrollView;
			set
			{
				if (ReferenceEquals(_scrollView, value))
					return;

				var oldScrollView = _scrollView;

				_scrollView = value;

				OnScrollViewChanged(oldScrollView, value);
			}
		}

		protected override void OnItemsHostAttached()
		{
			base.OnItemsHostAttached();

			ItemsHost.ScrollView = ScrollView;
		}

		protected override void OnItemsHostDetaching()
		{
			ItemsHost.ScrollView = null;

			base.OnItemsHostDetaching();
		}

		private protected virtual void OnScrollViewChanged(ScrollViewControl oldScrollView, ScrollViewControl newScrollView)
		{
			if (ItemsHost != null)
				ItemsHost.ScrollView = newScrollView;
		}

		IScrollViewPanel IDelegateScrollViewPanel.ScrollViewPanel => ItemsHostBaseInternal as IScrollViewPanel;
	}
}