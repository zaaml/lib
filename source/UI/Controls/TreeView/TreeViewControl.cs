// <copyright file="TreeViewControl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
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
using Zaaml.Core.Runtime;
using Zaaml.PresentationCore.CommandCore;
using Zaaml.PresentationCore.Data;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Core;
using Zaaml.UI.Controls.Interfaces;
using Zaaml.UI.Controls.Primitives.ContentPrimitives;
using Zaaml.UI.Controls.ScrollView;
using Zaaml.UI.Controls.TreeView.Data;
using Zaaml.UI.Data.Hierarchy;
using ScrollUnit = Zaaml.UI.Controls.ScrollView.ScrollUnit;

namespace Zaaml.UI.Controls.TreeView
{
	[TemplateContractType(typeof(TreeViewControlTemplateContract))]
	public class TreeViewControl : SelectorBase<TreeViewControl, TreeViewItem, TreeViewItemRootCollection, TreeViewItemsPresenter, TreeViewItemsPanel>, IIconContentSelectorControl, IIndexedFocusNavigatorAdvisor<TreeViewItem>
	{
		public static readonly DependencyProperty ItemGeneratorProperty = DPM.Register<TreeViewItemGeneratorBase, TreeViewControl>
			("ItemGenerator", l => l.OnItemGeneratorChanged);

		public static readonly DependencyProperty ItemContentTemplateProperty = DPM.Register<DataTemplate, TreeViewControl>
			("ItemContentTemplate", l => l.DefaultGeneratorImplementation.OnItemContentTemplateChanged);

		public static readonly DependencyProperty ItemContentTemplateSelectorProperty = DPM.Register<DataTemplateSelector, TreeViewControl>
			("ItemContentTemplateSelector", l => l.DefaultGeneratorImplementation.OnItemContentTemplateSelectorChanged);

		public static readonly DependencyProperty ItemContentStringFormatProperty = DPM.Register<string, TreeViewControl>
			("ItemContentStringFormat", l => l.DefaultGeneratorImplementation.OnItemContentStringFormatChanged);

		public static readonly DependencyProperty SourceCollectionProperty = DPM.Register<IEnumerable, TreeViewControl>
			("SourceCollection", i => i.OnSourceCollectionPropertyChangedPrivate);

		//TODO Make FilterTextBox to implement ITreeViewFilter. 
		public static readonly DependencyProperty ItemsFilterProperty = DPM.Register<ITreeViewItemFilter, TreeViewControl>
			("ItemsFilter", i => i.OnItemsFilterChangedPrivate);

		public static readonly DependencyProperty ExpandNodesOnFilteringProperty = DPM.Register<bool, TreeViewControl>
			("ExpandNodesOnFiltering", true, i => i.OnExpandNodesOnFilteringChangedPrivate);

		public static readonly DependencyProperty FilteringModeProperty = DPM.Register<TreeViewFilteringMode, TreeViewControl>
			("FilteringMode", TreeViewFilteringMode.Nodes, i => i.OnFilteringModeChangedPrivate);

		public static readonly DependencyProperty SelectionModeProperty = DPM.Register<TreeViewSelectionMode, TreeViewControl>
			("SelectionMode", TreeViewSelectionMode.Single, l => l.OnSelectionModePropertyChangedPrivate);

		private static readonly DependencyPropertyKey SelectionCollectionPropertyKey = DPM.RegisterReadOnly<TreeViewSelectionCollection, TreeViewControl>
			("SelectionCollectionPrivate");

		public static readonly DependencyProperty ItemGlyphTemplateProperty = DPM.Register<DataTemplate, TreeViewControl>
			("ItemGlyphTemplate");

		public static readonly DependencyProperty ItemContentMemberProperty = DPM.Register<string, TreeViewControl>
			("ItemContentMember", d => d.DefaultGeneratorImplementation.OnItemContentMemberChanged);

		public static readonly DependencyProperty ItemIconMemberProperty = DPM.Register<string, TreeViewControl>
			("ItemIconMember", d => d.DefaultGeneratorImplementation.OnItemIconMemberChanged);

		public static readonly DependencyProperty ItemIconSelectorProperty = DPM.Register<IIconSelector, TreeViewControl>
			("ItemIconSelector", d => d.DefaultGeneratorImplementation.OnItemIconSelectorChanged);

