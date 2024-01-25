// <copyright file="SpyVisualTreeViewControl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Utils;
using Zaaml.UI.Controls.Core;
using Zaaml.UI.Controls.Primitives.ContentPrimitives;
using Zaaml.UI.Controls.ScrollView;
using Zaaml.UI.Controls.TreeView;

namespace Zaaml.UI.Controls.Spy
{
	public class SpyVisualTreeViewControl : TreeViewControl
	{
		public static readonly DependencyProperty ElementProperty = DPM.Register<UIElement, SpyVisualTreeViewControl>
			("Element", d => d.OnElementPropertyChangedPrivate);

		private readonly ObservableCollection<SpyVisualTreeDataItem> _sourceCollection = new();
		private readonly SpyVisualTreeDataItemPool _spyVisualTreeDataItemPool = new();
		private SpyVisualTreeDataItem _rootDataItem;

		private bool _suspendSelection;

		public SpyVisualTreeViewControl()
		{
			//ItemContentMember = "Type";
			ItemSourceCollectionMember = "Children";

			ScrollUnit = ScrollUnit.Pixel;
			SourceCollection = _sourceCollection;

			ItemCollection.DefaultBringIntoViewMode = BringIntoViewMode.Center;
			
			LevelIndentSize = 15;

			var resourcePathUri = Assembly.GetExecutingAssembly().GetResourceUri("/Resources/Icons.xaml");

			Resources.MergedDictionaries.Add(ResourceDictionaryUtils.LoadResourceDictionary(resourcePathUri));
		}

		public UIElement Element
		{
			get => (UIElement) GetValue(ElementProperty);
			set => SetValue(ElementProperty, value);
		}

		internal override void OnItemAttachedInternal(TreeViewItem item)
		{
			base.OnItemAttachedInternal(item);

			item.Icon = (PathIcon)Resources["CodeIcon"];
		}

		internal override void OnItemDetachedInternal(TreeViewItem item)
		{
			item.Icon = null;

			base.OnItemDetachedInternal(item);
		}

		private SpyVisualTreeDataItem RootDataItem
		{
			get => _rootDataItem;
			set
			{
				if (ReferenceEquals(_rootDataItem, value))
					return;

				if (_rootDataItem != null)
				{
					_rootDataItem.Release();

					_sourceCollection.Clear();
				}

				_rootDataItem = value;

				if (_rootDataItem != null)
					_sourceCollection.Add(_rootDataItem);
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

					RootDataItem = root != null ? _spyVisualTreeDataItemPool.GetItem(root) : null;
				}
				else
					RootDataItem = null;

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

				if (newSelection.Source is SpyVisualTreeDataItem item)
					Element = item.Element;
			}
			finally
			{
				_suspendSelection = false;
			}
		}

		private bool TrySelectElement(UIElement element, bool load)
		{
			if (RootDataItem == null || element == null)
				return false;

			if (RootDataItem.TryFind(element, load, out var item))
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