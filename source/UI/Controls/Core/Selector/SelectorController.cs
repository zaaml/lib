// <copyright file="SelectorController.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Specialized;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;
using Zaaml.Core;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore.Extensions;

namespace Zaaml.UI.Controls.Core
{
	internal abstract class SelectorController<TItem> where TItem : FrameworkElement, ISelectable
	{
		private bool _allowNullSelection;
		private bool _preferSelection;
		private Selection<TItem> _selection = Selection<TItem>.Empty;
		private Selection<TItem> _selectionResume = Selection<TItem>.Empty;
		private bool _suspendChangedHandler;

		protected SelectorController(ISelector<TItem> selector, ISelectorAdvisor<TItem> advisor)
		{
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

		public object CurrentSelectedValue => CurrentSelection.Value;

		private Selection<TItem> CurrentSelection => IsSelectionChangeSuspended ? SelectionResume : Selection;

		public bool IsSelectionChangeSuspended => SuspendSelectionChangeCount > 0;

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

		public int SelectedIndex
		{
			get => Selection.Index;
			set
			{
				if (_suspendChangedHandler)
					return;

				SelectIndexCore(value, false);
			}
		}

		public TItem SelectedItem
		{
			get => Selection.Item;
			set
			{
				if (_suspendChangedHandler)
					return;

				SelectItemCore(value, false);
			}
		}

		public object SelectedItemSource
		{
			get => Selection.ItemSource;
			set
			{
				if (_suspendChangedHandler)
					return;

				SelectItemSourceCore(value, false);
			}
		}

		public object SelectedValue
		{
			get => Selection.Value;
			set
			{
				if (_suspendChangedHandler)
					return;

				SelectValueCore(value, false);
			}
		}

		private Selection<TItem> Selection
		{
			get => _selection;
			set
			{
				if (_selection.Equals(value))
					return;

				if (value.Index != -1)
					PreferredIndex = value.Index;

				_selection = value;
			}
		}

		private Selection<TItem> SelectionResume
		{
			get => _selectionResume;
			set
			{
				if (_selectionResume.Equals(value))
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

		private static bool CanSelect(TItem item)
		{
			return !(item is ISelectableEx selectableEx) || selectableEx.CanSelect;
		}

		public int CoerceSelectedIndex(int selectedIndex)
		{
			if (IsSelectionChangeSuspended)
			{
				SelectionResume = PreselectIndex(selectedIndex, true, SelectionResume);

				return Selection.Index;
			}

			var coerceSelectedIndex = _suspendChangedHandler ? SelectedIndex : PreselectIndex(selectedIndex, true, Selection).Index;

			if (Equals(coerceSelectedIndex, selectedIndex))
				return coerceSelectedIndex;

			_suspendChangedHandler = true;

			try
			{
				PushSelectedIndexBoundValue(coerceSelectedIndex);
			}
			finally
			{
				_suspendChangedHandler = false;
			}

			return coerceSelectedIndex;
		}

		public TItem CoerceSelectedItem(TItem selectedItem)
		{
			if (IsSelectionChangeSuspended)
			{
				SelectionResume = PreselectItem(selectedItem, true, SelectionResume);

				return Selection.Item;
			}

			var coerceSelectedItem = _suspendChangedHandler ? SelectedItem : PreselectItem(selectedItem, true, Selection).Item;

			if (Equals(coerceSelectedItem, selectedItem))
				return coerceSelectedItem;

			_suspendChangedHandler = true;

			try
			{
				PushSelectedItemBoundValue(coerceSelectedItem);
			}
			finally
			{
				_suspendChangedHandler = false;
			}

			return coerceSelectedItem;
		}

		public object CoerceSelectedItemSource(object selectedItemSource)
		{
			if (IsSelectionChangeSuspended)
			{
				SelectionResume = PreselectItemSource(selectedItemSource, true, SelectionResume);

				return Selection.ItemSource;
			}

			var coerceSelectedItemSource = _suspendChangedHandler ? SelectedItemSource : PreselectItemSource(selectedItemSource, true, Selection).ItemSource;

			if (Equals(coerceSelectedItemSource, selectedItemSource))
				return coerceSelectedItemSource;

			_suspendChangedHandler = true;

			try
			{
				PushSelectedItemSourceBoundValue(coerceSelectedItemSource);
			}
			finally
			{
				_suspendChangedHandler = false;
			}

			return coerceSelectedItemSource;
		}

		public object CoerceSelectedValue(object selectedValue)
		{
			if (IsSelectionChangeSuspended)
			{
				SelectionResume = PreselectValue(selectedValue, true, SelectionResume);

				return Selection.Value;
			}

			var coerceSelectedValue = _suspendChangedHandler ? SelectedValue : PreselectValue(selectedValue, true, Selection).Value;

			if (Equals(coerceSelectedValue, selectedValue))
				return coerceSelectedValue;

			_suspendChangedHandler = true;

			try
			{
				PushSelectedValueBoundValue(coerceSelectedValue);
			}
			finally
			{
				_suspendChangedHandler = false;
			}

			return coerceSelectedValue;
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
					SelectIndexCore(ActualPreferredIndex, true);
			}
			else if (SupportsItem)
			{
				if (SelectedItem != null) 
					return;

				for (var i = 0; i < Advisor.Count; i++)
				{
					if (Advisor.TryGetItem(i, out var item) == false || CanSelect(item) == false)
						continue;

					SelectItemCore(item, true);

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

		private object GetValue(TItem item, object itemSource)
		{
			return SupportsValue ? Selector.GetValue(item, itemSource) : null;
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
					Select(new Selection<TItem>(SelectedIndex, item, SelectedItemSource, SelectedValue));
			}
			else if (SupportsItem)
			{
				var selectedItem = IsSelectionChangeSuspended ? SelectionResume.Item : Selection.Item;

				if (item != null && item.IsSelected && ReferenceEquals(item, selectedItem) == false)
					SelectItem(item);
			}


		}

		private void OnItemCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Move)
			{
				UpdateSelectedIndex(true);

				return;
			}

			if (e.Action == NotifyCollectionChangedAction.Reset)
			{
				UpdateSelectedIndex(true);
			}
			else
			{
				if (e.NewItems != null)
				{
					var selectNext = SelectedItem;

					foreach (TItem item in e.NewItems)
					{
						if (item == null)
							continue;

						if (item.IsSelected)
							selectNext = item;
					}

					if (selectNext != null)
						SelectedItem = selectNext;
					else
						UpdateSelectedIndex(true);
				}

				if (e.OldItems != null)
				{
					var hasSelectedItem = false;

					foreach (TItem item in e.OldItems)
					{
						if (item == null)
							continue;

						if (ReferenceEquals(item, SelectedItem))
							hasSelectedItem = true;
					}

					if (hasSelectedItem)
						SelectedItem = null;
					else
						UpdateSelectedIndex(true);
				}
			}

			EnsureSelection();
		}

		private void OnItemCollectionSourceChanged(NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Move)
			{
				UpdateSelectedIndex(false);

				return;
			}

			if (e.Action == NotifyCollectionChangedAction.Reset)
			{
				UpdateSelectedIndex(false);
			}
			else
			{
				if (e.NewItems != null)
				{
					UpdateSelectedIndex(false);
				}

				if (e.OldItems != null)
				{
					var hasSelectedItemSource = false;

					foreach (var itemSource in e.OldItems)
					{
						if (itemSource == null)
							continue;

						if (ReferenceEquals(itemSource, SelectedItemSource))
							hasSelectedItemSource = true;
					}

					if (hasSelectedItemSource)
						SelectedItemSource = null;
					else
						UpdateSelectedIndex(false);
				}
			}

			EnsureSelection();
		}

		private void OnItemDetached([UsedImplicitly] int index, TItem item)
		{
			if (ReferenceEquals(SelectedItem, item))
				SelectIndexCore(ActualPreferredIndex, true);
		}

		public void OnItemSelectionChanged(TItem item)
		{
			if (_suspendChangedHandler)
				return;

			var isCurrent = ReferenceEquals(item, SelectedItem);
			var isSelected = item.IsSelected;

			if (isSelected == false && isCurrent)
				SelectedItem = null;

			if (isSelected && isCurrent == false)
				SelectedItem = item;
		}

		public void OnSelectedIndexPropertyChanged(int oldIndex, int newIndex)
		{
			SelectedIndex = newIndex;
		}

		public void OnSelectedItemPropertyChanged(TItem oldItem, TItem newItem)
		{
			SelectedItem = newItem;
		}

		public void OnSelectedItemSourcePropertyChanged(object oldItemSource, object newItemSource)
		{
			SelectedItemSource = newItemSource;
		}

		public void OnSelectedValuePropertyChanged(object oldValue, object newValue)
		{
			SelectedValue = newValue;
		}

		private void OnSelectionChanged(Selection<TItem> oldSelection, Selection<TItem> newSelection)
		{
			Selector.OnSelectionChanged(oldSelection, newSelection);
		}

		private void OnSelectionChangeSuspended()
		{
			SelectionResume = Selection;
		}

		private Selection<TItem> PreselectIndex(int value, bool force, Selection<TItem> selection)
		{
			if (SupportsIndex == false)
				return selection;

			if (force == false && selection.Index == value)
				return selection;

			var index = value;

			if (index == -1)
				return PreselectNull();

			if (TryGetItem(index, out var item))
			{
				if (CanSelect(item) == false)
					return selection;
			}

			var itemSource = GetItemSource(index);
			var itemValue = GetValue(item, itemSource);

			return new Selection<TItem>(index, item, itemSource, itemValue);
		}

		private Selection<TItem> PreselectItem(TItem value, bool force, Selection<TItem> selection)
		{
			if (SupportsItem == false)
				return selection;

			if (force == false && ReferenceEquals(selection.Item, value))
				return selection;

			if (value == null)
				return PreselectNull();

			var item = value;

			if (CanSelect(item) == false)
				return selection;

			var index = GetIndexOfItem(item);
			var itemSource = GetItemSource(item);
			var itemValue = GetValue(item, itemSource);

			return new Selection<TItem>(index, item, itemSource, itemValue);
		}

		private Selection<TItem> PreselectItemSource(object value, bool force, Selection<TItem> selection)
		{
			if (SupportsItemSource == false)
				return selection;

			if (force == false && ReferenceEquals(selection.ItemSource, value))
				return selection;

			var itemSource = value;

			if (itemSource == null)
				return PreselectNull();

			if (SupportsIndex)
			{
				var index = GetIndexOfItemSource(itemSource);

				if (index == -1)
					return PreselectNull();

				if (TryGetItem(index, out var item))
				{
					if (CanSelect(item) == false)
						return selection;
				}

				var itemValue = GetValue(item, itemSource);

				return new Selection<TItem>(index, item, itemSource, itemValue);
			}
			else
			{
				if (TryGetItemBySource(itemSource, out var item))
				{
					if (CanSelect(item) == false)
						return selection;
				}

				var itemValue = GetValue(item, itemSource);

				return new Selection<TItem>(-1, item, itemSource, itemValue);
			}
		}

		private Selection<TItem> PreselectNull()
		{
			var count = Advisor.Count;

			if (AllowNullSelection || count <= 0)
				return Selection<TItem>.Empty;

			for (var i = 0; i < count; i++)
			{
				if (TryGetItem(i, out var item))
				{
					if (CanSelect(item) == false)
						continue;
				}

				var itemSource = GetItemSource(item);
				var value = GetValue(item, itemSource);

				return new Selection<TItem>(0, item, itemSource, value);
			}

			return Selection<TItem>.Empty;
		}

		private Selection<TItem> PreselectValue(object value, bool force, Selection<TItem> selection)
		{
			if (SupportsValue == false)
				return selection;

			if (force == false && CompareValues(selection.Value, value))
				return selection;

			var itemValue = value;

			if (itemValue == null)
				return PreselectNull();

			var index = GetIndexOfValue(itemValue);

			if (TryGetItem(index, out var item))
			{
				if (CanSelect(item) == false)
					return selection;
			}

			var itemSource = GetItemSource(index);

			return new Selection<TItem>(index, item, itemSource, itemValue);
		}

		protected abstract void PushSelectedIndexBoundValue(object coerceSelectedIndex);

		protected abstract void PushSelectedItemBoundValue(TItem coerceSelectedItem);

		protected abstract void PushSelectedItemSourceBoundValue(object coerceSelectedItemSource);

		protected abstract void PushSelectedValueBoundValue(object coerceSelectedValue);

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

		protected abstract int ReadSelectedIndex();

		protected abstract TItem ReadSelectedItem();

		protected abstract object ReadSelectedItemSource();

		protected abstract object ReadSelectedValue();

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
			if (SuspendSelectionChangeCount > 0)
				SuspendSelectionChangeCount--;
			else
				return;

			if (SuspendSelectionChangeCount > 0)
				return;

			if (applySelection == false)
			{
				var selectedItemChanged = ReferenceEquals(SelectionResume.Item, Selection.Item) == false;

				if (selectedItemChanged && SupportsItem && SelectionResume.Item != null)
					SelectionResume.Item.IsSelected = false;

				SelectionResume = Selection;
			}

			Select(SelectionResume);
			EnsureSelection();
		}