		public static readonly DependencyProperty ItemValueMemberProperty = DPM.Register<string, TreeViewControl>
			("ItemValueMember", d => d.OnItemValueMemberPropertyChangedPrivate);

		public static readonly DependencyProperty ItemSelectionMemberProperty = DPM.Register<string, TreeViewControl>
			("ItemSelectionMember", d => d.DefaultGeneratorImplementation.SelectableGeneratorImplementation.OnItemSelectionMemberChanged);

		public static readonly DependencyProperty ItemSourceCollectionMemberProperty = DPM.Register<string, TreeViewControl>
			("ItemSourceCollectionMember", d => d.DefaultGeneratorImplementation.OnItemSourceCollectionMemberChanged);

		public static readonly DependencyProperty ItemCommandProperty = DPM.Register<ICommand, TreeViewControl>
			("ItemCommand");

		public static readonly DependencyProperty ItemCommandParameterSelectorProperty = DPM.Register<TreeViewItemCommandParameterSelector, TreeViewControl>
			("ItemCommandParameterSelector");

		public static readonly DependencyProperty ItemCommandTargetProperty = DPM.Register<DependencyObject, TreeViewControl>
			("ItemCommandTarget");

		public static readonly DependencyProperty ItemGlyphKindProperty = DPM.Register<TreeViewGlyphKind, TreeViewControl>
			("ItemGlyphKind");

		public static readonly DependencyProperty ScrollUnitProperty = DPM.Register<ScrollUnit, TreeViewControl>
			("ScrollUnit", ScrollUnit.Item, d => d.OnScrollUnitPropertyChangedPrivate);

		public static readonly DependencyProperty ViewProperty = DPM.Register<TreeViewBase, TreeViewControl>
			("View", d => d.OnViewPropertyChangedPrivate);

		private static readonly DependencyPropertyKey ActualViewTemplatePropertyKey = DPM.RegisterReadOnly<ControlTemplate, TreeViewControl>
			("ActualViewTemplate");

		public static readonly DependencyProperty LevelIndentSizeProperty = DPM.Register<double, TreeViewControl>
			("LevelIndentSize", 20);

		public static readonly DependencyProperty ActualViewTemplateProperty = ActualViewTemplatePropertyKey.DependencyProperty;

		private TreeViewItem _currentItem;
		private DefaultItemTemplateTreeViewItemGenerator _defaultGeneratorImplementation;
		private ITreeViewItemFilter _itemsDefaultFilter;
		private TreeViewData _treeViewData;

		internal event EventHandler<ValueChangedEventArgs<TreeViewData>> TreeViewDataChanged;

		internal event EventHandler<ValueChangingEventArgs<TreeViewData>> TreeViewDataChanging;

		public event EventHandler<TreeViewItemMouseButtonEventArgs> ItemMouseButtonDown;

		public event EventHandler<TreeViewItemMouseButtonEventArgs> ItemMouseDoubleClick;

		public event EventHandler<TreeViewItemMouseButtonEventArgs> ItemMouseButtonUp;

		public event EventHandler<TreeViewItemEventArgs> ItemIsExpandedChanged;

		public event EventHandler<TreeViewItemClickEventArgs> ItemClick;

		static TreeViewControl()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<TreeViewControl>();

			KeyboardNavigation.TabNavigationProperty.OverrideMetadata(typeof(TreeViewControl), new FrameworkPropertyMetadata(KeyboardNavigationMode.Once));
			KeyboardNavigation.ControlTabNavigationProperty.OverrideMetadata(typeof(TreeViewControl), new FrameworkPropertyMetadata(KeyboardNavigationMode.Continue));
			KeyboardNavigation.DirectionalNavigationProperty.OverrideMetadata(typeof(TreeViewControl), new FrameworkPropertyMetadata(KeyboardNavigationMode.Contained));
		}

		public TreeViewControl()
		{
			ItemCommandController = new ItemCommandController<TreeViewControl, TreeViewItem>(this);

			this.OverrideStyleKey<TreeViewControl>();

			VirtualItemCollection = new VirtualTreeViewItemCollection(this);
		}

		protected virtual bool ActualFocusItemOnClick => FocusItemOnClick;

		internal bool ActualFocusItemOnClickInternal => ActualFocusItemOnClick;

		protected virtual bool ActualFocusItemOnMouseHover => FocusItemOnMouseHover;

