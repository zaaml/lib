// <copyright file="VirtualItemCollection.Interface.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows;
using Zaaml.Core;

namespace Zaaml.UI.Controls.Core
{
	internal partial class VirtualItemCollection<T>
	{
		public IEnumerable<T> ActualItems => GetGeneratedItems();

		public ItemGenerator<T> Generator
		{
			get => _generator;
			set
			{
				if (ReferenceEquals(_generator, value))
					return;

				if (_generator != null)
				{
					ClearRecyclePool();
					DetachGenerator();
				}

				_generator = value;

				if (_generator != null)
				{
					AttachGenerator();
					IsRecycling = _generator.SupportsRecyclingInternal;
				}
			}
		}

		public bool IsRecycling
		{
			get => PackedDefinition.IsRecycling.GetValue(_packedValue);
			set
			{
				if (IsRecycling == value)
					return;

				PackedDefinition.IsRecycling.SetValue(ref _packedValue, value);

				if (value)
					return;

				ClearRecyclePool();
			}
		}

		private OperatingMode Mode { get; set; } = OperatingMode.Virtual;
		
		protected void Init(IEnumerable source, OperatingMode operatingMode)
		{
			SourceCollection = null;

			Mode = operatingMode;

			SourceCollection = source;
		}
		
		public IEnumerable SourceCollection
		{
			get => _sourceCollection;
			set
			{
				if (ReferenceEquals(_sourceCollection, value))
					return;

				var oldSource = _sourceCollection;
				
				{
					if (_sourceCollection is INotifyCollectionChanged notifyCollectionChanged)
						notifyCollectionChanged.CollectionChanged -= ObservableSourceOnCollectionChanged;
				}

				_sourceCollection = value;

				{
					if (_sourceCollection is INotifyCollectionChanged notifyCollectionChanged)
						notifyCollectionChanged.CollectionChanged += ObservableSourceOnCollectionChanged;
				}

				IndexedSource = SourceCollection != null ? new IndexedEnumerable(SourceCollection) : IndexedEnumerable.Empty;

				Reset();

				ObservableSourceOnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
			}
		}

		internal void AttachObserver(IItemCollectionObserver<T> observer)
		{
			_observers.Add(observer);
		}

		internal void DetachObserver(IItemCollectionObserver<T> observer)
		{
			_observers.Remove(observer);
		}

		public T EnsureItem(int index)
		{
			if (Mode == OperatingMode.Real)
				return RealGetItemFromIndex(index);
			
			return GetItemFromIndex(index) ?? Realize(index);
		}

		protected virtual T GetCurrent(int index)
		{
			if (Mode == OperatingMode.Real)
				return RealGetCurrent(index);
			
			if (index >= 0 && index < Count && Count > 0)
			{
				var nextItems = IsGenerating ? NextGeneratedItems : PrevGeneratedItems;

				return nextItems[index].Item;
			}

			return null;
		}

		public virtual int GetIndexFromItem(T item)
		{
			if (Mode == OperatingMode.Real)
				return RealGetIndexFromItem(item);
			
			return FindGeneratedItem(item).Index;
		}

		public int GetIndexFromSource(object source)
		{
			if (Mode == OperatingMode.Real)
				return RealGetIndexFromSource(source);
			
			return IndexedSource.IndexOf(source);
		}

		public T GetItemFromIndex(int index)
		{
			if (Mode == OperatingMode.Real)
				return RealGetItemFromIndex(index);

			var items = IsGenerating ? NextGeneratedItems : PrevGeneratedItems;
			
			if (index < 0 || index >= items.Count)
				return null;

			var generatedItem = items[index];

			if (generatedItem != null)
				return generatedItem.Item;

			var source = GetSourceFromIndex(index);
			
			if (source != null)
			{
				if (LockedSourceDictionary.TryGetValue(source, out var lockedItem))
				{
					return lockedItem.Item;
				}
			}

			return null;
		}

		public object GetSourceFromIndex(int index)
		{
			if (Mode == OperatingMode.Real)
				return RealGetSourceFromIndex(index);
			
