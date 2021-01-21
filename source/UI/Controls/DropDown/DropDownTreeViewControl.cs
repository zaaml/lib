// <copyright file="DropDownTreeViewControl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using Zaaml.Core;
using Zaaml.PresentationCore.Data;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.PropertyCore.Extensions;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Core;
using Zaaml.UI.Controls.Editors.Text;
using Zaaml.UI.Controls.ScrollView;
using Zaaml.UI.Controls.TreeView;
using Zaaml.UI.Data.Hierarchy;
using TreeViewItem = Zaaml.UI.Controls.TreeView.TreeViewItem;

namespace Zaaml.UI.Controls.DropDown
{
	[TemplateContractType(typeof(DropDownTreeViewTemplateContract))]
	[ContentProperty(nameof(TreeViewControl))]
	public class DropDownTreeViewControl : DropDownEditableSelectorBase<TreeViewControl, TreeViewItem>
	{
		public static readonly DependencyProperty TreeViewControlProperty = DPM.Register<TreeViewControl, DropDownTreeViewControl>
			("TreeViewControl", d => d.OnTreeViewChanged);

		public static readonly DependencyProperty PreserveExpandedNodesProperty = DPM.Register<bool, DropDownTreeViewControl>
			("PreserveExpandedNodes", false);

		private static readonly DependencyProperty ItemFilterProperty = DPM.Register<ITreeViewItemFilter, DropDownTreeViewControl>
			("ItemFilter", default, d => d.OnItemFilterPropertyChangedPrivate);

		public static readonly DependencyProperty SelectionPresenterProperty = DPM.Register<DropDownTreeViewSelectionPresenter, DropDownTreeViewControl>
			("SelectionPresenter", d => d.OnSelectionPresenterPropertyChangedPrivate);

		public static readonly DependencyProperty SelectionItemTemplateProperty = DPM.Register<DataTemplate, DropDownTreeViewControl>
			("SelectionItemTemplate", d => d.OnSelectionItemTemplatePropertyChangedPrivate);

		private static readonly DependencyPropertyKey ActualSelectionPresenterPropertyKey = DPM.RegisterReadOnly<DropDownTreeViewSelectionPresenter, DropDownTreeViewControl>
			("ActualSelectionPresenter");

		public static readonly DependencyProperty ActualSelectionPresenterProperty = ActualSelectionPresenterPropertyKey.DependencyProperty;

		private readonly Binding _itemFilterBinding;

		private DropDownTreeViewSelectionPresenter _defaultSelectionPresenter;

		internal event EventHandler<ValueChangedEventArgs<TreeViewControl>> TreeViewControlChanged;

		static DropDownTreeViewControl()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<DropDownTreeViewControl>();
		}

		public DropDownTreeViewControl()
		{
			this.OverrideStyleKey<DropDownTreeViewControl>();

			_itemFilterBinding = new Binding {Path = new PropertyPath(EditorTextProperty), Source = this, Mode = BindingMode.TwoWay};
		}

		public DropDownTreeViewSelectionPresenter ActualSelectionPresenter
		{
			get => (DropDownTreeViewSelectionPresenter) GetValue(ActualSelectionPresenterProperty);
			private set => this.SetReadOnlyValue(ActualSelectionPresenterPropertyKey, value);
		}

		protected override bool AutoPreserveEditorText => TreeViewControl?.ActualItemsFilter == null;

		private TreeViewItemTextFilter DefaultFilter { get; set; }

		private DropDownTreeViewSelectionPresenter DefaultSelectionPresenter => _defaultSelectionPresenter ??= CreteDefaultSelectionPresenter();

		protected override FrameworkElement EditorCore => FilterTextBox;

		private FilterTextBox FilterTextBox => TemplateContract.FilterTextBox;

		protected override ItemCollectionBase<TreeViewControl, TreeViewItem> ItemCollection => TreeViewControl.ItemCollection;

		protected override TreeViewControl ItemsControl => TreeViewControl;

		public bool PreserveExpandedNodes
		{
			get => (bool) GetValue(PreserveExpandedNodesProperty);
			set => SetValue(PreserveExpandedNodesProperty, value);
		}

		protected override ScrollViewControl ScrollView => TreeViewControl?.ScrollViewInternal;

		public DataTemplate SelectionItemTemplate
		{
			get => (DataTemplate) GetValue(SelectionItemTemplateProperty);
			set => SetValue(SelectionItemTemplateProperty, value);
		}

		public DropDownTreeViewSelectionPresenter SelectionPresenter
		{
			get => (DropDownTreeViewSelectionPresenter) GetValue(SelectionPresenterProperty);
			set => SetValue(SelectionPresenterProperty, value);
		}

		protected override FrameworkElement SelectionPresenterCore => SelectionPresenter;

		private DropDownTreeViewTemplateContract TemplateContract => (DropDownTreeViewTemplateContract) TemplateContractInternal;

		public TreeViewControl TreeViewControl
		{
			get => (TreeViewControl) GetValue(TreeViewControlProperty);
			set => SetValue(TreeViewControlProperty, value);
		}

		private DropDownTreeViewSelectionPresenter CreteDefaultSelectionPresenter()
		{
			return new DropDownTreeViewSelectionPresenter
			{
				IsDefault = true,
				DropDownTreeViewControl = this,
				ItemContentTemplate = SelectionItemTemplate,
				ContentMode = SelectionItemContentMode.None
			};
		}

		private protected override void ForceFilterUpdate()
		{
			base.ForceFilterUpdate();

			if (TreeViewControl?.ItemsFilter is ItemTextFilter filter)
				filter.ForceUpdate();
		}

		private protected override TimeSpan GetFilterDelay()
		{
			if (TreeViewControl?.ItemsFilter is ItemTextFilter filter)
				return filter.Delay;

			return base.GetFilterDelay();
		}

