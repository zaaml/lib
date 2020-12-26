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
				return ForcedCoerceSelection.Index;

			var preselectIndex = PreselectIndex(selectedIndex, true, SelectionResume, out var preSelection);

			if (IsSelectionSuspended)
			{
				if (preselectIndex)
				{
					SelectionResume = preSelection;
					ModifySelectionCollection(preSelection);
				}

				ForcedCoerceSelection = Selection;
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
				return ForcedCoerceSelection.Item;

			var preselectItem = PreselectItem(selectedItem, true, SelectionResume, out var preSelection);

			if (IsSelectionSuspended)
			{
				if (preselectItem)
				{
					SelectionResume = preSelection;
					ModifySelectionCollection(preSelection);
				}

				ForcedCoerceSelection = Selection;
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
				return ForcedCoerceSelection.Source;

			var preselectSource = PreselectSource(selectedSource, true, SelectionResume, out var preSelection);

			if (IsSelectionSuspended)
			{
				if (preselectSource)
				{
					SelectionResume = preSelection;
					ModifySelectionCollection(preSelection);
				}

				ForcedCoerceSelection = Selection;
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
				return ForcedCoerceSelection.Value;

			var preselectValue = PreselectValue(selectedValue, true, SelectionResume, out var preSelection);

			if (IsSelectionSuspended)
			{
				if (preselectValue)
				{
					SelectionResume = preSelection;
					ModifySelectionCollection(preSelection);
				}

				ForcedCoerceSelection = Selection;
				PushSelectedValueBoundValue(Selection.Value);

				return Selection.Value;
			}

			var coerceSelectedValue = SelectionHandling || preselectValue == false ? SelectedValue : preSelection.Value;

			if (Equals(coerceSelectedValue, selectedValue))
				return coerceSelectedValue;

			PushSelectedValueBoundValue(coerceSelectedValue);

			return coerceSelectedValue;
		}

		private Selection<TItem> ForcedCoerceSelection { get; set; }
	}
}