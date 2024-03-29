﻿using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.UI.Controls.Core;
using Zaaml.UI.Controls.Primitives;

namespace Zaaml.UI.Controls.TreeView
{
	public class TreeViewCheckGlyph : CheckGlyph
	{
		private static readonly DependencyPropertyKey TreeViewItemPropertyKey = DPM.RegisterReadOnly<TreeViewItem, TreeViewCheckGlyph>
			("TreeViewItem", g => g.OnTreeViewItemPropertyChangedPrivate);

		public static readonly DependencyProperty TreeViewItemProperty = TreeViewItemPropertyKey.DependencyProperty;

		private static readonly PropertyPath TreeViewItemIsSelectedPropertyPath = new PropertyPath(TreeViewItem.IsSelectedProperty);

		public TreeViewItem TreeViewItem
		{
			get => (TreeViewItem)GetValue(TreeViewItemProperty);
			private set => this.SetReadOnlyValue(TreeViewItemPropertyKey, value);
		}

		public TreeViewCheckGlyph()
		{
		}

		public TreeViewCheckGlyph(TreeViewItem listViewItem)
		{
			TreeViewItem = listViewItem;

			IsExplicit = true;
		}

		private bool IsExplicit { get; }

		private void OnTreeViewItemPropertyChangedPrivate(TreeViewItem oldTreeViewItem, TreeViewItem newTreeViewItem)
		{
			if (ReferenceEquals(oldTreeViewItem, newTreeViewItem))
				return;

			if (oldTreeViewItem != null)
				ClearValue(IsCheckedProperty);

			if (newTreeViewItem != null)
				SetBinding(IsCheckedProperty, new Binding { Path = TreeViewItemIsSelectedPropertyPath, Source = newTreeViewItem, Mode = BindingMode.TwoWay });
		}

		protected override void OnLoaded()
		{
			base.OnLoaded();

			if (IsExplicit)
				return;

			var templatedParent = TemplatedParent;

			if (templatedParent is ContentPresenter contentPresenter)
			{
				if (contentPresenter.TemplatedParent is TreeViewItem listViewItem)
					TreeViewItem = listViewItem;
			}
			else if (templatedParent is TreeViewItem listViewItem)
				TreeViewItem = listViewItem;
			else
				TreeViewItem = null;
		}

		protected override void OnUnloaded()
		{
			if (IsExplicit == false)
				TreeViewItem = null;

			base.OnUnloaded();
		}

		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
		{
			base.OnMouseLeftButtonDown(e);

			TreeViewItem?.OnCheckGlyphMouseLeftButtonDownInternal();
		}
	}
}