﻿// <copyright file="ItemsControlBase.Generic.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Markup;
using Zaaml.Core;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.UI.Controls.Primitives.PopupPrimitives;
using Zaaml.UI.Panels.Core;

namespace Zaaml.UI.Controls.Core
{
	[ContentProperty(nameof(ItemCollection))]
	public abstract class ItemsControlBase<TControl, TItem, TCollection, TPresenter, TPanel> : ItemsControlBase,
		IItemsControl<TItem>
		where TItem : FrameworkElement
		where TCollection : ItemCollectionBase<TControl, TItem>
		where TPresenter : ItemsPresenterBase<TControl, TItem, TCollection, TPanel>
		where TPanel : ItemsPanel<TItem>
		where TControl : System.Windows.Controls.Control
	{
		private static readonly DependencyPropertyKey ItemCollectionPropertyKey = DPM.RegisterReadOnly<TCollection, ItemsControlBase<TControl, TItem, TCollection, TPresenter, TPanel>>
			("ItemCollectionPrivate");

		// ReSharper disable once StaticMemberInGenericType
		public static readonly DependencyProperty ItemCollectionProperty = ItemCollectionPropertyKey.DependencyProperty;

		protected IEnumerable<TItem> ActualItems => ItemCollectionOverride.ActualItems;

		internal virtual bool IsItemsHostVisible => true;

		public TCollection ItemCollection => this.GetValueOrCreate(ItemCollectionPropertyKey, CreateItemCollectionPrivate);

		internal virtual IItemCollection<TItem> ItemCollectionOverride => ItemCollection;

		internal virtual bool HasSource => ItemCollectionOverride.SourceCollection != null;

		internal int ItemsCount => ItemCollectionOverride.ActualCount;

		protected TPresenter ItemsPresenter => TemplateContract.ItemsPresenter;

		internal TPresenter ItemsPresenterInternal => ItemsPresenter;

		internal override Type ItemType => typeof(TItem);

		protected IEnumerable SourceCollectionCore
		{
			get => ItemCollectionOverride.SourceCollection;
			set => ItemCollectionOverride.SourceCollection = value;
		}

		private ItemsControlBaseTemplateContract<TPresenter> TemplateContract => (ItemsControlBaseTemplateContract<TPresenter>) TemplateContractCore;

		internal int CoerceIndex(int index)
		{
			var itemsCount = ItemsCount;

			if (itemsCount == 0)
				return -1;

			return index.Clamp(0, itemsCount - 1);
		}

		protected abstract TCollection CreateItemCollection();

		private TCollection CreateItemCollectionPrivate()
		{
			return CreateItemCollection();
		}

		protected override TemplateContract CreateTemplateContract()
		{
			var templateContract = base.CreateTemplateContract();

			if (templateContract is ItemsControlBaseTemplateContract<TPresenter> == false)
				throw new InvalidOperationException("Invalid template contract. Must be derived from ItemsControlBaseTemplateContract<>");

			return templateContract;
		}

		protected int GetIndexFromItem(TItem item)
		{
			return ItemCollectionOverride.GetIndexFromItem(item);
		}

		protected TItem GetItemFromIndex(int index)
		{
			return ItemCollectionOverride.GetItemFromIndex(index);
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

		internal override void OnSourceChangedInt(IEnumerable oldSource, IEnumerable newSource)
		{
			ItemCollectionOverride.SourceCollection = newSource;
		}

		internal override void OnSourceChangedInternal()
		{
			UpdateHasItems();
		}

		protected override void OnTemplateContractAttached()
		{
			base.OnTemplateContractAttached();

			if (ItemsPresenter != null)
				ItemsPresenter.ItemsCore = ItemCollection;
		}

		protected override void OnTemplateContractDetaching()
		{
			if (ItemsPresenter != null)
				ItemsPresenter.ItemsCore = null;

			base.OnTemplateContractDetaching();
		}

		private void UpdateHasItems()
		{
			HasItems = ItemsCount > 0;
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