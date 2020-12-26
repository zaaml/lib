// <copyright file="ItemCollectionBase.ICollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections;

namespace Zaaml.UI.Controls.Core
{
	public abstract partial class ItemCollectionBase<TItemsControl, TItem>
	{
		void ICollection.CopyTo(Array array, int index)
		{
			CopyTo(array, index);
		}

		int ICollection.Count => Count;

		object ICollection.SyncRoot => SyncRoot;

		bool ICollection.IsSynchronized => IsSynchronized;
	}
}