// <copyright file="DropDownTreeViewControl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
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

namespace Zaaml.UI.Controls.DropDown
{
	[TemplateContractType(typeof(DropDownTreeViewTemplateContract))]
	[ContentProperty(nameof(TreeView))]
	public class DropDownTreeViewControl : DropDownEditableSelectorBase<TreeViewControl, TreeViewItem>
	{
		public static readonly DependencyProperty TreeViewProperty = DPM.Register<TreeViewControl, DropDownTreeViewControl>
			("TreeView", d => d.OnTreeViewChanged);

		public static readonly DependencyProperty PreserveExpandedNodesProperty = DPM.Register<bool, DropDownTreeViewControl>
			("PreserveExpandedNodes", false);

		private static readonly DependencyProperty ItemFilterProperty = DPM.Register<ITreeViewItemFilter, DropDownTreeViewControl>
			("ItemFilter", default, d => d.OnItemFilterPropertyChangedPrivate);

		private readonly Binding _itemFilterBinding;

		static DropDownTreeViewControl()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<DropDownTreeViewControl>();
		}

		public DropDownTreeViewControl()
		{
			this.OverrideStyleKey<DropDownTreeViewControl>();

			BindSelectedIcon(new Binding("TreeView.SelectedItem.Icon") {Source = this});
			BindSelectedContent(new Binding("TreeView.SelectedItem.Content") {Source = this});

			_itemFilterBinding = new Binding {Path = new PropertyPath(TextProperty), Source = this, Mode = BindingMode.TwoWay};
		}

		protected override bool AutoPreserveText => TreeView?.ItemsFilter == null;

		protected override FrameworkElement Editor => TemplateContract.FilterTextBox;

		protected override ItemCollectionBase<TreeViewControl, TreeViewItem> ItemCollection => TreeView.Items;

		protected override TreeViewControl ItemsControl => TreeView;

		public bool PreserveExpandedNodes
		{
			get => (bool) GetValue(PreserveExpandedNodesProperty);
			set => SetValue(PreserveExpandedNodesProperty, value);
		}

		protected override ScrollViewControl ScrollView => TreeView?.ScrollViewInternal;

		private DropDownTreeViewTemplateContract TemplateContract => (DropDownTreeViewTemplateContract) TemplateContractInternal;

		public TreeViewControl TreeView
		{
			get => (TreeViewControl) GetValue(TreeViewProperty);
			set => SetValue(TreeViewProperty, value);
		}

		private protected override void ForceFilterUpdate()
		{
			base.ForceFilterUpdate();

			if (TreeView?.ItemsFilter is ItemTextFilter filter)
				filter.ForceUpdate();
		}

		private protected override TimeSpan GetFilterDelay()
		{
			if (TreeView?.ItemsFilter is ItemTextFilter filter)
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

		internal override void OnIsDropDownOpenChangedInternal()
		{
			try
			{
				var treeView = TreeView;

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

		private void OnItemIsExpandedChanged(object sender, TreeViewItemEventEventArgs e)
		{
			foreach (var visualAncestor in TreeView.GetVisualAncestorsAndSelf().OfType<FrameworkElement>())
				visualAncestor.InvalidateMeasure();
		}

		private void OnTreeViewChanged(TreeViewControl oldTreeViewControl, TreeViewControl newTreeViewControl)
		{
			if (oldTreeViewControl != null)
			{
				oldTreeViewControl.ItemMouseButtonUp -= OnTreeViewItemMouseButtonUp;
				oldTreeViewControl.ItemIsExpandedChanged -= OnItemIsExpandedChanged;
				oldTreeViewControl.MouseButtonSelectionOptions = MouseButtonSelectionOptions.LeftButtonDown;
				oldTreeViewControl.FocusItemOnMouseHover = false;
				oldTreeViewControl.SelectItemOnFocus = true;
				oldTreeViewControl.PreserveMinSize = false;
				oldTreeViewControl.DefaultBringIntoViewMode = BringIntoViewMode.Default;

				ClearValue(ItemFilterProperty);
			}

			if (newTreeViewControl != null)
			{
				newTreeViewControl.ItemMouseButtonUp += OnTreeViewItemMouseButtonUp;
				newTreeViewControl.ItemIsExpandedChanged += OnItemIsExpandedChanged;
				newTreeViewControl.MouseButtonSelectionOptions = MouseButtonSelectionOptions.LeftButtonDown | MouseButtonSelectionOptions.LeftButtonUp;
				newTreeViewControl.FocusItemOnMouseHover = true;
				newTreeViewControl.SelectItemOnFocus = false;
				newTreeViewControl.PreserveMinSize = true;
				newTreeViewControl.DefaultBringIntoViewMode = BringIntoViewMode.Top;

				this.BindProperties(ItemFilterProperty, newTreeViewControl, TreeViewControl.ItemsFilterProperty);
			}

			OnItemsControlChanged(oldTreeViewControl, newTreeViewControl);
		}

		private void OnTreeViewItemMouseButtonUp(object sender, TreeViewItemMouseButtonEventArgs e)
		{
			if (e.MouseEventArgs.ChangedButton == MouseButton.Left)
			{
				CommitSelection();

				e.MouseEventArgs.Handled = true;
			}
		}
	}

	public class DropDownTreeViewTemplateContract : DropDownItemsControlTemplateContract
	{
		[TemplateContractPart(Required = false)]
		public FilterTextBox FilterTextBox { get; private set; }
	}
}