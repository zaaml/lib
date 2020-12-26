// <copyright file="ToggleSelectionListViewItem.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Specialized;
using System.Windows;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.UI.Controls.ListView
{
	public class ToggleSelectionListViewItem : ListViewItem
	{
		internal static readonly DependencyProperty IsCheckedInternalProperty = DPM.Register<bool?, ToggleSelectionListViewItem>
			("IsCheckedInternal");

		public ToggleSelectionListViewItem()
		{
			IsSelectable = false;
		}

		internal bool? IsCheckedInternal
		{
			get => (bool?) GetValue(IsCheckedInternalProperty);
			set => SetValue(IsCheckedInternalProperty, value);
		}

		protected override void OnClick()
		{
			ListViewControl?.ToggleSelectionCommand.Execute(null);
		}

		internal override void OnListViewControlChangedInternal(ListViewControl oldListView, ListViewControl newListView)
		{
			base.OnListViewControlChangedInternal(oldListView, newListView);

			if (oldListView != null)
				oldListView.SelectionCollection.CollectionChanged -= SelectionCollectionOnCollectionChanged;

			if (newListView != null)
				newListView.SelectionCollection.CollectionChanged += SelectionCollectionOnCollectionChanged;
		}

		private void SelectionCollectionOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			UpdateIsChecked();
		}

		private void UpdateIsChecked()
		{
			var listViewSelectionCollection = ListViewControl.SelectionCollection;
			var count = listViewSelectionCollection.Count;

			if (count == 0)
				IsCheckedInternal = false;
			else if (listViewSelectionCollection.SelectorController.GetSelectableCount() == count)
				IsCheckedInternal = true;
			else
				IsCheckedInternal = null;
		}
	}
}