		private void Select(Selection<TItem> newSelection)
		{
			if (IsSelectionChangeSuspended)
			{
				SelectionResume = newSelection;

				return;
			}

			_suspendChangedHandler = true;

			try
			{
				var selectedItemChanged = ReferenceEquals(SelectedItem, newSelection.Item) == false;

				if (selectedItemChanged && SupportsItem && SelectedItem != null)
					SelectedItem.IsSelected = false;

				var oldSelection = Selection;

				Selection = newSelection;

				if (selectedItemChanged && SupportsItem && SelectedItem != null)
					SelectedItem.IsSelected = true;

				var selectionChanged = false;

				if (SupportsItem)
				{
					if (ReferenceEquals(ReadSelectedItem(), SelectedItem) == false)
						WriteSelectedItem(SelectedItem);

					if (selectedItemChanged)
					{
						RaiseOnSelectedItemChanged(oldSelection.Item, newSelection.Item);

						selectionChanged = true;
					}
				}

				if (SupportsItemSource)
				{
					if (ReferenceEquals(ReadSelectedItemSource(), SelectedItemSource) == false)
						WriteSelectedItemSource(SelectedItemSource);

					if (ReferenceEquals(oldSelection.ItemSource, newSelection.ItemSource) == false)
					{
						RaiseOnSelectedItemSourceChanged(oldSelection.ItemSource, newSelection.ItemSource);

						selectionChanged = true;
					}
				}

				if (SupportsIndex)
				{
					if (ReadSelectedIndex() != SelectedIndex)
						WriteSelectedIndex(SelectedIndex);

					if (oldSelection.Index != newSelection.Index)
					{
						RaiseOnSelectedIndexChanged(oldSelection.Index, newSelection.Index);

						selectionChanged = true;
					}
				}

				if (SupportsValue)
				{
					if (CompareValues(ReadSelectedValue(), SelectedValue) == false)
						WriteSelectedValue(SelectedValue);

					if (CompareValues(oldSelection.Value, newSelection.Value) == false)
					{
						RaiseOnSelectedValueChanged(oldSelection.Value, newSelection.Value);

						selectionChanged = true;
					}
				}

				if (selectionChanged)
				{
					if (oldSelection.Item != null)
						Advisor.Unlock(oldSelection.Item);

					if (newSelection.Item != null)
						Advisor.Lock(newSelection.Item);

					OnSelectionChanged(oldSelection, newSelection);
				}
			}
			finally
			{
				_suspendChangedHandler = false;
			}
		}

