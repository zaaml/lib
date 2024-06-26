// <copyright file="SelectorController.Sync.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Specialized;
using Zaaml.Core;
using Zaaml.Core.Collections;
using Zaaml.Core.Collections.Specialized;

namespace Zaaml.UI.Controls.Core
{
	internal abstract partial class SelectorController<TItem>
	{
		internal void AdvisorOnItemAttached(int index, TItem item)
		{
			using (SelectionHandlingScope)
			{
				OnItemAttachedSafe(index, item);
			}
		}

		internal void AdvisorOnItemCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			using (SelectionHandlingScope)
			{
				OnItemCollectionChangedSafe(e);
			}
		}

		internal void AdvisorOnItemCollectionSourceChanged(NotifyCollectionChangedEventArgs e)
		{
			using (SelectionHandlingScope)
			{
				OnItemCollectionSourceChangedSafe(e);
			}
		}

		internal void AdvisorOnItemDetached(int index, TItem item)
		{
			using (SelectionHandlingScope)
			{
				OnItemDetachedSafe(index, item);
			}
		}

		private bool IsItemInSelection(TItem item)
		{
			if (item == null)
				return false;

			if (EqualsItem(item, CurrentSelectedItem))
				return true;

			return MultipleSelection && CurrentSelectionCollection.ContainsItem(item);
		}

		private bool IsSourceInSelection(object source)
		{
			if (source == null)
				return false;

			if (EqualsSource(source, CurrentSelectedSource))
				return true;

			return MultipleSelection && CurrentSelectionCollection.ContainsSource(source);
		}

		private bool IsIndexInSelection(int index)
		{
			if (Count == 0 || index < 0 || index >= Count)
				return false;

			if (index == CurrentSelectedIndex)
				return true;

			return MultipleSelection && CurrentSelectionCollection.ContainsIndex(index);
		}

		private bool IsSourceSelected(object source)
		{
			return Advisor.GetSourceSelected(source);
		}

		private void OnItemAttachedSafe(int index, TItem item)
		{
			VerifySafe();

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
					if (CurrentSelectionCollection.FindBySource(source, out var selection))
						if (EqualsItem(item, selection.Item) == false)
							CurrentSelectionCollection.UpdateSelection(selection.Index, selection.WithItem(item));

				if (EqualsSource(source, CurrentSelectedSource))
					if (EqualsItem(item, CurrentSelectedItem) == false)
						Sync(CurrentSelection.WithItem(item));

				{
					var itemSourceInSelection = IsSourceInSelection(source);

					if (itemSourceInSelection && itemSelected == false)
					{
						if (CanSelectItem(item))
							SetItemSelected(item, true);
						else
							UnselectItemSafe(item);
					}

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
							if (CurrentSelectionCollection.FindByIndex(index, out var selection))
								if (EqualsItem(item, selection.Item) == false)
									CurrentSelectionCollection.UpdateSelection(selection.Index, selection);

						if (selectedIndex == itemIndex)
							if (EqualsItem(item, CurrentSelectedItem) == false)
								Sync(CurrentSelection.WithItem(item));
					}
				}

				var itemInSelection = IsItemInSelection(item);

				if (itemInSelection && itemSelected == false)
				{
					if (CanSelectItem(item))
						SetItemSelected(item, true);
					else
						UnselectItemSafe(item);
				}

				if (itemSelected && itemInSelection == false)
					SelectItemSafe(item);
			}
		}

		private void OnItemCollectionChangedSafe(NotifyCollectionChangedEventArgs e)
		{
			VerifySafe();

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
							SelectItemSafe(item);
					}
				}

				if (e.OldItems != null)
				{
					foreach (TItem item in e.OldItems)
					{
						if (IsItemInSelection(item))
							UnselectItemSafe(item);
					}
				}
			}

			SyncIndex(true);
			EnsureSelection();
		}

		private void OnItemCollectionSourceChangedSafe(NotifyCollectionChangedEventArgs e)
		{
			VerifySafe();

			var forceReset = e is NotifyCollectionChangedEventArgsEx { OriginalChangedItems: IRepeatCollection };

			if (e.Action == NotifyCollectionChangedAction.Move)
			{
			}
			else if (e.Action == NotifyCollectionChangedAction.Reset || forceReset)
			{
				if (SupportsIndex)
				{
					if (MultipleSelection)
					{
						if (SupportsSource)
						{
							for (var index = 0; index < Count; index++)
							{
								var source = GetSource(index);
								
								if (IsSourceSelected(source) && IsIndexInSelection(index) == false)
								{
									TryGetItemByIndex(index, false, out var item);

									var selection = new Selection<TItem>(index, item, source, GetValue(item, source));

									CurrentSelectionCollection.Select(selection);
								}
							}
						}

						CurrentSelectionCollection.UpdateIndicesSources();
					}

					if (CurrentSelectedIndex != -1)
					{
						var source = GetSource(CurrentSelectedIndex);

						Sync(source != null ? CurrentSelection.WithSource(source) : CurrentSelection.WithIndex(-1));
					}
				}
			}
			else
			{
				if (e.NewItems != null)
				{
					foreach (var source in e.NewItems)
					{
						if (IsSourceSelected(source))
							SelectSourceSafe(source);
					}
				}

				if (e.OldItems != null)
				{
					foreach (var source in e.OldItems)
					{
						if (IsSourceInSelection(source))
							UnselectSourceSafe(source);
					}
				}
			}

			SyncIndex(false);
			EnsureSelection();
		}

		private void OnItemDetachedSafe([UsedImplicitly] int index, TItem item)
		{
			VerifySafe();

			if (HasSource)
			{
				if (IsLocked(item) == false)
				{
					var source = GetSource(item);

					if (MultipleSelection)
						if (CurrentSelectionCollection.FindBySource(source, out var selection))
							if (EqualsItem(item, selection.Item))
								CurrentSelectionCollection.UpdateSelection(selection.Index, selection.WithItem(null));

					if (EqualsSource(source, CurrentSelectedSource))
						if (EqualsItem(item, CurrentSelectedItem))
							Sync(CurrentSelection.WithItem(null));
				}
			}
			else
			{
				if (IsVirtualizing == false)
					UnselectItemSafe(item);
			}
		}

		private void Sync(Selection<TItem> selection)
		{
			ApplySelection(selection);
		}

		private void SyncIndex(int itemIndex)
		{
			if (itemIndex == CurrentSelectedIndex)
				return;

			if (MultipleSelection)
				if (CurrentSelectionCollection.FindByIndex(itemIndex, out var selection))
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

		public void SyncValue()
		{
			using (SelectionHandlingScope)
			{
				var item = SelectionResume.Item;
				var source = SelectionResume.Source;

				if (item == null && source == null)
					return;

				var value = GetValue(item, source);

				SelectionResume = SelectionResume.WithValue(value);
			}
		}
	}
}