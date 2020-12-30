// <copyright file="SelectorController.Coerce.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.UI.Controls.Core
{
	internal abstract partial class SelectorController<TItem>
	{
		public int CoerceSelectedIndex(int selectedIndex)
		{
			if (SelectionHandling)
				return Selection.Index;

			var preselectIndex = PreselectIndex(selectedIndex, true, SelectionResume, out var preSelection);

			if (IsSelectionSuspended)
			{
				if (preselectIndex)
					SelectionResume = preSelection;

				PushSelectedIndexBoundValue(Selection.Index);

				return Selection.Index;
			}

			var coerceSelectedIndex = SelectionHandling || preselectIndex == false ? SelectedIndex : preSelection.Index;

			if (Equals(coerceSelectedIndex, selectedIndex))
				return coerceSelectedIndex;

			PushSelectedIndexBoundValue(coerceSelectedIndex);

			return coerceSelectedIndex;
		}

		public TItem CoerceSelectedItem(TItem selectedItem)
		{
			if (SelectionHandling)
				return Selection.Item;

			var preselectItem = PreselectItem(selectedItem, true, SelectionResume, out var preSelection);

			if (IsSelectionSuspended)
			{
				if (preselectItem)
					SelectionResume = preSelection;

				PushSelectedItemBoundValue(Selection.Item);

				return Selection.Item;
			}

			var coerceSelectedItem = preselectItem == false ? SelectedItem : preSelection.Item;

			if (Equals(coerceSelectedItem, selectedItem))
				return coerceSelectedItem;

			PushSelectedItemBoundValue(coerceSelectedItem);

			return coerceSelectedItem;
		}

		public object CoerceSelectedSource(object selectedSource)
		{
			if (SelectionHandling)
				return Selection.Source;

			var preselectSource = PreselectSource(selectedSource, true, SelectionResume, out var preSelection);

			if (IsSelectionSuspended)
			{
				if (preselectSource)
					SelectionResume = preSelection;

				PushSelectedSourceBoundValue(Selection.Source);

				return Selection.Source;
			}

			var coerceSelectedSource = preselectSource == false ? SelectedSource : preSelection.Source;

			if (Equals(coerceSelectedSource, selectedSource))
				return coerceSelectedSource;

			PushSelectedSourceBoundValue(coerceSelectedSource);

			return coerceSelectedSource;
		}

		public object CoerceSelectedValue(object selectedValue)
		{
			if (SelectionHandling)
				return Selection.Value;

			var preselectValue = PreselectValue(selectedValue, true, SelectionResume, out var preSelection);

			if (IsSelectionSuspended)
			{
				if (preselectValue)
					SelectionResume = preSelection;

				PushSelectedValueBoundValue(Selection.Value);

				return Selection.Value;
			}

			var coerceSelectedValue = SelectionHandling || preselectValue == false ? SelectedValue : preSelection.Value;

			if (Equals(coerceSelectedValue, selectedValue))
				return coerceSelectedValue;

			PushSelectedValueBoundValue(coerceSelectedValue);

			return coerceSelectedValue;
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
	}
}