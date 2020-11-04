using System;
using Zaaml.UI.Data.Hierarchy;

namespace Zaaml.UI.Controls.ListView.Data
{
	internal sealed class ListViewItemDataCollection : HierarchyNodeViewCollection<ListViewData, ListViewItemDataCollection, ListViewItemData>
	{
		public ListViewItemDataCollection(ListViewData listViewData, ListViewItemData parentViewItemData, Func<object, ListViewItemData> listViewItemDadaFactory) : base(listViewData, parentViewItemData, listViewItemDadaFactory)
		{
		}
	}
}