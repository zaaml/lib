// <copyright file="ItemCollectionBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using Zaaml.Core;
using Zaaml.PresentationCore.PropertyCore;
using NativeControl = System.Windows.Controls.Control;

namespace Zaaml.UI.Controls.Core
{
	public abstract class ItemCollectionBase
	{
		internal static readonly DependencyProperty ItemCollectionProperty = DPM.RegisterAttached<object, ItemCollectionBase>
			("ItemCollection");
	}

	public abstract class ItemCollectionBase<TItem> : ItemCollectionBase, IItemCollection<TItem>
		where TItem : FrameworkElement
	{
		protected abstract int ActualCountCore { get; }

		protected abstract IEnumerable<TItem> ActualItemsCore { get; }

		protected abstract IEnumerable SourceCollectionCore { get; set; }

		protected abstract void BringIntoViewCore(int index);

		protected abstract TItem EnsureItemCore(int index);

		protected abstract int GetIndexFromItemCore(TItem item);

		protected abstract int GetIndexFromSourceCore(object source);

		protected abstract TItem GetItemFromSourceCore(object source);

		protected abstract TItem GetItemFromIndexCore(int index);

		protected abstract bool TryEnsureItemFromSourceCore(object source, out TItem item);

		protected abstract bool TryEnsureItemFromIndexCore(int index, out TItem item);

		protected abstract object GetSourceCore(TItem item);

		protected abstract object GetSourceFromIndexCore(int index);

		protected abstract void LockItemCore(TItem item);

		protected abstract void UnlockItemCore(TItem item);

		int IItemCollection<TItem>.ActualCount => ActualCountCore;

		IEnumerable<TItem> IItemCollection<TItem>.ActualItems => ActualItemsCore;

		IEnumerable IItemCollection<TItem>.SourceCollection
		{
			get => SourceCollectionCore;
			set => SourceCollectionCore = value;
		}

		void IItemCollection<TItem>.BringIntoView(int index)
		{
			BringIntoViewCore(index);
		}

		int IItemCollection<TItem>.GetIndexFromItem(TItem item)
		{
			return GetIndexFromItemCore(item);
		}

		TItem IItemCollection<TItem>.GetItemFromIndex(int index)
		{
			return GetItemFromIndexCore(index);
		}

		object IItemCollection<TItem>.GetSourceFromIndex(int index)
		{
			return GetSourceFromIndexCore(index);
		}

		int IItemCollection<TItem>.GetIndexFromSource(object source)
		{
			return GetIndexFromSourceCore(source);
		}

		void IItemCollection<TItem>.LockItem(TItem item)
		{
			LockItemCore(item);
		}

		void IItemCollection<TItem>.UnlockItem(TItem item)
		{
			UnlockItemCore(item);
		}

		TItem IItemCollection<TItem>.EnsureItem(int index)
		{
			return EnsureItemCore(index);
		}
	}

