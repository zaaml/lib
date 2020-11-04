// <copyright file="ItemCollectionBase.Interface.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;

namespace Zaaml.UI.Controls.Core
{
	public abstract partial class ItemCollectionBase<TItemsControl, TItem>
	{
		#region Properties

		public int Count => IsClient ? CountImpl : 0;

		private static bool IsFixedSize => false;

		public bool IsReadOnly => false;

		private static bool IsSynchronized => false;

		public TItem this[int index]
		{
			get
			{
				VerifyClient();

				return GetItemImpl(index);
			}
			set
			{
				VerifyClient();

				SetItemImpl(index, value);
			}
		}

		private object SyncRoot => this;

		#endregion

		#region  Methods

		public int Add(TItem item)
		{
			VerifyClient();

			return AddImpl(item);
		}

		public void Clear()
		{
			VerifyClient();

			ClearImpl();
		}

		private bool Contains(TItem item)
		{
			return IsClient && ContainsImpl(item);
		}

		private void CopyTo(Array array, int index)
		{
			if (IsClient == false)
				return;

			CopyToImpl(array, index);
		}

		private void CopyTo(TItem[] array, int index)
		{
			if (IsClient == false)
				return;

			CopyToImpl(array, index);
		}

		public IEnumerator<TItem> GetEnumerator()
		{
			return IsClient ? GetEnumeratorImpl() : Enumerable.Empty<TItem>().GetEnumerator();
		}

		public int IndexOf(TItem item)
		{
			return IsClient ? IndexOfImpl(item) : -1;
		}

		public void Insert(int index, TItem item)
		{
			VerifyClient();

			InsertImpl(index, item);
		}

		public bool Remove(TItem item)
		{
			VerifyClient();

			return RemoveImpl(item);
		}

		public void RemoveAt(int index)
		{
			VerifyClient();

			RemoveAtImpl(index);
		}

		private void VerifyClient()
		{
			if (IsClient == false)
				throw new InvalidOperationException();
		}

		#endregion
	}
}