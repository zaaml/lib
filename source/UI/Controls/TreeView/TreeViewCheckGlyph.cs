using System.Windows;
using System.Windows.Data;
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

			var parent = TemplatedParent;

			if (parent is ContentPresenter contentPresenter)
			{
				if (contentPresenter.TemplatedParent is TreeViewItem listViewItem)
					TreeViewItem = listViewItem;
			}
			else if (parent is TreeViewItem listViewItem)
				TreeViewItem = listViewItem;
			else
				TreeViewItem = null;
		}

		protected override void OnUnloaded()
		{
			TreeViewItem = null;

			base.OnUnloaded();
		}
	}
}