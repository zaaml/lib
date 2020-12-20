﻿// <copyright file="ListViewControl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Zaaml.Core;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.Input;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Core;
using Zaaml.UI.Controls.Interfaces;
using Zaaml.UI.Controls.ListView.Data;
using Zaaml.UI.Controls.ScrollView;

namespace Zaaml.UI.Controls.ListView
{
	[TemplateContractType(typeof(ListViewControlTemplateContract))]
	public class ListViewControl : IndexedSelectorBase<ListViewControl, ListViewItem, ListViewItemCollection, ListViewItemsPresenter, ListViewPanel>, IContentItemsControl, IIndexedFocusNavigatorAdvisor<ListViewItem>
	{
		public static readonly DependencyProperty ItemGeneratorProperty = DPM.Register<ListViewItemGeneratorBase, ListViewControl>
			("ItemGenerator", l => l.OnItemGeneratorChanged);

		public static readonly DependencyProperty ItemContentTemplateProperty = DPM.Register<DataTemplate, ListViewControl>
			("ItemContentTemplate", l => l.DefaultGeneratorImpl.OnItemContentTemplateChanged);

		public static readonly DependencyProperty ItemContentTemplateSelectorProperty = DPM.Register<DataTemplateSelector, ListViewControl>
			("ItemContentTemplateSelector", l => l.DefaultGeneratorImpl.OnItemContentTemplateSelectorChanged);

		public static readonly DependencyProperty ItemContentStringFormatProperty = DPM.Register<string, ListViewControl>
			("ItemContentStringFormat", l => l.DefaultGeneratorImpl.OnItemContentStringFormatChanged);

		public static readonly DependencyProperty ItemsSourceProperty = DPM.Register<IEnumerable, ListViewControl>
			("ItemsSource", i => i.OnItemsSourceChangedPrivate);

		public static readonly DependencyProperty ItemsFilterProperty = DPM.Register<IListViewItemFilter, ListViewControl>
			("ItemsFilter", default, d => d.OnItemsFilterPropertyChangedPrivate);

		public static readonly DependencyProperty SelectionModeProperty = DPM.Register<ListViewSelectionMode, ListViewControl>
			("SelectionMode", ListViewSelectionMode.Single, l => l.OnSelectionModePropertyChangedPrivate);

		private static readonly DependencyPropertyKey SelectionCollectionPropertyKey = DPM.RegisterReadOnly<ListViewSelectionCollection, ListViewControl>
			("SelectionCollectionPrivate");

		public static readonly DependencyProperty ItemGlyphTemplateProperty = DPM.Register<DataTemplate, ListViewControl>
			("ItemGlyphTemplate");

		private DelegateContentItemGeneratorImpl<ListViewItem, DefaultListViewItemGenerator> _defaultGeneratorImpl;
		private ListViewData _listViewData;

		public event EventHandler<ListViewItemMouseButtonEventArgs> ItemMouseButtonDown;

		public event EventHandler<ListViewItemMouseButtonEventArgs> ItemMouseButtonUp;

		internal event EventHandler<ValueChangedEventArgs<ListViewData>> ListViewDataChanged;

		internal event EventHandler<ValueChangingEventArgs<ListViewData>> ListViewDataChanging;

		static ListViewControl()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<ListViewControl>();

			KeyboardNavigation.TabNavigationProperty.OverrideMetadata(typeof(ListViewControl), new FrameworkPropertyMetadata(KeyboardNavigationMode.Once));
			KeyboardNavigation.ControlTabNavigationProperty.OverrideMetadata(typeof(ListViewControl), new FrameworkPropertyMetadata(KeyboardNavigationMode.Continue));
			KeyboardNavigation.DirectionalNavigationProperty.OverrideMetadata(typeof(ListViewControl), new FrameworkPropertyMetadata(KeyboardNavigationMode.Contained));
		}

		public ListViewControl()
		{
			this.OverrideStyleKey<ListViewControl>();

			VirtualItemCollection = new VirtualListViewItemCollection(this);
		}

