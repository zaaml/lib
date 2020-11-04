// <copyright file="TreeViewDataFilter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using Zaaml.UI.Data.Hierarchy;

namespace Zaaml.UI.Controls.TreeView.Data
{
	internal sealed class TreeViewDataFilter : IHierarchyViewFilter<TreeViewData, TreeViewItemDataCollection, TreeViewItemData>
	{
		private ITreeViewItemFilter _treeViewFilter;

		public TreeViewDataFilter(TreeViewData treeViewData)
		{
			TreeViewData = treeViewData;
		}

		public ITreeViewItemFilter Filter
		{
			get => _treeViewFilter;
			set
			{
				if (ReferenceEquals(_treeViewFilter, value))
					return;

				if (_treeViewFilter != null)
					_treeViewFilter.Changed -= OnTreeViewFilterChanged;

				_treeViewFilter = value;

				if (_treeViewFilter != null)
					_treeViewFilter.Changed += OnTreeViewFilterChanged;

				UpdateFilter();
			}
		}

		public TreeViewData TreeViewData { get; }

		private void OnTreeViewFilterChanged(object sender, EventArgs e)
		{
			UpdateFilter();
		}

		private void UpdateFilter()
		{
			TreeViewData.TreeViewControl.OnFilterUpdatingInternal();

			UpdateIsEnabled();

			TreeViewData.RefreshFilter();

			TreeViewData.TreeViewControl.OnFilterUpdatedInternal();
		}

		private void UpdateIsEnabled()
		{
			IsEnabled = _treeViewFilter?.IsEnabled ?? false;
		}

		public bool Pass(TreeViewItemData viewItemData)
		{
			return _treeViewFilter.Pass(viewItemData.Data);
		}

		public bool IsEnabled { get; private set; }
	}
}