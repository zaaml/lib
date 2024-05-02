// <copyright file="ListViewControl.Selection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Windows.Input;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.ListView
{
	public partial class ListViewControl
	{
		public ICommand ToggleSelectionCommand { get; }

		private bool CanExecuteToggleSelectionCommand(object parameter)
		{
			return true;
		}

		protected virtual bool CanSelectIndex(int index)
		{
			return true;
		}

		internal bool CanSelectIndexInternal(int index)
		{
			return CanSelectIndex(index);
		}

		protected virtual bool CanSelectItem(ListViewItem listViewItem)
		{
			if (listViewItem is ToggleSelectionListViewItem)
				return false;

			return CanSelectItemPrivate(listViewItem);
		}

		internal bool CanSelectItemInternal(ListViewItem listViewItem)
		{
			return CanSelectItem(listViewItem);
		}

		private bool CanSelectItemPrivate(ListViewItem listViewItem)
		{
			return true;
		}

		protected virtual bool CanSelectSource(object listViewItemSource)
		{
			if (listViewItemSource is ToggleSelectionListViewItem)
				return false;

			return true;
		}

		internal bool CanSelectSourceInternal(object source)
		{
			return CanSelectSource(source);
		}

		protected virtual bool CanSelectValue(object listViewItemValue)
		{
			return true;
		}

		internal bool CanSelectValueInternal(object value)
		{
			return CanSelectValue(value);
		}

		internal override SelectorController<ListViewControl, ListViewItem> CreateSelectorController()
		{
			return new ListViewSelectorController(this);
		}

		protected override bool GetIsSelected(ListViewItem item)
		{
			return item.IsSelected;
		}

		public void InvertSelection()
		{
			SelectorController.InvertSelection();
		}

		private void OnToggleSelectionCommandExecuted(object parameter)
		{
			if (parameter == null)
			{
				SelectorController.ToggleSelection(null);
			}

			if (parameter is ListViewToggleSelectionCommandMode toggleMode)
			{
				switch (toggleMode)
				{
					case ListViewToggleSelectionCommandMode.Invert:
						InvertSelection();
						break;
					case ListViewToggleSelectionCommandMode.Select:
						SelectAll();
						break;
					case ListViewToggleSelectionCommandMode.Unselect:
						UnselectAll();
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
			else if (parameter is bool boolToggleMode)
			{
				SelectorController.ToggleSelection(boolToggleMode);
			}
		}

		internal void Select(ListViewItem listViewItem)
		{
			if (SelectorController.SelectionHandling)
				return;

			if (ActualFocusItemOnSelect)
				FocusItem(listViewItem);

			SelectorController.SelectItem(listViewItem);
		}

		public void SelectAll()
		{
			SelectorController.SelectAll();
		}

		public void SelectSourceCollection(IEnumerable<object> itemSources)
		{
			SelectorController.SelectSourceCollection(itemSources);
		}

		protected override void SetIsSelected(ListViewItem item, bool value)
		{
			item.SetIsSelectedInternal(value);
		}

		private void ToggleItemSelection(ListViewItem listViewItem)
		{
			if (listViewItem.IsSelected == false)
				listViewItem.SelectInternal();
			else if (SelectionMode == ListViewSelectionMode.Multiple)
				listViewItem.UnselectInternal();
		}

		internal void Unselect(ListViewItem listViewItem)
		{
			SelectorController.UnselectItem(listViewItem);
		}

		public void UnselectAll()
		{
			SelectorController.UnselectAll();
		}

		public void UnselectSourceCollection(IEnumerable<object> itemSources)
		{
			SelectorController.UnselectSourceCollection(itemSources);
		}
	}
}