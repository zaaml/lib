// <copyright file="SpyVisualTreeViewControl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Utils;
using Zaaml.UI.Controls.Core;
using Zaaml.UI.Controls.ScrollView;
using Zaaml.UI.Controls.TreeView;

namespace Zaaml.UI.Controls.Spy
{
	public class SpyVisualTreeViewControl : TreeViewControl
	{
		public static readonly DependencyProperty ElementProperty = DPM.Register<UIElement, SpyVisualTreeViewControl>
			("Element", d => d.OnElementPropertyChangedPrivate);

		private readonly ObservableCollection<SpyVisualTreeItem> _sourceCollection = new();
		private readonly SpyVisualTreeItemPool _spyVisualTreeItemPool = new();
		private SpyVisualTreeItem _rootItem;

		private bool _suspendSelection;

		public SpyVisualTreeViewControl()
		{
			ItemContentMember = "Type";
			ItemSourceCollectionMember = "Children";

			ScrollUnit = ScrollUnit.Pixel;
			SourceCollection = _sourceCollection;

			ItemCollection.DefaultBringIntoViewMode = BringIntoViewMode.Center;

			LevelIndent = 15;
		}

		public UIElement Element
		{
			get => (UIElement) GetValue(ElementProperty);
			set => SetValue(ElementProperty, value);
		}

		private SpyVisualTreeItem RootItem
		{
			get => _rootItem;
			set
			{
				if (ReferenceEquals(_rootItem, value))
					return;

				if (_rootItem != null)
				{
					_rootItem.Release();

					_sourceCollection.Clear();
				}

				_rootItem = value;

				if (_rootItem != null)
					_sourceCollection.Add(_rootItem);
			}
		}

		private void OnElementPropertyChangedPrivate(UIElement oldValue, UIElement newValue)
		{
			if (_suspendSelection)
				return;

			try
			{
				_suspendSelection = true;

				if (TrySelectElement(newValue, true))
					return;

				if (newValue != null)
				{
					var root = newValue.GetAncestorsAndSelf(VisualTreeEnumerationStrategy.Instance).OfType<UIElement>().LastOrDefault();

					RootItem = root != null ? _spyVisualTreeItemPool.GetItem(root) : null;
				}
				else
					RootItem = null;

				TrySelectElement(newValue, true);
			}
			finally
			{
				_suspendSelection = false;
			}
		}

		protected override void OnSelectedItemChanged(TreeViewItem oldItem, TreeViewItem newItem)
		{
			base.OnSelectedItemChanged(oldItem, newItem);

			if (newItem != null)
				EnqueueBringSelectedItemIntoView();
		}

		protected override void OnSelectionChanged(Selection<TreeViewItem> oldSelection, Selection<TreeViewItem> newSelection)
		{
			base.OnSelectionChanged(oldSelection, newSelection);

			if (_suspendSelection)
				return;

			try
			{
				_suspendSelection = true;

				if (newSelection.Source is SpyVisualTreeItem item)
					Element = item.Element;
			}
			finally
			{
				_suspendSelection = false;
			}
		}

		private bool TrySelectElement(UIElement element, bool load)
		{
			if (RootItem == null || element == null)
				return false;

			if (RootItem.TryFind(element, load, out var item))
			{
				CollapseAll();

				SelectedSource = item;

				ExpandSelectedBranch();

				return true;
			}

			return false;
		}
	}
}