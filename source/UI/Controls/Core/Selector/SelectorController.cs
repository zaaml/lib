// <copyright file="SelectorController.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Linq;
using System.Windows;
using Zaaml.Core.Extensions;

namespace Zaaml.UI.Controls.Core
{
	internal abstract partial class SelectorController<TItem> where TItem : FrameworkElement
	{
		private bool _allowNullSelection;
		private TItem _lockedItem;
		private bool _multipleSelection;
		private bool _preferSelection;
		private Selection<TItem> _selection = Selection<TItem>.Empty;
		private Selection<TItem> _selectionResume = Selection<TItem>.Empty;

		protected SelectorController(ISelector<TItem> selector, ISelectorAdvisor<TItem> advisor)
		{
			SelectionCollection = new SelectionCollectionImpl(this, false);
			SelectionCollectionResume = new SelectionCollectionImpl(this, true);

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

		internal int Count => Advisor.Count;

		public int CurrentSelectedIndex => CurrentSelection.Index;

		public TItem CurrentSelectedItem => CurrentSelection.Item;

		public object CurrentSelectedSource => CurrentSelection.Source;

		public object CurrentSelectedValue => CurrentSelection.Value;

		private Selection<TItem> CurrentSelection
		{
			get
			{
				if (SelectionHandlingCount < 0)
					return Selection;

				return SelectionHandling || IsSelectionSuspended ? SelectionResume : Selection;
			}
		}

		private SelectionCollectionImpl CurrentSelectionCollection
		{
			get
			{
				if (SelectionHandlingCount < 0)
					return SelectionCollection;

				return SelectionHandling || IsSelectionSuspended ? SelectionCollectionResume : SelectionCollection;
			}
		}

		private bool HasSource => Advisor.HasSource;

		private bool IsInitializing { get; set; }

		public bool IsSelectionSuspended => SuspendSelectionCount > 0;

		private bool IsVirtualizing => Advisor.IsVirtualizing;

		private TItem LockedItem
		{
			get => _lockedItem;
			set
			{
				if (ReferenceEquals(_lockedItem, value))
					return;

				if (_lockedItem != null)
					Advisor.Unlock(_lockedItem);

				_lockedItem = value;

				if (_lockedItem != null)
					Advisor.Lock(_lockedItem);
			}
		}

		public bool MultipleSelection
		{
			get => _multipleSelection;
			set
			{
				if (_multipleSelection == value)
					return;

				_multipleSelection = value;

				CurrentSelectionCollection.IsInverted = false;
				CurrentSelectionCollection.Clear();

				if (_multipleSelection)
					CurrentSelectionCollection.Select(CurrentSelection);
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

		public int SelectedCount => CurrentSelectionCollection.Count;

		public int SelectedIndex => Selection.Index;

		public TItem SelectedItem => Selection.Item;

		public object SelectedSource => Selection.Source;

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

		private SelectionCollectionImpl SelectionCollection { get; set; }

		private SelectionCollectionImpl SelectionCollectionResume { get; set; }

		public bool SelectionHandling => SelectionHandlingCount != 0;

		private int SelectionHandlingCount { get; set; }

		private SelectionHandlingScopeImpl SelectionHandlingScope => new SelectionHandlingScopeImpl(this);

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

		public ISelector<TItem> Selector { get; }

		protected abstract bool SupportsIndex { get; }

		protected abstract bool SupportsItem { get; }

		protected abstract bool SupportsSource { get; }

		protected abstract bool SupportsValue { get; }

		public int SuspendSelectionCount { get; private set; }

		private int Version { get; set; }


		private bool AreSelectionEquals(Selection<TItem> first, Selection<TItem> second)
		{
			return first.Index == second.Index &&
			       ReferenceEquals(first.Item, second.Item) &&
			       ReferenceEquals(first.Source, second.Source) &&
			       CompareValues(first.Value, second.Value);
		}

		public void BeginInit()
		{
			IsInitializing = true;

			SuspendSelection();
		}

		private bool CanSelect(Selection<TItem> selection)
		{
			return CanSelectIndex(selection.Index) && CanSelectItem(selection.Item) && CanSelectSource(selection.Source) && CanSelectValue(selection.Value);
		}

		private bool CanSelectIndex(int index)
		{
			if (SupportsIndex == false)
				return true;

			return index == -1 || Advisor.CanSelectIndex(index);
		}

		private bool CanSelectItem(TItem item)
		{
			if (SupportsItem == false)
				return true;

			return item == null || Advisor.CanSelectItem(item);
		}

		private bool CanSelectSource(object source)
		{
			if (SupportsSource == false)
				return true;

			return source == null || Advisor.CanSelectSource(source);
		}

		private bool CanSelectValue(object value)
		{
			if (SupportsValue == false)
				return true;

			return value == null || Advisor.CanSelectValue(value);
		}

		private void CoerceSelectionResume()
		{
			var selectionResume = SelectionResume;

			CoerceSelection(false, ref selectionResume);

			if (CanSelect(selectionResume) == false)
			{
				SelectionCollectionResume.Unselect(selectionResume);

				if (MultipleSelection)
				{
					if (SelectionCollectionResume.Count > 0)
						selectionResume = SelectionCollectionResume.First();
					else if (PreselectNull(out selectionResume))
						SelectionCollectionResume.Select(selectionResume);
				}
				else
					PreselectNull(out selectionResume);
			}

			SelectionResume = selectionResume;
		}

		private bool CompareValues(object itemValue, object value)
		{
			return Advisor.CompareValues(itemValue, value);
		}

		public void EndInit()
		{
			ResumeSelection();

			IsInitializing = false;
		}

		internal void EnsureSelection()
		{
			if (IsSelectionSuspended)
				return;

			if (AllowNullSelection && PreferSelection == false)
				return;

			if (SupportsIndex)
			{
				if (SelectedIndex == -1)
				{
					if (SelectIndexCore(ActualPreferredIndex, true))
						return;

					SelectFirstPossible();
				}
			}
			else if (SupportsItem)
			{
				if (SelectedItem != null)
					return;

				SelectFirstPossible();
			}
		}

		private void EnterSelectionHandling()
		{
			if (SelectionHandlingCount < 0)
				return;

			if (SelectionHandlingCount == 0)
			{
				if (IsSelectionSuspended == false)
				{
					SelectionResume = Selection;
					SelectionCollectionResume.CopyFrom(SelectionCollection);
				}
			}

			SelectionHandlingCount++;
		}

		private int GetIndexOfItem(TItem item)
		{
			return SupportsItem && item != null && Count > 0 ? Advisor.GetIndexOfItem(item) : -1;
		}

		private int GetIndexOfSource(object source)
		{
			return SupportsSource && source != null && Count > 0 ? Advisor.GetIndexOfSource(source) : -1;
		}

		private int GetIndexOfValue(object value)
		{
			return SupportsValue && Count > 0 ? Advisor.GetIndexOfValue(value) : -1;
		}

		private bool GetIsItemSelected(TItem item)
		{
			return item != null && Advisor.GetItemSelected(item);
		}

		public int GetSelectableCount()
		{
			var count = Count;
			var selectableCount = 0;

			for (var i = 0; i < count; i++)
			{
				if (Advisor.TryCreateSelection(i, false, out var selection) && CanSelect(selection))
					selectableCount++;
			}

			return selectableCount;
		}

		internal SelectionCollectionImpl.SelectionCollectionEnumerator GetSelectionCollectionEnumerator()
		{
			return CurrentSelectionCollection.GetEnumerator();
		}

		private object GetSource(int index)
		{
			return SupportsSource && IsWithinRange(index) ? Advisor.GetSource(index) : null;
		}

		private object GetSource(TItem item)
		{
			return SupportsSource && item != null ? Advisor.GetSource(item) : null;
		}

		private object GetValue(TItem item, object source)
		{
			return SupportsValue && (item != null || source != null) ? Advisor.GetValue(item, source) : null;
		}

		public void InvertSelection()
		{
			if (MultipleSelection == false)
				return;

			using (SelectionHandlingScope)
			{
				InvertSelectionSafe();
			}
		}

		private void InvertSelectionSafe()
		{
			VerifySafe();

			foreach (var selection in CurrentSelectionCollection)
				SetItemSelected(selection.Item, false);

			CurrentSelectionCollection.IsInverted = !CurrentSelectionCollection.IsInverted;

			foreach (var selection in CurrentSelectionCollection)
			{
				if (CanSelectItem(selection.Item))
					SetItemSelected(selection.Item, true);
				else
					CurrentSelectionCollection.DeferUnselect(selection);
			}

			CurrentSelectionCollection.CommitDeferUnselect();

			SelectFirst();
		}

		private bool IsLocked(TItem item)
		{
			return ReferenceEquals(LockedItem, item);
		}

		private bool IsWithinRange(int index)
		{
			var count = Count;

			return index >= 0 && count != 0 && index < count;
		}

		private void LeaveSelectionHandling()
		{
			SelectionHandlingCount--;

			if (SelectionHandlingCount == 0)
			{
				if (IsSelectionSuspended == false)
					CommitSelection();
			}
		}

		public void OnSelectedIndexPropertyChanged(int oldIndex, int newIndex)
		{
			if (SelectionHandling)
				return;

			using (SelectionHandlingScope)
			{
				UnselectIndexSafe(oldIndex);
				SelectIndexSafe(newIndex);
			}
		}

		public void OnSelectedItemPropertyChanged(TItem oldItem, TItem newItem)
		{
			if (SelectionHandling)
				return;

			using (SelectionHandlingScope)
			{
				UnselectItemSafe(oldItem);
				SelectItemSafe(newItem);
			}
		}

		public void OnSelectedSourcePropertyChanged(object oldSource, object newSource)
		{
			if (SelectionHandling)
				return;

			using (SelectionHandlingScope)
			{
				UnselectSourceSafe(oldSource);
				SelectSourceSafe(newSource);
			}
		}

		public void OnSelectedValuePropertyChanged(object oldValue, object newValue)
		{
			if (SelectionHandling)
				return;

			using (SelectionHandlingScope)
			{
				UnselectValueSafe(oldValue);
				SelectValueSafe(newValue);
			}
		}

		private void OnSelectionChangeSuspended()
		{
			SelectionResume = Selection;
			SelectionCollectionResume.CopyFrom(SelectionCollection);
		}

		public void RestoreSelection()
		{
			if (IsSelectionSuspended == false)
				return;

			SelectionResume = Selection;
			SelectionCollectionResume.CopyFrom(SelectionCollection);
		}

		public void ResumeSelection()
		{
			if (SelectionHandling)
				return;

			if (SuspendSelectionCount > 0)
				SuspendSelectionCount--;
			else
				return;

			if (SuspendSelectionCount == 0)
				CommitSelection();
		}

		private void SetItemSelected(TItem item, bool selection)
		{
			if (item == null)
				return;

			Advisor.SetItemSelected(item, selection);
		}

		public void SuspendSelection()
		{
			if (SelectionHandling)
				return;

			SuspendSelectionCount++;

			if (SuspendSelectionCount == 1)
				OnSelectionChangeSuspended();
		}

		public void ToggleSelection(bool? select)
		{
			if (select == null)
				ToggleSelectionCore();
			else if (select.Value)
				SelectAll();
			else
				UnselectAll();
		}

		private void ToggleSelectionCore()
		{
			using (SelectionHandlingScope)
			{
				var selectionCount = 0;
				var currentSelectionCount = CurrentSelectionCollection.Count;

				CurrentSelectionCollection.Clear();

				CurrentSelectionCollection.IsInverted = true;

				foreach (var selection in CurrentSelectionCollection)
				{
					if (CanSelect(selection))
						selectionCount++;
				}

				if (selectionCount > currentSelectionCount)
				{
					foreach (var selection in CurrentSelectionCollection)
					{
						if (CanSelect(selection))
							SetItemSelected(selection.Item, true);
						else
							CurrentSelectionCollection.DeferUnselect(selection);
					}

					CurrentSelectionCollection.CommitDeferUnselect();
				}
				else
				{
					foreach (var selection in CurrentSelectionCollection)
						SetItemSelected(selection.Item, false);

					CurrentSelectionCollection.IsInverted = false;
				}

				ApplySelectionSafe(CurrentSelectionCollection.Count > 0 ? CurrentSelectionCollection.First() : Selection<TItem>.Empty);
			}
		}

		private bool TryGetItem(int index, bool ensure, out TItem item)
		{
			if (SupportsItem == false || IsWithinRange(index) == false)
			{
				item = default;

				return false;
			}

			return Advisor.TryGetItem(index, ensure, out item);
		}

		private bool TryGetItemBySource(object source, bool ensure, out TItem item)
		{
			if (SupportsSource == false || source == null)
			{
				item = default;

				return false;
			}

			return Advisor.TryGetItem(source, ensure, out item);
		}

		private void VerifySafe()
		{
			if (SelectionHandling == false)
				throw new InvalidOperationException();
		}

		private readonly ref struct SelectionHandlingScopeImpl
		{
			private readonly SelectorController<TItem> _selectorController;

			public SelectionHandlingScopeImpl(SelectorController<TItem> selectorController)
			{
				_selectorController = selectorController;

				_selectorController.EnterSelectionHandling();
			}

			public void Dispose()
			{
				_selectorController.LeaveSelectionHandling();
			}
		}
	}
}