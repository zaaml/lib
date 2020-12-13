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
using Zaaml.PresentationCore.Input;
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
	public class TreeViewControl : SelectorBase<TreeViewControl, TreeViewItem, TreeViewItemRootCollection, TreeViewItemsPresenter, TreeViewPanel>, IContentItemsControl, IIndexedFocusNavigatorAdvisor<TreeViewItem>
	{
		public static readonly DependencyProperty ItemGeneratorProperty = DPM.Register<TreeViewItemGeneratorBase, TreeViewControl>
			("ItemGenerator", l => l.OnItemGeneratorChanged);

		public static readonly DependencyProperty ItemContentTemplateProperty = DPM.Register<DataTemplate, TreeViewControl>
			("ItemContentTemplate", l => l.DefaultGeneratorImpl.OnItemContentTemplateChanged);

		public static readonly DependencyProperty ItemContentTemplateSelectorProperty = DPM.Register<DataTemplateSelector, TreeViewControl>
			("ItemContentTemplateSelector", l => l.DefaultGeneratorImpl.OnItemContentTemplateSelectorChanged);

		public static readonly DependencyProperty ItemContentStringFormatProperty = DPM.Register<string, TreeViewControl>
			("ItemContentStringFormat", l => l.DefaultGeneratorImpl.OnItemContentStringFormatChanged);

		public static readonly DependencyProperty ItemsSourceProperty = DPM.Register<IEnumerable, TreeViewControl>
			("ItemsSource", i => i.OnItemsSourceChangedPrivate);

		//TODO Make FilterTextBox to implement ITreeViewFilter. 
		public static readonly DependencyProperty ItemsFilterProperty = DPM.Register<ITreeViewItemFilter, TreeViewControl>
			("ItemsFilter", i => i.OnItemsFilterChangedPrivate);

		public static readonly DependencyProperty ExpandNodesOnFilteringProperty = DPM.Register<bool, TreeViewControl>
			("ExpandNodesOnFiltering", true, i => i.OnExpandNodesOnFilteringChangedPrivate);

		public static readonly DependencyProperty FilteringModeProperty = DPM.Register<TreeViewFilteringMode, TreeViewControl>
			("FilteringMode", TreeViewFilteringMode.Nodes, i => i.OnFilteringModeChangedPrivate);

		private DelegateContentItemGeneratorImpl<TreeViewItem, DefaultTreeViewItemGenerator> _defaultGeneratorImpl;
		private TreeViewData _treeViewData;

		internal event EventHandler<ValueChangedEventArgs<TreeViewData>> TreeViewDataChanged;

		internal event EventHandler<ValueChangingEventArgs<TreeViewData>> TreeViewDataChanging;

		public event EventHandler<TreeViewItemMouseButtonEventArgs> ItemMouseButtonDown;

		public event EventHandler<TreeViewItemMouseButtonEventArgs> ItemMouseButtonUp;

		public event EventHandler<TreeViewItemEventEventArgs> ItemIsExpandedChanged;

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

		protected virtual bool ActualFocusItemOnMouseHover => FocusItemOnMouseHover;

		protected virtual bool ActualFocusItemOnSelect => FocusItemOnSelect;

		internal TreeViewItemGeneratorBase ActualGenerator => ItemGenerator ?? DefaultGenerator;

		private TreeViewItemGeneratorBase DefaultGenerator => DefaultGeneratorImpl.Generator;

		private DelegateContentItemGeneratorImpl<TreeViewItem, DefaultTreeViewItemGenerator> DefaultGeneratorImpl => _defaultGeneratorImpl ??= new DelegateContentItemGeneratorImpl<TreeViewItem, DefaultTreeViewItemGenerator>(this);

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

		internal bool FocusItemOnMouseHover { get; set; }

		internal bool FocusItemOnSelect { get; set; } = true;

		private bool IsFocusOnMouseEventLocked { get; set; }

		public TreeViewItemGeneratorBase ItemGenerator
		{
			get => (TreeViewItemGeneratorBase) GetValue(ItemGeneratorProperty);
			set => SetValue(ItemGeneratorProperty, value);
		}

		public ITreeViewItemFilter ItemsFilter
		{
			get => (ITreeViewItemFilter) GetValue(ItemsFilterProperty);
			set => SetValue(ItemsFilterProperty, value);
		}

		internal override IItemCollection<TreeViewItem> ItemsProxy => VirtualItemCollection;

		public IEnumerable ItemsSource
		{
			get => (IEnumerable) GetValue(ItemsSourceProperty);
			set => SetValue(ItemsSourceProperty, value);
		}

		internal MouseButtonSelectionOptions MouseButtonSelectionOptions { get; set; } = MouseButtonSelectionOptions.LeftButtonDown;

		internal ScrollViewControl ScrollViewInternal => ScrollView;

		private protected virtual bool ShouldSelectCollapsedItemParent => false;

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
				Source = ItemsSource ?? Items,
				DataFilter =
				{
					Filter = ItemsFilter
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

		protected override Size MeasureOverride(Size availableSize)
		{
			EnsureVirtualItemCollection();

			return base.MeasureOverride(availableSize);
		}

		private bool MouseSelect(TreeViewItem treeViewItem, MouseButtonKind mouseButtonKind, MouseButtonEventKind eventKind)
		{
			if (MouseButtonSelectionHelper.ShouldSelect(mouseButtonKind, eventKind, MouseButtonSelectionOptions) && treeViewItem.ActualCanSelect)
			{
				treeViewItem.SelectInternal();

				return true;
			}

			return false;
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
			var item = TreeViewFocusNavigator.FocusedItemCache ?? SelectedItem ?? Items.ActualItemsInternal.FirstOrDefault(null);

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

		internal override void OnItemDetachedInternal(TreeViewItem item)
		{
			base.OnItemDetachedInternal(item);

			item.TreeViewControl = null;
		}

		internal virtual void OnItemGeneratorChanged(TreeViewItemGeneratorBase oldGenerator, TreeViewItemGeneratorBase newGenerator)
		{
			Items.Generator = ActualGenerator;

			if (oldGenerator != null)
				oldGenerator.GeneratorChangedCore -= OnGeneratorChanged;

			if (newGenerator != null)
				newGenerator.GeneratorChangedCore += OnGeneratorChanged;

			UpdateData();
		}

		internal void OnItemIsExpandedChangedInternal(TreeViewItem treeViewItem)
		{
			SelectCollapsedItemParent(treeViewItem);

			ItemIsExpandedChanged?.Invoke(this, new TreeViewItemEventEventArgs(treeViewItem));
		}

		internal void OnItemMouseButton(TreeViewItem treeViewItem, MouseButtonEventArgs e)
		{
			e.Handled = MouseSelect(treeViewItem, MouseUtils.FromMouseButton(e.ChangedButton), MouseUtils.FromButtonState(e.ButtonState));

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

		private void OnItemsFilterChangedPrivate(ITreeViewItemFilter oldFilter, ITreeViewItemFilter newFilter)
		{
			if (ReferenceEquals(oldFilter, newFilter))
				return;

			if (TreeViewData == null)
				return;

			TreeViewData.DataFilter.Filter = ItemsFilter;

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

		protected override void OnSelectionChanged(Selection<TreeViewItem> oldSelection, Selection<TreeViewItem> newSelection)
		{
			base.OnSelectionChanged(oldSelection, newSelection);

			oldSelection.Item?.SetIsSelectedInternal(false);
			newSelection.Item?.SetIsSelectedInternal(true);
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

			var currentSelectedItem = SelectorController.CurrentSelectedItem;

			if (ReferenceEquals(currentSelectedItem, treeViewItem) == false)
				currentSelectedItem?.SetIsSelectedInternal(false);

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
				SelectorController.SelectItemSource(data);
		}

		private protected override void SetIsSelectedInternal(TreeViewItem item, bool value)
		{
			item.SetIsSelectedInternal(value);
		}

		private void UpdateData()
		{
			TreeViewData = null;

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

	public class TreeViewControlTemplateContract : SelectorBaseTemplateContract<TreeViewItemsPresenter>
	{
	}
}