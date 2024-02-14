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
using Zaaml.PresentationCore.CommandCore;
using Zaaml.PresentationCore.Data;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Core;
using Zaaml.UI.Controls.Interfaces;
using Zaaml.UI.Controls.ListView.Data;
using Zaaml.UI.Controls.Primitives.ContentPrimitives;
using Zaaml.UI.Controls.ScrollView;
using Zaaml.UI.Controls.TreeView;
using ScrollUnit = Zaaml.UI.Controls.ScrollView.ScrollUnit;

namespace Zaaml.UI.Controls.ListView
{
	[TemplateContractType(typeof(ListViewControlTemplateContract))]
	public partial class ListViewControl : IndexedSelectorBase<ListViewControl, ListViewItem, ListViewItemCollection, ListViewItemsPresenter, ListViewItemsPanel>, IIconContentSelectorControl, IIndexedFocusNavigatorAdvisor<ListViewItem>
	{
		public static readonly DependencyProperty ItemGeneratorProperty = DPM.Register<ListViewItemGeneratorBase, ListViewControl>
			("ItemGenerator", l => l.OnItemGeneratorChanged);

		public static readonly DependencyProperty ItemContentTemplateProperty = DPM.Register<DataTemplate, ListViewControl>
			("ItemContentTemplate", l => l.DefaultGeneratorImplementation.OnItemContentTemplateChanged);

		public static readonly DependencyProperty ItemContentTemplateSelectorProperty = DPM.Register<DataTemplateSelector, ListViewControl>
			("ItemContentTemplateSelector", l => l.DefaultGeneratorImplementation.OnItemContentTemplateSelectorChanged);

		public static readonly DependencyProperty ItemContentStringFormatProperty = DPM.Register<string, ListViewControl>
			("ItemContentStringFormat", l => l.DefaultGeneratorImplementation.OnItemContentStringFormatChanged);

		public static readonly DependencyProperty SourceCollectionProperty = DPM.Register<IEnumerable, ListViewControl>
			("SourceCollection", i => i.OnSourceCollectionPropertyChangedPrivate);

		public static readonly DependencyProperty ItemsFilterProperty = DPM.Register<IListViewItemFilter, ListViewControl>
			("ItemsFilter", d => d.OnItemsFilterPropertyChangedPrivate);

		public static readonly DependencyProperty SelectionModeProperty = DPM.Register<ListViewSelectionMode, ListViewControl>
			("SelectionMode", ListViewSelectionMode.Single, l => l.OnSelectionModePropertyChangedPrivate);

		private static readonly DependencyPropertyKey SelectionCollectionPropertyKey = DPM.RegisterReadOnly<ListViewSelectionCollection, ListViewControl>
			("SelectionCollectionPrivate");

		public static readonly DependencyProperty ItemGlyphTemplateProperty = DPM.Register<DataTemplate, ListViewControl>
			("ItemGlyphTemplate");

		public static readonly DependencyProperty ItemContentMemberProperty = DPM.Register<string, ListViewControl>
			("ItemContentMember", l => l.DefaultGeneratorImplementation.OnItemContentMemberChanged);

		public static readonly DependencyProperty ItemIconSelectorProperty = DPM.Register<IIconSelector, ListViewControl>
			("ItemIconSelector", d => d.DefaultGeneratorImplementation.OnItemIconSelectorChanged);

		public static readonly DependencyProperty ItemValueMemberProperty = DPM.Register<string, ListViewControl>
			("ItemValueMember", l => l.OnItemValueMemberPropertyChangedPrivate);

		public static readonly DependencyProperty ItemIconMemberProperty = DPM.Register<string, ListViewControl>
			("ItemIconMember", l => l.DefaultGeneratorImplementation.OnItemIconMemberChanged);

		public static readonly DependencyProperty ItemSelectionMemberProperty = DPM.Register<string, ListViewControl>
			("ItemSelectionMember", l => l.DefaultGeneratorImplementation.SelectableGeneratorImplementation.OnItemSelectionMemberChanged);

		public static readonly DependencyProperty ItemCommandProperty = DPM.Register<ICommand, ListViewControl>
			("ItemCommand");

		public static readonly DependencyProperty ItemCommandParameterSelectorProperty = DPM.Register<ListViewItemCommandParameterSelector, ListViewControl>
			("ItemCommandParameterSelector");

		public static readonly DependencyProperty ItemCommandTargetProperty = DPM.Register<DependencyObject, ListViewControl>
			("ItemCommandTarget");

