// <copyright file="ListViewDataFilter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

// <copyright file="ListViewDataFilter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using Zaaml.UI.Data.Hierarchy;

namespace Zaaml.UI.Controls.ListView.Data
{
	internal sealed class ListViewDataFilter : IHierarchyViewFilter<ListViewData, ListViewItemDataCollection, ListViewItemData>
	{
		private IListViewItemFilter _listViewFilter;

		public ListViewDataFilter(ListViewData listViewData)
		{
			ListViewData = listViewData;
		}

		public IListViewItemFilter Filter
		{
			get => _listViewFilter;
			set
			{
				if (ReferenceEquals(_listViewFilter, value))
					return;

				if (_listViewFilter != null)
					_listViewFilter.Changed -= ListViewFilterOnChanged;

				_listViewFilter = value;

				if (_listViewFilter != null)
					_listViewFilter.Changed += ListViewFilterOnChanged;

				UpdateFilter();
			}
		}

		public ListViewData ListViewData { get; }

		private void ListViewFilterOnChanged(object sender, EventArgs e)
		{
			UpdateFilter();
		}

		private void UpdateFilter()
		{
			ListViewData.ListViewControl.OnFilterUpdatingInternal();

			UpdateIsEnabled();

			ListViewData.RefreshFilter();

			ListViewData.ListViewControl.OnFilterUpdatedInternal();
		}

		private void UpdateIsEnabled()
		{
			IsEnabled = _listViewFilter?.IsEnabled ?? false;
		}

		public bool Pass(ListViewItemData viewItemData)
		{
			return _listViewFilter.Pass(viewItemData.Data);
		}

		public bool IsEnabled { get; private set; }
	}
}