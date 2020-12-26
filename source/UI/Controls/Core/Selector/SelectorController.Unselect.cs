// <copyright file="SelectorController.Unselect.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Zaaml.UI.Controls.Core
{
	internal abstract partial class SelectorController<TItem>
	{
		public void UnselectAll()
		{
			using (SelectionHandlingScope)
			{
				UnselectAllSafe();
				EnsureSelection();
				
				RaiseSelectionCollectionChanged(ResetNotifyCollectionChangedEventArgs);
			}
		}

		private void UnselectAllSafe()
		{
			VerifySafe();
			
			foreach (var selection in CurrentSelectionCollection)
				SetItemSelected(selection.Item, false);

			CurrentSelectionCollection.Clear();
			CommitSelectionSafe(Selection<TItem>.Empty);

			IsInverted = false;
		}

		public bool UnselectIndex(int index)
		{
			if (SelectionHandling)
				return false;

			using (SelectionHandlingScope)
				return UnselectIndexSafe(index);
		}

		private bool UnselectIndexSafe(int index)
		{
			VerifySafe();
			
			return IsInverted == false ? UnselectIndexCore(index, false) : EnsureInvertedSelection(SelectIndexCore(index, false));
		}

		private bool UnselectIndexCore(int index, bool force)
		{
			CurrentSelectionCollection.RemoveIndex(index);

			if (SelectedIndex == index)
			{
				if (MultipleSelection)
				{
					SelectFirst();

					return true;
				}

				return SelectIndex(-1);
			}

			EnsureSelection();

			return true;
		}

		public bool UnselectItem(TItem item)
		{
			if (SelectionHandling)
				return false;

			using (SelectionHandlingScope)
				return UnselectItemSafe(item);
		}

		private bool UnselectItemSafe(TItem item)
		{
			VerifySafe();

			return IsInverted == false ? UnselectItemCore(item, false) : EnsureInvertedSelection(SelectItemCore(item, false));
		}
		
		private bool UnselectItemCore(TItem item, bool force)
		{
			CurrentSelectionCollection.RemoveItem(item);

			if (ReferenceEquals(CurrentSelectedItem, item))
			{
				if (MultipleSelection)
				{
					SelectFirst();

					return true;
				}

				return SelectItem(null);
			}

			EnsureSelection();

			return true;
		}

		public bool UnselectSource(object source)
		{
			if (SelectionHandling)
				return false;

			using (SelectionHandlingScope)
				return UnselectSourceSafe(source);
		}

		private bool UnselectSourceSafe(object source)
		{
			VerifySafe();
			
			return IsInverted == false ? UnselectSourceCore(source, false) : EnsureInvertedSelection(SelectSourceCore(source, false));
		}

		public void UnselectSourceCollection(IEnumerable<object> sourceCollection)
		{
			using (SelectionHandlingScope)
			{
				if (IsInverted)
				{
					foreach (var source in sourceCollection)
					{
						if (CurrentSelectionCollection.FindBySource(source, false, out var selection) == false)
						{
							if (Advisor.TryCreateSelection(source, false, out var advisorSelection) && ReferenceEquals(source, advisorSelection.Source))
							{
								CurrentSelectionCollection.Add(advisorSelection, false);

								SetItemSelected(advisorSelection.Item, false);
							}
						}
					}
				}
				else
				{
					foreach (var source in sourceCollection)
					{
						if (CurrentSelectionCollection.TryRemoveSource(source, out var selection))
							SetItemSelected(selection.Item, false);
					}
				}

				RaiseSelectionCollectionChanged(ResetNotifyCollectionChangedEventArgs);

				SelectFirst();
			}

			EnsureSelection();
		}

		private bool UnselectSourceCore(object source, bool force)
		{
			CurrentSelectionCollection.RemoveSource(source);

			if (ReferenceEquals(SelectedSource, source))
			{
				if (MultipleSelection)
				{
					SelectFirst();

					return true;
				}

				return SelectSource(null);
			}

			EnsureSelection();

			return true;
		}

		public bool UnselectValue(object value)
		{
			if (SelectionHandling)
				return false;

			using (SelectionHandlingScope)
				return UnselectValueSafe(value);
		}

		private bool UnselectValueSafe(object value)
		{
			VerifySafe();
			
			return IsInverted == false ? UnselectValueCore(value, false) : EnsureInvertedSelection(SelectValueCore(value, false));
		}

		private bool UnselectValueCore(object value, bool force)
		{
			CurrentSelectionCollection.RemoveValue(value);

			if (ReferenceEquals(SelectedValue, value))
			{
				if (MultipleSelection)
				{
					SelectFirst();

					return true;
				}

				return SelectValue(null);
			}

			EnsureSelection();

			return true;
		}
	}
}