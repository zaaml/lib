// <copyright file="DropDownTreeViewSelectionPresenter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using Zaaml.Core;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Core;
using Zaaml.UI.Controls.Editors.Core;
using Zaaml.UI.Controls.TreeView;

namespace Zaaml.UI.Controls.DropDown
{
	public class DropDownTreeViewSelectionPresenter : DropDownSelectionPresenterBase<DropDownTreeViewSelectionItem, TreeViewItem>
	{
		private static readonly DependencyPropertyKey TreeViewControlPropertyKey = DPM.RegisterReadOnly<TreeViewControl, DropDownTreeViewSelectionPresenter>
			("TreeViewControl", default, d => d.OnTreeViewControlPropertyChangedPrivate);

		private static readonly DependencyPropertyKey DropDownTreeViewControlPropertyKey = DPM.RegisterReadOnly<DropDownTreeViewControl, DropDownTreeViewSelectionPresenter>
			("DropDownTreeViewControl", default, d => d.OnDropDownTreeViewControlPropertyChangedPrivate);

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
			get => (DropDownTreeViewControl)GetValue(DropDownTreeViewControlProperty);
			internal set => this.SetReadOnlyValue(DropDownTreeViewControlPropertyKey, value);
		}

		public TreeViewControl TreeViewControl
		{
			get => (TreeViewControl)GetValue(TreeViewControlProperty);
			private set => this.SetReadOnlyValue(TreeViewControlPropertyKey, value);
		}

		private void OnDropDownTreeViewControlPropertyChangedPrivate(DropDownTreeViewControl oldValue, DropDownTreeViewControl newValue)
		{
			if (ReferenceEquals(oldValue, newValue))
				return;

			if (oldValue != null)
			{
				oldValue.EditingEnded -= OnEditingEnded;
				oldValue.EditingStarted -= OnEditingStarted;
				oldValue.TreeViewControlChanged -= OnTreeViewControlChanged;
			}

			if (newValue != null)
			{
				newValue.EditingEnded += OnEditingEnded;
				newValue.EditingStarted += OnEditingStarted;
				newValue.TreeViewControlChanged += OnTreeViewControlChanged;
			}

			UpdateTreeViewControl();
		}

		private void OnEditingEnded(object sender, EditingEndedEventArgs e)
		{
			UpdateContent();
		}

		private void OnEditingStarted(object sender, EventArgs e)
		{
		}

		private void OnTreeViewControlChanged(object sender, ValueChangedEventArgs<TreeViewControl> e)
		{
			UpdateTreeViewControl();
		}

		private void OnTreeViewControlPropertyChangedPrivate(TreeViewControl oldValue, TreeViewControl newValue)
		{
			if (ReferenceEquals(oldValue, newValue))
				return;

			if (oldValue != null)
				oldValue.SelectionChanged -= OnTreeViewSelectionChanged;

			if (newValue != null)
				newValue.SelectionChanged += OnTreeViewSelectionChanged;

			SelectionCollection = newValue?.SelectionCollection;
		}

		private void OnTreeViewSelectionChanged(object sender, SelectionChangedEventArgs<TreeViewItem> e)
		{
			UpdateContent();
		}

		private void UpdateContent()
		{
		}

		private void UpdateTreeViewControl()
		{
			TreeViewControl = DropDownTreeViewControl?.TreeViewControl;
		}
	}

	public class DropDownTreeViewSelectionItem : SelectionItem<TreeViewItem>
	{
	}

	public class DropDownTreeViewSelectionItemsPresenter : SelectionItemsPresenter<DropDownTreeViewSelectionItem, TreeViewItem>
	{
	}

	public class DropDownTreeViewSelectionItemsPanel : SelectionItemsPanel<DropDownTreeViewSelectionItem, TreeViewItem>
	{
	}
}