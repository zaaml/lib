// <copyright file="ItemCollectionBase.Host.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Zaaml.Core;
using Zaaml.PresentationCore.Extensions;

namespace Zaaml.UI.Controls.Core
{
	public abstract partial class ItemCollectionBase<TItemsControl, TItem>
	{
		private readonly ItemHostProxyCollection<TItem> _itemHostCollection = new ItemHostProxyCollection<TItem>();
		private IItemsHost<TItem> _itemsHost;

		internal IItemsHost<TItem> ItemsHost
		{
			get => _itemsHost;
			set
			{
				if (ReferenceEquals(_itemsHost, value))
					return;

				if (SourceCollectionInternal == null)
				{
					var attachLogical = _itemsHost != null && value == null;
					var detachLogical = _itemsHost == null && value != null;

					if (detachLogical)
					{
						foreach (var item in _itemHostCollection)
							DetachLogicalPrivate(item);
					}

					_itemsHost = value;
					_itemHostCollection.ItemsHost = value;

					if (attachLogical)
					{
						foreach (var item in _itemHostCollection)
							AttachLogicalPrivate(item);
					}
				}
				else
				{
					var notifyCollectionChanged = _sourceCollection as INotifyCollectionChanged;

					if (_sourceView != null)
					{
						_sourceView.Dispose();
						_sourceView = null;

						if (notifyCollectionChanged != null && _itemsHost == null)
							notifyCollectionChanged.CollectionChanged += ObservableSourceOnCollectionChanged;
					}

					_itemsHost = value;
					_itemHostCollection.ItemsHost = value;

					if (_itemsHost == null)
						return;

					_sourceView = _itemsHost is IVirtualItemsHost<TItem> virtualItemsHost && virtualItemsHost.IsVirtualizing
						? (ItemCollectionSourceBase<TItemsControl, TItem>) new VirtualItemCollectionSource<TItemsControl, TItem>(virtualItemsHost, this)
						: new ItemCollectionSource<TItemsControl, TItem>(_itemsHost, this);

					if (notifyCollectionChanged != null)
						notifyCollectionChanged.CollectionChanged -= ObservableSourceOnCollectionChanged;

					_sourceView.Source = _sourceCollection;
					_sourceView.Generator = GeneratorCore ?? DefaultGenerator;
				}
			}
		}

		protected virtual void AttachLogicalCore(TItem item)
		{
			var itemsControl = ItemsControl;

			if (itemsControl == null || IsLogicalParent == false)
				return;

			if (item.GetVisualParent() == null)
				itemsControl.AddLogicalChild(item);
		}

		private void AttachLogicalPrivate(TItem item)
		{
			if (IsLogicalParent)
				AttachLogicalCore(item);
		}

		protected virtual void DetachLogicalCore(TItem item)
		{
			var itemsControl = ItemsControl;

			if (itemsControl == null)
				return;

			if (ReferenceEquals(itemsControl, item.GetLogicalParent()))
				itemsControl.RemoveLogicalChild(item);
		}

		private void DetachLogicalPrivate(TItem item)
		{
			if (IsLogicalParent)
				DetachLogicalCore(item);
		}

		private void HostClear()
		{
			for (var index = _itemHostCollection.Count - 1; index >= 0; index--)
			{
				var item = _itemHostCollection[index];

				OnDetaching(index, item);

				_itemHostCollection.RemoveAt(index);

				OnDetached(index, item);
			}
		}

		private void HostInsert(int index, List<TItem> items)
		{
			foreach (var item in items)
			{
				OnAttaching(index, item);

				_itemHostCollection.Insert(index, item);

				OnAttached(index, item);

				index++;
			}
		}

		private void HostRemove(int index, List<TItem> items)
		{
			foreach (var item in items)
			{
				OnDetaching(index, item);

				_itemHostCollection.RemoveAt(index);

				OnDetached(index, item);

				index++;
			}
		}

		private void OnAttached(int index, TItem item)
		{
			try
			{
				foreach (var observer in _observers)
					observer.OnItemAttached(index, item);
			}
			catch (Exception e)
			{
				LogService.LogError(e);
			}

			AttachLogicalPrivate(item);

			OnItemAttached(item);

			ItemsControl?.OnItemAttached(item);
		}

		private void OnAttaching(int index, TItem item)
		{
			OnItemAttaching(item);

			ItemsControl?.OnItemAttaching(item);

			try
			{
				foreach (var observer in _observers)
					observer.OnItemAttaching(index, item);
			}
			catch (Exception e)
			{
				LogService.LogError(e);
			}
		}

		private void OnDetached(int index, TItem item)
		{
			try
			{
				foreach (var observer in _observers)
					observer.OnItemDetached(index, item);
			}
			catch (Exception e)
			{
				LogService.LogError(e);
			}

			OnItemDetached(item);

			ItemsControl?.OnItemDetached(item);
		}

		private void OnDetaching(int index, TItem item)
		{
			DetachLogicalPrivate(item);

			OnItemDetaching(item);

			ItemsControl?.OnItemDetaching(item);

			try
			{
				foreach (var observer in _observers)
					observer.OnItemDetaching(index, item);
			}
			catch (Exception e)
			{
				LogService.LogError(e);
			}
		}

		protected virtual void OnItemAttached(TItem item)
		{
		}

		protected virtual void OnItemAttaching(TItem item)
		{
		}

		protected virtual void OnItemDetached(TItem item)
		{
		}

		protected virtual void OnItemDetaching(TItem item)
		{
		}
	}
}