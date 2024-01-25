// <copyright file="BaseLayout.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.UI.Controls.Docking
{
	public abstract class BaseLayout : DependencyObject
	{
		public static readonly DependencyProperty SelectedItemProperty = DPM.Register<DockItem, BaseLayout>
			("SelectedItem", t => t.OnSelectedItemChangedPrivate);

		private static readonly Dictionary<Type, List<DependencyProperty>> LayoutPropertiesDict = new();
		private static readonly Dictionary<Type, LayoutSerializer> LayoutSerializers = new();

		private int _layoutIndex;

		private BaseLayoutView _view;

		protected BaseLayout()
		{
			Items = new DockItemCollection(OnDockItemAddedPrivate, OnDockItemRemovedPrivate);
		}

		private DockControllerBase Controller { get; set; }

		internal DockControl DockControl { get; set; }

		public DockItemCollection Items { get; }

		public abstract LayoutKind LayoutKind { get; }

		internal IEnumerable<DependencyProperty> LayoutProperties => GetLayoutProperties(GetType());

		public DockItem SelectedItem
		{
			get => (DockItem)GetValue(SelectedItemProperty);
			set => SetValue(SelectedItemProperty, value);
		}

		internal BaseLayoutView View
		{
			get => _view;
			set
			{
				if (ReferenceEquals(_view, value))
					return;

				if (_view != null)
				{
					foreach (var item in Items)
						RemoveViewItem(item);

					_view.LayoutInternal = null;
				}

				_view = value;

				if (_view != null)
				{
					_view.LayoutInternal = this;

					foreach (var item in Items)
						AddViewItem(item);
				}
			}
		}

		private void RemoveViewItem(DockItem item)
		{
			if (item.AttachToView)
				View?.Items.Remove(item);
		}

		private void AddViewItem(DockItem item)
		{
			if (item.AttachToView)
				View?.Items.Add(item);
		}

		internal virtual void AttachController(DockControllerBase controller)
		{
			Controller = controller;

			controller.SelectionScope.SelectedItemChanged += OnSelectionScopeSelectedItemChanged;
		}

		internal void CopyLayoutSetting(DependencyObject source, IEnumerable<DependencyObject> targetItems)
		{
			foreach (var target in targetItems.Where(d => d != null && !ReferenceEquals(source, d)))
				LayoutSettings.CopySettings(source, target, LayoutProperties);
		}

		internal void CopyLayoutSetting(DependencyObject source, DependencyObject target)
		{
			LayoutSettings.CopySettings(source, target, LayoutProperties);
		}

		internal virtual void DetachController(DockControllerBase controller)
		{
			controller.SelectionScope.SelectedItemChanged -= OnSelectionScopeSelectedItemChanged;

			Controller = null;
		}

		protected virtual int GetDockItemOrder(DockItem dockItem)
		{
			return 0;
		}

		internal int GetDockItemOrderInternal(DockItem dockItem)
		{
			var dockItemOrderInternal = dockItem.GetLayoutIndex(this);

			if (dockItemOrderInternal == null)
			{
				var index = _layoutIndex++;

				dockItem.SetLayoutIndex(this, index);

				return index;
			}

			return dockItemOrderInternal.Value;
		}

		internal static IEnumerable<DependencyProperty> GetLayoutProperties(Type layoutType)
		{
			return LayoutPropertiesDict.GetValueOrDefault(layoutType) ?? Enumerable.Empty<DependencyProperty>();
		}

		internal static IEnumerable<DependencyProperty> GetLayoutProperties<T>() where T : BaseLayout
		{
			return GetLayoutProperties(typeof(T));
		}

		internal static LayoutSerializer GetLayoutSerializer(Type layoutType)
		{
			return LayoutSerializers.GetValueOrCreate(layoutType, DefaultLayoutSerializer.FromType);
		}

		internal static Type GetLayoutType(DockItemState dockState)
		{
			switch (dockState)
			{
				case DockItemState.Dock:

					return typeof(DockLayout);

				case DockItemState.Float:

					return typeof(FloatLayout);

				case DockItemState.AutoHide:

					return typeof(AutoHideLayout);

				case DockItemState.Document:

					return typeof(DocumentLayout);

				case DockItemState.Hidden:

					return typeof(HiddenLayout);
			}

			throw new ArgumentOutOfRangeException(nameof(dockState));
		}

		internal void InvalidateLayoutInternal()
		{
			InvalidateView();
		}

		private void InvalidateMeasureArrange(DockItem item)
		{
			item.InvalidateMeasure();
			item.InvalidateArrange();

			InvalidateView();
		}

		private void InvalidateView()
		{
			if (View == null)
				return;

			View.InvalidateMeasure();
			View.InvalidateArrange();
		}

		public virtual bool IsVisible(DockItem item)
		{
			return View?.IsItemVisible(item) == true;
		}

		protected virtual void OnDockItemAdded(DockItem item)
		{
		}

		private void OnDockItemAddedPrivate(DockItem item)
		{
			if (item.GetLayoutIndex(this) == null)
				item.SetLayoutIndex(this, _layoutIndex++);

			InvalidateMeasureArrange(item);

			SyncSelection();

			OnDockItemAdded(item);

			AddViewItem(item);
		}

		internal int GetNewLayoutIndex()
		{
			return _layoutIndex++;
		}

		protected virtual void OnDockItemRemoved(DockItem item)
		{
		}

		private void OnDockItemRemovedPrivate(DockItem item)
		{
			RemoveViewItem(item);

			InvalidateMeasureArrange(item);

			SyncSelection();

			OnDockItemRemoved(item);
		}

		internal void OnItemAttachToViewChangedInternal(DockItem dockItem)
		{
			if (View == null)
				return;

			if (dockItem.AttachToView)
				View.Items.Add(dockItem);
			else
				View.Items.Remove(dockItem);
		}

		protected static void OnLayoutPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is ILayoutPropertyChangeListener listener)
				listener.OnLayoutPropertyChanged(e);
		}

		protected virtual void OnSelectedItemChanged()
		{
		}

		private void OnSelectedItemChangedPrivate(DockItem oldItem, DockItem newItem)
		{
			View?.OnSelectedItemChangedInternal(oldItem, newItem);

			OnSelectedItemChanged();
		}

		private void OnSelectionScopeSelectedItemChanged(object sender, EventArgs e)
		{
			SyncSelection();
		}

		internal static void RegisterLayoutProperties(Type layoutType, IEnumerable<DependencyProperty> properties)
		{
			LayoutPropertiesDict[layoutType] = properties.ToList();
		}

		internal static void RegisterLayoutProperties<T>(IEnumerable<DependencyProperty> properties) where T : BaseLayout
		{
			RegisterLayoutProperties(typeof(T), properties);
		}

		internal static void RegisterLayoutSerializer<TLayout>(LayoutSerializer serializer) where TLayout : BaseLayout
		{
			LayoutSerializers[typeof(TLayout)] = serializer;
		}

		internal static bool ShouldSerializeProperty(Type layoutType, DependencyObject dependencyObject, DependencyProperty dependencyProperty)
		{
			if (DependencyPropertyUtils.GetValueSource(dependencyObject, dependencyProperty) == PropertyValueSource.Default)
				return false;

			var value = dependencyObject.GetValue(dependencyProperty);
			var defaultValue = DependencyPropertyUtils.GetDefaultValue(dependencyObject, dependencyProperty);

			if (value == null || defaultValue == null)
				return true;

			if (Equals(value, defaultValue))
				return false;

			if (string.Equals(value.ToString(), defaultValue.ToString(), StringComparison.OrdinalIgnoreCase))
				return false;

			return true;
		}

		private void SyncSelection()
		{
			var selectedItem = SelectedItem;

			if (Items.Contains(selectedItem) == false)
				selectedItem = null;

			var selectionScopeSelectedItem = Controller?.SelectionScope.SelectedItem;

			if (selectionScopeSelectedItem != null && Items.Contains(selectionScopeSelectedItem))
				selectedItem = selectionScopeSelectedItem;

			SelectedItem = selectedItem;
		}
	}

	internal interface ILayoutPropertyChangeListener
	{
		void OnLayoutPropertyChanged(DependencyPropertyChangedEventArgs e);
	}
}