		private protected override bool ActualSelectItemOnFocus => SelectionMode != ListViewSelectionMode.Multiple && base.ActualSelectItemOnFocus;

		protected virtual bool ActualFocusItemOnMouseHover => FocusItemOnMouseHover;

		protected virtual bool ActualFocusItemOnSelect => FocusItemOnSelect;

		private ListViewItemGeneratorBase ActualGenerator => ItemGenerator ?? DefaultGenerator;

		private ListViewItemGeneratorBase DefaultGenerator => DefaultGeneratorImpl.Generator;

		private DelegateContentItemGeneratorImpl<ListViewItem, DefaultListViewItemGenerator> DefaultGeneratorImpl => _defaultGeneratorImpl ??= new DelegateContentItemGeneratorImpl<ListViewItem, DefaultListViewItemGenerator>(this);

		internal int FocusedIndexInternal => ListViewFocusNavigator.FocusedIndex;

		internal bool FocusItemOnMouseHover { get; set; }

		internal bool FocusItemOnSelect { get; set; } = true;

		private bool IsFocusOnMouseEventLocked { get; set; }

		public ListViewItemGeneratorBase ItemGenerator
		{
			get => (ListViewItemGeneratorBase) GetValue(ItemGeneratorProperty);
			set => SetValue(ItemGeneratorProperty, value);
		}

		public DataTemplate ItemGlyphTemplate
		{
			get => (DataTemplate) GetValue(ItemGlyphTemplateProperty);
			set => SetValue(ItemGlyphTemplateProperty, value);
		}

		public IListViewItemFilter ItemsFilter
		{
			get => (IListViewItemFilter) GetValue(ItemsFilterProperty);
			set => SetValue(ItemsFilterProperty, value);
		}

		internal override IItemCollection<ListViewItem> ItemsProxy => VirtualItemCollection;

		public IEnumerable ItemsSource
		{
			get => (IEnumerable) GetValue(ItemsSourceProperty);
			set => SetValue(ItemsSourceProperty, value);
		}

		internal ListViewData ListViewData
		{
			get => _listViewData;
			private set
			{
				if (ReferenceEquals(_listViewData, value))
					return;

				var oldList = _listViewData;

				ListViewDataChanging?.Invoke(this, new ValueChangingEventArgs<ListViewData>(_listViewData, value));

				_listViewData = _listViewData.DisposeExchange(value);

				ListViewDataChanged?.Invoke(this, new ValueChangedEventArgs<ListViewData>(oldList, value));
			}
		}

		private ListViewFocusNavigator ListViewFocusNavigator => (ListViewFocusNavigator) FocusNavigator;

		internal MouseButtonSelectionOptions MouseButtonSelectionOptions { get; set; } = MouseButtonSelectionOptions.LeftButtonDown;

		internal ScrollViewControl ScrollViewInternal => ScrollView;

		public ListViewSelectionCollection SelectionCollection => this.GetValueOrCreate(SelectionCollectionPropertyKey, () => new ListViewSelectionCollection((ListViewSelectorController) SelectorController));

		public static DependencyProperty SelectionCollectionProperty => SelectionCollectionPropertyKey.DependencyProperty;

		public ListViewSelectionMode SelectionMode
		{
			get => (ListViewSelectionMode) GetValue(SelectionModeProperty);
			set => SetValue(SelectionModeProperty, value);
		}

		internal VirtualListViewItemCollection VirtualItemCollection { get; }

		protected virtual bool CanSelectItem(ListViewItem listViewItem)
		{
			return true;
		}

		internal bool CanSelectItemInternal(ListViewItem listViewItem)
		{
			return CanSelectItem(listViewItem);
		}

		internal override FocusNavigator<ListViewItem> CreateFocusNavigator()
		{
			return new ListViewFocusNavigator(this);
		}

		protected override ListViewItemCollection CreateItemCollection()
		{
			return new ListViewItemCollection(this)
			{
				Generator = ActualGenerator
			};
		}

