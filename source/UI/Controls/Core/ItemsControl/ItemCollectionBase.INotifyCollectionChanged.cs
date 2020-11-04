// <copyright file="ItemCollectionBase.INotifyCollectionChanged.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Specialized;

namespace Zaaml.UI.Controls.Core
{
	public abstract partial class ItemCollectionBase<TItemsControl, TItem> : INotifyCollectionChanged
	{
		#region Interface Implementations

		#region INotifyCollectionChanged

		event NotifyCollectionChangedEventHandler INotifyCollectionChanged.CollectionChanged
		{
			add => CollectionChangedImpl += value;
			remove => CollectionChangedImpl -= value;
		}

		#endregion

		#endregion
	}
}