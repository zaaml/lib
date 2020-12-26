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
using Zaaml.PresentationCore.CommandCore;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Core;
using Zaaml.UI.Controls.Interfaces;
using Zaaml.UI.Controls.ScrollView;
using Zaaml.UI.Controls.TreeView.Data;
using Zaaml.UI.Data.Hierarchy;

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

		public static readonly DependencyProperty ItemValueMemberProperty = DPM.Register<string, TreeViewControl>
			("ItemValueMember", d => d.DefaultGeneratorImplementation.SelectableGeneratorImplementation.OnItemValueMemberChanged);

		public static readonly DependencyProperty ItemSelectionMemberProperty = DPM.Register<string, TreeViewControl>
			("ItemSelectionMember", d => d.DefaultGeneratorImplementation.SelectableGeneratorImplementation.OnItemSelectionMemberChanged);

		public static readonly DependencyProperty ItemSourceCollectionMemberProperty = DPM.Register<string, TreeViewControl>
			("ItemSourceCollectionMember", d => d.DefaultGeneratorImplementation.OnItemSourceCollectionMemberChanged);

		public static readonly DependencyProperty ItemCommandProperty = DPM.Register<ICommand, TreeViewControl>
			("ItemCommand");

		public static readonly DependencyProperty ItemCommandTargetProperty = DPM.Register<DependencyObject, TreeViewControl>
			("ItemCommandTarget");

		public static readonly DependencyProperty ItemGlyphKindProperty = DPM.Register<TreeViewGlyphKind, TreeViewControl>
			("ItemGlyphKind");

		private DefaultItemTemplateTreeViewItemGenerator _defaultGeneratorImplementation;

		private ITreeViewItemFilter _itemsDefaultFilter;
		private TreeViewData _treeViewData;

		internal event EventHandler<ValueChangedEventArgs<TreeViewData>> TreeViewDataChanged;

		internal event EventHandler<ValueChangingEventArgs<TreeViewData>> TreeViewDataChanging;

		public event EventHandler<TreeViewItemMouseButtonEventArgs> ItemMouseButtonDown;

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
			this.OverrideStyleKey<TreeViewControl>();

			VirtualItemCollection = new VirtualTreeViewItemCollection(this);
		}

		protected virtual bool ActualFocusItemOnClick => FocusItemOnClick;

		internal bool ActualFocusItemOnClickInternal => ActualFocusItemOnClick;

		protected virtual bool ActualFocusItemOnMouseHover => FocusItemOnMouseHover;

		protected virtual bool ActualFocusItemOnSelect => FocusItemOnSelect;

		internal TreeViewItemGeneratorBase ActualGenerator => ItemGenerator ?? DefaultGenerator;

		private protected override bool ActualSelectItemOnFocus => SelectionMode != TreeViewSelectionMode.Multiple && base.ActualSelectItemOnFocus;

		private TreeViewItemGeneratorBase DefaultGenerator => DefaultGeneratorImplementation.Generator;

		private DefaultItemTemplateTreeViewItemGenerator DefaultGeneratorImplementation => _defaultGeneratorImplementation ??= new DefaultItemTemplateTreeViewItemGenerator(this);

		internal DefaultItemTemplateTreeViewItemGenerator DefaultGeneratorImplementationInternal => DefaultGeneratorImplementation;

		public bool ExpandNodesOnFiltering
		{
			get => (bool) GetValue(ExpandNodesOnFilteringProperty);
			set => SetValue(ExpandNodesOnFilteringProperty, value);
		}

		public TreeViewFilteringMode FilteringMode
		{
			get => (TreeViewFilteringMode) GetValue(FilteringModeProperty);
			set => SetValue(FilteringModeProperty, value);
		}

		internal FilteringStrategy<TreeViewData, TreeViewItemDataCollection, TreeViewItemData> FilteringStrategy { get; private set; } = NodesFilteringStrategy<TreeViewData, TreeViewItemDataCollection, TreeViewItemData>.Instance;

		internal int FocusedIndexInternal => TreeViewFocusNavigator.FocusedIndex;

		internal bool FocusItemOnClick { get; set; } = true;

		internal bool FocusItemOnMouseHover { get; set; }

		internal bool FocusItemOnSelect { get; set; } = true;

		private bool IsFocusOnMouseEventLocked { get; set; }

		internal ClickMode ItemClickMode { get; set; } = ClickMode.Release;

		internal override IItemCollection<TreeViewItem> ItemCollectionOverride => VirtualItemCollection;

		public ICommand ItemCommand
		{
			get => (ICommand) GetValue(ItemCommandProperty);
			set => SetValue(ItemCommandProperty, value);
		}

		public DependencyObject ItemCommandTarget
		{
			get => (DependencyObject) GetValue(ItemCommandTargetProperty);
			set => SetValue(ItemCommandTargetProperty, value);
		}

		public TreeViewItemGeneratorBase ItemGenerator
		{
			get => (TreeViewItemGeneratorBase) GetValue(ItemGeneratorProperty);
			set => SetValue(ItemGeneratorProperty, value);
		}

		public TreeViewGlyphKind ItemGlyphKind
		{
			get => (TreeViewGlyphKind) GetValue(ItemGlyphKindProperty);
			set => SetValue(ItemGlyphKindProperty, value);
		}

		public DataTemplate ItemGlyphTemplate
		{
			get => (DataTemplate) GetValue(ItemGlyphTemplateProperty);
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
			get => (ITreeViewItemFilter) GetValue(ItemsFilterProperty);
			set => SetValue(ItemsFilterProperty, value);
		}

		public string ItemSourceCollectionMember
		{
			get => (string) GetValue(ItemSourceCollectionMemberProperty);
			set => SetValue(ItemSourceCollectionMemberProperty, value);
		}

		internal ScrollViewControl ScrollViewInternal => ScrollView;

		public TreeViewSelectionCollection SelectionCollection => this.GetValueOrCreate(SelectionCollectionPropertyKey, () => new TreeViewSelectionCollection((TreeViewSelectorController) SelectorController));

		public TreeViewSelectionMode SelectionMode
		{
			get => (TreeViewSelectionMode) GetValue(SelectionModeProperty);
			set => SetValue(SelectionModeProperty, value);
		}

		private protected virtual bool ShouldSelectCollapsedItemParent => false;

		public IEnumerable SourceCollection
		{
			get => (IEnumerable) GetValue(SourceCollectionProperty);
			set => SetValue(SourceCollectionProperty, value);
		}

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

		private TreeViewFocusNavigator TreeViewFocusNavigator => (TreeViewFocusNavigator) FocusNavigator;

		internal VirtualTreeViewItemCollection VirtualItemCollection { get; }

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
					Filter = ItemsFilter ?? ItemsDefaultFilter
				}
			};
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

		public void ExpandSelectedBranch()
		{
			var current = SelectedItem?.TreeViewItemData?.Parent;

			while (current != null)
			{
				current.SetIsExpanded(true, ExpandCollapseMode.Default);

				current = current.Parent;
			}
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

			if (CommandHelper.CanExecute(itemCommand, treeViewItem, itemCommandTarget ?? this))
				CommandHelper.Execute(itemCommand, treeViewItem, itemCommandTarget ?? this);
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
			SelectCollapsedItemParent(treeViewItem);

			ItemIsExpandedChanged?.Invoke(this, new TreeViewItemEventArgs(treeViewItem));
		}

		internal void OnItemMouseButton(TreeViewItem treeViewItem, MouseButtonEventArgs e)
		{
			if (e.ButtonState == MouseButtonState.Released)
				ItemMouseButtonUp?.Invoke(this, new TreeViewItemMouseButtonEventArgs(treeViewItem, e));
			else
				ItemMouseButtonDown?.Invoke(this, new TreeViewItemMouseButtonEventArgs(treeViewItem, e));
		}

		internal void OnItemMouseEnter(TreeViewItem treeViewItem, MouseEventArgs e)
		{
			if (ActualFocusItemOnMouseHover && IsFocusOnMouseEventLocked == false)
				FocusItem(treeViewItem);
		}

		internal void OnItemMouseLeave(TreeViewItem treeViewItem, MouseEventArgs e)
		{
		}

		internal void OnItemMouseMove(TreeViewItem treeViewItem, MouseEventArgs mouseEventArgs)
		{
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
		}

		protected override void OnTemplateContractDetaching()
		{
			ItemsPresenter.TreeViewControl = null;

			base.OnTemplateContractDetaching();
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

			var itemsFilter = ItemsFilter ?? ItemsDefaultFilter;

			if (ReferenceEquals(itemsFilter, TreeViewData.DataFilter.Filter))
				return;

			TreeViewData.DataFilter.Filter = itemsFilter;

			InvalidateMeasure();
		}

		public string ItemSelectionMember
		{
			get => (string) GetValue(ItemSelectionMemberProperty);
			set => SetValue(ItemSelectionMemberProperty, value);
		}

		public string ItemValueMember
		{
			get => (string) GetValue(ItemValueMemberProperty);
			set => SetValue(ItemValueMemberProperty, value);
		}

		public string ItemIconMember
		{
			get => (string) GetValue(ItemIconMemberProperty);
			set => SetValue(ItemIconMemberProperty, value);
		}

		public string ItemContentMember
		{
			get => (string) GetValue(ItemContentMemberProperty);
			set => SetValue(ItemContentMemberProperty, value);
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