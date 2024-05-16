// <copyright file="SelectorController.Commit.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Specialized;
using System.ComponentModel;
using Zaaml.PresentationCore;

namespace Zaaml.UI.Controls.Core
{
	internal abstract partial class SelectorController<TItem>
	{
		public event EventHandler<NotifyCollectionChangedEventArgs> SelectionCollectionChanged;
		public event PropertyChangedEventHandler SelectionCollectionPropertyChanged;

		private void ApplySelection(Selection<TItem> selection)
		{
			if (IsSelectionSuspended)
				SelectionResume = selection;
			else
			{
				using (SelectionHandlingScope)
					ApplySelectionSafe(selection);
			}
		}

		private void ApplySelectionSafe(Selection<TItem> selection)
		{
			VerifySafe();

			SelectionResume = selection;
		}

		private void CommitSelection()
		{
			if (SelectionHandlingCount != 0)
				throw new InvalidOperationException();

			try
			{
				SelectionHandlingCount = -1;

				CoerceSelectionResume();

				if (Selection.Equals(SelectionResume) && SelectionCollection.Version == SelectionCollectionResume.Version)
					return;

				var selection = SelectionResume;
				var oldSelection = Selection;

				var oldSelectedItem = oldSelection.Item;
				var oldSelectedIndex = oldSelection.Index;
				var oldSelectedValue = oldSelection.Value;
				var oldSelectedSource = oldSelection.Source;

				var newSelectedItem = selection.Item;
				var newSelectedIndex = selection.Index;
				var newSelectedValue = selection.Value;
				var newSelectedSource = selection.Source;

				Selection = selection;

				if (SupportsItem)
				{
					if (EqualsItem(ReadSelectedItem(), newSelectedItem) == false)
					{
						WriteSelectedItem(newSelectedItem);

						newSelectedItem = ReadSelectedItem();
					}
				}

				if (SupportsSource)
				{
					if (EqualsSource(ReadSelectedSource(), newSelectedSource) == false)
					{
						WriteSelectedSource(newSelectedSource);

						newSelectedSource = ReadSelectedSource();
					}
				}

				if (SupportsIndex)
				{
					if (ReadSelectedIndex() != newSelectedIndex)
					{
						WriteSelectedIndex(newSelectedIndex);

						newSelectedIndex = ReadSelectedIndex();
					}
				}

				if (SupportsValue)
				{
					if (EqualsValue(ReadSelectedValue(), newSelectedValue) == false)
					{
						WriteSelectedValue(newSelectedValue);

						newSelectedValue = ReadSelectedValue();
					}
				}

				var newSelection = new Selection<TItem>(newSelectedIndex, newSelectedItem, newSelectedSource, newSelectedValue);

				if (newSelection.Equals(Selection) == false)
					throw new InvalidOperationException();

				if (MultipleSelection)
				{
					foreach (var oldSelectionCollection in SelectionCollection)
					{
						if (SelectionCollectionResume.ContainsItem(oldSelectionCollection.Item) == false)
							SetItemSelected(oldSelectionCollection.Item, false);
					}

					foreach (var newSelectionCollection in SelectionCollectionResume)
					{
						if (SelectionCollection.ContainsItem(newSelectionCollection.Item) == false)
							SetItemSelected(newSelectionCollection.Item, true);
					}

					SelectionCollection.CopyFrom(SelectionCollectionResume);
					SelectionCollectionResume.Clear();
				}
				else if (EqualsItem(oldSelectedItem, newSelectedItem) == false)
				{
					SetItemSelected(oldSelectedItem, false);
					SetItemSelected(newSelectedItem, true);
				}

				var raiseSelectionChanged = false;

				if (EqualsItem(newSelectedItem, oldSelectedItem) == false)
				{
					RaiseOnSelectedItemChanged(oldSelectedItem, newSelectedItem);

					raiseSelectionChanged = true;
				}

				if (EqualsSource(newSelectedSource, oldSelectedSource) == false)
				{
					RaiseOnSelectedSourceChanged(oldSelectedSource, newSelectedSource);

					raiseSelectionChanged = true;
				}

				if (newSelectedIndex != oldSelectedIndex)
				{
					RaiseOnSelectedIndexChanged(oldSelectedIndex, newSelectedIndex);

					raiseSelectionChanged = true;
				}

				if (EqualsValue(oldSelectedValue, newSelectedValue) == false)
				{
					RaiseOnSelectedValueChanged(oldSelectedValue, newSelectedValue);

					raiseSelectionChanged = true;
				}

				if (raiseSelectionChanged)
					RaiseOnSelectionChanged(oldSelection, newSelection);

				LockedItem = SelectedItem;

				SelectionCollectionChanged?.Invoke(SelectionCollection, Constants.NotifyCollectionChangedReset);
				SelectionCollectionPropertyChanged?.Invoke(SelectionCollection, CountPropertyChangedEventArgs);
			}
			finally
			{
				SelectionHandlingCount = 0;
			}
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