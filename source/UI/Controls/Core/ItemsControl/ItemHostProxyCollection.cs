// <copyright file="ItemHostProxyCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections;
using System.Collections.Generic;

namespace Zaaml.UI.Controls.Core
{
	internal sealed class ItemHostProxyCollection<TItem> : IEnumerable<TItem> where TItem : System.Windows.Controls.Control
	{
		#region Fields

		private readonly List<TItem> _items = new List<TItem>();
		private IItemsHost<TItem> _itemsHost;

		#endregion

		#region Properties

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

		#endregion

		#region  Methods

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

		#endregion

		#region Interface Implementations

		#region IEnumerable

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable) _items).GetEnumerator();
		}

		#endregion

		#region IEnumerable<TItem>

		public IEnumerator<TItem> GetEnumerator()
		{
			return _items.GetEnumerator();
		}

		#endregion

		#endregion
	}
}