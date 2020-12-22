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
		private bool _selectionHandling;
		private Selection<TItem> _selectionResume = Selection<TItem>.Empty;

		public event EventHandler<NotifyCollectionChangedEventArgs> SelectionCollectionChanged;
		public event PropertyChangedEventHandler SelectionCollectionPropertyChanged;

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

		internal int Count => Advisor.Count;

		public int CurrentSelectedIndex => CurrentSelection.Index;

		public TItem CurrentSelectedItem => CurrentSelection.Item;

		public object CurrentSelectedSource => CurrentSelection.Source;

		public object CurrentSelectedValue => CurrentSelection.Value;

		private Selection<TItem> CurrentSelection => IsSelectionChangeSuspended ? SelectionResume : Selection;

		private SelectionCollectionImpl CurrentSelectionCollection => IsSelectionChangeSuspended ? SelectionCollectionResume : SelectionCollection;

		private bool HasSource => Advisor.HasSource;

		private bool IsInitializing { get; set; }

		private bool IsInverted { get; set; }

		public bool IsSelectionChangeSuspended => SuspendSelectionChangeCount > 0;

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

		public int SuspendSelectionChangeCount { get; private set; }

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

			SuspendSelectionChange();
		}

		private bool CanSelect(TItem item)
		{
			return Advisor.CanSelect(item);
		}

		private bool CompareValues(object itemValue, object value)
		{
			return Advisor.CompareValues(itemValue, value);
		}

		public void EndInit()
		{
			ResumeSelectionChange();

			IsInitializing = false;
		}

		private bool EnsureInvertedSelection(bool selectionResult)
		{
			if (_selectionHandling)
				return false;
			
			if (selectionResult == false)
				return false;

			SelectFirst();

			return true;
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

		private object GetSource(int index)
		{
			return SupportsSource && IsWithinRange(index) ? Advisor.GetSource(index) : null;
		}

		private object GetSource(TItem item)
		{
			return SupportsSource && item != null ? Advisor.GetSource(item) : null;
		}

		internal SelectionCollectionImpl.SelectionCollectionEnumerator GetSelectionCollectionEnumerator()
		{
			return CurrentSelectionCollection.GetEnumerator();
		}

		private object GetValue(TItem item, object source)
		{
			return SupportsValue ? Selector.GetValue(item, source) : null;
		}

		public void InvertSelection()
		{
			if (MultipleSelection == false)
				return;

			try
			{
				_selectionHandling = true;

				InvertSelectionSafe();
			}
			finally
			{
				_selectionHandling = false;
			}
			
			EnsureSelection();
		}

		private void InvertSelectionSafe()
		{
			foreach (var selection in CurrentSelectionCollection)
				SetItemSelected(selection.Item, false);

			IsInverted = !IsInverted;

			var currentSelection = CurrentSelectionCollection.Count > 0 ? CurrentSelectionCollection.First() : Selection<TItem>.Empty;

			foreach (var selection in CurrentSelectionCollection)
				SetItemSelected(selection.Item, true);

			RaiseSelectionCollectionChanged(ResetNotifyCollectionChangedEventArgs);
			CommitSelectionSafe(currentSelection);
		}

		private bool IsLocked(TItem item)
		{
			return ReferenceEquals(LockedItem, item);
		}

		private bool IsWithinRange(int index)
		{
			return index >= 0 && Count != 0 && index < Count;
		}

		private void ModifySelectionCollection(Selection<TItem> selection, bool raiseEvent = true)
		{
			if (MultipleSelection == false)
			{
				if (IsSelectionChangeSuspended == false)
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

		public void OnSelectedSourcePropertyChanged(object oldSource, object newSource)
		{
			if (_selectionHandling)
				return;

			SuspendSelectionChange();

			UnselectSource(oldSource);
			SelectSource(newSource);

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
			SelectionCollectionResume.CopyFrom(SelectionCollection);
		}

		private void RaiseSelectionCollectionChanged(NotifyCollectionChangedEventArgs args)
		{
			if (IsSelectionChangeSuspended)
				return;

			SelectionCollectionChanged?.Invoke(this, args);
		}

		private void RaiseSelectionCollectionPropertyChanged(PropertyChangedEventArgs args)
		{
			if (IsSelectionChangeSuspended)
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
			if (IsSelectionChangeSuspended)
			{
				SelectionResume = Selection;
				SelectionCollectionResume.CopyFrom(SelectionCollection);
			}
		}

		public void ResumeSelectionChange()
		{
			if (_selectionHandling)
				return;

			if (SuspendSelectionChangeCount > 0)
				SuspendSelectionChangeCount--;
			else
				return;

			if (SuspendSelectionChangeCount > 0)
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

		public void SuspendSelectionChange()
		{
			if (_selectionHandling)
				return;

			SuspendSelectionChangeCount++;

			if (SuspendSelectionChangeCount == 1)
				OnSelectionChangeSuspended();
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

			return Advisor.TryGetItemBySource(source, ensure, out item);
		}
	}
}