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
		// ReSharper disable once SuggestBaseTypeForParameter
		protected ItemCollectionSelectorAdvisor(ISelector<TItem> selector, ItemCollectionBase<TControl, TItem> collection) : base(selector, collection)
		{
			collection.AttachObserver(this);
		}

		public override int Count => ItemCollection.ActualCountInternal;

		public override bool HasSource => ItemCollection.HasSourceInternal;

		public override bool IsVirtualizing => ItemCollection.VirtualCollection != null;

		private ItemCollectionBase<TControl, TItem> ItemCollection => (ItemCollectionBase<TControl, TItem>) Collection;

		public override int GetIndexOfItem(TItem item)
		{
			return ItemCollection.GetIndexFromItemInternal(item);
		}

		public override int GetIndexOfSource(object source)
		{
			return ItemCollection.GetIndexFromSourceInternal(source);
		}

		public override object GetSource(int index)
		{
			return ItemCollection.GetSourceFromIndexInternal(index);
		}

		public override object GetSource(TItem item)
		{
			return ItemCollection.GetSourceInternal(item);
		}

		public override bool GetSourceSelected(object source)
		{
			return false;
		}

		public override void Lock(TItem item)
		{
			ItemCollection.LockItemInternal(item);
		}

		public override void SetSourceSelected(object source, bool value)
		{
		}

		public override bool TryCreateSelection(int index, bool ensure, out Selection<TItem> selection)
		{
			var source = GetSource(index);

			if (TryGetItem(source, ensure, out var item) == false)
				TryGetItem(index, ensure, out item);

			var value = GetValue(item, source);

			selection = new Selection<TItem>(index, item, source, value);

			return selection.IsEmpty == false;
		}

		public override bool TryCreateSelection(object source, bool ensure, out Selection<TItem> selection)
		{
			var index = GetIndexOfSource(source);

			if (TryGetItem(source, ensure, out var item) == false && index != -1)
				TryGetItem(index, ensure, out item);

			var value = GetValue(item, source);

			selection = new Selection<TItem>(index, item, source, value);

			return true;
		}

		public override bool TryGetItem(int index, bool ensure, out TItem item)
		{
			return ensure ? ItemCollection.TryEnsureItemInternal(index, out item) : (item = ItemCollection.GetItemFromIndexInternal(index)) != null;
		}

		public override bool TryGetItem(object source, bool ensure, out TItem item)
		{
			return ensure ? ItemCollection.TryEnsureItemInternal(source, out item) : (item = ItemCollection.GetItemFromSourceInternal(source)) != null;
		}

		public override void Unlock(TItem item)
		{
			ItemCollection.UnlockItemInternal(item);
		}

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
	}
}