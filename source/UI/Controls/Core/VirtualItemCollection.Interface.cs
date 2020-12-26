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
		public int ActualCount => Mode == OperatingMode.Real ? RealActualCount : VirtualActualCount;
		
		public IEnumerable<T> ActualItems => Mode == OperatingMode.Real ? RealGetGeneratedItems() : VirtualGetGeneratedItems();

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

		public IEnumerable SourceCollection
		{
			get => _sourceCollection;
			set
			{
				if (ReferenceEquals(_sourceCollection, value))
					return;

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
			return Mode == OperatingMode.Real ? RealEnsureItem(index) : VirtualEnsureItem(index);
		}

		protected virtual T GetCurrent(int index)
		{
			return Mode == OperatingMode.Real ? RealGetCurrent(index) : VirtualGetCurrent(index);
		}

		public virtual int GetIndexFromItem(T item)
		{
			return Mode == OperatingMode.Real ? RealGetIndexFromItem(item) : VirtualGetIndexFromItem(item);
		}

		public int GetIndexFromSource(object source)
		{
			return Mode == OperatingMode.Real ? RealGetIndexFromSource(source) : VirtualGetIndexFromSource(source);
		}

		public T GetItemFromIndex(int index)
		{
			return Mode == OperatingMode.Real ? RealGetItemFromIndex(index) : VirtualGetItemFromIndex(index);
		}

		public object GetSourceFromIndex(int index)
		{
			return Mode == OperatingMode.Real ? RealGetSourceFromIndex(index) : VirtualGetSourceFromIndex(index);
		}

		public object GetSourceFromItem(T item)
		{
			return Mode == OperatingMode.Real ? RealGetSourceFromItem(item) : FindGeneratedItem(item).Item?.Source;
		}

		protected void Init(IEnumerable source, OperatingMode operatingMode)
		{
			SourceCollection = null;

			Mode = operatingMode;

			SourceCollection = source;
		}

		public void LockItem(T item)
		{
			if (Mode == OperatingMode.Real)
				RealLockItem(item);
			else
				VirtualLockItem(item);
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

		private UIElement Realize(int index)
		{
			return Mode == OperatingMode.Real ? RealRealize(index) : VirtualRealize(index);
		}

		public void UnlockItem(T item)
		{
			if (Mode == OperatingMode.Real)
				RealUnlockItem(item);
			else
				VirtualUnlockItem(item);
		}

		int IVirtualItemCollection.GetIndexFromItem(FrameworkElement frameworkElement)
		{
			return GetIndexFromItem((T) frameworkElement);
		}

		UIElement IVirtualItemCollection.Realize(int index)
		{
			return Realize(index);
		}

		UIElement IVirtualItemCollection.GetCurrent(int index)
		{
			return GetCurrent(index);
		}

		void IVirtualItemCollection.LeaveGeneration()
		{
			if (IsGenerating == false)
				throw new InvalidOperationException();

			if (Mode == OperatingMode.Real)
				RealLeaveGeneration();
			else
				VirtualLeaveGeneration();
		}

		int IVirtualItemCollection.Count => ActualCount;

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
				RealEnterGeneration();
			else
				VirtualEnterGeneration();
		}
	}
}