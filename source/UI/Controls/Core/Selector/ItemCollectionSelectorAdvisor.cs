// <copyright file="ItemCollectionSelectorAdvisor.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Specialized;

namespace Zaaml.UI.Controls.Core
{
	internal class ItemCollectionSelectorAdvisor<TControl, TItem> : CollectionSelectorAdvisorBase<TItem>, IItemCollectionObserver<TItem> 
		where TItem : System.Windows.Controls.Control, ISelectable 
		where TControl : System.Windows.Controls.Control
	{
		#region Ctors

		// ReSharper disable once SuggestBaseTypeForParameter
		public ItemCollectionSelectorAdvisor(ISelector<TItem> selector, ItemCollectionBase<TControl, TItem> collection) : base(selector, collection)
		{
			collection.AttachObserver(this);
		}

		#endregion

		#region Properties

		public override int Count => ItemCollection.ActualCountInternal;

		public override bool HasSource => ItemCollection.SourceInternal != null;

		private ItemCollectionBase<TControl, TItem> ItemCollection => (ItemCollectionBase<TControl, TItem>) Collection;

		#endregion

		#region  Methods

		public override int GetIndexOfItem(TItem item)
		{
			return ItemCollection.GetIndexFromItemInternal(item);
		}

		public override int GetIndexOfItemSource(object itemSource)
		{
			return ItemCollection.GetIndexFromItemSourceInternal(itemSource);
		}

		public override bool TryGetItem(int index, out TItem item)
		{
			return ItemCollection.TryEnsureItemFromIndexInternal(index, out item);
		}

		public override bool TryGetItemBySource(object itemSource, out TItem item)
		{
			return ItemCollection.TryEnsureItemFromSourceInternal(itemSource, out item);
		}

		public override object GetItemSource(int index)
		{
			return ItemCollection.GetItemSourceFromIndexInternal(index);
		}

		public override object GetItemSource(TItem item)
		{
			return ItemCollection.GetItemSourceInternal(item);
		}

		public override void Lock(TItem item)
		{
			ItemCollection.LockItemInternal(item);
		}

		public override void Unlock(TItem item)
		{
			ItemCollection.UnlockItemInternal(item);
		}

		#endregion

		#region Interface Implementations

		#region IItemCollectionObserver<T>

		void IItemCollectionObserver<TItem>.OnItemDetaching(int index, TItem item)
		{
		}

		void IItemCollectionObserver<TItem>.OnCollectionChanged(NotifyCollectionChangedEventArgs args)
		{
			Controller?.AdvisorOnItemCollectionChanged(args);
		}

		void IItemCollectionObserver<TItem>.OnSourceCollectionChanged(NotifyCollectionChangedEventArgs args)
		{
			Controller?.AdvisorOnItemCollectionSourceChanged(args);
		}

		void IItemCollectionObserver<TItem>.OnItemAttached(int index, TItem item)
		{
			Controller?.AdvisorOnItemAttached(index, item);
		}

		void IItemCollectionObserver<TItem>.OnItemDetached(int index, TItem item)
		{
			Controller?.AdvisorOnItemDetached(index, item);
		}

		void IItemCollectionObserver<TItem>.OnItemAttaching(int index, TItem item)
		{
		}

		#endregion

		#endregion
	}
}