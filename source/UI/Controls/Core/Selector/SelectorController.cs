// <copyright file="SelectorController.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Specialized;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;
using Zaaml.Core;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore.Extensions;

namespace Zaaml.UI.Controls.Core
{
	internal abstract partial class SelectorController<TItem> where TItem : FrameworkElement, ISelectable
	{
		private bool _allowNullSelection;
		private bool _multipleSelection;
		private bool _preferSelection;
		private Selection<TItem> _selection = Selection<TItem>.Empty;
		private bool _selectionHandling;
		private Selection<TItem> _selectionResume = Selection<TItem>.Empty;

		protected SelectorController(ISelector<TItem> selector, ISelectorAdvisor<TItem> advisor)
		{
			SelectionCollection = new SelectionCollectionImpl(this);
			SelectionCollectionResume = new SelectionCollectionImpl(this);

			Selector = selector;

			Advisor = advisor;
			Advisor.Controller = this;
		}

		private int ActualPreferredIndex => Count > 0 ? PreferredIndex.Clamp(0, Count - 1) : -1;

		private ISelectorAdvisor<TItem> Advisor { get; }

		public bool AllowNullSelection
		{
			get => _allowNullSelection;
			set
			{
				if (_allowNullSelection == value)
					return;

				_allowNullSelection = value;

				EnsureSelection();
			}
		}

		private Selection<TItem> CoerceSelection { get; set; }

		internal int Count => Advisor.Count;

		public int CurrentSelectedIndex => CurrentSelection.Index;

		public TItem CurrentSelectedItem => CurrentSelection.Item;

		public object CurrentSelectedItemSource => CurrentSelection.ItemSource;

		public object CurrentSelectedValue => CurrentSelection.Value;

		private Selection<TItem> CurrentSelection => IsSelectionChangeSuspended ? SelectionResume : Selection;

		private SelectionCollectionImpl CurrentSelectionCollection => IsSelectionChangeSuspended ? SelectionCollectionResume : SelectionCollection;

		public bool IsSelectionChangeSuspended => SuspendSelectionChangeCount > 0;

		public bool MultipleSelection
		{
			get => _multipleSelection;
			set
			{
				if (_multipleSelection == value)
					return;

				_multipleSelection = value;

				if (_multipleSelection == false)
					ResetMultipleSelection();
			}
		}

		public int PreferredIndex { get; set; }

		public bool PreferSelection
		{
			get => _preferSelection;
			set
			{
				if (_preferSelection == value)
					return;

				_preferSelection = value;

				EnsureSelection();
			}
		}

		public int ResumeSelectedIndex => SelectionResume.Index;

		public TItem ResumeSelectedItem => SelectionResume.Item;

		public object ResumeSelectedValue => SelectionResume.Value;

		public int SelectedIndex => Selection.Index;

		public TItem SelectedItem => Selection.Item;

		public object SelectedItemSource => Selection.ItemSource;

		public object SelectedValue => Selection.Value;

		private Selection<TItem> Selection
		{
			get => _selection;
			set
			{
				if (AreSelectionEquals(_selection, value))
					return;

				if (value.Index != -1)
					PreferredIndex = value.Index;

				_selection = value;
			}
		}

		private SelectionCollectionImpl SelectionCollection { get; }

		private SelectionCollectionImpl SelectionCollectionResume { get; }

		private Selection<TItem> SelectionResume
		{
			get => _selectionResume;
			set
			{
				if (AreSelectionEquals(_selectionResume, value))
					return;

				if (value.Index != -1)
					PreferredIndex = value.Index;

				_selectionResume = value;
			}
		}

		public SelectNextMode SelectNextMode { get; set; }

		public ISelector<TItem> Selector { get; }

		protected abstract bool SupportsIndex { get; }

		protected abstract bool SupportsItem { get; }

		protected abstract bool SupportsItemSource { get; }

		protected abstract bool SupportsValue { get; }

		public int SuspendSelectionChangeCount { get; private set; }

		internal void AdvisorOnItemAttached(int index, TItem item)
		{
			OnItemAttached(index, item);
		}