		protected virtual bool ActualFocusItemOnSelect => FocusItemOnSelect;

		internal TreeViewItemGeneratorBase ActualGenerator => ItemGenerator ?? DefaultGenerator;

		internal ITreeViewItemFilter ActualItemsFilter => ItemsFilter ?? ItemsDefaultFilter;

		private protected override bool ActualSelectItemOnFocus => SelectionMode != TreeViewSelectionMode.Multiple && base.ActualSelectItemOnFocus;

		public ControlTemplate ActualViewTemplate
		{
			get => (ControlTemplate)GetValue(ActualViewTemplateProperty);
			private set => this.SetReadOnlyValue(ActualViewTemplatePropertyKey, value);
		}

		private TreeGridViewHeadersPresenter ColumnHeadersPresenter => TemplateContract.ColumnHeadersPresenter;

		internal TreeGridViewHeadersPresenter ColumnHeadersPresenterInternal => ColumnHeadersPresenter;

		private TreeViewItem CurrentItem
		{
			set
			{
				if (ReferenceEquals(_currentItem, value))
					return;

				if (_currentItem != null)
					ItemCollection.UnlockItemInternal(_currentItem);

				_currentItem = value;

				if (_currentItem != null)
					ItemCollection.LockItemInternal(_currentItem);
			}
		}

		private TreeViewItemGeneratorBase DefaultGenerator => DefaultGeneratorImplementation.Generator;

		private DefaultItemTemplateTreeViewItemGenerator DefaultGeneratorImplementation => _defaultGeneratorImplementation ??= new DefaultItemTemplateTreeViewItemGenerator(this);

		internal DefaultItemTemplateTreeViewItemGenerator DefaultGeneratorImplementationInternal => DefaultGeneratorImplementation;

		public bool ExpandNodesOnFiltering
		{
			get => (bool)GetValue(ExpandNodesOnFilteringProperty);
			set => SetValue(ExpandNodesOnFilteringProperty, value.Box());
		}

		public TreeViewFilteringMode FilteringMode
		{
			get => (TreeViewFilteringMode)GetValue(FilteringModeProperty);
			set => SetValue(FilteringModeProperty, value);
		}

		internal FilteringStrategy<TreeViewData, TreeViewItemDataCollection, TreeViewItemData> FilteringStrategy { get; private set; } = NodesFilteringStrategy<TreeViewData, TreeViewItemDataCollection, TreeViewItemData>.Instance;

		internal int FocusedIndexInternal => TreeViewFocusNavigator.FocusedIndex;

		internal bool FocusItemOnClick { get; set; } = true;

		internal bool FocusItemOnMouseHover { get; set; }

		internal bool FocusItemOnSelect { get; set; } = true;

		protected override bool HasLogicalOrientation => true;

		private bool IsFocusOnMouseEventLocked { get; set; }

		internal ItemClickMode ItemClickMode { get; set; } = ItemClickMode.DoubleClick;

		internal override IItemCollection<TreeViewItem> ItemCollectionOverride => VirtualItemCollection;

		public ICommand ItemCommand
		{
			get => (ICommand)GetValue(ItemCommandProperty);
			set => SetValue(ItemCommandProperty, value);
		}

		internal IItemCommandController<TreeViewItem> ItemCommandController { get; }

		public TreeViewItemCommandParameterSelector ItemCommandParameterSelector
		{
			get => (TreeViewItemCommandParameterSelector)GetValue(ItemCommandParameterSelectorProperty);
			set => SetValue(ItemCommandParameterSelectorProperty, value);
		}

		public DependencyObject ItemCommandTarget
		{
			get => (DependencyObject)GetValue(ItemCommandTargetProperty);
			set => SetValue(ItemCommandTargetProperty, value);
		}

		public TreeViewItemGeneratorBase ItemGenerator
		{
			get => (TreeViewItemGeneratorBase)GetValue(ItemGeneratorProperty);
			set => SetValue(ItemGeneratorProperty, value);
		}

		public TreeViewGlyphKind ItemGlyphKind
		{
			get => (TreeViewGlyphKind)GetValue(ItemGlyphKindProperty);
			set => SetValue(ItemGlyphKindProperty, value);
		}

		public DataTemplate ItemGlyphTemplate
		{
			get => (DataTemplate)GetValue(ItemGlyphTemplateProperty);
			set => SetValue(ItemGlyphTemplateProperty, value);
		}

