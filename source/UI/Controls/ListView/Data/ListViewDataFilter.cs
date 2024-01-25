// <copyright file="ListViewDataFilter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

// <copyright file="ListViewDataFilter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using Zaaml.UI.Controls.Core;
using Zaaml.UI.Data.Hierarchy;

namespace Zaaml.UI.Controls.ListView.Data
{
	internal sealed class ListViewDataFilter : IHierarchyViewFilter<ListViewData, ListViewItemDataCollection, ListViewItemData>
	{
		private IListViewItemFilter _listViewFilter;
		private readonly FilterServiceProvider _filterServiceProvider;

		public ListViewDataFilter(ListViewData listViewData)
		{
			ListViewData = listViewData;
			_filterServiceProvider = new FilterServiceProvider(this);
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
			return _listViewFilter.Pass(viewItemData.Data, _filterServiceProvider);
		}

		public bool IsEnabled { get; private set; }

		private sealed class FilterServiceProvider : IServiceProvider, IItemsControlProvider
		{
			public ListViewDataFilter ListViewDataFilter { get; }

			public FilterServiceProvider(ListViewDataFilter treeViewDataFilter)
			{
				ListViewDataFilter = treeViewDataFilter;
			}

			public object GetService(Type serviceType)
			{
				return serviceType == typeof(IItemsControlProvider) ? this : null;
			}

			public ItemsControlBase ItemsControl => ListViewDataFilter?.ListViewData?.ListViewControl;
		}
	}
}