		internal void AdvisorOnItemCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			OnItemCollectionChanged(e);
		}

		internal void AdvisorOnItemCollectionSourceChanged(NotifyCollectionChangedEventArgs e)
		{
			OnItemCollectionSourceChanged(e);
		}

		internal void AdvisorOnItemDetached(int index, TItem item)
		{
			OnItemDetached(index, item);
		}

		private bool AreSelectionEquals(Selection<TItem> first, Selection<TItem> second)
		{
			return first.Index == second.Index &&
			       ReferenceEquals(first.Item, second.Item) &&
			       ReferenceEquals(first.ItemSource, second.ItemSource) &&
			       CompareValues(first.Value, second.Value);
		}

		private static bool CanSelect(TItem item)
		{
			return !(item is ISelectableEx selectableEx) || selectableEx.CanSelect;
		}

		public int CoerceSelectedIndex(int selectedIndex)
		{
			if (_selectionHandling)
				return CoerceSelection.Index;

			var preselectIndex = PreselectIndex(selectedIndex, true, SelectionResume, out var preSelection);

			if (IsSelectionChangeSuspended)
			{
				if (preselectIndex)
					SelectionResume = preSelection;

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
					SelectionResume = preSelection;

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

		public object CoerceSelectedItemSource(object selectedItemSource)
		{
			if (_selectionHandling)
				return CoerceSelection.ItemSource;

			var preselectItemSource = PreselectItemSource(selectedItemSource, true, SelectionResume, out var preSelection);

			if (IsSelectionChangeSuspended)
			{
				if (preselectItemSource)
					SelectionResume = preSelection;

				CoerceSelection = Selection;
				PushSelectedItemSourceBoundValue(Selection.ItemSource);

				return Selection.ItemSource;
			}

			var coerceSelectedItemSource = preselectItemSource == false ? SelectedItemSource : preSelection.ItemSource;

			if (Equals(coerceSelectedItemSource, selectedItemSource))
				return coerceSelectedItemSource;

			PushSelectedItemSourceBoundValue(coerceSelectedItemSource);

			return coerceSelectedItemSource;
		}

		public object CoerceSelectedValue(object selectedValue)
		{
			if (_selectionHandling)
				return CoerceSelection.Value;

			var preselectValue = PreselectValue(selectedValue, true, SelectionResume, out var preSelection);

			if (IsSelectionChangeSuspended)
			{
				if (preselectValue)
					SelectionResume = preSelection;

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

		private void CommitSelection(Selection<TItem> selection, bool applySelection, bool addToSelection)
		{
			try
			{
				_selectionHandling = true;

				var selectedItemChanged = false;
				var selectedIndexChanged = false;
				var selectedValueChanged = false;
				var selectedItemSourceChanged = false;

				var oldSelectedItem = SupportsItem ? ReadSelectedItem() : default;
				var oldSelectedIndex = SupportsIndex ? ReadSelectedIndex() : default;
				var oldSelectedValue = SupportsValue ? ReadSelectedValue() : default;
				var oldSelectedItemSource = SupportsItemSource ? ReadSelectedItemSource() : default;

				TItem newSelectedItem = default;
				int newSelectedIndex = default;
				object newSelectedValue = default;
				object newSelectedItemSource = default;

				CoerceSelection = selection;

				if (SupportsItem)
				{
					if (ReferenceEquals(oldSelectedItem, selection.Item) == false)
						WriteSelectedItem(selection.Item);

					newSelectedItem = ReadSelectedItem();
					selectedItemChanged = ReferenceEquals(oldSelectedItem, newSelectedItem) == false;
				}

				if (SupportsItemSource)
				{
					if (ReferenceEquals(oldSelectedItemSource, selection.ItemSource) == false)
						WriteSelectedItemSource(selection.ItemSource);

					newSelectedItemSource = ReadSelectedItemSource();
					selectedItemSourceChanged = ReferenceEquals(oldSelectedItemSource, newSelectedItemSource) == false;
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

				var oldSelection = new Selection<TItem>(oldSelectedIndex, oldSelectedItem, oldSelectedItemSource, oldSelectedValue);
				var newSelection = new Selection<TItem>(newSelectedIndex, newSelectedItem, newSelectedItemSource, newSelectedValue);

				Selection = newSelection;

				if (selectedItemChanged)
				{
					if (oldSelectedItem != null)
					{
						Advisor.Unlock(oldSelectedItem);

						if (applySelection && addToSelection == false)
							SetItemSelection(oldSelectedItem, false);
					}

					if (newSelectedItem != null)
					{
						Advisor.Lock(newSelectedItem);

						if (applySelection)
							SetItemSelection(newSelectedItem, true);
					}
				}

				if (selectedItemChanged)
					RaiseOnSelectedItemChanged(oldSelectedItem, newSelectedItem);

				if (selectedItemSourceChanged)
					RaiseOnSelectedItemSourceChanged(oldSelectedItemSource, newSelectedItemSource);

				if (selectedIndexChanged)
					RaiseOnSelectedIndexChanged(oldSelectedIndex, newSelectedIndex);

				if (selectedValueChanged)
					RaiseOnSelectedValueChanged(oldSelectedValue, newSelectedValue);

				var raiseSelectionChanged = selectedItemChanged |
				                            selectedIndexChanged |
				                            selectedValueChanged |
				                            selectedItemSourceChanged;

				if (raiseSelectionChanged)
					RaiseOnSelectionChanged(oldSelection, newSelection);
			}
			finally
			{
				_selectionHandling = false;
			}
		}

		private bool CompareValues(object itemValue, object value)
		{
			return Advisor.CompareValues(itemValue, value);
		}

		internal void EnsureSelection()
		{
			if (IsSelectionChangeSuspended)
				return;

			if (AllowNullSelection && PreferSelection == false)
				return;

			if (SupportsIndex)
			{
				if (SelectedIndex == -1)
					SelectIndexCore(ActualPreferredIndex, true, false);
			}
			else if (SupportsItem)
			{
				if (SelectedItem != null)
					return;

				for (var i = 0; i < Advisor.Count; i++)
				{
					if (Advisor.TryGetItem(i, out var item) == false || CanSelect(item) == false)
						continue;

					SelectItemCore(item, true, false);

					if (ReferenceEquals(SelectedItem, item))
						break;
				}
			}
		}

		private int GetIndexOfItem(TItem item)
		{
			return SupportsItem && item != null && Count > 0 ? Advisor.GetIndexOfItem(item) : -1;
		}

		private int GetIndexOfItemSource(object itemSource)
		{
			return SupportsItemSource && itemSource != null && Count > 0 ? Advisor.GetIndexOfItemSource(itemSource) : -1;
		}

		private int GetIndexOfValue(object value)
		{
			return SupportsValue && Count > 0 ? Advisor.GetIndexOfValue(value) : -1;
		}

		private object GetItemSource(int index)
		{
			return SupportsItemSource && IsWithinRange(index) ? Advisor.GetItemSource(index) : null;
		}

		private object GetItemSource(TItem item)
		{
			return SupportsItemSource && item != null ? Advisor.GetItemSource(item) : null;
		}

		internal SelectionCollectionImpl.SelectionCollectionEnumerator GetSelectionCollectionEnumerator()
		{
			return CurrentSelectionCollection.GetEnumerator();
		}

		private object GetValue(TItem item, object itemSource)
		{
			return SupportsValue ? Selector.GetValue(item, itemSource) : null;
		}

		private bool IsIndexInSelection(int index)
		{
			if (index == -1)
				return false;

			if (index == CurrentSelectedIndex)
				return true;

			return MultipleSelection && CurrentSelectionCollection.ContainsIndex(index);
		}

		private bool IsIndexSelected(int index)
		{
			return false;
		}

		private bool IsItemInSelection(TItem item)
		{
			if (item == null)
				return false;

			if (ReferenceEquals(item, CurrentSelectedItem))
				return true;

			return MultipleSelection && CurrentSelectionCollection.ContainsItem(item);
		}

		private bool IsItemSelected(TItem item)
		{
			return item != null && item.IsSelected;
		}

		private bool IsItemSourceInSelection(object itemSource)
		{
			if (itemSource == null)
				return false;

			if (ReferenceEquals(itemSource, CurrentSelectedItemSource))
				return true;

			return MultipleSelection && CurrentSelectionCollection.ContainsItemSource(itemSource);
		}

		private bool IsItemSourceSelected(object itemSource)
		{
			return false;
		}

		private bool IsValueInSelection(object value)
		{
			if (value == null)
				return false;

			if (CompareValues(value, CurrentSelectedValue))
				return true;

			return MultipleSelection && CurrentSelectionCollection.ContainsValue(value);
		}

		private bool IsValueSelected(object value)
		{
			return false;
		}

		private bool IsWithinRange(int index)
		{
			return index >= 0 && Count != 0 && index < Count;
		}

		private void OnItemAttached(int index, TItem item)
		{
			if (SupportsIndex)
			{
				var selectedIndex = IsSelectionChangeSuspended ? SelectionResume.Index : SelectedIndex;

				if (selectedIndex == -1)
					return;

				var itemIndex = index;

				if (itemIndex == -1)
					itemIndex = GetIndexOfItem(item);

				if (IsSelectionChangeSuspended)
				{
					if (SelectionResume.Index == itemIndex)
						SelectionResume = new Selection<TItem>(SelectionResume.Index, item, SelectionResume.ItemSource, SelectionResume.Value);
				}
				else if (SelectedIndex == itemIndex)
					UpdateSelectedItem(new Selection<TItem>(SelectedIndex, item, SelectedItemSource, SelectedValue));
			}
			else if (SupportsItem)
			{
				if (IsItemSelected(item) && IsItemInSelection(item) == false)
					SelectItem(item, true);
			}
		}

		private void OnItemCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Reset)
			{
			}
			else if (e.Action == NotifyCollectionChangedAction.Move)
			{
			}
			else
			{
				if (e.NewItems != null)
				{
					foreach (TItem item in e.NewItems)
					{
						if (IsItemSelected(item))
							SelectItem(item);
					}
				}

				if (e.OldItems != null)
				{
					foreach (TItem item in e.OldItems)
					{
						if (IsItemInSelection(item))
							UnselectItem(item);
					}
				}
			}

			UpdateSelectedIndex(true);
			EnsureSelection();
		}

		private void OnItemCollectionSourceChanged(NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Move)
			{
			}
			else if (e.Action == NotifyCollectionChangedAction.Reset)
			{
			}
			else
			{
				if (e.NewItems != null)
				{
					foreach (var itemSource in e.NewItems)
					{
						if (IsItemSourceSelected(itemSource))
							SelectItemSource(itemSource);
					}
				}

				if (e.OldItems != null)
				{
					foreach (var itemSource in e.OldItems)
					{
						if (IsItemSourceInSelection(itemSource))
							UnselectItemSource(itemSource);
					}
				}
			}

			UpdateSelectedIndex(false);
			EnsureSelection();
		}

		private void OnItemDetached([UsedImplicitly] int index, TItem item)
		{
			if (ReferenceEquals(SelectedItem, item))
				SelectIndexCore(ActualPreferredIndex, true, false);
		}

		public void OnItemSelectionChanged(TItem item)
		{
			if (_selectionHandling)
				return;

			var isCurrent = IsItemInSelection(item);
			var isSelected = IsItemSelected(item);

			SuspendSelectionChange();

			if (isSelected == false && isCurrent)
				UnselectItem(item);

			if (isSelected && isCurrent == false)
				SelectItem(item);

			ResumeSelectionChange();
		}

		public void OnSelectedIndexPropertyChanged(int oldIndex, int newIndex)
		{
			if (_selectionHandling)
				return;

			SuspendSelectionChange();

			UnselectIndex(oldIndex);
			SelectIndex(newIndex);

			ResumeSelectionChange();
		}

		public void OnSelectedItemPropertyChanged(TItem oldItem, TItem newItem)
		{
			if (_selectionHandling)
				return;

			SuspendSelectionChange();

			UnselectItem(oldItem);
			SelectItem(newItem);

			ResumeSelectionChange();
		}

		public void OnSelectedItemSourcePropertyChanged(object oldItemSource, object newItemSource)
		{
			if (_selectionHandling)
				return;

			SuspendSelectionChange();

			UnselectItemSource(oldItemSource);
			SelectItemSource(newItemSource);

			ResumeSelectionChange();
		}

		public void OnSelectedValuePropertyChanged(object oldValue, object newValue)
		{
			if (_selectionHandling)
				return;

			SuspendSelectionChange();

			UnselectValue(oldValue);
			SelectValue(newValue);

			ResumeSelectionChange();
		}

		private void OnSelectionChangeSuspended()
		{
			SelectionResume = Selection;
		}

		private bool PreselectIndex(int value, bool force, Selection<TItem> selection, out Selection<TItem> preSelection)
		{
			preSelection = selection;

			if (SupportsIndex == false)
				return false;

			if (force == false && selection.Index == value)
				return false;

			var index = value;

			if (index == -1)
				return PreselectNull(selection, out preSelection);

			if (TryGetItem(index, out var item))
			{
				if (CanSelect(item) == false)
					return false;
			}

			var itemSource = GetItemSource(index);
			var itemValue = GetValue(item, itemSource);

			preSelection = new Selection<TItem>(index, item, itemSource, itemValue);

			return true;
		}

		private bool PreselectItem(TItem value, bool force, Selection<TItem> selection, out Selection<TItem> preSelection)
		{
			preSelection = selection;

			if (SupportsItem == false)
				return false;

			if (force == false && ReferenceEquals(selection.Item, value))
				return false;

			if (value == null)
				return PreselectNull(selection, out preSelection);

			var item = value;

			if (CanSelect(item) == false)
				return false;

			var index = GetIndexOfItem(item);
			var itemSource = GetItemSource(item);
			var itemValue = GetValue(item, itemSource);

			preSelection = new Selection<TItem>(index, item, itemSource, itemValue);

			return true;
		}

		private bool PreselectItemSource(object value, bool force, Selection<TItem> selection, out Selection<TItem> preSelection)
		{
			preSelection = selection;

			if (SupportsItemSource == false)
				return false;

			if (force == false && ReferenceEquals(selection.ItemSource, value))
				return false;

			var itemSource = value;

			if (itemSource == null)
				return PreselectNull(selection, out preSelection);

			if (SupportsIndex)
			{
				var index = GetIndexOfItemSource(itemSource);

				if (index == -1)
					return PreselectNull(selection, out preSelection);

				if (TryGetItem(index, out var item))
				{
					if (CanSelect(item) == false)
						return false;
				}

				var itemValue = GetValue(item, itemSource);

				preSelection = new Selection<TItem>(index, item, itemSource, itemValue);

				return true;
			}
			else
			{
				if (TryGetItemBySource(itemSource, out var item))
				{
					if (CanSelect(item) == false)
						return false;
				}

				var itemValue = GetValue(item, itemSource);

				preSelection = new Selection<TItem>(-1, item, itemSource, itemValue);

				return true;
			}
		}

		private bool PreselectNull(Selection<TItem> selection, out Selection<TItem> preSelection)
		{
			preSelection = selection;

			var count = Advisor.Count;

			if (AllowNullSelection || count <= 0)
				return false;

			for (var i = 0; i < count; i++)
			{
				if (TryGetItem(i, out var item))
				{
					if (CanSelect(item) == false)
						continue;
				}

				var itemSource = GetItemSource(item);
				var value = GetValue(item, itemSource);

				preSelection = new Selection<TItem>(0, item, itemSource, value);

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
				return false;

			var itemValue = value;

			if (itemValue == null)
				return PreselectNull(selection, out preSelection);

			var index = GetIndexOfValue(itemValue);

			if (TryGetItem(index, out var item))
			{
				if (CanSelect(item) == false)
					return false;
			}

			var itemSource = GetItemSource(index);

			preSelection = new Selection<TItem>(index, item, itemSource, itemValue);

			return true;
		}

		private void PushSelectedIndexBoundValue(int index)
		{
			try
			{
				_selectionHandling = true;

				PushSelectedIndexBoundValueCore(index);
			}
			finally
			{
				_selectionHandling = false;
			}
		}

		protected abstract void PushSelectedIndexBoundValueCore(object coerceSelectedIndex);

		private void PushSelectedItemBoundValue(TItem coerceSelectedItem)
		{
			try
			{
				_selectionHandling = true;

				PushSelectedItemBoundValueCore(coerceSelectedItem);
			}
			finally
			{
				_selectionHandling = false;
			}
		}

		protected abstract void PushSelectedItemBoundValueCore(TItem coerceSelectedItem);

		private void PushSelectedItemSourceBoundValue(object coerceSelectedItemSource)
		{
			try
			{
				_selectionHandling = true;

				PushSelectedItemSourceBoundValueCore(coerceSelectedItemSource);
			}
			finally
			{
				_selectionHandling = false;
			}
		}

		protected abstract void PushSelectedItemSourceBoundValueCore(object coerceSelectedItemSource);

		private void PushSelectedValueBoundValue(object coerceSelectedValue)
		{
			try
			{
				_selectionHandling = true;

				PushSelectedValueBoundValueCore(coerceSelectedValue);
			}
			finally
			{
				_selectionHandling = false;
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

		private void RaiseOnSelectedItemSourceChanged(object oldItemSource, object newItemSource)
		{
			Selector.OnSelectedItemSourceChanged(oldItemSource, newItemSource);
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

		protected abstract object ReadSelectedItemSource();

		protected abstract object ReadSelectedValue();

		private void ResetMultipleSelection()
		{
		}

		public void RestoreSelection()
		{
			if (IsSelectionChangeSuspended)
				SelectionResume = Selection;
		}

		public void ResumeSelectionChange()
		{
			ResumeSelectionChange(true);
		}

		public void ResumeSelectionChange(bool applySelection)
		{
			if (_selectionHandling)
				return;

			if (SuspendSelectionChangeCount > 0)
				SuspendSelectionChangeCount--;
			else
				return;

			if (SuspendSelectionChangeCount > 0)
				return;

			CommitSelection(SelectionResume, applySelection, SelectionCollectionResume.ContainsSelection(SelectionResume));
			EnsureSelection();
		}

		private int Version { get; set; }
		
		private void SelectCore(Selection<TItem> selection, bool addToSelection)
		{
			if (IsSelectionChangeSuspended)
				SelectionResume = selection;
			else
				CommitSelection(selection, true, addToSelection);

			if (addToSelection)
				CurrentSelectionCollection.Add(selection);
			else
				CurrentSelectionCollection.Clear();

			Version++;
		}

		private void RaiseSelectionCollectionChanged()
		{
			if (IsSelectionChangeSuspended)
				return;
			
			
		}

		private void UpdateSelectedItem(Selection<TItem> selection)
		{
			if (ReferenceEquals(CurrentSelectedItemSource, selection.ItemSource)) 
				CommitSelection(selection, true, true);

			CurrentSelectionCollection.UpdateSelectedItem(selection);
		}

		public bool SelectIndex(int index)
		{
			return SelectIndexCore(index, false, false);
		}

		public bool SelectIndex(int index, bool addToSelection)
		{
			return SelectIndexCore(index, false, addToSelection);
		}

		private bool SelectIndexCore(int index, bool force, bool addToSelection)
		{
			if (_selectionHandling)
				return false;

			if (PreselectIndex(index, force, CurrentSelection, out var preSelection))
			{
				SelectCore(preSelection, addToSelection);

				return true;
			}

			return false;
		}

		public bool SelectItem(TItem item)
		{
			return SelectItemCore(item, false, false);
		}

		public bool SelectItem(TItem item, bool addToSelection)
		{
			return SelectItemCore(item, false, addToSelection);
		}

		private bool SelectItemCore(TItem item, bool force, bool addToSelection)
		{
			if (_selectionHandling)
				return false;

			if (PreselectItem(item, force, CurrentSelection, out var preSelection))
			{
				SelectCore(preSelection, addToSelection);

				return true;
			}

			return false;
		}

		public bool SelectItemSource(object itemSource)
		{
			return SelectItemSourceCore(itemSource, false, false);
		}

		public bool SelectItemSource(object itemSource, bool addToSelection)
		{
			return SelectItemSourceCore(itemSource, false, addToSelection);
		}

		private bool SelectItemSourceCore(object itemSource, bool force, bool addToSelection)
		{
			if (_selectionHandling)
				return false;

			if (PreselectItemSource(itemSource, force, CurrentSelection, out var preSelection))
			{
				SelectCore(preSelection, addToSelection);

				return true;
			}

			return false;
		}

		public int SelectNext(int index, SelectNextMode mode)
		{
			if (Count == 0)
				index = -1;
			else if (Count == 1)
				index = 0;
			else
			{
				switch (mode)
				{
					case SelectNextMode.First:

						index = 0;

						break;

					case SelectNextMode.PrevOrNearest:

						index = index > 0 ? index - 1 : index + 1;

						break;

					case SelectNextMode.Prev:

						index = index == 0 ? Count - 1 : index - 1;

						break;

					case SelectNextMode.Next:

						index = index == Count - 1 ? 0 : index + 1;

						break;

					case SelectNextMode.NextOrNearest:

						index = index < Count - 1 ? index + 1 : index - 1;

						break;

					case SelectNextMode.Last:

						index = Count - 1;

						break;
				}
			}

			return index;
		}

		public void SelectNext(bool force = false)
		{
			SelectIndexCore(SelectNext(SelectedIndex, SelectNextMode), force, false);
		}

		public bool SelectValue(object value)
		{
			return SelectValueCore(value, false, false);
		}

		public bool SelectValue(object value, bool addToSelection)
		{
			return SelectValueCore(value, false, addToSelection);
		}

		private bool SelectValueCore(object value, bool force, bool addToSelection)
		{
			if (_selectionHandling)
				return false;

			if (PreselectValue(value, force, CurrentSelection, out var preSelection))
			{
				SelectCore(preSelection, addToSelection);

				return true;
			}

			return false;
		}

		private void SetItemSelection(TItem item, bool selection)
		{
			item.IsSelected = selection;
		}

		public void SuspendSelectionChange()
		{
			if (_selectionHandling)
				return;

			SuspendSelectionChangeCount++;

			if (SuspendSelectionChangeCount == 1)
				OnSelectionChangeSuspended();
		}

		private bool TryGetItem(int index, out TItem item)
		{
			if (SupportsItem == false || IsWithinRange(index) == false)
			{
				item = default;

				return false;
			}

			return Advisor.TryGetItem(index, out item);
		}

		private bool TryGetItemBySource(object itemSource, out TItem item)
		{
			if (SupportsItemSource == false || itemSource == null)
			{
				item = default;

				return false;
			}

			return Advisor.TryGetItemBySource(itemSource, out item);
		}

		public void UnselectIndex(int index)
		{
			if (_selectionHandling)
				return;

			if (SelectedIndex == index)
				SelectIndex(-1);

			EnsureSelection();
		}

		public void UnselectItem(TItem item)
		{
			if (_selectionHandling)
				return;

			if (ReferenceEquals(SelectedItem, item))
				SelectItem(null);

			EnsureSelection();
		}

		public void UnselectItemSource(object itemSource)
		{
			if (_selectionHandling)
				return;

			if (ReferenceEquals(SelectedItemSource, itemSource))
				SelectItemSource(null);

			EnsureSelection();
		}

		public void UnselectValue(object value)
		{
			if (_selectionHandling)
				return;

			if (ReferenceEquals(SelectedValue, value))
				SelectValue(null);

			EnsureSelection();
		}

		private void UpdateSelectedIndex(bool forItem)
		{
			if (SupportsIndex == false)
				return;

			if (forItem)
			{
				var itemIndex = SelectedItem != null ? GetIndexOfItem(SelectedItem) : -1;

				UpdateSelectedIndex(itemIndex);
			}
			else
			{
				var itemSourceIndex = SelectedItemSource != null ? GetIndexOfItemSource(SelectedItemSource) : -1;

				UpdateSelectedIndex(itemSourceIndex);
			}
		}

		private void UpdateSelectedIndex(int itemIndex)
		{
			if (itemIndex == SelectedIndex)
				return;

			SuspendSelectionChange();

			if (SelectedIndex != -1)
				UnselectIndex(SelectedIndex);

			if (itemIndex != -1)
				SelectIndex(itemIndex);

			ResumeSelectionChange();
		}

		public void UpdateSelectedValue()
		{
			if (SupportsIndex && SelectedIndex == -1)
				return;

			SelectValue(GetValue(SelectedItem, SelectedItemSource));
		}

		protected abstract void WriteSelectedIndex(int index);

		protected abstract void WriteSelectedItem(TItem item);

		protected abstract void WriteSelectedItemSource(object itemSource);

		protected abstract void WriteSelectedValue(object value);
	}

	internal class SelectorController<TSelector, TItem> : SelectorController<TItem>
		where TSelector : FrameworkElement, ISelector<TItem>
		where TItem : FrameworkElement, ISelectable
	{
		public SelectorController(TSelector selector, ISelectorAdvisor<TItem> advisor) : base(selector, advisor)
		{
		}

		private TSelector SelectorInt => (TSelector) Selector;

		protected override bool SupportsIndex => SelectorInt.SelectedIndexProperty != null;

		protected override bool SupportsItem => SelectorInt.SelectedItemProperty != null;

		protected override bool SupportsItemSource => SelectorInt.SelectedItemSourceProperty != null;

		protected override bool SupportsValue => SelectorInt.SelectedValueProperty != null;

		private void PushBoundValue(DependencyProperty property, [UsedImplicitly] object coercedValue)
		{
			if (!(SelectorInt.ReadLocalValue(property) is BindingExpression localBindingExpression))
				return;

			SelectorInt.Dispatcher.BeginInvoke(DispatcherPriority.DataBind, () => localBindingExpression.UpdateSource());
		}

		protected override void PushSelectedIndexBoundValueCore(object coerceSelectedIndex)
		{
			PushBoundValue(SelectorInt.SelectedIndexProperty, coerceSelectedIndex);
		}

		protected override void PushSelectedItemBoundValueCore(TItem coerceSelectedItem)
		{
			PushBoundValue(SelectorInt.SelectedItemProperty, coerceSelectedItem);
		}

		protected override void PushSelectedItemSourceBoundValueCore(object coerceSelectedItemSource)
		{
			PushBoundValue(SelectorInt.SelectedItemSourceProperty, coerceSelectedItemSource);
		}

		protected override void PushSelectedValueBoundValueCore(object coerceSelectedValue)
		{
			PushBoundValue(SelectorInt.SelectedValueProperty, coerceSelectedValue);
		}

		private void PushValue(DependencyProperty property, object value)
		{
			SelectorInt.SetCurrentValueInternal(property, value);
		}

		protected override int ReadSelectedIndex()
		{
			return (int) SelectorInt.GetValue(SelectorInt.SelectedIndexProperty);
		}

		protected override TItem ReadSelectedItem()
		{
			return (TItem) SelectorInt.GetValue(SelectorInt.SelectedItemProperty);
		}

		protected override object ReadSelectedItemSource()
		{
			return SelectorInt.GetValue(SelectorInt.SelectedItemSourceProperty);
		}

		protected override object ReadSelectedValue()
		{
			return SelectorInt.GetValue(SelectorInt.SelectedValueProperty);
		}

		protected override void WriteSelectedIndex(int index)
		{
			PushValue(SelectorInt.SelectedIndexProperty, index);
		}

		protected override void WriteSelectedItem(TItem item)
		{
			PushValue(SelectorInt.SelectedItemProperty, item);
		}

		protected override void WriteSelectedItemSource(object itemSource)
		{
			PushValue(SelectorInt.SelectedItemSourceProperty, itemSource);
		}

		protected override void WriteSelectedValue(object value)
		{
			PushValue(SelectorInt.SelectedValueProperty, value);
		}
	}
}