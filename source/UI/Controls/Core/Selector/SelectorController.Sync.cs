// <copyright file="SelectorController.SourceChanges.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Specialized;
using Zaaml.Core;

namespace Zaaml.UI.Controls.Core
{
	internal abstract partial class SelectorController<TItem>
	{
		internal void AdvisorOnItemAttached(int index, TItem item)
		{
			OnItemAttached(index, item);
		}

		internal void AdvisorOnItemCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			OnItemCollectionChanged(e);
		}

		internal void AdvisorOnItemCollectionSourceChanged(NotifyCollectionChangedEventArgs e)
		{
			OnItemCollectionSourceChanged(e);
		}

		internal void AdvisorOnItemDetached(int index, TItem item)
		{
			OnItemDetached(index, item);
		}

		private void OnItemCollectionSourceChanged(NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Move)
			{
			}
			else if (e.Action == NotifyCollectionChangedAction.Reset)
			{
				if (MultipleSelection)
					CurrentSelectionCollection.UpdateIndicesSources();

				if (CurrentSelectedIndex != -1)
				{
					var source = GetSource(ResumeSelectedIndex);

					Sync(source != null ? CurrentSelection.WithSource(source) : CurrentSelection.WithIndex(-1));
				}
			}
			else
			{
				if (e.NewItems != null)
				{
					foreach (var source in e.NewItems)
					{
						if (IsSourceSelected(source))
							SelectSource(source);
					}
				}

				if (e.OldItems != null)
				{
					foreach (var source in e.OldItems)
					{
						if (IsSourceInSelection(source))
							UnselectSource(source);
					}
				}
			}

			SyncIndex(false);
			EnsureSelection();
		}

		private void OnItemDetached([UsedImplicitly] int index, TItem item)
		{
			if (HasSource)
			{
				if (IsLocked(item) == false)
				{
					var source = GetSource(item);

					if (MultipleSelection)
						if (CurrentSelectionCollection.FindBySource(source, false, out var selection))
							if (ReferenceEquals(item, selection.Item))
								CurrentSelectionCollection.UpdateSelection(selection.Index, selection.WithItem(null));

					if (ReferenceEquals(source, CurrentSelectedSource))
						if (ReferenceEquals(item, CurrentSelectedItem))
							Sync(CurrentSelection.WithItem(null));
				}
			}
			else
			{
				if (IsVirtualizing == false)
					UnselectItem(item);
			}
		}

		private void OnItemCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Reset)
			{
			}
			else if (e.Action == NotifyCollectionChangedAction.Move)
			{
			}
			else
			{
				if (e.NewItems != null)
				{
					foreach (TItem item in e.NewItems)
					{
						if (GetIsItemSelected(item))
							SelectItem(item);
					}
				}

				if (e.OldItems != null)
				{
					foreach (TItem item in e.OldItems)
					{
						if (IsItemInSelection(item))
							UnselectItem(item);
					}
				}
			}

			SyncIndex(true);
			EnsureSelection();
		}

		private void OnItemAttached(int index, TItem item)
		{
			var itemSelected = GetIsItemSelected(item);

			if (HasSource)
			{
				// TODO Implement Sync mechanism to determine if item source is actually selected. 
				// TODO For example ListViewItem can have two way binding on IsSelected property to its DataContext and DataContext changes its selection state.
				// TODO In this case selected item source will stay in SelectionCollection and code below will behave incorrectly.
				// TODO Sync mechanism could be implemented through event in Selector control. As another option check if Item Selection property is bound to Data Context
				// TODO and perform immediate update on the binding to update Item IsSelected property.

				var source = GetSource(item);

				if (MultipleSelection)
					if (CurrentSelectionCollection.FindBySource(source, false, out var selection))
						if (ReferenceEquals(item, selection.Item) == false)
							CurrentSelectionCollection.UpdateSelection(selection.Index, selection.WithItem(item));

				if (ReferenceEquals(source, CurrentSelectedSource))
					if (ReferenceEquals(item, CurrentSelectedItem) == false)
						Sync(CurrentSelection.WithItem(item));

				{
					var itemSourceInSelection = IsSourceInSelection(source);

					if (itemSourceInSelection && itemSelected == false)
						SetItemSelected(item, true);

					if (itemSourceInSelection == false && itemSelected)
						SetItemSelected(item, false);
				}
			}
			else
			{
				if (SupportsIndex)
				{
					var selectedIndex = CurrentSelectedIndex;

					if (selectedIndex != -1)
					{
						var itemIndex = index == -1 ? GetIndexOfItem(item) : index;

						if (MultipleSelection)
							if (CurrentSelectionCollection.FindByIndex(index, false, out var selection))
								if (ReferenceEquals(item, selection.Item) == false)
									CurrentSelectionCollection.UpdateSelection(selection.Index, selection);

						if (selectedIndex == itemIndex)
							if (ReferenceEquals(item, CurrentSelectedItem) == false)
								Sync(CurrentSelection.WithItem(item));
					}
				}

				var itemInSelection = IsItemInSelection(item);

				if (itemInSelection && itemSelected == false)
					SetItemSelected(item, true);

				if (itemSelected && itemInSelection == false)
					SelectItem(item);
			}
		}

		private void Sync(Selection<TItem> selection)
		{
			if (IsSelectionChangeSuspended)
				SelectionResume = selection;
			else
				CommitSelection(selection);
		}

		public void SyncValue()
		{
			if (SelectedItem != null || SelectedSource != null)
				SelectValue(GetValue(SelectedItem, SelectedSource));
		}

		private void SyncIndex(int itemIndex)
		{
			if (itemIndex == CurrentSelectedIndex)
				return;

			if (MultipleSelection)
				if (CurrentSelectionCollection.FindByIndex(itemIndex, false, out var selection))
					if (itemIndex != selection.Index)
						CurrentSelectionCollection.UpdateSelection(itemIndex, selection);

			Sync(CurrentSelection.WithIndex(itemIndex));
		}

		private void SyncIndex(bool forItem)
		{
			if (SupportsIndex == false)
				return;

			SyncIndex(forItem ? GetIndexOfItem(CurrentSelectedItem) : GetIndexOfSource(CurrentSelectedSource));
		}

		private bool IsSourceInSelection(object source)
		{
			if (source == null)
				return false;

			if (ReferenceEquals(source, CurrentSelectedSource))
				return true;

			return MultipleSelection && CurrentSelectionCollection.ContainsSource(source);
		}

		private bool IsItemInSelection(TItem item)
		{
			if (item == null)
				return false;

			if (ReferenceEquals(item, CurrentSelectedItem))
				return true;

			return MultipleSelection && CurrentSelectionCollection.ContainsItem(item);
		}

		private bool IsSourceSelected(object source)
		{
			return false;
		}
	}
}