		private protected override FocusNavigator<TreeViewItem> GetFocusNavigator(TreeViewControl control)
		{
			return control?.FocusNavigator;
		}

		private protected override SelectorController<TreeViewItem> GetSelectorController(TreeViewControl control)
		{
			return control?.SelectorController;
		}

		protected override Size MeasureOverride(Size availableSize)
		{
			if (ActualSelectionPresenter == null)
				UpdateActualSelectionPresenter();

			return base.MeasureOverride(availableSize);
		}

		internal override void OnIsDropDownOpenChangedInternal()
		{
			try
			{
				var treeView = TreeViewControl;

				if (treeView == null)
					return;

				if (IsDropDownOpen)
				{
					if (PreserveExpandedNodes == false)
						treeView.CollapseAll(ExpandCollapseMode.Default);

					treeView.ExpandSelectedBranch();
				}
			}
			finally
			{
				base.OnIsDropDownOpenChangedInternal();
			}
		}

		private void OnItemFilterPropertyChangedPrivate(ITreeViewItemFilter oldValue, ITreeViewItemFilter newValue)
		{
			if (ReferenceEquals(oldValue, newValue))
				return;

			if (oldValue is ItemTextFilter oldItemFilter)
			{
				if (ReferenceEquals(oldItemFilter.ReadLocalBinding(ItemTextFilter.FilterTextProperty), _itemFilterBinding))
					oldItemFilter.ClearValue(ItemTextFilter.FilterTextProperty);
			}

			if (newValue is ItemTextFilter newItemFilter)
			{
				if (newItemFilter.HasLocalValue(ItemTextFilter.FilterTextProperty) == false)
					newItemFilter.SetBinding(ItemTextFilter.FilterTextProperty, _itemFilterBinding);
			}
		}

		private void OnItemIsExpandedChanged(object sender, TreeViewItemEventArgs e)
		{
			foreach (var visualAncestor in TreeViewControl.GetVisualAncestorsAndSelf().OfType<FrameworkElement>())
				visualAncestor.InvalidateMeasure();
		}

		private void OnSelectionItemTemplatePropertyChangedPrivate()
		{
			if (_defaultSelectionPresenter != null)
				_defaultSelectionPresenter.ItemContentTemplate = SelectionItemTemplate;
		}

		private void OnSelectionPresenterPropertyChangedPrivate(DropDownTreeViewSelectionPresenter oldValue, DropDownTreeViewSelectionPresenter newValue)
		{
			if (ReferenceEquals(oldValue, newValue))
				return;

			if (oldValue != null)
				oldValue.DropDownTreeViewControl = null;

			if (newValue != null)
				newValue.DropDownTreeViewControl = this;

			UpdateActualSelectionPresenter();
		}

		private void OnTreeViewChanged(TreeViewControl oldTreeViewControl, TreeViewControl newTreeViewControl)
		{
			if (oldTreeViewControl != null)
			{
				oldTreeViewControl.ItemsDefaultFilter = DefaultFilter = null;
				oldTreeViewControl.ItemMouseButtonUp -= OnTreeViewItemMouseButtonUp;
				oldTreeViewControl.ItemIsExpandedChanged -= OnItemIsExpandedChanged;
				oldTreeViewControl.ItemClickMode = ClickMode.Release;
				oldTreeViewControl.FocusItemOnMouseHover = false;
				oldTreeViewControl.SelectItemOnFocus = true;
				oldTreeViewControl.PreserveMinSize = false;
				oldTreeViewControl.DefaultBringIntoViewMode = BringIntoViewMode.Default;

				ClearValue(ItemFilterProperty);
			}

			if (newTreeViewControl != null)
			{
				newTreeViewControl.ItemsDefaultFilter = DefaultFilter = new TreeViewItemTextFilter();
				newTreeViewControl.ItemMouseButtonUp += OnTreeViewItemMouseButtonUp;
				newTreeViewControl.ItemIsExpandedChanged += OnItemIsExpandedChanged;
				newTreeViewControl.ItemClickMode = ClickMode.Release;
				newTreeViewControl.FocusItemOnMouseHover = true;
				newTreeViewControl.SelectItemOnFocus = false;
				newTreeViewControl.PreserveMinSize = true;
				newTreeViewControl.DefaultBringIntoViewMode = BringIntoViewMode.Top;

				this.BindProperties(ItemFilterProperty, newTreeViewControl, TreeViewControl.ItemsFilterProperty, targetNullValue: DefaultFilter);
			}

			OnItemsControlChanged(oldTreeViewControl, newTreeViewControl);

			TreeViewControlChanged?.Invoke(this, new ValueChangedEventArgs<TreeViewControl>(oldTreeViewControl, newTreeViewControl));
		}

		private void OnTreeViewItemMouseButtonUp(object sender, TreeViewItemMouseButtonEventArgs e)
		{
			if (e.MouseEventArgs.ChangedButton == MouseButton.Left)
			{
				if (TreeViewControl.SelectionMode == TreeViewSelectionMode.Single)
					CommitSelection();

				e.MouseEventArgs.Handled = true;
			}
		}

		private protected override void RaiseFocusedItemClick()
		{
			base.RaiseFocusedItemClick();

			FocusNavigator.FocusedItem?.RaiseClickInternal();
		}

		private void UpdateActualSelectionPresenter()
		{
			ActualSelectionPresenter = SelectionPresenter ?? DefaultSelectionPresenter;
		}
	}

	public class DropDownTreeViewTemplateContract : DropDownItemsControlTemplateContract
	{
		[TemplateContractPart(Required = false)]
		public FilterTextBox FilterTextBox { get; [UsedImplicitly] private set; }
	}
}