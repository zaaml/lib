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

		private static readonly Dictionary<Type, DependencyProperty> LayoutIndexProperties = new Dictionary<Type, DependencyProperty>();
		private static readonly Dictionary<Type, List<DependencyProperty>> LayoutPropertiesDict = new Dictionary<Type, List<DependencyProperty>>();
		private static readonly Dictionary<Type, LayoutSerializer> LayoutSerializers = new Dictionary<Type, LayoutSerializer>();

		private BaseLayoutView _view;

		protected BaseLayout()
		{
			Items = new DockItemCollection(OnDockItemAddedPrivate, OnDockItemRemovedPrivate);
		}

		private DockControllerBase Controller { get; set; }

		internal DockControl DockControl { get; set; }

		internal DockItemIndexProvider IndexProvider { get; } = new DockItemIndexProvider();

		public DockItemCollection Items { get; }

		public abstract LayoutKind LayoutKind { get; }

		internal IEnumerable<DependencyProperty> LayoutProperties => GetLayoutProperties(GetType());

		public DockItem SelectedItem
		{
			get => (DockItem) GetValue(SelectedItemProperty);
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
					_view.LayoutInternal = null;

				_view = value;

				if (_view != null)
					_view.LayoutInternal = this;
			}
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

		internal static int GetActualDockItemIndex<T>(DockItem item) where T : BaseLayout
		{
			return (int?) item.GetValue(GetDockItemIndexProperty<T>()) ?? 0;
		}

		internal static int GetActualDockItemIndex(DockItem item, Type layoutType)
		{
			return (int?) item.GetValue(GetDockItemIndexProperty(layoutType)) ?? 0;
		}

		internal static int? GetDockItemIndex<T>(DockItem item) where T : BaseLayout
		{
			return (int?) item.GetValue(GetDockItemIndexProperty<T>());
		}

		internal static int? GetDockItemIndex(DockItem item, Type layoutType)
		{
			return (int?) item.GetValue(GetDockItemIndexProperty(layoutType));
		}

		internal int? GetDockItemIndex(DockItem item)
		{
			return (int?) item.GetValue(GetDockItemIndexProperty(GetType()));
		}

		internal static DependencyProperty GetDockItemIndexProperty(Type layoutType)
		{
			return LayoutIndexProperties.GetValueOrCreate(layoutType, t => DPM.RegisterAttached<int?>($"{t.Name}DockItemIndex", layoutType, OnDockItemIndexPropertyChanged));
		}

		internal static DependencyProperty GetDockItemIndexProperty<T>() where T : BaseLayout
		{
			return GetDockItemIndexProperty(typeof(T));
		}

		internal static IEnumerable<DependencyProperty> GetLayoutProperties(Type layoutType)
		{
			return LayoutPropertiesDict.GetValueOrDefault(layoutType) ?? Enumerable.Empty<DependencyProperty>();
		}

		internal static IEnumerable<DependencyProperty> GetLayoutProperties<T>(IEnumerable<DependencyProperty> properties) where T : BaseLayout
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

		private void InvalidateMeasureArrange(DockItem item)
		{
			item.InvalidateMeasure();
			item.InvalidateArrange();

			View?.InvalidateMeasure();
			View?.InvalidateArrange();
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
			View?.Items.Add(item);

			InvalidateMeasureArrange(item);

			SyncSelection();

			IndexProvider.SyncDockItemIndex(GetDockItemIndex(item));

			OnDockItemAdded(item);
		}

		protected static void OnDockItemIndexPropertyChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs args)
		{
			var dockItem = depObj as DockItem;
			var actualLayout = dockItem?.ActualLayout;

			if (actualLayout == null)
				return;

			if (ReferenceEquals(GetDockItemIndexProperty(actualLayout.GetType()), args.Property))
			{
				// TODO ActualLayout could be wrong
				actualLayout.IndexProvider.OnDockItemIndexChanged(dockItem.ActualLayout?.GetDockItemIndex(dockItem));
			}
		}

		protected virtual void OnDockItemRemoved(DockItem item)
		{
		}

		private void OnDockItemRemovedPrivate(DockItem item)
		{
			View?.Items.Remove(item);

			InvalidateMeasureArrange(item);

			SyncSelection();

			OnDockItemRemoved(item);
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
			LayoutPropertiesDict[layoutType] = properties.Concat(GetDockItemIndexProperty(layoutType)).ToList();
		}

		internal static void RegisterLayoutProperties<T>(IEnumerable<DependencyProperty> properties) where T : BaseLayout
		{
			RegisterLayoutProperties(typeof(T), properties);
		}

		internal static void RegisterLayoutSerializer<TLayout>(LayoutSerializer serializer) where TLayout : BaseLayout
		{
			LayoutSerializers[typeof(TLayout)] = serializer;
		}

		internal static void SetDockItemIndex(DockItem item, Type layoutType, int? index)
		{
			item.SetValue(GetDockItemIndexProperty(layoutType), index);
		}

		internal static void SetDockItemIndex<T>(DockItem item, int? index) where T : BaseLayout
		{
			item.SetValue(GetDockItemIndexProperty<T>(), index);
		}

		internal void SetDockItemIndex(DockItem item, int? value)
		{
			item.SetValue(GetDockItemIndexProperty(GetType()), value);
		}

		internal static bool ShouldSerializeProperty(Type layoutType, DependencyObject dependencyObject, DependencyProperty dependencyProperty)
		{
			if (ReferenceEquals(GetDockItemIndexProperty(layoutType), dependencyProperty))
				return false;

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

		internal void UpdateDockItemIndex(DockItem item)
		{
			SetDockItemIndex(item, GetDockItemIndex(item) ?? IndexProvider.NewIndex);
		}
	}

	internal interface ILayoutPropertyChangeListener
	{
		void OnLayoutPropertyChanged(DependencyPropertyChangedEventArgs e);
	}
}