			return IndexedSource[index];
		}

		public object GetSourceFromItem(T item)
		{
			if (Mode == OperatingMode.Real)
				return RealGetSourceFromItem(item);
			
			return FindGeneratedItem(item).Item?.Source;
		}

		public void LockItem(T item)
		{
			if (Mode == OperatingMode.Real)
			{
				RealLockItem(item);
				
				return;
			}
			
			if (LockedItemDictionary.TryGetValue(item, out var lockedItem) == false)
			{
				var generatedItem = FindGeneratedItemEverywhere(item).Item;

				if (generatedItem == null)
					return;

				generatedItem.Lock();
				LockedItemDictionary.Add(item, generatedItem);
				AddLockedSource(generatedItem);
			}
			else
			{
				lockedItem.Lock();
			}
		}

		protected virtual void ObservableSourceOnCollectionChanged(NotifyCollectionChangedEventArgs args)
		{
			try
			{
				foreach (var observer in _observers)
					observer.OnSourceCollectionChanged(args);
			}
			catch (Exception e)
			{
				LogService.LogError(e);
			}
		}

		protected virtual void OnGeneratedItemAttached(int index, T item)
		{
			try
			{
				foreach (var observer in _observers)
					observer.OnItemAttached(index, item);
			}
			catch (Exception e)
			{
				LogService.LogError(e);
			}
		}

		protected virtual void OnGeneratedItemDetached(int index, T item)
		{
			try
			{
				foreach (var observer in _observers)
					observer.OnItemDetached(index, item);
			}
			catch (Exception e)
			{
				LogService.LogError(e);
			}
		}

		public void UnlockItem(T item)
		{
			if (Mode == OperatingMode.Real)
			{
				RealUnlockItem(item);

				return;
			}
			
			if (LockedItemDictionary.TryGetValue(item, out var generatedItem) == false)
				LogService.LogWarning($"Item {item} is not locked.");
			else
			{
				if (generatedItem.Unlock() == false)
					return;

				LockedItemDictionary.Remove(item);
				RemoveLockedSource(generatedItem);
			}
		}

		int IVirtualItemCollection.GetIndexFromItem(FrameworkElement frameworkElement)
		{
			return GetIndexFromItem((T) frameworkElement);
		}

		UIElement IVirtualItemCollection.Realize(int index)
		{
			if (Mode == OperatingMode.Real)
				return RealRealize(index);
			
			return Realize(index);
		}

		UIElement IVirtualItemCollection.GetCurrent(int index)
		{
			if (Mode == OperatingMode.Real)
				return RealGetCurrent(index);
			
			return GetCurrent(index);
		}

		void IVirtualItemCollection.LeaveGeneration()
		{
			if (IsGenerating == false)
				throw new InvalidOperationException();

			if (Mode == OperatingMode.Real)
			{
				RealLeaveGeneration();
				
				return;
			}

			var items = PrevGeneratedItems;

			PrevGeneratedItems = NextGeneratedItems;
			NextGeneratedItems = items;

			items.Clear();

			EnsureCount();

			Version++;
			IsGenerating = false;
		}

		public int Count => IndexedSource.Count;

		IVirtualItemsHost IVirtualItemCollection.ItemHost
		{
			get => ItemHost;
			set => ItemHost = value;
		}

		void IVirtualItemCollection.EnterGeneration()
		{
			if (IsGenerating)
				throw new InvalidOperationException();

			if (Mode == OperatingMode.Real)
			{
				RealEnterGeneration();
				
				return;
			}
			
			IsGenerating = true;

			foreach (var tempGeneratedItemPair in EnumerateRealizedItems(TempGeneratedItems))
			{
				var tempGeneratedItem = tempGeneratedItemPair.Item;

				tempGeneratedItem.IsInTemp = false;

				if (IsLocked(tempGeneratedItem) == false)
					ReleaseItem(tempGeneratedItem, true);
			}

			SuspendReleaseGeneratedItems = true;

			TempGeneratedItems.Clear();

			SuspendReleaseGeneratedItems = false;

			EnsureCount();
		}
	}
}