		public static readonly DependencyProperty ItemGlyphKindProperty = DPM.Register<ListViewGlyphKind, ListViewControl>
			("ItemGlyphKind", ListViewGlyphKind.None);

		public static readonly DependencyProperty ScrollUnitProperty = DPM.Register<ScrollUnit, ListViewControl>
			("ScrollUnit", ScrollUnit.Item, d => d.OnScrollUnitPropertyChangedPrivate);

		public static readonly DependencyProperty ViewProperty = DPM.Register<ListViewBase, ListViewControl>
			("View", d => d.OnViewPropertyChangedPrivate);

		private static readonly DependencyPropertyKey ActualViewTemplatePropertyKey = DPM.RegisterReadOnly<ControlTemplate, ListViewControl>
			("ActualViewTemplate");

		public static readonly DependencyProperty ActualViewTemplateProperty = ActualViewTemplatePropertyKey.DependencyProperty;

		private DefaultItemTemplateListViewItemGenerator _defaultGeneratorImpl;
		private IListViewItemFilter _itemsDefaultFilter;
		private ListViewData _listViewData;

		public event EventHandler<ListViewItemMouseButtonEventArgs> ItemMouseButtonDown;

		public event EventHandler<ListViewItemMouseButtonEventArgs> ItemMouseButtonUp;

		public event EventHandler<ListViewItemMouseButtonEventArgs> ItemMouseDoubleClick;

		public event EventHandler<ListViewItemClickEventArgs> ItemClick;

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
			ItemCommandController = new ItemCommandController<ListViewControl, ListViewItem>(this);

			this.OverrideStyleKey<ListViewControl>();

			ToggleSelectionCommand = new RelayCommand(OnToggleSelectionCommandExecuted, CanExecuteToggleSelectionCommand);
			VirtualItemCollection = new VirtualListViewItemCollection(this);
		}

		protected virtual bool ActualFocusItemOnClick => FocusItemOnClick;

		internal bool ActualFocusItemOnClickInternal => ActualFocusItemOnClick;

		protected virtual bool ActualFocusItemOnMouseHover => FocusItemOnMouseHover;

		protected virtual bool ActualFocusItemOnSelect => FocusItemOnSelect;

		private ListViewItemGeneratorBase ActualGenerator => ItemGenerator ?? DefaultGenerator;

		internal IListViewItemFilter ActualItemsFilter => ItemsFilter ?? ItemsDefaultFilter;

		private protected override bool ActualSelectItemOnFocus => SelectionMode != ListViewSelectionMode.Multiple && base.ActualSelectItemOnFocus;

		public ControlTemplate ActualViewTemplate
		{
			get => (ControlTemplate)GetValue(ActualViewTemplateProperty);
			private set => this.SetReadOnlyValue(ActualViewTemplatePropertyKey, value);
		}

		private ListViewItemGeneratorBase DefaultGenerator => DefaultGeneratorImplementation.Generator;

		private DefaultItemTemplateListViewItemGenerator DefaultGeneratorImplementation => _defaultGeneratorImpl ??= new DefaultItemTemplateListViewItemGenerator(this);

		internal DefaultItemTemplateListViewItemGenerator DefaultGeneratorImplementationInternal => DefaultGeneratorImplementation;

		internal int FocusedIndexInternal => ListViewFocusNavigator.FocusedIndex;

		internal bool FocusItemOnClick { get; set; } = true;

		internal bool FocusItemOnMouseHover { get; set; }

		internal bool FocusItemOnSelect { get; set; } = true;

		private ListGridViewHeadersPresenter GridViewHeadersPresenter => TemplateContract.GridViewHeadersPresenter;

		internal ListGridViewHeadersPresenter GridViewHeadersPresenterInternal => GridViewHeadersPresenter;

		protected override bool HasLogicalOrientation => true;

		private bool IsFocusOnMouseEventLocked { get; set; }

		internal ItemClickMode ItemClickMode { get; set; } = ItemClickMode.DoubleClick;

		internal override IItemCollection<ListViewItem> ItemCollectionOverride => VirtualItemCollection;

		public ICommand ItemCommand
		{
			get => (ICommand)GetValue(ItemCommandProperty);
			set => SetValue(ItemCommandProperty, value);
		}

		internal IItemCommandController<ListViewItem> ItemCommandController { get; }

		public ListViewItemCommandParameterSelector ItemCommandParameterSelector
		{
			get => (ListViewItemCommandParameterSelector)GetValue(ItemCommandParameterSelectorProperty);
			set => SetValue(ItemCommandParameterSelectorProperty, value);
		}