		internal ITreeViewItemFilter ItemsDefaultFilter
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

		public ITreeViewItemFilter ItemsFilter
		{
			get => (ITreeViewItemFilter)GetValue(ItemsFilterProperty);
			set => SetValue(ItemsFilterProperty, value);
		}

		public string ItemSourceCollectionMember
		{
			get => (string)GetValue(ItemSourceCollectionMemberProperty);
			set => SetValue(ItemSourceCollectionMemberProperty, value);
		}

		public double LevelIndentSize
		{
			get => (double)GetValue(LevelIndentSizeProperty);
			set => SetValue(LevelIndentSizeProperty, value);
		}

		protected override Orientation LogicalOrientation => Orientation.Vertical;

		public ScrollUnit ScrollUnit
		{
			get => (ScrollUnit)GetValue(ScrollUnitProperty);
			set => SetValue(ScrollUnitProperty, value);
		}

		internal ScrollViewControl ScrollViewInternal => ScrollView;

		public TreeViewSelectionCollection SelectionCollection => this.GetValueOrCreate(SelectionCollectionPropertyKey, () => new TreeViewSelectionCollection((TreeViewSelectorController)SelectorController));

		public TreeViewSelectionMode SelectionMode
		{
			get => (TreeViewSelectionMode)GetValue(SelectionModeProperty);
			set => SetValue(SelectionModeProperty, value);
		}

		private protected virtual bool ShouldSelectCollapsedItemParent => false;

		public IEnumerable SourceCollection
		{
			get => (IEnumerable)GetValue(SourceCollectionProperty);
			set => SetValue(SourceCollectionProperty, value);
		}

		private TreeViewControlTemplateContract TemplateContract => (TreeViewControlTemplateContract)TemplateContractCore;

		internal TreeViewData TreeViewData
		{
			get => _treeViewData;
			private set
			{
				if (ReferenceEquals(_treeViewData, value))
					return;

				var oldTree = _treeViewData;

				TreeViewDataChanging?.Invoke(this, new ValueChangingEventArgs<TreeViewData>(_treeViewData, value));

				_treeViewData = _treeViewData.DisposeExchange(value);

				TreeViewDataChanged?.Invoke(this, new ValueChangedEventArgs<TreeViewData>(oldTree, value));
			}
		}

		private TreeViewFocusNavigator TreeViewFocusNavigator => (TreeViewFocusNavigator)FocusNavigator;

		public TreeViewBase View
		{
			get => (TreeViewBase)GetValue(ViewProperty);
			set => SetValue(ViewProperty, value);
		}

		internal VirtualTreeViewItemCollection VirtualItemCollection { get; }

		private protected override void BringItemIntoView(TreeViewItem item, bool updateLayout)
		{
			ExpandBranchInternal(item);

			base.BringItemIntoView(item, updateLayout);
		}

		protected virtual bool CanSelectItem(TreeViewItem treeViewItem)
		{
			return true;
		}

		internal bool CanSelectItemInternal(TreeViewItem treeViewItem)
		{
			return CanSelectItem(treeViewItem);
		}

		public void CollapseAll()
		{
			TreeViewData?.CollapseAll(ExpandCollapseMode.Auto);
		}

		internal void CollapseAll(ExpandCollapseMode mode)
		{
			TreeViewData?.CollapseAll(mode);
		}

		internal override FocusNavigator<TreeViewItem> CreateFocusNavigator()
		{
			return new TreeViewFocusNavigator(this);
		}

		protected override TreeViewItemRootCollection CreateItemCollection()
		{
			return new TreeViewItemRootCollection(this)
			{
				Generator = ActualGenerator
			};
		}

		internal override SelectorController<TreeViewControl, TreeViewItem> CreateSelectorController()
		{
			return new TreeViewSelectorController(this);
		}

		private TreeViewData CreateTreeViewData()
		{
			return new TreeViewData(this, new TreeViewControlAdvisor(ActualGenerator))
			{
				Source = SourceCollection ?? ItemCollection,
				DataFilter =
				{
					Filter = ActualItemsFilter
				}
			};
		}

		private protected override void EnqueueBringItemIntoView(TreeViewItem item)
		{
			ExpandBranchInternal(item);

			base.EnqueueBringItemIntoView(item);
		}