	public abstract partial class ItemCollectionBase<TItemsControl, TItem> : ItemCollectionBase<TItem>
		where TItem : FrameworkElement
		where TItemsControl : NativeControl
	{
		private static readonly IEnumerable<TItem> EmptyActualItems = Enumerable.Empty<TItem>();

		private readonly List<IItemCollectionObserver<TItem>> _observers = new List<IItemCollectionObserver<TItem>>();

		private ItemGenerator<TItem> _generatorCore;
		private IEnumerable _source;
		private ItemCollectionSourceBase<TItemsControl, TItem> _sourceView;

		protected ItemCollectionBase(TItemsControl control)
		{
			Control = control;

			Control.SetValue(ItemCollectionProperty, _dependencyObjectCollection);
		}

		protected override int ActualCountCore => ActualCountInternal;

		internal int ActualCountInternal => ActualSource == null ? Count : ActualIndexedSource.Count;

		private IEnumerable ActualSource => VirtualCollection?.SourceCollection ?? SourceInternal;

		private IndexedEnumerable ActualIndexedSource => VirtualCollection?.IndexedSourceInternal ?? IndexedSource;

		protected override IEnumerable<TItem> ActualItemsCore => ActualItemsInternal;

		internal IEnumerable<TItem> ActualItemsInternal
		{
			get
			{
				if (ActualSource == null)
					return this;

				if (VirtualCollection != null)
					return VirtualCollection.ActualItems;

				if (_sourceView != null)
					return _sourceView.ActualItems;

				return EmptyActualItems;
			}
		}

		public TItemsControl Control { get; }

		protected abstract ItemGenerator<TItem> DefaultGenerator { get; }

		protected ItemGenerator<TItem> GeneratorCore
		{
			get => _generatorCore;
			set
			{
				if (ReferenceEquals(_generatorCore, value))
					return;

				_generatorCore = value;

				if (_sourceView != null)
					_sourceView.Generator = value;
			}
		}

		internal virtual VirtualItemCollection<TItem> VirtualCollection => null;

		private IndexedEnumerable IndexedSource { get; set; } = IndexedEnumerable.Empty;

		private bool IsClient => ActualSource == null;

		protected bool IsLogicalParent => ItemsHost == null;

		internal virtual IItemsControl<TItem> ItemsControl => Control as IItemsControl<TItem>;

		protected override IEnumerable SourceCollectionCore
		{
			get => SourceInternal;
			set => SourceInternal = value;
		}
		
		private protected bool HasSource => VirtualCollection != null ? VirtualCollection.SourceCollection != null : SourceInternal != null;

		internal bool HasSourceInternal => HasSource;
		
		internal IEnumerable SourceInternal
		{
			get => _source;
			set
			{
				if (ReferenceEquals(_source, value))
					return;

				{
					if (_source is INotifyCollectionChanged notifyCollectionChanged)
						notifyCollectionChanged.CollectionChanged -= ObservableSourceOnCollectionChanged;
				}

				_source = value;

				if (_sourceView != null)
					_sourceView.Source = value;

				IndexedSource = value != null ? new IndexedEnumerable(value) : IndexedEnumerable.Empty;

				{
					if (_source is INotifyCollectionChanged notifyCollectionChanged)
						notifyCollectionChanged.CollectionChanged += ObservableSourceOnCollectionChanged;
				}

				OnSourceChangedPrivate();
			}
		}

		internal void AttachGeneratedItem(int index, TItem item)
		{
			if (_itemsHost is IVirtualItemsHost)
			{
				var itemsControl = ItemsControl;

				if (itemsControl != null)
				{
					itemsControl.OnItemAttaching(item);
					itemsControl.OnItemAttached(item);
				}

				return;
			}

			_changeList.Add(item);

			HostInsert(index, _changeList);

			_changeList.Clear();
		}

		internal void AttachObserver(IItemCollectionObserver<TItem> observer)
		{
			_observers.Add(observer);
		}

		protected override void BringIntoViewCore(int index)
		{
			BringIntoViewInternal(index);
		}

		internal BringIntoViewMode DefaultBringIntoViewMode { get; set; }

		internal void BringIntoViewInternal(int index)
		{
			ItemsHost?.BringIntoView(new BringIntoViewRequest<TItem>(index, DefaultBringIntoViewMode));
		}

		internal void BringIntoViewInternal(TItem item)
		{
			ItemsHost?.BringIntoView(new BringIntoViewRequest<TItem>(item, DefaultBringIntoViewMode));
		}

		internal void BringIntoViewInternal(BringIntoViewRequest<TItem> request)
		{
			ItemsHost?.BringIntoView(request);
		}

		internal void DetachGeneratedItem(int index, TItem item)
		{
			if (_itemsHost is IVirtualItemsHost)
			{
				var itemsControl = ItemsControl;

				if (itemsControl != null)
				{
					itemsControl.OnItemDetaching(item);
					itemsControl.OnItemDetached(item);
				}

				return;
			}

			_changeList.Add(item);

			HostRemove(index, _changeList);

			_changeList.Clear();
		}

		internal void DetachObserver(IItemCollectionObserver<TItem> observer)
		{
			_observers.Remove(observer);
		}

		protected override TItem EnsureItemCore(int index)
		{
			if (ActualSource == null)
				return GetItemFromIndexCore(index);

			if (VirtualCollection != null)
				return VirtualCollection.EnsureItem(index);

			if (_sourceView != null)
				return _sourceView.EnsureItem(index);

			return null;
		}

		internal TItem EnsureItemInternal(int index)
		{
			return EnsureItemCore(index);
		}

		protected override int GetIndexFromItemCore(TItem item)
		{
			if (ActualSource == null)
				return IndexOf(item);

			if (VirtualCollection != null)
				return VirtualCollection.GetIndexFromItem(item);

			if (_sourceView != null)
				return _sourceView.GetIndexFromItemInt(item);

			return -1;
		}

		internal int GetIndexFromItemInternal(TItem item)
		{
			return GetIndexFromItemCore(item);
		}

		protected override int GetIndexFromSourceCore(object source)
		{
			return ActualSource == null ? -1 : ActualIndexedSource.IndexOf(source);
		}

		internal int GetIndexFromSourceInternal(object source)
		{
			return GetIndexFromSourceCore(source);
		}

		protected override TItem GetItemFromSourceCore(object source)
		{
			var index = GetIndexFromSourceCore(source);

			return index != -1 ? GetItemFromIndexCore(index) : default;
		}

		protected override bool TryEnsureItemFromSourceCore(object source, out TItem item)
		{
			var index = GetIndexFromSourceCore(source);

			if (index != -1)
				return TryEnsureItemFromIndexCore(index, out item);

			item = default;

			return false;
		}

		internal bool TryEnsureItemFromSourceInternal(object source, out TItem item)
		{
			return TryEnsureItemFromSourceCore(source, out item);
		}

		internal TItem GetItemFromIndexInternal(int index)
		{
			return GetItemFromIndexCore(index);
		}
		
		internal TItem GetItemFromSourceInternal(object source)
		{
			return GetItemFromSourceCore(source);
		}
		
		protected override TItem GetItemFromIndexCore(int index)
		{
			if (ActualSource == null)
				return this[index];

			if (VirtualCollection != null)
				return VirtualCollection.GetItemFromIndex(index);
			
			if (_sourceView != null)
				return _sourceView.GetItemFromIndexInt(index);

			return null;
		}

		protected override bool TryEnsureItemFromIndexCore(int index, out TItem item)
		{
			if (ActualSource == null)
			{
				item = this[index];

				return item != null;
			}

			item = VirtualCollection != null ? VirtualCollection.EnsureItem(index) : _sourceView?.EnsureItem(index);

			return item != null;
		}

		internal bool TryEnsureItemFromIndexInternal(int index, out TItem item)
		{
			return TryEnsureItemFromIndexCore(index, out item);
		}

		protected override object GetSourceCore(TItem item)
		{
			if (VirtualCollection != null)
			{
				return VirtualCollection.GetSourceFromItem(item);
			}
			
			var index = GetIndexFromItemCore(item);

			return index != -1 ? GetSourceFromIndexCore(index) : default;
		}

		protected override object GetSourceFromIndexCore(int index)
		{
			return ActualSource == null ? null : ActualIndexedSource[index];
		}

		internal object GetSourceFromIndexInternal(int index)
		{
			return GetSourceFromIndexCore(index);
		}

		internal object GetSourceInternal(TItem item)
		{
			return GetSourceCore(item);
		}

		protected override void LockItemCore(TItem item)
		{
			LockItemInternal(item);
		}

		internal virtual void LockItemInternal(TItem item)
		{
			if (VirtualCollection != null)
				VirtualCollection.LockItem(item);
			else
				_sourceView?.LockItem(item);
		}

		private void ObservableSourceOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			OnCollectionChangedPrivate(sender, e);
		}

