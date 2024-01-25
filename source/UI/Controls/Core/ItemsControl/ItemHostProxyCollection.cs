// <copyright file="ItemHostProxyCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections;
using System.Collections.Generic;
using System.Windows;

namespace Zaaml.UI.Controls.Core
{
	internal sealed class ItemHostProxyCollection<TItem> : IEnumerable<TItem> where TItem : FrameworkElement
	{
		private readonly List<TItem> _items = new();
		private IItemsHost<TItem> _itemsHost;

		public int Count => _items.Count;

		public TItem this[int index] => _items[index];

		public IItemsHost<TItem> ItemsHost
		{
			get => _itemsHost;
			set
			{
				if (ReferenceEquals(_itemsHost, value))
					return;

				ClearItemsHost();

				_itemsHost = value;

				InitItemsHost();
			}
		}

		private void ClearItemsHost()
		{
			_itemsHost?.Items.ClearInternal();
		}

		private void InitItemsHost()
		{
			_itemsHost?.Items.InitInternal(_items);
		}

		public void Insert(int index, TItem item)
		{
			_items.Insert(index, item);

			_itemsHost?.Items.InsertInternal(index, item);
		}

		public void RemoveAt(int index)
		{
			_items.RemoveAt(index);

			_itemsHost?.Items.RemoveAtInternal(index);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)_items).GetEnumerator();
		}

		public IEnumerator<TItem> GetEnumerator()
		{
			return _items.GetEnumerator();
		}
	}
}