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
			ModifySelectionCollection(selection);

			if (MultipleSelection)
			{
				SelectFirst();

				return;
			}

			if (IsSelectionSuspended)
				SelectionResume = selection;
			else
				CommitSelection(selection);

			Version++;
		}

		private void SelectFirst()
		{
			var selection = CurrentSelectionCollection.Count > 0 ? CurrentSelectionCollection.First() : Selection<TItem>.Empty;

			if (IsSelectionSuspended)
				SelectionResume = selection;
			else
				CommitSelection(selection);

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
				return IsInverted == false ? SelectIndexCore(index, false) : EnsureInvertedSelection(UnselectIndexCore(index, false));
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

		public bool SelectItem(TItem item)
		{
			if (SelectionHandling)
				return false;
			
			using (SelectionHandlingScope)
				return IsInverted == false ? SelectItemCore(item, false) : EnsureInvertedSelection(UnselectItemCore(item, false));
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

		public bool SelectSource(object source)
		{
			if (SelectionHandling)
				return false;

			using (SelectionHandlingScope)
				return IsInverted == false ? SelectSourceCore(source, false) : EnsureInvertedSelection(UnselectSourceCore(source, false));
		}

		public void SelectSourceCollection(IEnumerable<object> sourceCollection)
		{
			using (SelectionHandlingScope)
			{
				if (MultipleSelection)
				{
					if (IsInverted)
					{
						foreach (var source in sourceCollection)
						{
							if (CurrentSelectionCollection.TryRemoveSource(source, out var selection))
								SetItemSelected(selection.Item, true);
						}
					}
					else
					{
						foreach (var source in sourceCollection)
						{
							if (CurrentSelectionCollection.FindBySource(source, false, out _) == false)
							{
								if (Advisor.TryCreateSelection(source, false, out var selection) && selection.IsEmpty == false)
								{
									CurrentSelectionCollection.Add(selection);

									SetItemSelected(selection.Item, true);
								}
							}
						}
					}

					RaiseSelectionCollectionChanged(ResetNotifyCollectionChangedEventArgs);
					SelectFirst();
				}
				else
					SelectSource(sourceCollection.FirstOrDefault());
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

		public bool SelectValue(object value)
		{
			if (SelectionHandling)
				return false;

			using (SelectionHandlingScope)
				return IsInverted == false ? SelectValueCore(value, false) : EnsureInvertedSelection(UnselectValueCore(value, false));
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
	}
}