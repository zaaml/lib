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
			("IsCheckedInternal", l => l.OnIsCheckedInternalPropertyChangedPrivate);

		public ToggleSelectionListViewItem()
		{
			IsSelectable = false;
		}

		internal bool? IsCheckedInternal
		{
			get => (bool?) GetValue(IsCheckedInternalProperty);
			set => SetValue(IsCheckedInternalProperty, value);
		}

		private protected override bool IsSelectedState => IsCheckedInternal == true;

		protected override void OnClick()
		{
			ListViewControl?.ToggleSelectionCommand.Execute(null);
		}

		private void OnIsCheckedInternalPropertyChangedPrivate()
		{
			UpdateVisualState(true);
		}

		internal override void OnListViewControlChangedInternal(ListViewControl oldListView, ListViewControl newListView)
		{
			base.OnListViewControlChangedInternal(oldListView, newListView);

			if (oldListView != null)
				oldListView.SelectionCollection.CollectionChanged -= SelectionCollectionOnCollectionChanged;

			if (newListView != null)
				newListView.SelectionCollection.CollectionChanged += SelectionCollectionOnCollectionChanged;

			UpdateIsChecked();
		}

		private void SelectionCollectionOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			UpdateIsChecked();
		}

		private void UpdateIsChecked()
		{
			if (ListViewControl == null)
				return;

			var listViewSelectionCollection = ListViewControl.SelectionCollection;
			var count = listViewSelectionCollection.Count;

			if (count == 0)
				IsCheckedInternal = false;
			else if (listViewSelectionCollection.SelectorController.GetSelectableCount() == count)
				IsCheckedInternal = true;
			else
				IsCheckedInternal = null;
		}

		private protected override void OnCheckGlyphToggleSelectionInternal()
		{
			ListViewControl?.ToggleSelectionCommand.Execute(null);
		}
	}
}