		private ListViewData CreateListViewData()
		{
			return new ListViewData(this)
			{
				Source = ItemsSource ?? Items,
				DataFilter =
				{
					Filter = ItemsFilter
				}
			};
		}

		internal override SelectorController<ListViewControl, ListViewItem> CreateSelectorController()
		{
			return new ListViewSelectorController(this);
		}

		internal ListViewData EnsureListViewData()
		{
			return ListViewData ??= CreateListViewData();
		}

		internal void EnsureVirtualItemCollection()
		{
			EnsureListViewData();

			VirtualItemCollection.ListViewData = ListViewData;
			VirtualItemCollection.Generator = ActualGenerator;
		}

		protected override Size MeasureOverride(Size availableSize)
		{
			EnsureListViewData();

			VirtualItemCollection.ListViewData = ListViewData;
			VirtualItemCollection.Generator = ActualGenerator;

			return base.MeasureOverride(availableSize);
		}

		private void ToggleItemSelection(ListViewItem listViewItem)
		{
			if (listViewItem.IsSelected == false)
				listViewItem.SelectInternal();
			else if (SelectionMode == ListViewSelectionMode.Multiple) 
				listViewItem.UnselectInternal();
		}

		private bool MouseSelect(ListViewItem listViewItem, MouseButtonKind mouseButtonKind, MouseButtonEventKind eventKind)
		{
			if (MouseButtonSelectionHelper.ShouldSelect(mouseButtonKind, eventKind, MouseButtonSelectionOptions.LeftButtonDown) && listViewItem.ActualCanSelect)
			{
				FocusItem(listViewItem);
				ToggleItemSelection(listViewItem);

				return true;
			}

			return false;
		}

		internal override void OnCollectionChangedInternal(object sender, NotifyCollectionChangedEventArgs args)
		{
			base.OnCollectionChangedInternal(sender, args);

			InvalidatePanelCore();
		}

		internal void OnFilterUpdatedInternal()
		{
			var item = ListViewFocusNavigator.FocusedItemCache ?? SelectedItem ?? Items.ActualItemsInternal.FirstOrDefault(null);

			BringItemIntoView(item, true);

			ListViewFocusNavigator.FocusedIndex = item != null ? GetIndexFromItem(item) : -1;
		}

		internal void OnFilterUpdatingInternal()
		{
		}

		private void OnGeneratorChanged(object sender, EventArgs e)
		{
			UpdateData();
		}

		private void OnIsCheckedPropertyChangedPrivate(bool? oldValue, bool? newValue)
		{
		}

		internal override void OnItemAttachedInternal(ListViewItem item)
		{
			item.ListViewControl = this;

			base.OnItemAttachedInternal(item);
		}

		internal override void OnItemDetachedInternal(ListViewItem item)
		{
			base.OnItemDetachedInternal(item);

			item.ListViewControl = null;
		}

		internal virtual void OnItemGeneratorChanged(ListViewItemGeneratorBase oldGenerator, ListViewItemGeneratorBase newGenerator)
		{
			Items.Generator = ActualGenerator;

			if (oldGenerator != null)
				oldGenerator.GeneratorChangedCore -= OnGeneratorChanged;

			if (newGenerator != null)
				newGenerator.GeneratorChangedCore += OnGeneratorChanged;

			UpdateData();
		}

		internal void OnItemMouseButton(ListViewItem listViewItem, MouseButtonEventArgs e)
		{
			e.Handled = MouseSelect(listViewItem, MouseUtils.FromMouseButton(e.ChangedButton), MouseUtils.FromButtonState(e.ButtonState));

			if (e.ButtonState == MouseButtonState.Released)
				ItemMouseButtonUp?.Invoke(this, new ListViewItemMouseButtonEventArgs(listViewItem, e));
			else
				ItemMouseButtonDown?.Invoke(this, new ListViewItemMouseButtonEventArgs(listViewItem, e));
		}

