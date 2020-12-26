// <copyright file="SelectorController.Commit.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.UI.Controls.Core
{
	internal abstract partial class SelectorController<TItem>
	{
		private void CommitSelection(Selection<TItem> selection)
		{
			using (SelectionHandlingScope)
			{
				CommitSelectionSafe(selection);
			}
		}

		private void CoerceSelection(bool ensureItem, ref Selection<TItem> selection)
		{
			var source = selection.Source;
			var item = selection.Item;
			var index = selection.Index;
			var value = selection.Value;

			if (source == null)
			{
				if (selection.Item != null)
					source = GetSource(item);
				else if (index != -1)
					source = GetSource(index);
			}

			if (item == null)
			{
				if (source != null)
					TryGetItemBySource(source, ensureItem, out item);

				if (item == null && index != -1)
					TryGetItem(index, ensureItem, out item);
			}

			if (index == -1)
			{
				if (source != null)
					index = GetIndexOfSource(source);

				if (index == -1 && item != null)
					index = GetIndexOfItem(item);
			}

			value ??= GetValue(item, source);

			selection = new Selection<TItem>(index, item, source, value);
		}

		private void CommitSelectionSafe(Selection<TItem> selection)
		{
			VerifySafe();

			var originalSelection = selection;

			CoerceSelection(false, ref selection);

			if (CanSelect(selection) == false)
			{
				CurrentSelectionCollection.Unselect(originalSelection, true);

				PreselectNull(Selection<TItem>.Empty, out selection);
			}

			var selectedItemChanged = false;
			var selectedIndexChanged = false;
			var selectedValueChanged = false;
			var selectedSourceChanged = false;

			var oldSelectedItem = SupportsItem ? ReadSelectedItem() : null;
			var oldSelectedIndex = SupportsIndex ? ReadSelectedIndex() : -1;
			var oldSelectedValue = SupportsValue ? ReadSelectedValue() : null;
			var oldSelectedSource = SupportsSource ? ReadSelectedSource() : null;

			TItem newSelectedItem = null;
			var newSelectedIndex = -1;
			object newSelectedValue = null;
			object newSelectedSource = null;

			ForcedCoerceSelection = selection;

			if (SupportsItem)
			{
				if (ReferenceEquals(oldSelectedItem, selection.Item) == false)
					WriteSelectedItem(selection.Item);

				newSelectedItem = ReadSelectedItem();
				selectedItemChanged = ReferenceEquals(oldSelectedItem, newSelectedItem) == false;
			}

			if (SupportsSource)
			{
				if (ReferenceEquals(oldSelectedSource, selection.Source) == false)
					WriteSelectedSource(selection.Source);

				newSelectedSource = ReadSelectedSource();
				selectedSourceChanged = ReferenceEquals(oldSelectedSource, newSelectedSource) == false;
			}

			if (SupportsIndex)
			{
				if (oldSelectedIndex != selection.Index)
					WriteSelectedIndex(selection.Index);

				newSelectedIndex = ReadSelectedIndex();
				selectedIndexChanged = oldSelectedIndex != newSelectedIndex;
			}

			if (SupportsValue)
			{
				if (CompareValues(oldSelectedValue, selection.Value) == false)
					WriteSelectedValue(selection.Value);

				newSelectedValue = ReadSelectedValue();
				selectedValueChanged = CompareValues(oldSelectedValue, newSelectedValue) == false;
			}

			var oldSelection = new Selection<TItem>(oldSelectedIndex, oldSelectedItem, oldSelectedSource, oldSelectedValue);
			var newSelection = new Selection<TItem>(newSelectedIndex, newSelectedItem, newSelectedSource, newSelectedValue);

			Selection = newSelection;

			if (selectedItemChanged)
			{
				if (oldSelectedItem != null)
				{
					if (MultipleSelection == false)
						SetItemSelected(oldSelectedItem, false);
				}

				if (newSelectedItem != null)
					SetItemSelected(newSelectedItem, true);
			}
			else
			{
				if (newSelectedItem != null && GetIsItemSelected(newSelectedItem) == false)
					SetItemSelected(newSelectedItem, true);
			}

			if (selectedItemChanged)
				RaiseOnSelectedItemChanged(oldSelectedItem, newSelectedItem);

			if (selectedSourceChanged)
				RaiseOnSelectedSourceChanged(oldSelectedSource, newSelectedSource);

			if (selectedIndexChanged)
				RaiseOnSelectedIndexChanged(oldSelectedIndex, newSelectedIndex);

			if (selectedValueChanged)
				RaiseOnSelectedValueChanged(oldSelectedValue, newSelectedValue);

			var raiseSelectionChanged = selectedItemChanged |
			                            selectedIndexChanged |
			                            selectedValueChanged |
			                            selectedSourceChanged;

			if (raiseSelectionChanged)
				RaiseOnSelectionChanged(oldSelection, newSelection);

			LockedItem = SelectedItem;
		}

		private void PushSelectedIndexBoundValue(int index)
		{
			using (SelectionHandlingScope)
			{
				PushSelectedIndexBoundValueCore(index);
			}
		}

		protected abstract void PushSelectedIndexBoundValueCore(object coerceSelectedIndex);

		private void PushSelectedItemBoundValue(TItem coerceSelectedItem)
		{
			using (SelectionHandlingScope)
			{
				PushSelectedItemBoundValueCore(coerceSelectedItem);
			}
		}

		protected abstract void PushSelectedItemBoundValueCore(TItem coerceSelectedItem);

		private void PushSelectedSourceBoundValue(object coerceSelectedSource)
		{
			using (SelectionHandlingScope)
			{
				PushSelectedSourceBoundValueCore(coerceSelectedSource);
			}
		}

		protected abstract void PushSelectedSourceBoundValueCore(object coerceSelectedSource);

		private void PushSelectedValueBoundValue(object coerceSelectedValue)
		{
			using (SelectionHandlingScope)
			{
				PushSelectedValueBoundValueCore(coerceSelectedValue);
			}
		}

		protected abstract void PushSelectedValueBoundValueCore(object coerceSelectedValue);

		private void RaiseOnSelectedIndexChanged(int oldIndex, int newIndex)
		{
			Selector.OnSelectedIndexChanged(oldIndex, newIndex);
		}

		private void RaiseOnSelectedItemChanged(TItem oldItem, TItem newItem)
		{
			Selector.OnSelectedItemChanged(oldItem, newItem);
		}

		private void RaiseOnSelectedSourceChanged(object oldISource, object newSource)
		{
			Selector.OnSelectedSourceChanged(oldISource, newSource);
		}

		private void RaiseOnSelectedValueChanged(object oldValue, object newValue)
		{
			Selector.OnSelectedValueChanged(oldValue, newValue);
		}

		private void RaiseOnSelectionChanged(Selection<TItem> oldSelection, Selection<TItem> newSelection)
		{
			Selector.OnSelectionChanged(oldSelection, newSelection);
		}

		protected abstract int ReadSelectedIndex();

		protected abstract TItem ReadSelectedItem();

		protected abstract object ReadSelectedSource();

		protected abstract object ReadSelectedValue();

		protected abstract void WriteSelectedIndex(int index);

		protected abstract void WriteSelectedItem(TItem item);

		protected abstract void WriteSelectedSource(object source);

		protected abstract void WriteSelectedValue(object value);
	}
}