		internal TreeViewData EnsureTreeViewData()
		{
			return TreeViewData ??= CreateTreeViewData();
		}

		internal void EnsureVirtualItemCollection()
		{
			TreeViewData ??= CreateTreeViewData();

			VirtualItemCollection.TreeViewData = TreeViewData;
			VirtualItemCollection.Generator = ActualGenerator;
		}

		public void ExpandAll()
		{
			TreeViewData?.ExpandAll(ExpandCollapseMode.Auto);
		}

		internal void ExpandAll(ExpandCollapseMode mode)
		{
			TreeViewData?.ExpandAll(mode);
		}

		public void ExpandBranch(TreeViewItem treeViewItem)
		{
			if (ReferenceEquals(treeViewItem.TreeViewControl, this) == false)
				throw new InvalidOperationException("Item belongs to another TreeViewControl.");

			ExpandBranchInternal(treeViewItem);
		}

		private protected static void ExpandBranchInternal(TreeViewItem treeViewItem)
		{
			var current = treeViewItem?.TreeViewItemData?.Parent;

			while (current != null)
			{
				current.SetIsExpanded(true, ExpandCollapseMode.Default);

				current = current.Parent;
			}
		}

		public void ExpandSelectedBranch()
		{
			ExpandBranchInternal(SelectedItem);
		}

		protected override bool GetIsSelected(TreeViewItem item)
		{
			return item.IsSelected;
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

		private void OnExpandNodesOnFilteringChangedPrivate()
		{
		}

		private void OnFilteringModeChangedPrivate()
		{
			FilteringStrategy = FilteringMode switch
			{
				TreeViewFilteringMode.Nodes => NodesFilteringStrategy<TreeViewData, TreeViewItemDataCollection, TreeViewItemData>.Instance,
				TreeViewFilteringMode.ParentBranch => ParentBranchFilteringStrategy<TreeViewData, TreeViewItemDataCollection, TreeViewItemData>.Instance,
				TreeViewFilteringMode.EntireBranch => EntireBranchFilteringStrategy<TreeViewData, TreeViewItemDataCollection, TreeViewItemData>.Instance,
				TreeViewFilteringMode.Recursive => RecursiveFilteringStrategy<TreeViewData, TreeViewItemDataCollection, TreeViewItemData>.Instance,
				_ => throw new ArgumentOutOfRangeException()
			};

			RefreshFilterPrivate();
		}

		internal void OnFilterUpdatedInternal()
		{
			var item = TreeViewFocusNavigator.FocusedItemCache ?? SelectedItem ?? ItemCollection.ActualItemsInternal.FirstOrDefault(null);

			BringItemIntoView(item, true);

			TreeViewFocusNavigator.FocusedIndex = item != null ? GetIndexFromItem(item) : -1;
		}

		internal void OnFilterUpdatingInternal()
		{
		}

		private void OnGeneratorChanged(object sender, EventArgs e)
		{
			UpdateData();
		}

		internal override void OnItemAttachedInternal(TreeViewItem item)
		{
			item.TreeViewControl = this;

			base.OnItemAttachedInternal(item);
		}

		internal void OnItemClick(TreeViewItem treeViewItem)
		{
			ItemClick?.Invoke(this, new TreeViewItemClickEventArgs(treeViewItem));

			var itemCommand = ItemCommand;
			var itemCommandTarget = ItemCommandTarget;
			var itemCommandParameter = ItemCommandParameterSelector?.SelectCommandParameter(treeViewItem) ?? treeViewItem.Value ?? treeViewItem;

			if (CommandHelper.CanExecute(itemCommand, itemCommandParameter, itemCommandTarget ?? this))
				CommandHelper.Execute(itemCommand, itemCommandParameter, itemCommandTarget ?? this);
		}

		internal override void OnItemDetachedInternal(TreeViewItem item)
		{
			base.OnItemDetachedInternal(item);

			item.TreeViewControl = null;
		}

		internal virtual void OnItemGeneratorChanged(TreeViewItemGeneratorBase oldGenerator, TreeViewItemGeneratorBase newGenerator)
		{
			ItemCollection.Generator = ActualGenerator;

			if (oldGenerator != null)
				oldGenerator.GeneratorChangedCore -= OnGeneratorChanged;

			if (newGenerator != null)
				newGenerator.GeneratorChangedCore += OnGeneratorChanged;

			UpdateData();
		}

		internal void OnItemIsExpandedChangedInternal(TreeViewItem treeViewItem)
		{
			CurrentItem = treeViewItem;

			SelectCollapsedItemParent(treeViewItem);

			ItemIsExpandedChanged?.Invoke(this, new TreeViewItemEventArgs(treeViewItem));
		}

		internal void OnItemKeyDown(TreeViewItem treeViewItem, KeyEventArgs keyEventArgs)
		{
			ItemCommandController.OnItemKeyDown(treeViewItem, keyEventArgs);
		}

		internal void OnItemKeyUp(TreeViewItem treeViewItem, KeyEventArgs keyEventArgs)
		{
			ItemCommandController.OnItemKeyUp(treeViewItem, keyEventArgs);
		}

		internal void OnItemMouseButton(TreeViewItem treeViewItem, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left)
			{
				if (e.ButtonState == MouseButtonState.Pressed)
					ItemCommandController.OnItemMouseLeftButtonDown(treeViewItem, e);
				else
					ItemCommandController.OnItemMouseLeftButtonUp(treeViewItem, e);
			}

			if (e.ButtonState == MouseButtonState.Released)
				ItemMouseButtonUp?.Invoke(this, new TreeViewItemMouseButtonEventArgs(treeViewItem, e));
			else
				ItemMouseButtonDown?.Invoke(this, new TreeViewItemMouseButtonEventArgs(treeViewItem, e));
		}