		public DependencyObject ItemCommandTarget
		{
			get => (DependencyObject)GetValue(ItemCommandTargetProperty);
			set => SetValue(ItemCommandTargetProperty, value);
		}

		public ListViewItemGeneratorBase ItemGenerator
		{
			get => (ListViewItemGeneratorBase)GetValue(ItemGeneratorProperty);
			set => SetValue(ItemGeneratorProperty, value);
		}

		public ListViewGlyphKind ItemGlyphKind
		{
			get => (ListViewGlyphKind)GetValue(ItemGlyphKindProperty);
			set => SetValue(ItemGlyphKindProperty, value);
		}

		public DataTemplate ItemGlyphTemplate
		{
			get => (DataTemplate)GetValue(ItemGlyphTemplateProperty);
			set => SetValue(ItemGlyphTemplateProperty, value);
		}

		internal IListViewItemFilter ItemsDefaultFilter
		{
			get => _itemsDefaultFilter;
			set
			{
				if (ReferenceEquals(_itemsDefaultFilter, value))
					return;

				_itemsDefaultFilter = value;

				UpdateFilter();
			}
		}

		public IListViewItemFilter ItemsFilter
		{
			get => (IListViewItemFilter)GetValue(ItemsFilterProperty);
			set => SetValue(ItemsFilterProperty, value);
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

		private ListViewFocusNavigator ListViewFocusNavigator => (ListViewFocusNavigator)FocusNavigator;

		protected override Orientation LogicalOrientation => Orientation.Vertical;

		public ScrollUnit ScrollUnit
		{
			get => (ScrollUnit)GetValue(ScrollUnitProperty);
			set => SetValue(ScrollUnitProperty, value);
		}

		internal ScrollViewControl ScrollViewInternal => ScrollView;

		public ListViewSelectionCollection SelectionCollection => this.GetValueOrCreate(SelectionCollectionPropertyKey, () => new ListViewSelectionCollection((ListViewSelectorController)SelectorController));

		public static DependencyProperty SelectionCollectionProperty => SelectionCollectionPropertyKey.DependencyProperty;

		public ListViewSelectionMode SelectionMode
		{
			get => (ListViewSelectionMode)GetValue(SelectionModeProperty);
			set => SetValue(SelectionModeProperty, value);
		}

		public IEnumerable SourceCollection
		{
			get => (IEnumerable)GetValue(SourceCollectionProperty);
			set => SetValue(SourceCollectionProperty, value);
		}

		private ListViewControlTemplateContract TemplateContract => (ListViewControlTemplateContract)TemplateContractCore;

		public ListViewBase View
		{
			get => (ListViewBase)GetValue(ViewProperty);
			set => SetValue(ViewProperty, value);
		}

		internal VirtualListViewItemCollection VirtualItemCollection { get; }

		internal void AttachLogical(ListViewItem item)
		{
			LogicalChildMentor.AddLogicalChild(item);
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
				Source = new MixedItemSourceCollection<ListViewItem>(ItemCollection, SourceCollection, int.MaxValue),
				DataFilter =
				{
					Filter = ActualItemsFilter
				}
			};
		}

		internal void DetachLogical(ListViewItem item)
		{
			LogicalChildMentor.RemoveLogicalChild(item);
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
			EnsureVirtualItemCollection();

			return base.MeasureOverride(availableSize);
		}

		internal override void OnCollectionChangedInternal(object sender, NotifyCollectionChangedEventArgs args)
		{
			base.OnCollectionChangedInternal(sender, args);

			InvalidatePanelCore();
		}

