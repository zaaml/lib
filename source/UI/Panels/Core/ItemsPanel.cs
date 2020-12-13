// <copyright file="ItemsPanel.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.UI.Controls.Core;
using Zaaml.UI.Controls.ScrollView;
using NativeControl = System.Windows.Controls.Control;

namespace Zaaml.UI.Panels.Core
{
	public abstract class ItemsPanel<TItem> : Panel, IItemsHost<TItem>
		where TItem : NativeControl
	{
		private ItemHostCollection<TItem> _hostCollection;

		internal ItemHostCollection<TItem> ItemsInternal => _hostCollection ??= CreateHostCollectionInternal();

		internal ScrollViewControl ScrollView { get; set; }

		private protected virtual void BringIntoView(BringIntoViewRequest<TItem> request)
		{
		}

		internal virtual ItemHostCollection<TItem> CreateHostCollectionInternal()
		{
			return new PanelHostCollection<TItem>(this);
		}

		private protected virtual void EnqueueBringIntoView(BringIntoViewRequest<TItem> request)
		{
		}

		private protected virtual ItemLayoutInformation GetLayoutInformation(int index)
		{
			return ItemLayoutInformation.Empty;
		}

		private protected virtual ItemLayoutInformation GetLayoutInformation(TItem item)
		{
			return ItemLayoutInformation.Empty;
		}


		void IItemsHost<TItem>.BringIntoView(BringIntoViewRequest<TItem> request)
		{
			BringIntoView(request);
		}

		void IItemsHost<TItem>.EnqueueBringIntoView(BringIntoViewRequest<TItem> request)
		{
			EnqueueBringIntoView(request);
		}

		ItemLayoutInformation IItemsHost<TItem>.GetLayoutInformation(int index)
		{
			return GetLayoutInformation(index);
		}

		ItemLayoutInformation IItemsHost<TItem>.GetLayoutInformation(TItem item)
		{
			return GetLayoutInformation(item);
		}

		ItemHostCollection<TItem> IItemsHost<TItem>.Items => ItemsInternal;
	}
}