		public void SelectIndex(int index)
		{
			SelectedIndex = index;
		}

		private void SelectIndexCore(int index, bool force)
		{
			Select(PreselectIndex(index, force, CurrentSelection));
		}

		public void SelectItem(TItem item)
		{
			SelectedItem = item;
		}

		private void SelectItemCore(TItem item, bool force)
		{
			Select(PreselectItem(item, force, CurrentSelection));
		}

		public void SelectItemSource(object itemSource)
		{
			SelectedItemSource = itemSource;
		}

		private void SelectItemSourceCore(object itemSource, bool force)
		{
			Select(PreselectItemSource(itemSource, force, CurrentSelection));
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
			SelectIndexCore(SelectNext(SelectedIndex, SelectNextMode), force);
		}

		public void SelectValue(object value)
		{
			SelectedValue = value;
		}

		private void SelectValueCore(object value, bool force)
		{
			Select(PreselectValue(value, force, CurrentSelection));
		}

		public void SuspendSelectionChange()
		{
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

		private void UpdateSelectedIndex(bool forItem)
		{
			if (forItem)
			{
				var itemIndex = SelectedItem != null ? GetIndexOfItem(SelectedItem) : -1;

				if (itemIndex != -1 && SelectedIndex != itemIndex)
					SelectedIndex = itemIndex;
			}
			else
			{
				var itemSourceIndex = SelectedItemSource != null ? GetIndexOfItemSource(SelectedItemSource) : -1;

				if (itemSourceIndex != -1 && SelectedIndex != itemSourceIndex)
					SelectedIndex = itemSourceIndex;
			}
		}

		public void UpdateSelectedValue()
		{
			if (SelectedIndex == -1)
				return;

			SelectedValue = GetValue(SelectedItem, SelectedItemSource);
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

#if SILVERLIGHT
			SelectorInt.Dispatcher.BeginInvoke(() => localBindingExpression.UpdateSource());
#else
			SelectorInt.Dispatcher.BeginInvoke(DispatcherPriority.DataBind, () => localBindingExpression.UpdateSource());
#endif
		}

		protected override void PushSelectedIndexBoundValue(object coerceSelectedIndex)
		{
			PushBoundValue(SelectorInt.SelectedIndexProperty, coerceSelectedIndex);
		}

		protected override void PushSelectedItemBoundValue(TItem coerceSelectedItem)
		{
			PushBoundValue(SelectorInt.SelectedItemProperty, coerceSelectedItem);
		}

		protected override void PushSelectedItemSourceBoundValue(object coerceSelectedItemSource)
		{
			PushBoundValue(SelectorInt.SelectedItemSourceProperty, coerceSelectedItemSource);
		}

		protected override void PushSelectedValueBoundValue(object coerceSelectedValue)
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