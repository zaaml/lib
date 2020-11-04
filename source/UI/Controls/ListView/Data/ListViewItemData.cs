// <copyright file="ListViewItemData.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

// <copyright file="ListViewItemData.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.UI.Data.Hierarchy;

namespace Zaaml.UI.Controls.ListView.Data
{
	internal sealed class ListViewItemData : HierarchyNodeView<ListViewData, ListViewItemDataCollection, ListViewItemData>
	{
		public ListViewItemData(ListViewData listViewData, ListViewItemData parent) : base(listViewData, parent)
		{
		}

		public int FlatIndex => ListViewData.FindIndex(this);

		public ListViewData ListViewData => Hierarchy;

		public ListViewItem ListViewItem { get; set; }

		protected override ListViewItemData CreateChildNodeCore(object nodeData)
		{
			var listNode = new ListViewItemData(ListViewData, this)
			{
				Data = nodeData,
			};

			return listNode;
		}

		protected override ListViewItemDataCollection CreateNodeCollectionCore()
		{
			return new ListViewItemDataCollection(ListViewData, this, CreateChildNode);
		}

		public override string ToString()
		{
			return Data?.ToString() ?? "Empty";
		}
	}
}