		internal void OnItemMouseEnter(ListViewItem listViewItem, MouseEventArgs e)
		{
			if (ActualFocusItemOnMouseHover && IsFocusOnMouseEventLocked == false)
				FocusItem(listViewItem);
		}

		internal void OnItemMouseLeave(ListViewItem listViewItem, MouseEventArgs e)
		{
		}

		internal void OnItemMouseMove(ListViewItem listViewItem, MouseEventArgs mouseEventArgs)
		{
		}

		private void OnItemsCheckBoxVisibilityPropertyChangedPrivate(ElementVisibility oldValue, ElementVisibility newValue)
		{
		}

		private void OnItemsFilterPropertyChangedPrivate(IListViewItemFilter oldFilter, IListViewItemFilter newFilter)
		{
			if (ReferenceEquals(oldFilter, newFilter))
				return;

			if (ListViewData == null)
				return;

			ListViewData.DataFilter.Filter = ItemsFilter;

			InvalidateMeasure();
		}

		private void OnItemsSourceChangedPrivate(IEnumerable oldSource, IEnumerable newSource)
		{
			ItemsSourceCore = newSource;

			UpdateData();
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);

			if (e.Handled)
				return;

			e.Handled = FocusNavigator.HandleNavigationKey(e.Key);
		}

		internal void OnNavigationKeyHandled()
		{
			IsFocusOnMouseEventLocked = true;
		}

		protected override void OnPreviewMouseMove(MouseEventArgs e)
		{
			IsFocusOnMouseEventLocked = false;

			base.OnPreviewMouseMove(e);
		}

		private void OnSelectionModePropertyChangedPrivate()
		{
			SelectorController.MultipleSelection = SelectionMode == ListViewSelectionMode.Multiple;
		}

		protected override void OnTemplateContractAttached()
		{
			base.OnTemplateContractAttached();

			ItemsPresenter.ListViewControl = this;
		}

		protected override void OnTemplateContractDetaching()
		{
			ItemsPresenter.ListViewControl = null;

			base.OnTemplateContractDetaching();
		}

		internal void Select(ListViewItem listViewItem)
		{
			if (ActualFocusItemOnSelect)
				FocusItem(listViewItem);

			SelectorController.SelectItem(listViewItem, SelectionMode == ListViewSelectionMode.Multiple);
		}

		internal void Unselect(ListViewItem listViewItem)
		{
			SelectorController.UnselectItem(listViewItem);
		}
		
		private protected override void SetIsSelectedInternal(ListViewItem item, bool value)
		{
			item.SetIsSelectedInternal(value);
		}

		private void UpdateData()
		{
			ListViewData = null;

			ItemsPresenter?.ItemsHostInternal?.InvalidateMeasure();

			InvalidateMeasure();
		}

		public string ItemContentStringFormat
		{
			get => (string) GetValue(ItemContentStringFormatProperty);
			set => SetValue(ItemContentStringFormatProperty, value);
		}

		public DataTemplate ItemContentTemplate
		{
			get => (DataTemplate) GetValue(ItemContentTemplateProperty);
			set => SetValue(ItemContentTemplateProperty, value);
		}

		public DataTemplateSelector ItemContentTemplateSelector
		{
			get => (DataTemplateSelector) GetValue(ItemContentTemplateSelectorProperty);
			set => SetValue(ItemContentTemplateSelectorProperty, value);
		}

		int IIndexedFocusNavigatorAdvisor<ListViewItem>.GetIndexFromItem(ListViewItem item)
		{
			return VirtualItemCollection.GetIndexFromItem(item);
		}

		ListViewItem IIndexedFocusNavigatorAdvisor<ListViewItem>.GetItemFromIndex(int index)
		{
			return VirtualItemCollection.GetItemFromIndex(index);
		}
	}

	public class ListViewControlTemplateContract : IndexedSelectorBaseTemplateContract<ListViewItemsPresenter>
	{
	}
}