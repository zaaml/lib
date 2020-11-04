// <copyright file="ItemHostCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections;
using System.Collections.Generic;

namespace Zaaml.UI.Controls.Core
{
	internal abstract class ItemHostCollection<TItem> : IEnumerable<TItem>
		where TItem : System.Windows.Controls.Control
	{
		#region Properties

		protected List<TItem> Items { get; } = new List<TItem>();

		#endregion

		#region  Methods

		protected abstract void ClearCore();

		internal void ClearInternal()
		{
			Items.Clear();

			ClearCore();
		}

		protected abstract void InitCore(ICollection<TItem> items);

		internal void InitInternal(ICollection<TItem> items)
		{
			Items.Clear();
			Items.AddRange(items);

			InitCore(items);
		}

		protected abstract void InsertCore(int index, TItem item);

		internal void InsertInternal(int index, TItem item)
		{
			Items.Insert(index, item);

			InsertCore(index, item);
		}

		protected abstract void RemoveAtCore(int index);

		internal void RemoveAtInternal(int index)
		{
			Items.RemoveAt(index);

			RemoveAtCore(index);
		}

		#endregion

		#region Interface Implementations

		#region IEnumerable

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion

		#region IEnumerable<TItem>

		public IEnumerator<TItem> GetEnumerator()
		{
			return Items.GetEnumerator();
		}

		#endregion

		#endregion
	}
}