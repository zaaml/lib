// <copyright file="ItemCollectionBase.ICollection.Generic.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Zaaml.UI.Controls.Core
{
	public abstract partial class ItemCollectionBase<TItemsControl, TItem>
	{
		void ICollection<TItem>.Add(TItem item)
		{
			Add(item);
		}

		void ICollection<TItem>.Clear()
		{
			Clear();
		}

		bool ICollection<TItem>.Contains(TItem item)
		{
			return Contains(item);
		}

		void ICollection<TItem>.CopyTo(TItem[] array, int arrayIndex)
		{
			CopyTo(array, arrayIndex);
		}

		bool ICollection<TItem>.Remove(TItem item)
		{
			return Remove(item);
		}

		int ICollection<TItem>.Count => Count;

		bool ICollection<TItem>.IsReadOnly => IsReadOnly;
	}
}