		internal void OnFilterUpdatedInternal()
		{
			var item = ListViewFocusNavigator.FocusedItemCache ?? SelectedItem ?? ItemCollection.ActualItemsInternal.FirstOrDefault(null);

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

		internal void OnItemAttachedCollection(ListViewItem item)
		{
			item.ListViewControl = this;

			AttachLogical(item);
		}

		internal override void OnItemAttachedInternal(ListViewItem item)
		{
			item.ListViewControl = this;

			base.OnItemAttachedInternal(item);
		}

		internal void OnItemClick(ListViewItem listViewItem)
		{
			ItemClick?.Invoke(this, new ListViewItemClickEventArgs(listViewItem));

			var itemCommand = ItemCommand;
			var itemCommandTarget = ItemCommandTarget;
			var itemCommandParameter = ItemCommandParameterSelector?.SelectCommandParameter(listViewItem) ?? listViewItem.Value ?? listViewItem;

			if (CommandHelper.CanExecute(itemCommand, itemCommandParameter, itemCommandTarget ?? this))
				CommandHelper.Execute(itemCommand, itemCommandParameter, itemCommandTarget ?? this);
		}

		internal void OnItemDetachedCollection(ListViewItem item)
		{
			DetachLogical(item);

			item.ListViewControl = null;
		}

		internal override void OnItemDetachedInternal(ListViewItem item)
		{
			base.OnItemDetachedInternal(item);

			item.ListViewControl = null;
		}

		internal virtual void OnItemGeneratorChanged(ListViewItemGeneratorBase oldGenerator, ListViewItemGeneratorBase newGenerator)
		{
			ItemCollection.Generator = ActualGenerator;

			if (oldGenerator != null)
				oldGenerator.GeneratorChangedCore -= OnGeneratorChanged;

			if (newGenerator != null)
				newGenerator.GeneratorChangedCore += OnGeneratorChanged;

			UpdateData();
		}

		internal void OnItemKeyDown(ListViewItem listViewItem, KeyEventArgs keyEventArgs)
		{
			ItemCommandController.OnItemKeyDown(listViewItem, keyEventArgs);
		}

		internal void OnItemKeyUp(ListViewItem listViewItem, KeyEventArgs keyEventArgs)
		{
			ItemCommandController.OnItemKeyUp(listViewItem, keyEventArgs);
		}

		internal void OnItemMouseButton(ListViewItem listViewItem, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left)
			{
				if (e.ButtonState == MouseButtonState.Pressed)
					ItemCommandController.OnItemMouseLeftButtonDown(listViewItem, e);
				else
					ItemCommandController.OnItemMouseLeftButtonUp(listViewItem, e);
			}

			if (e.ButtonState == MouseButtonState.Released)
				ItemMouseButtonUp?.Invoke(this, new ListViewItemMouseButtonEventArgs(listViewItem, e));
			else
				ItemMouseButtonDown?.Invoke(this, new ListViewItemMouseButtonEventArgs(listViewItem, e));
		}

		public void OnItemMouseDoubleClick(ListViewItem listViewItem, MouseButtonEventArgs e)
		{
			ItemCommandController.OnItemMouseDoubleClick(listViewItem, e);

			ItemMouseDoubleClick?.Invoke(this, new ListViewItemMouseButtonEventArgs(listViewItem, e));
		}

		internal void OnItemMouseEnter(ListViewItem listViewItem, MouseEventArgs e)
		{
			ItemCommandController.OnItemMouseEnter(listViewItem, e);

			if (ActualFocusItemOnMouseHover && IsFocusOnMouseEventLocked == false)
				FocusItem(listViewItem);
		}

		internal void OnItemMouseLeave(ListViewItem listViewItem, MouseEventArgs e)
		{
			ItemCommandController.OnItemMouseLeave(listViewItem, e);
		}

		internal void OnItemMouseMove(ListViewItem listViewItem, MouseEventArgs mouseEventArgs)
		{
			ItemCommandController.OnItemMouseMove(listViewItem, mouseEventArgs);
		}

		internal void OnItemPostClick(ListViewItem listViewItem)
		{
			TryToggleItemSelectionInternal(listViewItem, true);
		}

		internal void TryToggleItemSelectionInternal(ListViewItem listViewItem, bool focus)
		{
			if (listViewItem.ActualCanSelect == false)
				return;

			if (focus)
				FocusItem(listViewItem);

			ToggleItemSelection(listViewItem);
		}

		internal void OnItemPreClick(ListViewItem listViewItem)
		{
		}

		private void OnItemsFilterPropertyChangedPrivate(IListViewItemFilter oldFilter, IListViewItemFilter newFilter)
		{
			if (ReferenceEquals(oldFilter, newFilter))
				return;

			UpdateFilter();
		}

		internal void OnItemValueChanged(ListViewItem listViewItem)
		{
			if (listViewItem.IsSelected)
				SelectorController.SyncValue();
		}

		private void OnItemValueMemberPropertyChangedPrivate(string oldValue, string newValue)
		{
			DefaultGeneratorImplementation.SelectableGeneratorImplementation.OnItemValueMemberChanged(oldValue, newValue);

			try
			{
				SelectedValueEvaluator = new MemberEvaluator(newValue);
			}
			catch (Exception ex)
			{
				LogService.LogError(ex);
			}

			SelectorController.SyncValue();
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);

			if (e.Handled)
				return;

