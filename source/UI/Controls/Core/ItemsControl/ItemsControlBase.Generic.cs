// <copyright file="ItemsControlBase.Generic.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using Zaaml.Core;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.Input;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.UI.Controls.Primitives.PopupPrimitives;
using Zaaml.UI.Controls.ScrollView;
using Zaaml.UI.Panels.Core;
using Zaaml.UI.Utils;

namespace Zaaml.UI.Controls.Core
{
	[ContentProperty(nameof(Items))]
	public abstract class ItemsControlBase<TControl, TItem, TCollection, TPresenter, TPanel> : ItemsControlBase,
		IItemsControl<TItem>, IIndexedFocusNavigatorAdvisor<TItem>
		where TItem : System.Windows.Controls.Control
		where TCollection : ItemCollectionBase<TControl, TItem>
		where TPresenter : ItemsPresenterBase<TControl, TItem, TCollection, TPanel>
		where TPanel : ItemsPanel<TItem>
		where TControl : System.Windows.Controls.Control
	{
		private static readonly DependencyPropertyKey ItemsPropertyKey =
			DPM.RegisterReadOnly<TCollection, ItemsControlBase<TControl, TItem, TCollection, TPresenter, TPanel>>
				("ItemsInt");

		// ReSharper disable once StaticMemberInGenericType
		public static readonly DependencyProperty ItemsProperty = ItemsPropertyKey.DependencyProperty;

		private FocusNavigator<TItem> _focusNavigator;
		private WeakReference<TItem> _weakFocusedItem;

		protected IEnumerable<TItem> ActualItems => ItemsProxy.ActualItems;

		protected virtual bool CanScrollIntoView => IsItemsHostVisible;

		internal BringIntoViewMode DefaultBringIntoViewMode
		{
			get => Items.DefaultBringIntoViewMode;
			set => Items.DefaultBringIntoViewMode = value;
		}

		protected TItem FocusedItem => FocusNavigator.FocusedItem;

		private TItem FocusedItemPrivate
		{
			get => _weakFocusedItem != null && _weakFocusedItem.TryGetTarget(out var item) ? item : default;
			set
			{
				var focusedItem = FocusedItemPrivate;

				if (ReferenceEquals(focusedItem, value))
					return;

				if (focusedItem != null)
					Items.UnlockItemInternal(focusedItem);

				_weakFocusedItem = value != null ? new WeakReference<TItem>(value) : null;

				if (value != null)
					Items.LockItemInternal(value);
			}
		}

		internal FocusNavigator<TItem> FocusNavigator => _focusNavigator ??= CreateFocusNavigator();

		protected virtual bool HasFocus => this.HasFocus();

		internal virtual bool IsItemsHostVisible => true;

		public TCollection Items => this.GetValueOrCreate(ItemsPropertyKey, CreateItemCollectionPrivate);

		internal int ItemsCount => ItemsProxy.ActualCount;

		protected TPresenter ItemsPresenter => TemplateContract.ItemsPresenter;

		internal TPresenter ItemsPresenterInternal => ItemsPresenter;

		internal virtual IItemCollection<TItem> ItemsProxy => Items;

		protected IEnumerable ItemsSourceCore
		{
			get => ItemsProxy.Source;
			set => ItemsProxy.Source = value;
		}

		internal override Type ItemType => typeof(TItem);

		internal virtual bool NeedFocus => FocusHelper.IsKeyboardFocusWithin(this);

		private ItemsControlBaseTemplateContract<TPresenter> TemplateContract =>
			(ItemsControlBaseTemplateContract<TPresenter>) TemplateContractInternal;

		private protected void BringItemIntoView(TItem item, bool updateLayout)
		{
			if (item == null)
			{
				ScrollView?.ExecuteScrollCommand(ScrollCommandKind.ScrollToHome);

				return;
			}

			if (!(ItemsPresenter?.ItemsHostInternal is IItemsHost<TItem> host))
				return;

			var bringIntoViewRequest = new BringIntoViewRequest<TItem>(item, DefaultBringIntoViewMode, 0);

			if (updateLayout)
			{
				host.EnqueueBringIntoView(bringIntoViewRequest);

				UpdateLayout();

				host.BringIntoView(bringIntoViewRequest);
			}
			else
				host.BringIntoView(bringIntoViewRequest);
		}

		internal int CoerceIndex(int index)
		{
			var itemsCount = ItemsCount;

			if (itemsCount == 0)
				return -1;

			return index.Clamp(0, itemsCount - 1);
		}

		internal virtual FocusNavigator<TItem> CreateFocusNavigator()
		{
			return new IndexedFocusNavigator<ItemsControlBase<TControl, TItem, TCollection, TPresenter, TPanel>, TItem>(this);
		}

		protected abstract TCollection CreateItemCollection();

		private TCollection CreateItemCollectionPrivate()
		{
			return CreateItemCollection();
		}

		protected sealed override TemplateContract CreateTemplateContract()
		{
			var templateContract = base.CreateTemplateContract();

			if (templateContract is ItemsControlBaseTemplateContract<TPresenter> == false)
				throw new InvalidOperationException(
					"Invalid template contract. Must be derived from ItemsControlBaseTemplateContract<>");

			return templateContract;
		}

		protected virtual void FocusItem(TItem item)
		{
			var focusScope = FocusManager.GetFocusScope(item);

			if (focusScope != null && ReferenceEquals(item, focusScope) == false)
				FocusManager.SetFocusedElement(focusScope, item);
		}

		protected int GetIndexFromItem(TItem item)
		{
			return ItemsProxy.GetIndexFromItem(item);
		}

		protected TItem GetItemFromIndex(int index)
		{
			return ItemsProxy.GetItemFromIndex(index);
		}

		private protected virtual void InvalidatePanelCore()
		{
			var popup = Popup.FromElement(this);

			if (popup != null)
				this.InvalidateAncestorsMeasure(popup);

			ItemsPresenter?.ItemsHostInternal?.InvalidateMeasure();
		}

		internal void InvalidatePanelInternal()
		{
			InvalidatePanelCore();
		}

		protected bool IsNavigationKey(Key key)
		{
			return FocusNavigator.IsNavigationKey(key);
		}

		internal bool IsOnCurrentPage(int index)
		{
			return IsOnCurrentPage(index, out var itemsHostRect, out var listBoxItemRect);
		}

		internal bool IsOnCurrentPage(int index, out Rect itemsHostRect, out Rect itemRect)
		{
			itemsHostRect = Rect.Empty;
			itemRect = Rect.Empty;

			try
			{
				var frameworkElement = (FrameworkElement) ScrollView?.ScrollViewPresenterInternal ?? ScrollView;

				if (frameworkElement == null)
					return true;

				itemsHostRect = new Rect(0.0, 0.0, frameworkElement.ActualWidth, frameworkElement.ActualHeight);

				var freItem = GetItemFromIndex(index);

				if (freItem == null || freItem.IsVisualDescendantOf(frameworkElement) == false)
				{
					itemRect = Rect.Empty;

					return false;
				}

				var transform = freItem.TransformToVisual(frameworkElement);

				itemRect = new Rect(transform.Transform(new Point()),
					transform.Transform(new Point(freItem.ActualWidth, freItem.ActualHeight)));

				if (HasLogicalOrientation == false)
					return itemsHostRect.Contains(itemRect);

				var orientation = LogicalOrientation;

				return itemsHostRect.GetMinPart(orientation) <= itemRect.GetMinPart(orientation) &&
				       itemRect.GetMaxPart(orientation) <= itemsHostRect.GetMaxPart(orientation);
			}
			catch (Exception ex)
			{
				LogService.LogError(ex);
			}

			return false;
		}

		internal override void OnCollectionChangedInternal(object sender, NotifyCollectionChangedEventArgs args)
		{
			UpdateHasItems();
		}

		protected virtual void OnItemAttached(TItem item)
		{
		}

		internal virtual void OnItemAttachedInternal(TItem item)
		{
			OnItemAttached(item);
		}

		private void OnItemAttachedPrivate(TItem item)
		{
			OnItemAttachedInternal(item);

			FocusNavigator.OnItemAttachedInternal(item);

			UpdateHasItems();
		}

		protected virtual void OnItemAttaching(TItem item)
		{
		}

		internal virtual void OnItemAttachingInternal(TItem item)
		{
			OnItemAttaching(item);
		}

		private void OnItemAttachingPrivate(TItem item)
		{
			OnItemAttachingInternal(item);
		}

		protected virtual void OnItemDetached(TItem item)
		{
		}

		internal virtual void OnItemDetachedInternal(TItem item)
		{
			OnItemDetached(item);
		}

		private void OnItemDetachedPrivate(TItem item)
		{
			FocusNavigator.OnItemDetachedInternal(item);

			OnItemDetachedInternal(item);
		}

		protected virtual void OnItemDetaching(TItem item)
		{
		}

		internal virtual void OnItemDetachingInternal(TItem item)
		{
			OnItemDetaching(item);
		}

		private void OnItemDetachingPrivate(TItem item)
		{
			OnItemDetachingInternal(item);
		}

		private protected virtual void OnItemGotFocus(TItem item)
		{
			FocusNavigator.OnItemGotFocusInternal(item);
		}

		internal void OnItemGotFocusInternal(TItem item)
		{
			OnItemGotFocus(item);

			FocusedItemPrivate = item;
		}

		private protected virtual void OnItemLostFocus(TItem item)
		{
			FocusNavigator.OnItemLostFocusInternal(item);
		}

		internal void OnItemLostFocusInternal(TItem item)
		{
			if (ReferenceEquals(item, FocusedItemPrivate))
				FocusedItemPrivate = null;

			OnItemLostFocus(item);
		}

		internal override void OnItemsSourceChangedInt(IEnumerable oldSource, IEnumerable newSource)
		{
			ItemsProxy.Source = newSource;
		}

		internal override void OnSourceChangedInternal()
		{
			UpdateHasItems();
		}

		protected override void OnTemplateContractAttached()
		{
			base.OnTemplateContractAttached();

			if (ItemsPresenter != null)
				ItemsPresenter.ItemsCore = Items;
		}

		protected override void OnTemplateContractDetaching()
		{
			if (ItemsPresenter != null)
				ItemsPresenter.ItemsCore = null;

			base.OnTemplateContractDetaching();
		}

		internal void ScrollIntoView(int index)
		{
			if (CanScrollIntoView == false)
				return;

			if (index == -1 || ScrollView == null || IsItemsHostVisible == false)
				return;

			if (IsOnCurrentPage(index, out var itemsHostRect, out var itemRect))
				return;

			if (IsVirtualizing)
			{
				ItemsProxy.BringIntoView(index);

				ScrollView?.UpdateLayout();
			}
			else
			{
				var orientation = LogicalOrientation;
				var orientedViewer = new OrientedScrollViewerWrapper(ScrollView, orientation);
				var offset = orientedViewer.Offset;

				var delta = 0.0;

				var hostMin = itemsHostRect.GetMinPart(orientation);
				var hostMax = itemsHostRect.GetMaxPart(orientation);

				var itemMin = itemRect.GetMinPart(orientation);
				var itemMax = itemRect.GetMaxPart(orientation);

				if (hostMax < itemMax)
				{
					delta = itemMax - hostMax;
					offset += delta;
				}

				if (itemMin - delta < hostMin)
					offset -= hostMin - (itemMin - delta);

				orientedViewer.ScrollToOffset(offset);
			}
		}

		internal void SyncFocusedIndex(int index)
		{
			throw Error.Refactoring;
			//_focusedIndex = index.Clamp(-1, ItemsCount);
		}

		private void UpdateHasItems()
		{
			HasItems = ItemsCount > 0;
		}

		bool IFocusNavigatorAdvisor<TItem>.IsVirtualizing => IsVirtualizing;

		int IFocusNavigatorAdvisor<TItem>.ItemsCount => ItemsCount;

		Orientation IFocusNavigatorAdvisor<TItem>.LogicalOrientation => LogicalOrientation;

		bool IFocusNavigatorAdvisor<TItem>.HasFocus => HasFocus;

		ScrollViewControl IFocusNavigatorAdvisor<TItem>.ScrollView => ScrollView;

		bool IFocusNavigatorAdvisor<TItem>.IsItemsHostVisible => IsItemsHostVisible;

		TItem IIndexedFocusNavigatorAdvisor<TItem>.EnsureItem(int index)
		{
			return ItemsProxy.EnsureItem(index);
		}

		TItem IIndexedFocusNavigatorAdvisor<TItem>.GetItemFromIndex(int index)
		{
			return GetItemFromIndex(index);
		}

		bool IIndexedFocusNavigatorAdvisor<TItem>.IsOnCurrentPage(int index)
		{
			return IsOnCurrentPage(index);
		}

		void IIndexedFocusNavigatorAdvisor<TItem>.ScrollIntoView(int index)
		{
			ScrollIntoView(index);
		}

		int IIndexedFocusNavigatorAdvisor<TItem>.GetIndexFromItem(TItem item)
		{
			return GetIndexFromItem(item);
		}

		void IIndexedFocusNavigatorAdvisor<TItem>.LockItem(TItem item)
		{
			ItemsProxy.LockItem(item);
		}

		void IIndexedFocusNavigatorAdvisor<TItem>.UnlockItem(TItem item)
		{
			ItemsProxy.UnlockItem(item);
		}

		IEnumerable<TItem> IItemsControl<TItem>.ActualItems => ActualItems;

		void IItemsControl<TItem>.OnItemDetached(TItem item)
		{
			OnItemDetachedPrivate(item);
		}

		void IItemsControl<TItem>.OnItemAttaching(TItem item)
		{
			OnItemAttachingPrivate(item);
		}

		void IItemsControl<TItem>.OnItemDetaching(TItem item)
		{
			OnItemDetachingPrivate(item);
		}

		void IItemsControl<TItem>.OnItemAttached(TItem item)
		{
			OnItemAttachedPrivate(item);
		}
	}

	public class ItemsControlBaseTemplateContract<TPresenter> : ItemsControlBaseTemplateContract
		where TPresenter : ItemsPresenterBase
	{
		[TemplateContractPart(Required = false)]
		public TPresenter ItemsPresenter { get; [UsedImplicitly] private set; }

		protected override ItemsPresenterBase ItemsPresenterBase => ItemsPresenter;
	}
}