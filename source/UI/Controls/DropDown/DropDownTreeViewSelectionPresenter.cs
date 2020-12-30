// <copyright file="DropDownTreeViewSelectionPresenter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Linq;
using System.Windows;
using Zaaml.Core;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Core;
using Zaaml.UI.Controls.Primitives.ContentPrimitives;
using Zaaml.UI.Controls.TreeView;

namespace Zaaml.UI.Controls.DropDown
{
	public class DropDownTreeViewSelectionPresenter : DropDownSelectionPresenterBase<DropDownTreeViewSelectionItem, TreeViewItem>
	{
		private static readonly DependencyPropertyKey TreeViewControlPropertyKey = DPM.RegisterReadOnly<TreeViewControl, DropDownTreeViewSelectionPresenter>
			("TreeViewControl", d => d.OnTreeViewControlPropertyChangedPrivate);

		private static readonly DependencyPropertyKey DropDownTreeViewControlPropertyKey = DPM.RegisterReadOnly<DropDownTreeViewControl, DropDownTreeViewSelectionPresenter>
			("DropDownTreeViewControl", d => d.OnDropDownTreeViewControlPropertyChangedPrivate);

		public static readonly DependencyProperty DropDownTreeViewControlProperty = DropDownTreeViewControlPropertyKey.DependencyProperty;

		public static readonly DependencyProperty TreeViewControlProperty = TreeViewControlPropertyKey.DependencyProperty;

		static DropDownTreeViewSelectionPresenter()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<DropDownTreeViewSelectionPresenter>();
		}

		public DropDownTreeViewSelectionPresenter()
		{
			this.OverrideStyleKey<DropDownTreeViewSelectionPresenter>();
		}

		public DropDownTreeViewControl DropDownTreeViewControl
		{
			get => (DropDownTreeViewControl) GetValue(DropDownTreeViewControlProperty);
			internal set => this.SetReadOnlyValue(DropDownTreeViewControlPropertyKey, value);
		}

		internal bool IsDefault { get; set; }

		public TreeViewControl TreeViewControl
		{
			get => (TreeViewControl) GetValue(TreeViewControlProperty);
			private set => this.SetReadOnlyValue(TreeViewControlPropertyKey, value);
		}

		private protected override void AttachOverride(SelectionItem<TreeViewItem> selectionItem, Selection<TreeViewItem> selection)
		{
			selectionItem.ItemsControl = this;
			selectionItem.Selection = selection;

			var defaultGeneratorImplementation = TreeViewControl.DefaultGeneratorImplementationInternal;
			var contentBinding = defaultGeneratorImplementation.ItemContentMemberBindingInternal;
			var iconBinding = defaultGeneratorImplementation.ItemIconMemberBindingInternal;
			var treeViewItem = selection.Item;

			selectionItem.DataContext = selection.Source;

			if (treeViewItem != null && ItemCollectionBase.GetInItemCollection(treeViewItem))
			{
				selectionItem.Content = treeViewItem.Content;
				selectionItem.Icon = treeViewItem.Icon;
			}
			else if (selection.Source is TreeViewItem treeViewItemSource)
			{
				selectionItem.Content = treeViewItemSource.Content;
				selectionItem.Icon = treeViewItemSource.Icon;
			}
			else
			{
				if (contentBinding != null)
					ItemGenerator<SelectionItem<TreeViewItem>>.InstallBinding(selectionItem, IconContentPresenter.ContentProperty, contentBinding);
				else
					selectionItem.Content = selection.Source;

				if (iconBinding != null)
					ItemGenerator<SelectionItem<TreeViewItem>>.InstallBinding(selectionItem, IconPresenterBase.IconProperty, iconBinding);
			}
		}

		private protected override void DetachOverride(SelectionItem<TreeViewItem> selectionItem, Selection<TreeViewItem> selection)
		{
			selectionItem.ItemsControl = null;
			selectionItem.Selection = Selection<TreeViewItem>.Empty;

			selectionItem.ClearValue(DataContextProperty);
			selectionItem.ClearValue(IconContentPresenter.ContentProperty);
			selectionItem.ClearValue(IconPresenterBase.IconProperty);
		}

		private protected override bool IsAttachDetachOverriden => IsDefault;

		private void OnDropDownTreeViewControlPropertyChangedPrivate(DropDownTreeViewControl oldValue, DropDownTreeViewControl newValue)
		{
			if (ReferenceEquals(oldValue, newValue))
				return;

			if (oldValue != null)
				oldValue.TreeViewControlChanged -= OnTreeViewControlChanged;

			if (newValue != null)
				newValue.TreeViewControlChanged += OnTreeViewControlChanged;

			UpdateTreeViewControl();
		}

		private void OnTreeViewControlChanged(object sender, ValueChangedEventArgs<TreeViewControl> e)
		{
			UpdateTreeViewControl();
		}

		private void OnTreeViewControlPropertyChangedPrivate(TreeViewControl oldValue, TreeViewControl newValue)
		{
			if (ReferenceEquals(oldValue, newValue))
				return;

			SelectionCollection = newValue?.SelectionCollection;
		}

		private void UpdateTreeViewControl()
		{
			TreeViewControl = DropDownTreeViewControl?.TreeViewControl;
		}
	}

	public class DropDownTreeViewSelectionItem : SelectionItem<TreeViewItem>
	{
		private protected override bool IsLast
		{
			get
			{
				var selectionPresenter = (DropDownTreeViewSelectionPresenter) ItemsControl;

				return ReferenceEquals(Selection.Source, selectionPresenter?.SelectionCollection.LastOrDefault().Source);
			}
		}
	}

	public class DropDownTreeViewSelectionItemsPresenter : SelectionItemsPresenter<DropDownTreeViewSelectionItem, TreeViewItem>
	{
	}

	public class DropDownTreeViewSelectionItemsPanel : SelectionItemsPanel<DropDownTreeViewSelectionItem, TreeViewItem>
	{
	}
}