		public void OnItemMouseDoubleClick(TreeViewItem treeViewItem, MouseButtonEventArgs e)
		{
			ItemCommandController.OnItemMouseDoubleClick(treeViewItem, e);

			ItemMouseDoubleClick?.Invoke(this, new TreeViewItemMouseButtonEventArgs(treeViewItem, e));
		}

		internal void OnItemMouseEnter(TreeViewItem treeViewItem, MouseEventArgs e)
		{
			ItemCommandController.OnItemMouseEnter(treeViewItem, e);

			if (ActualFocusItemOnMouseHover && IsFocusOnMouseEventLocked == false)
				FocusItem(treeViewItem);
		}

		internal void OnItemMouseLeave(TreeViewItem treeViewItem, MouseEventArgs e)
		{
			ItemCommandController.OnItemMouseLeave(treeViewItem, e);
		}

		internal void OnItemMouseMove(TreeViewItem treeViewItem, MouseEventArgs mouseEventArgs)
		{
			ItemCommandController.OnItemMouseMove(treeViewItem, mouseEventArgs);
		}

		internal void OnItemPostClick(TreeViewItem treeViewItem)
		{
			if (treeViewItem.ActualCanSelect == false)
				return;

			FocusItem(treeViewItem);
			ToggleItemSelection(treeViewItem);
		}

		internal void OnItemPreClick(TreeViewItem listViewItem)
		{
		}

		private void OnItemsFilterChangedPrivate(ITreeViewItemFilter oldFilter, ITreeViewItemFilter newFilter)
		{
			if (ReferenceEquals(oldFilter, newFilter))
				return;

			UpdateFilter();
		}

