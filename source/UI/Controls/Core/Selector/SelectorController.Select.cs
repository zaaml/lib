// <copyright file="SelectorController.Select.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Linq;

namespace Zaaml.UI.Controls.Core
{
	internal abstract partial class SelectorController<TItem>
	{
		public void SelectAll()
		{
			if (MultipleSelection == false)
				return;

			using (SelectionHandlingScope)
			{
				UnselectAllSafe();
				InvertSelectionSafe();
			}

			EnsureSelection();
		}

		private void SelectCore(Selection<TItem> selection)
		{
			CurrentSelectionCollection.Select(selection);

			if (MultipleSelection)
			{
				SelectFirst();

				return;
			}

			ApplySelection(selection);

			Version++;
		}

		private void SelectFirst()
		{
			ApplySelection(CurrentSelectionCollection.Count > 0 ? CurrentSelectionCollection.First() : Selection<TItem>.Empty);

			Version++;
		}

		private void SelectFirstPossible()
		{
			var count = Count;

			for (var i = 0; i < count; i++)
			{
				if (Advisor.TryGetItem(i, true, out var item) == false || CanSelectItem(item) == false)
					continue;

				if (SelectItemCore(item, true))
					break;
			}
		}

		public bool SelectIndex(int index)
		{
			if (SelectionHandling)
				return false;

			using (SelectionHandlingScope)
				return SelectIndexSafe(index);
		}

		private bool SelectIndexCore(int index, bool force)
		{
			if (PreselectIndex(index, force, CurrentSelection, out var preSelection))
			{
				SelectCore(preSelection);

				return true;
			}

			return false;
		}

		private bool SelectIndexSafe(int index)
		{
			VerifySafe();

			return SelectIndexCore(index, false);
		}

		public bool SelectItem(TItem item)
		{
			if (SelectionHandling)
				return false;

			using (SelectionHandlingScope)
				return SelectItemSafe(item);
		}

		private bool SelectItemCore(TItem item, bool force)
		{
			if (PreselectItem(item, force, CurrentSelection, out var preSelection))
			{
				SelectCore(preSelection);

				return true;
			}

			return false;
		}

		private bool SelectItemSafe(TItem item)
		{
			VerifySafe();

			return SelectItemCore(item, false);
		}

		public bool SelectSource(object source)
		{
			if (SelectionHandling)
				return false;

			using (SelectionHandlingScope)
				return SelectSourceSafe(source);
		}

		public void SelectSourceCollection(IEnumerable<object> sourceCollection)
		{
			using (SelectionHandlingScope)
			{
				if (MultipleSelection)
				{
					foreach (var source in sourceCollection)
					{
						if (CurrentSelectionCollection.FindBySource(source, out _) == false)
						{
							if (Advisor.TryCreateSelection(source, false, out var selection) && selection.IsEmpty == false)
							{
								CurrentSelectionCollection.Select(selection);

								SetItemSelected(selection.Item, true);
							}
						}
					}

					SelectFirst();
				}
				else
					SelectSourceSafe(sourceCollection.FirstOrDefault());
			}
		}

		private bool SelectSourceCore(object source, bool force)
		{
			if (PreselectSource(source, force, CurrentSelection, out var preSelection))
			{
				SelectCore(preSelection);

				return true;
			}

			return false;
		}

		private bool SelectSourceSafe(object source)
		{
			VerifySafe();

			return SelectSourceCore(source, false);
		}

		public bool SelectValue(object value)
		{
			if (SelectionHandling)
				return false;

			using (SelectionHandlingScope)
				return SelectValueSafe(value);
		}

		private bool SelectValueCore(object value, bool force)
		{
			if (PreselectValue(value, force, CurrentSelection, out var preSelection))
			{
				SelectCore(preSelection);

				return true;
			}

			return false;
		}

		private bool SelectValueSafe(object value)
		{
			VerifySafe();

			return SelectValueCore(value, false);
		}

		public bool ToggleItem(TItem item)
		{
			return MultipleSelection && GetIsItemSelected(item) ? UnselectItem(item) : SelectItem(item);
		}
	}
}