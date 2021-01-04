// <copyright file="ListViewSelectorAdvisor.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.ListView
{
	internal sealed class ListViewSelectorAdvisor : SelectorBaseControllerAdvisor<ListViewControl, ListViewItem, ListViewItemCollection, ListViewItemsPresenter, ListViewItemsPanel>
	{
		public ListViewSelectorAdvisor(ListViewControl listViewControl) : base(listViewControl)
		{
			ListViewControl = listViewControl;
			ListViewControl.VirtualItemCollection.AttachObserver(this);
		}

		public override bool HasSource => ListViewControl.SourceCollection != null;

		public ListViewControl ListViewControl { get; }

		public override bool CanSelectIndex(int index)
		{
			return ListViewControl.CanSelectIndexInternal(index);
		}

		public override bool CanSelectItem(ListViewItem item)
		{
			if (item is ToggleSelectionListViewItem)
				return false;

			return item.ActualCanSelect;
		}

		public override bool CanSelectSource(object source)
		{
			if (source is ToggleSelectionListViewItem)
				return false;

			return ListViewControl.CanSelectSourceInternal(source);
		}

		public override bool CanSelectValue(object value)
		{
			return ListViewControl.CanSelectValueInternal(value);
		}

		public override int Count => ListViewControl.ListViewData?.IndexedSource.Count ?? 0;

		public override bool TryGetItem(int index, bool ensure, out ListViewItem item)
		{
			if (index == -1)
			{
				item = null;

				return false;
			}

			ListViewControl.EnsureVirtualItemCollection();

			var viewItemCollection = ListViewControl.VirtualItemCollection;

			item = ensure ? viewItemCollection.EnsureItem(index) : viewItemCollection.GetItemFromIndex(index);

			return item != null;
		}

		public override int GetIndexOfSource(object source)
		{
			return ListViewControl.ListViewData?.IndexedSource.IndexOf(source) ?? -1;
		}

		public override object GetSource(int index)
		{
			return ListViewControl.ListViewData?.IndexedSource[index];
		}

		public override bool TryGetItem(object source, bool ensure, out ListViewItem item)
		{
			ListViewControl.EnsureVirtualItemCollection();

			var listViewData = ListViewControl.EnsureListViewData();
			var listViewItemData = listViewData.FindNode(source);

			if (listViewItemData == null)
			{
				item = default;

				return false;
			}

			item = listViewItemData.ListViewItem;

			if (item != null)
				return true;

			return TryGetItem(listViewData.FindIndex(listViewItemData), ensure, out item);
		}
	}
}