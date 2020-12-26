// <copyright file="ItemCollectionBase.IList.Generic.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Zaaml.UI.Controls.Core
{
	public abstract partial class ItemCollectionBase<TItemsControl, TItem> : IList<TItem>
	{
		int IList<TItem>.IndexOf(TItem item)
		{
			return IndexOf(item);
		}

		void IList<TItem>.Insert(int index, TItem item)
		{
			Insert(index, item);
		}

		void IList<TItem>.RemoveAt(int index)
		{
			RemoveAt(index);
		}

		TItem IList<TItem>.this[int index]
		{
			get => this[index];
			set => this[index] = value;
		}
	}
}