		internal void OnItemValueChanged(TreeViewItem treeViewItem)
		{
			if (treeViewItem.IsSelected)
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

		protected override void OnSelectionChanged(Selection<TreeViewItem> oldSelection, Selection<TreeViewItem> newSelection)
		{
			base.OnSelectionChanged(oldSelection, newSelection);

			oldSelection.Item?.SetIsSelectedInternal(false);
			newSelection.Item?.SetIsSelectedInternal(true);
		}

		private void OnSelectionModePropertyChangedPrivate()
		{
			SelectorController.MultipleSelection = SelectionMode == TreeViewSelectionMode.Multiple;
		}

		private void OnSourceCollectionPropertyChangedPrivate(IEnumerable oldSource, IEnumerable newSource)
		{
			UpdateData();
		}

		protected override void OnTemplateContractAttached()
		{
			base.OnTemplateContractAttached();

			ItemsPresenter.TreeViewControl = this;

			if (ScrollView != null)
				ScrollView.PreserveScrollBarVisibility = true;

			if (ColumnHeadersPresenter != null)
			{
				ColumnHeadersPresenter.TreeViewControl = this;
				ColumnHeadersPresenter.ScrollViewControl = ScrollView;
			}
		}

		protected override void OnTemplateContractDetaching()
		{
			if (ColumnHeadersPresenter != null)
			{
				ColumnHeadersPresenter.TreeViewControl = this;
				ColumnHeadersPresenter.ScrollViewControl = null;
			}

			ItemsPresenter.TreeViewControl = null;

			base.OnTemplateContractDetaching();
		}

		private void OnViewPropertyChangedPrivate(TreeViewBase oldValue, TreeViewBase newValue)
		{
			if (ReferenceEquals(oldValue, newValue))
				return;

			if (oldValue != null)
			{
				if (ReferenceEquals(oldValue.TreeViewControl, this) == false)
					throw new InvalidOperationException();

				oldValue.TreeViewControl = null;
			}

			if (newValue != null)
			{
				if (newValue.TreeViewControl != null)
					throw new InvalidOperationException();

				newValue.TreeViewControl = this;
			}

			UpdateViewTemplate();
		}

		internal void RaiseClick(TreeViewItem treeViewItem)
		{
			ItemCommandController.RaiseOnClick(treeViewItem);
		}

		public void RefreshFilter()
		{
			RefreshFilterPrivate();
		}

		private void RefreshFilterPrivate()
		{
			TreeViewData?.RefreshFilter();
		}

		internal void Select(TreeViewItem treeViewItem)
		{
			if (ActualFocusItemOnSelect)
				FocusItem(treeViewItem);

			SelectorController.SelectItem(treeViewItem);
		}

		private protected void SelectCollapsedItemParent(TreeViewItem treeViewItem)
		{
			if (ShouldSelectCollapsedItemParent == false)
				return;

			if (treeViewItem.IsExpanded)
				return;

			var treeNode = treeViewItem.TreeViewItemData;
			var selectedTreeNode = SelectedItem?.TreeViewItemData;

			if (treeNode == null || selectedTreeNode == null || treeNode.IsAncestorOf(selectedTreeNode) == false)
				return;

			SelectorController.SelectItem(treeViewItem);
		}

		private void SelectTreeNode(TreeViewItemData treeViewItemData)
		{
			var data = treeViewItemData.Data;

			if (data is TreeViewItem treeViewItem)
				SelectorController.SelectItem(treeViewItem);
			else
				SelectorController.SelectSource(data);
		}

		protected override void SetIsSelected(TreeViewItem item, bool value)
		{
			item.SetIsSelectedInternal(value);
		}

		private void ToggleItemSelection(TreeViewItem treeViewItem)
		{
			if (treeViewItem.IsSelected == false)
				treeViewItem.SelectInternal();
			else if (SelectionMode == TreeViewSelectionMode.Multiple)
				treeViewItem.UnselectInternal();
		}

		internal void Unselect(TreeViewItem treeViewItem)
		{
			SelectorController.UnselectItem(treeViewItem);
		}

		private void UpdateData()
		{
			TreeViewData = null;

			ItemsPresenter?.ItemsHostInternal?.InvalidateMeasure();

			InvalidateMeasure();
		}

		private void UpdateFilter()
		{
			if (TreeViewData == null)
				return;

			var itemsFilter = ActualItemsFilter;

			if (ReferenceEquals(itemsFilter, TreeViewData.DataFilter.Filter))
				return;

			TreeViewData.DataFilter.Filter = itemsFilter;

			InvalidateMeasure();
		}

		internal void UpdateViewTemplate()
		{
			var actualViewTemplate = View?.GetTemplateInternal(this);

			if (ReferenceEquals(ActualViewTemplate, actualViewTemplate) == false)
				ActualViewTemplate = actualViewTemplate;
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

		public string ItemContentMember
		{
			get => (string)GetValue(ItemContentMemberProperty);
			set => SetValue(ItemContentMemberProperty, value);
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

		int IFocusNavigatorAdvisor<TreeViewItem>.ItemsCount => TreeViewData?.VisibleFlatCount ?? 0;

		int IIndexedFocusNavigatorAdvisor<TreeViewItem>.GetIndexFromItem(TreeViewItem item)
		{
			return VirtualItemCollection.GetIndexFromItem(item);
		}

		TreeViewItem IIndexedFocusNavigatorAdvisor<TreeViewItem>.GetItemFromIndex(int index)
		{
			return VirtualItemCollection.GetItemFromIndex(index);
		}
	}
}