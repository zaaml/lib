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

			if (TryGetItem(index, force, out var item))
			{
				if (CanSelect(item) == false)
					return false;
			}

			var source = GetSource(index);
			var itemValue = GetValue(item, source);

			preSelection = new Selection<TItem>(index, item, source, itemValue);

			return true;
		}

		private bool PreselectItem(TItem value, bool force, Selection<TItem> selection, out Selection<TItem> preSelection)
		{
			preSelection = selection;

			if (SupportsItem == false)
				return false;

			if (force == false && ReferenceEquals(selection.Item, value))
				return true;

			if (value == null)
				return PreselectNull(selection, out preSelection);

			var item = value;

			if (CanSelect(item) == false)
				return false;

			var index = GetIndexOfItem(item);
			var source = GetSource(item);
			var itemValue = GetValue(item, source);

			preSelection = new Selection<TItem>(index, item, source, itemValue);

			return true;
		}

		private bool PreselectSource(object value, bool force, Selection<TItem> selection, out Selection<TItem> preSelection)
		{
			preSelection = selection;

			if (SupportsSource == false)
				return false;

			if (force == false && ReferenceEquals(selection.Source, value))
				return true;

			var source = value;

			if (source == null)
				return PreselectNull(selection, out preSelection);

			if (SupportsIndex)
			{
				var index = GetIndexOfSource(source);

				if (index == -1)
					return PreselectNull(selection, out preSelection);

				if (TryGetItem(index, force, out var item))
				{
					if (CanSelect(item) == false)
						return false;
				}

				var itemValue = GetValue(item, source);

				preSelection = new Selection<TItem>(index, item, source, itemValue);

				return true;
			}
			else
			{
				if (TryGetItemBySource(source, true, out var item))
				{
					if (CanSelect(item) == false)
						return false;
				}

				var itemValue = GetValue(item, source);

				preSelection = new Selection<TItem>(-1, item, source, itemValue);

				return true;
			}
		}

		private bool PreselectNull(Selection<TItem> selection, out Selection<TItem> preSelection)
		{
			preSelection = selection;

			var count = Advisor.Count;

			if (AllowNullSelection || count <= 0)
			{
				preSelection = Selection<TItem>.Empty;

				return true;
			}

			for (var i = 0; i < count; i++)
			{
				if (TryGetItem(i, true , out var item))
				{
					if (CanSelect(item) == false)
						continue;
				}
				
				var source = GetSource(item);
				var value = GetValue(item, source);

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

			var itemValue = value;

			if (itemValue == null)
				return PreselectNull(selection, out preSelection);

			var index = GetIndexOfValue(itemValue);

			if (TryGetItem(index, force, out var item))
			{
				if (CanSelect(item) == false)
					return false;
			}

			var source = GetSource(index);

			preSelection = new Selection<TItem>(index, item, source, itemValue);

			return true;
		}
	}
}