			e.Handled = FocusNavigator.HandleNavigationKey(e.Key);
		}

		protected override void OnLoaded()
		{
			base.OnLoaded();

			EnsureVirtualItemCollection();
		}

		protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
		{
			base.OnLostKeyboardFocus(e);

			ItemCommandController.OnLostKeyboardFocus(e);
		}

		protected override void OnLostMouseCapture(MouseEventArgs e)
		{
			base.OnLostMouseCapture(e);

			ItemCommandController.OnLostMouseCapture(e);
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

		private void OnScrollUnitPropertyChangedPrivate(ScrollUnit oldValue, ScrollUnit newValue)
		{
			InvalidatePanelCore();
		}

		private void OnSelectionModePropertyChangedPrivate()
		{
			SelectorController.MultipleSelection = SelectionMode == ListViewSelectionMode.Multiple;
		}

		private void OnSourceCollectionPropertyChangedPrivate(IEnumerable oldSource, IEnumerable newSource)
		{
			UpdateData();
		}

		protected override void OnTemplateContractAttached()
		{
			base.OnTemplateContractAttached();

			ItemsPresenter.ListViewControl = this;

			if (ScrollView != null)
				ScrollView.PreserveScrollBarVisibility = true;

			if (GridViewHeadersPresenter != null)
			{
				GridViewHeadersPresenter.ListViewControl = this;
				GridViewHeadersPresenter.ScrollViewControl = ScrollView;
			}
		}

		protected override void OnTemplateContractDetaching()
		{
			if (GridViewHeadersPresenter != null)
			{
				GridViewHeadersPresenter.ListViewControl = null;
				GridViewHeadersPresenter.ScrollViewControl = null;
			}

			ItemsPresenter.ListViewControl = null;

			base.OnTemplateContractDetaching();
		}

		private void OnViewPropertyChangedPrivate(ListViewBase oldValue, ListViewBase newValue)
		{
			if (ReferenceEquals(oldValue, newValue))
				return;

			if (oldValue != null)
			{
				if (ReferenceEquals(oldValue.ListViewControl, this) == false)
					throw new InvalidOperationException();

				oldValue.ListViewControl = null;
			}

			if (newValue != null)
			{
				if (newValue.ListViewControl != null)
					throw new InvalidOperationException();

				newValue.ListViewControl = this;
			}

			UpdateViewTemplate();
		}

		internal void RaiseClick(ListViewItem listViewItem)
		{
			ItemCommandController.RaiseOnClick(listViewItem);
		}

		private void UpdateData()
		{
			ListViewData = null;

			ItemsPresenter?.ItemsHostInternal?.InvalidateMeasure();

			InvalidateMeasure();
		}

		private void UpdateFilter()
		{
			if (ListViewData == null)
				return;

			var itemsFilter = ActualItemsFilter;

			if (ReferenceEquals(itemsFilter, ListViewData.DataFilter.Filter))
				return;

			ListViewData.DataFilter.Filter = itemsFilter;

			InvalidateMeasure();
		}

		internal void UpdateViewTemplate()
		{
			var actualViewTemplate = View?.GetTemplateInternal(this);

			if (ReferenceEquals(ActualViewTemplate, actualViewTemplate) == false)
				ActualViewTemplate = actualViewTemplate;
		}

		public string ItemContentMember
		{
			get => (string)GetValue(ItemContentMemberProperty);
			set => SetValue(ItemContentMemberProperty, value);
		}

		public string ItemIconMember
		{
			get => (string)GetValue(ItemIconMemberProperty);
			set => SetValue(ItemIconMemberProperty, value);
		}
		
		public IIconSelector ItemIconSelector
		{
			get => (IIconSelector)GetValue(ItemIconSelectorProperty);
			set => SetValue(ItemIconSelectorProperty, value);
		}

		public string ItemSelectionMember
		{
			get => (string)GetValue(ItemSelectionMemberProperty);
			set => SetValue(ItemSelectionMemberProperty, value);
		}

		public string ItemValueMember
		{
			get => (string)GetValue(ItemValueMemberProperty);
			set => SetValue(ItemValueMemberProperty, value);
		}

		public string ItemContentStringFormat
		{
			get => (string)GetValue(ItemContentStringFormatProperty);
			set => SetValue(ItemContentStringFormatProperty, value);
		}

		public DataTemplate ItemContentTemplate
		{
			get => (DataTemplate)GetValue(ItemContentTemplateProperty);
			set => SetValue(ItemContentTemplateProperty, value);
		}

		public DataTemplateSelector ItemContentTemplateSelector
		{
			get => (DataTemplateSelector)GetValue(ItemContentTemplateSelectorProperty);
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
}