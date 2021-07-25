// <copyright file="TreeViewData.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using Zaaml.Core.Collections;
using Zaaml.UI.Data.Hierarchy;

namespace Zaaml.UI.Controls.TreeView.Data
{
	internal sealed class TreeViewData : HierarchyView<TreeViewData, TreeViewItemDataCollection, TreeViewItemData>
	{
		public TreeViewData(TreeViewControl treeViewControl, ITreeViewAdvisor treeAdvisor)
		{
			TreeViewControl = treeViewControl;
			TreeAdvisor = treeAdvisor;
			DataFilter = new TreeViewDataFilter(this);
			FlatTreeView = new FlatSourceTreeView(this);
		}

		public TreeViewDataFilter DataFilter { get; }

		protected override IHierarchyViewFilter<TreeViewData, TreeViewItemDataCollection, TreeViewItemData> FilterCore => DataFilter;

		protected override FilteringStrategy<TreeViewData, TreeViewItemDataCollection, TreeViewItemData> FilteringStrategy => TreeViewControl.FilteringStrategy;

		public FlatSourceTreeView FlatTreeView { get; }

		internal override bool ShouldExpand => IsFilteredInternal && TreeViewControl.ExpandNodesOnFiltering;

		public ITreeViewAdvisor TreeAdvisor { get; }

		public TreeViewControl TreeViewControl { get; }

		protected override TreeViewItemData CreateChildCore(object data)
		{
			return new TreeViewItemData(this, null)
			{
				Data = data
			};
		}

		protected override TreeViewItemDataCollection CreateNodeCollectionCore()
		{
			return new TreeViewItemDataCollection(this, null, CreateChild);
		}

		protected override void DisposeCore()
		{
			base.DisposeCore();

			DataFilter.Filter = null;
		}

		protected override IEnumerable GetDataNodesCore(TreeViewItemData viewItemData)
		{
			return TreeAdvisor.GetNodes(viewItemData.Data);
		}

		protected override bool IsDataExpandedCore(TreeViewItemData viewItemData)
		{
			return TreeAdvisor.IsExpanded(viewItemData.Data);
		}

		public void RefreshFilter()
		{
			RefreshFilterCore();
		}

		public sealed class FlatSourceTreeView : IReadOnlyList<object>, INotifyCollectionChanged
		{
			public FlatSourceTreeView(TreeViewData treeViewData)
			{
				TreeViewData = treeViewData;

				(TreeViewData.FlatListViewCore as INotifyCollectionChanged).CollectionChanged += OnCollectionChanged;
			}

			private TreeViewData TreeViewData { get; }

			public ReadOnlyListEnumerator<object> GetEnumerator()
			{
				return new ReadOnlyListEnumerator<object>(this);
			}

			private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
			{
				CollectionChanged?.Invoke(this, e);
			}

			public event NotifyCollectionChangedEventHandler CollectionChanged;

			IEnumerator<object> IEnumerable<object>.GetEnumerator()
			{
				return GetEnumerator();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}

			public int Count => TreeViewData.FlatListViewCore.Count;

			public object this[int index] => TreeViewData.FlatListViewCore.ElementAt(index).Data;
		}
	}
}