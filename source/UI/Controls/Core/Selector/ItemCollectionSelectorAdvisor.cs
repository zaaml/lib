// <copyright file="ItemCollectionSelectorAdvisor.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Specialized;

namespace Zaaml.UI.Controls.Core
{
	internal abstract class ItemCollectionSelectorAdvisor<TControl, TItem> : CollectionSelectorAdvisorBase<TItem>, IItemCollectionObserver<TItem> 
		where TItem : System.Windows.Controls.Control
		where TControl : System.Windows.Controls.Control
	{
		#region Ctors

		// ReSharper disable once SuggestBaseTypeForParameter
		protected ItemCollectionSelectorAdvisor(ISelector<TItem> selector, ItemCollectionBase<TControl, TItem> collection) : base(selector, collection)
		{
			collection.AttachObserver(this);
		}

		#endregion

		#region Properties

		public override int Count => ItemCollection.ActualCountInternal;

		public override bool HasSource => ItemCollection.HasSourceInternal;

		public override bool IsVirtualizing => ItemCollection.VirtualCollection != null;

		private ItemCollectionBase<TControl, TItem> ItemCollection => (ItemCollectionBase<TControl, TItem>) Collection;

		#endregion

		#region  Methods

		public override int GetIndexOfItem(TItem item)
		{
			return ItemCollection.GetIndexFromItemInternal(item);
		}

		public override int GetIndexOfSource(object source)
		{
			return ItemCollection.GetIndexFromSourceInternal(source);
		}

		public override bool TryGetItem(int index, bool ensure, out TItem item)
		{
			return ensure ? ItemCollection.TryEnsureItemFromIndexInternal(index, out item) : (item = ItemCollection.GetItemFromIndexInternal(index)) != null;
		}

		public override bool TryGetItemBySource(object source, bool ensure, out TItem item)
		{
			return ensure ? ItemCollection.TryEnsureItemFromSourceInternal(source, out item) : (item = ItemCollection.GetItemFromSourceInternal(source)) != null;
		}

		public override object GetSource(int index)
		{
			return ItemCollection.GetSourceFromIndexInternal(index);
		}

		public override object GetSource(TItem item)
		{
			return ItemCollection.GetSourceInternal(item);
		}

		public override void SetSourceSelected(TItem item, bool value)
		{
		}

		public override bool TryGetSelection(int index, bool ensure, out Selection<TItem> selection)
		{
			TryGetItem(index, ensure, out var item);
			var source = GetSource(index);
			var value = SelectorCore.GetValue(item, source);

			selection = new Selection<TItem>(index, item, source, value);

			return selection.IsEmpty == false;
		}

		public override bool TryGetSelection(object source, bool ensure, out Selection<TItem> selection)
		{
			var index = GetIndexOfSource(source);
			
			if (index == -1)
			{
				selection = Selection<TItem>.Empty;

				return false;
			}

			TryGetItem(index, ensure, out var item);

			var value = SelectorCore.GetValue(item, source);

			selection = new Selection<TItem>(index, item, source, value);
			
			return true;
		}

		public override void Lock(TItem item)
		{
			ItemCollection.LockItemInternal(item);
		}

		public override void Unlock(TItem item)
		{
			ItemCollection.UnlockItemInternal(item);
		}

		public override bool GetSourceSelected(TItem item)
		{
			return false;
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