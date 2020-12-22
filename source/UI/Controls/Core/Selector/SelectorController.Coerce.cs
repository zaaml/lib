// <copyright file="SelectorController.Coerce.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.UI.Controls.Core
{
	internal abstract partial class SelectorController<TItem>
	{
		public int CoerceSelectedIndex(int selectedIndex)
		{
			if (_selectionHandling)
				return CoerceSelection.Index;

			var preselectIndex = PreselectIndex(selectedIndex, true, SelectionResume, out var preSelection);

			if (IsSelectionChangeSuspended)
			{
				if (preselectIndex)
				{
					SelectionResume = preSelection;
					ModifySelectionCollection(preSelection);
				}

				CoerceSelection = Selection;
				PushSelectedIndexBoundValue(Selection.Index);

				return Selection.Index;
			}

			var coerceSelectedIndex = _selectionHandling || preselectIndex == false ? SelectedIndex : preSelection.Index;

			if (Equals(coerceSelectedIndex, selectedIndex))
				return coerceSelectedIndex;

			PushSelectedIndexBoundValue(coerceSelectedIndex);

			return coerceSelectedIndex;
		}

		public TItem CoerceSelectedItem(TItem selectedItem)
		{
			if (_selectionHandling)
				return CoerceSelection.Item;

			var preselectItem = PreselectItem(selectedItem, true, SelectionResume, out var preSelection);

			if (IsSelectionChangeSuspended)
			{
				if (preselectItem)
				{
					SelectionResume = preSelection;
					ModifySelectionCollection(preSelection);
				}

				CoerceSelection = Selection;
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
			if (_selectionHandling)
				return CoerceSelection.Source;

			var preselectSource = PreselectSource(selectedSource, true, SelectionResume, out var preSelection);

			if (IsSelectionChangeSuspended)
			{
				if (preselectSource)
				{
					SelectionResume = preSelection;
					ModifySelectionCollection(preSelection);
				}

				CoerceSelection = Selection;
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
			if (_selectionHandling)
				return CoerceSelection.Value;

			var preselectValue = PreselectValue(selectedValue, true, SelectionResume, out var preSelection);

			if (IsSelectionChangeSuspended)
			{
				if (preselectValue)
				{
					SelectionResume = preSelection;
					ModifySelectionCollection(preSelection);
				}

				CoerceSelection = Selection;
				PushSelectedValueBoundValue(Selection.Value);

				return Selection.Value;
			}

			var coerceSelectedValue = _selectionHandling || preselectValue == false ? SelectedValue : preSelection.Value;

			if (Equals(coerceSelectedValue, selectedValue))
				return coerceSelectedValue;

			PushSelectedValueBoundValue(coerceSelectedValue);

			return coerceSelectedValue;
		}

		private Selection<TItem> CoerceSelection { get; set; }
	}
}