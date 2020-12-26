// <copyright file="SelectorController.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Specialized;
using System.ComponentModel;
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

		public event EventHandler<NotifyCollectionChangedEventArgs> SelectionCollectionChanged;
		public event PropertyChangedEventHandler SelectionCollectionPropertyChanged;

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

		private Selection<TItem> CurrentSelection => IsSelectionSuspended ? SelectionResume : Selection;

		private SelectionCollectionImpl CurrentSelectionCollection => IsSelectionSuspended ? SelectionCollectionResume : SelectionCollection;

		private bool HasSource => Advisor.HasSource;

		private bool IsInitializing { get; set; }

		private bool IsInverted { get; set; }

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

		public bool SelectionHandling => SelectionHandlingCount > 0;

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
		
		private bool CanSelectIndex(int index)
		{
			if (SupportsIndex == false)
				return true;
			
			return index == -1 || Advisor.CanSelectIndex(index);
		}
		
		private bool CanSelectValue(object value)
		{
			if (SupportsValue == false)
				return true;
			
			return value == null || Advisor.CanSelectValue(value);
		}

		private bool CanSelect(Selection<TItem> selection)
		{
			return CanSelectIndex(selection.Index) && CanSelectItem(selection.Item) && CanSelectSource(selection.Source) && CanSelectValue(selection.Value);
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

		private bool EnsureInvertedSelection(bool selectionResult)
		{
			if (SelectionHandling)
				return false;

			if (selectionResult == false)
				return false;

			SelectFirst();

			return true;
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
			return SupportsValue ? Selector.GetValue(item, source) : null;
		}

		public void InvertSelection()
		{
			if (MultipleSelection == false)
				return;

			using (SelectionHandlingScope)
			{
				InvertSelectionSafe();
			}

			EnsureSelection();
		}

		private void InvertSelectionSafe()
		{
			VerifySafe();

			foreach (var selection in CurrentSelectionCollection)
				SetItemSelected(selection.Item, false);

			IsInverted = !IsInverted;

			var currentSelection = CurrentSelectionCollection.Count > 0 ? CurrentSelectionCollection.First() : Selection<TItem>.Empty;

			foreach (var selection in CurrentSelectionCollection)
			{
				if (CanSelectItem(selection.Item))
					SetItemSelected(selection.Item, true);
				else
					CurrentSelectionCollection.DeferUnselect(selection);
			}

			CurrentSelectionCollection.CommitDeferUnselect();

			RaiseSelectionCollectionChanged(ResetNotifyCollectionChangedEventArgs);
			CommitSelectionSafe(currentSelection);
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
		}

		private void ModifySelectionCollection(Selection<TItem> selection, bool raiseEvent = true)
		{
			if (MultipleSelection == false)
			{
				if (IsSelectionSuspended == false)
				{
					if (raiseEvent)
						RaiseSelectionCollectionChanged(ResetNotifyCollectionChangedEventArgs);
				}

				return;
			}

			if (selection.IsEmpty == false)
				CurrentSelectionCollection.Add(selection);
		}

		public void OnSelectedIndexPropertyChanged(int oldIndex, int newIndex)
		{
			if (SelectionHandling)
				return;

			SuspendSelection();

			UnselectIndex(oldIndex);
			SelectIndex(newIndex);

			ResumeSelection();
		}

		public void OnSelectedItemPropertyChanged(TItem oldItem, TItem newItem)
		{
			if (SelectionHandling)
				return;

			SuspendSelection();

			UnselectItem(oldItem);
			SelectItem(newItem);

			ResumeSelection();
		}

		public void OnSelectedSourcePropertyChanged(object oldSource, object newSource)
		{
			if (SelectionHandling)
				return;

			SuspendSelection();

			UnselectSource(oldSource);
			SelectSource(newSource);

			ResumeSelection();
		}

		public void OnSelectedValuePropertyChanged(object oldValue, object newValue)
		{
			if (SelectionHandling)
				return;

			SuspendSelection();

			UnselectValue(oldValue);
			SelectValue(newValue);

			ResumeSelection();
		}

		private void OnSelectionChangeSuspended()
		{
			SelectionResume = Selection;
			SelectionCollectionResume.CopyFrom(SelectionCollection);
		}

		private void RaiseSelectionCollectionChanged(NotifyCollectionChangedEventArgs args)
		{
			if (IsSelectionSuspended)
				return;

			SelectionCollectionChanged?.Invoke(this, args);
		}

		private void RaiseSelectionCollectionPropertyChanged(PropertyChangedEventArgs args)
		{
			if (IsSelectionSuspended)
				return;

			SelectionCollectionPropertyChanged?.Invoke(this, args);
		}

		private void ResetMultipleSelection()
		{
			CurrentSelectionCollection.Clear(false);

			if (MultipleSelection)
			{
				if (CurrentSelection.IsEmpty == false)
					CurrentSelectionCollection.Add(CurrentSelection);
			}
			else
				IsInverted = false;
		}

		public void RestoreSelection()
		{
			if (IsSelectionSuspended)
			{
				SelectionResume = Selection;
				SelectionCollectionResume.CopyFrom(SelectionCollection);
			}
		}

		public void ResumeSelection()
		{
			if (SelectionHandling)
				return;

			if (SuspendSelectionCount > 0)
				SuspendSelectionCount--;
			else
				return;

			if (SuspendSelectionCount > 0)
				return;

			SelectionCollection.CopyFrom(SelectionCollectionResume);
			RaiseSelectionCollectionChanged(ResetNotifyCollectionChangedEventArgs);

			CommitSelection(SelectionResume);
			EnsureSelection();
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
		
		private void ToggleSelectionCore()
		{
			using (SelectionHandlingScope)
			{
				var selectionCount = 0;
				var currentSelectionCount = CurrentSelectionCollection.Count;

				CurrentSelectionCollection.Clear(false);

				IsInverted = true;

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

					IsInverted = false;
				}

				var currentSelection = CurrentSelectionCollection.Count > 0 ? CurrentSelectionCollection.First() : Selection<TItem>.Empty;

				RaiseSelectionCollectionChanged(ResetNotifyCollectionChangedEventArgs);
				CommitSelectionSafe(currentSelection);
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