		internal virtual void OnCollectionChangedInternal()
		{
		}

		private void OnCollectionChangedPrivate(object sender, NotifyCollectionChangedEventArgs e)
		{
			try
			{
				if (ReferenceEquals(this, sender))
				{
					foreach (var observer in _observers)
						observer.OnCollectionChanged(e);
				}
				else
				{
					foreach (var observer in _observers)
						observer.OnSourceCollectionChanged(e);
				}
			}
			catch (Exception ex)
			{
				LogService.LogError(ex);
			}

			ItemsControl?.OnCollectionChanged(sender, e);

			OnCollectionChangedInternal();
		}

		internal virtual void OnSourceChangedInternal()
		{
		}

		private void OnSourceChangedPrivate()
		{
			try
			{
				var e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);

				foreach (var observer in _observers)
					observer.OnSourceCollectionChanged(e);
			}
			catch (Exception ex)
			{
				LogService.LogError(ex);
			}

			OnSourceChangedInternal();

			ItemsControl?.OnSourceChanged();
		}

		internal void OnSourceCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			OnCollectionChangedPrivate(SourceInternal, e);
		}

		protected override void UnlockItemCore(TItem item)
		{
			UnlockItemInternal(item);
		}

		internal virtual void UnlockItemInternal(TItem item)
		{
			if (VirtualCollection != null)
				VirtualCollection.UnlockItem(item);
			else
				_sourceView?.UnlockItem(item);
		}
	}
}