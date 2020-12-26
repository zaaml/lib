// <copyright file="SelectorController.Preselect.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.UI.Controls.Core
{
	internal abstract partial class SelectorController<TItem>
	{
		private bool PreselectIndex(int value, bool force, Selection<TItem> selection, out Selection<TItem> preSelection)
		{
			preSelection = selection;

			if (SupportsIndex == false)
				return false;

			if (force == false && selection.Index == value)
				return true;

			var index = value;

			if (index == -1)
				return PreselectNull(selection, out preSelection);

			TryGetItem(index, force, out var item);
			var source = GetSource(index);
			var itemValue = GetValue(item, source);

			preSelection = new Selection<TItem>(index, item, source, itemValue);

			if (CanSelect(preSelection)) 
				return true;
			
			preSelection = Selection<TItem>.Empty;

			return false;
		}

		private bool PreselectItem(TItem item, bool force, Selection<TItem> selection, out Selection<TItem> preSelection)
		{
			preSelection = selection;

			if (SupportsItem == false)
				return false;

			if (force == false && ReferenceEquals(selection.Item, item))
				return true;

			if (item == null)
				return PreselectNull(selection, out preSelection);

			var source = GetSource(item);
			var index = GetIndexOfSource(source);

			if (index == -1)
				index = GetIndexOfItem(item);

			var itemValue = GetValue(item, source);

			preSelection = new Selection<TItem>(index, item, source, itemValue);

			if (CanSelect(preSelection))
				return true;

			preSelection = Selection<TItem>.Empty;

			return false;
		}

		private bool PreselectSource(object source, bool force, Selection<TItem> selection, out Selection<TItem> preSelection)
		{
			preSelection = selection;

			if (SupportsSource == false)
				return false;

			if (force == false && ReferenceEquals(selection.Source, source))
				return true;

			if (source == null)
				return PreselectNull(selection, out preSelection);

			TryGetItemBySource(source, force, out var item);
			var value = GetValue(item, source);
			var index = GetIndexOfSource(source);
			
			preSelection = new Selection<TItem>(index, item, source, value);

			if (CanSelect(preSelection))
				return true;

			preSelection = Selection<TItem>.Empty;

			return false;
		}

		private bool PreselectNull(Selection<TItem> selection, out Selection<TItem> preSelection)
		{
			preSelection = selection;

			var count = Count;

			if (AllowNullSelection || count <= 0)
			{
				preSelection = Selection<TItem>.Empty;

				return true;
			}

			for (var i = 0; i < count; i++)
			{
				if (CanSelectIndex(i) == false)
					continue;
				
				if (TryGetItem(i, true , out var item))
				{
					if (CanSelectItem(item) == false)
						continue;
				}
				
				var source = GetSource(item);
				
				if (CanSelectSource(source) == false)
					continue;
				
				var value = GetValue(item, source);

				if (CanSelectValue(value) == false)
					continue;
				
				preSelection = new Selection<TItem>(0, item, source, value);

				return true;
			}

			return false;
		}

		private bool PreselectValue(object value, bool force, Selection<TItem> selection, out Selection<TItem> preSelection)
		{
			preSelection = selection;

			if (SupportsValue == false)
				return false;

			if (force == false && CompareValues(selection.Value, value))
				return true;

			if (value == null)
				return PreselectNull(selection, out preSelection);

			var index = GetIndexOfValue(value);
			var source = GetSource(index);
			TryGetItem(index, force, out var item);
			
			preSelection = new Selection<TItem>(index, item, source, value);
			
			if (CanSelect(preSelection))
				return true;

			preSelection = Selection<TItem>.Empty;

			return false;
		}
	}
}