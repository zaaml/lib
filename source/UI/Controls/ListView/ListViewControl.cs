// <copyright file="ListViewControl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
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
	[TemplateContractType(typeof(ListViewTemplateContract))]
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

		private bool MouseSelect(ListViewItem listViewItem, MouseButtonKind mouseButtonKind, MouseButtonEventKind eventKind)
		{
			if (MouseButtonSelectionHelper.ShouldSelect(mouseButtonKind, eventKind, MouseButtonSelectionOptions) && listViewItem.ActualCanSelect)
			{
				listViewItem.SelectInternal();

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

		internal override void OnItemAttachedInternal(ListViewItem item)
		{
			item.ListView = this;

			base.OnItemAttachedInternal(item);
		}

		internal override void OnItemDetachedInternal(ListViewItem item)
		{
			base.OnItemDetachedInternal(item);

			item.ListView = null;
		}

		internal virtual void OnItemGeneratorChanged(ListViewItemGeneratorBase oldGenerator, ListViewItemGeneratorBase newGenerator)
		{
			Items.Generator = ActualGenerator;
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

			var currentSelectedItem = SelectorController.CurrentSelectedItem;

			if (ReferenceEquals(currentSelectedItem, listViewItem) == false)
				currentSelectedItem?.SetIsSelectedInternal(false);

			SelectorController.SelectedItem = listViewItem;
		}

		private protected override void SetIsSelectedInternal(ListViewItem item, bool value)
		{
			item.SetIsSelectedInternal(value);
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

	public class ListViewTemplateContract : ItemsControlBaseTemplateContract<ListViewItemsPresenter>
	{
	}
}