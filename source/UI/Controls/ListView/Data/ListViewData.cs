// <copyright file="ListViewData.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections;
using Zaaml.UI.Data.Hierarchy;

namespace Zaaml.UI.Controls.ListView.Data
{
	internal sealed class ListViewData : HierarchyView<ListViewData, ListViewItemDataCollection, ListViewItemData>
	{
		public ListViewData(ListViewControl listViewControl)
		{
			ListViewControl = listViewControl;
			DataFilter = new ListViewDataFilter(this);
		}

		public ListViewDataFilter DataFilter { get; }

		protected override IHierarchyViewFilter<ListViewData, ListViewItemDataCollection, ListViewItemData> FilterCore => DataFilter;

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
	}
}