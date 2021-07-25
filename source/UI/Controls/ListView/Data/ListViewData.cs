// <copyright file="ListViewData.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using Zaaml.Core.Collections;
using Zaaml.UI.Data.Hierarchy;

namespace Zaaml.UI.Controls.ListView.Data
{
	internal sealed class ListViewData : HierarchyView<ListViewData, ListViewItemDataCollection, ListViewItemData>
	{
		public ListViewData(ListViewControl listViewControl)
		{
			ListViewControl = listViewControl;
			DataFilter = new ListViewDataFilter(this);
			FlatListView = new FlatSourceListView(this);
		}

		public ListViewDataFilter DataFilter { get; }

		protected override IHierarchyViewFilter<ListViewData, ListViewItemDataCollection, ListViewItemData> FilterCore => DataFilter;

		public FlatSourceListView FlatListView { get; }

		public ListViewControl ListViewControl { get; }

		protected override ListViewItemData CreateChildCore(object data)
		{
			return new ListViewItemData(this, null)
			{
				Data = data
			};
		}

		protected override ListViewItemDataCollection CreateNodeCollectionCore()
		{
			return new ListViewItemDataCollection(this, null, CreateChild);
		}

		protected override void DisposeCore()
		{
			base.DisposeCore();

			DataFilter.Filter = null;
		}

		protected override IEnumerable GetDataNodesCore(ListViewItemData viewItemData)
		{
			return new ListViewItemDataCollection(this, null, CreateChild);
		}

		protected override bool IsDataExpandedCore(ListViewItemData viewItemData)
		{
			return false;
		}

		public void RefreshFilter()
		{
			RefreshFilterCore();
		}

		public sealed class FlatSourceListView : IReadOnlyList<object>, INotifyCollectionChanged
		{
			public FlatSourceListView(ListViewData listViewData)
			{
				ListViewData = listViewData;

				(ListViewData.FlatListViewCore as INotifyCollectionChanged).CollectionChanged += OnCollectionChanged;
			}

			private ListViewData ListViewData { get; }

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

			public int Count => ListViewData.FlatListViewCore.Count;

			public object this[int index] => ListViewData.FlatListViewCore.ElementAt(index).Data;
		}
	}
}