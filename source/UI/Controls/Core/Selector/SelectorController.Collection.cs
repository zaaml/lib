// <copyright file="SelectorController.Collection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Zaaml.UI.Controls.Core
{
	[SuppressMessage("ReSharper", "StaticMemberInGenericType")]
	internal abstract partial class SelectorController<TItem>
	{
		private static readonly NotifyCollectionChangedEventArgs ResetNotifyCollectionChangedEventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
		private static readonly PropertyChangedEventArgs CountPropertyChangedEventArgs = new PropertyChangedEventArgs("Count");

		internal sealed class SelectionCollectionImpl : IEnumerable<Selection<TItem>>
		{
			// ReSharper disable once StaticMemberInGenericType
			private readonly List<int> _indicesToRemove = new List<int>();
			private readonly List<int> _indicesToUpdate = new List<int>();
			private readonly bool _isResumeCollection;

			public SelectionCollectionImpl(SelectorController<TItem> controller, bool isResumeCollection)
			{
				_isResumeCollection = isResumeCollection;

				Controller = controller;
			}

			public SelectorController<TItem> Controller { get; }

			public int Count
			{
				get
				{
					if (Controller.MultipleSelection)
						return Controller.IsInverted ? Controller.Count - ListCount : ListCount;

					return SingleSelection.IsEmpty ? 0 : 1;
				}
			}

			private List<Selection<TItem>> DeferList { get; } = new List<Selection<TItem>>();

			public bool IsInverted => Controller.IsInverted;

			private IEnumerable<int> Keys => List.Select(s => s.Index);

			private List<Selection<TItem>> List { get; } = new List<Selection<TItem>>();

			private int ListCount => List.Count;

			private Selection<TItem> SingleSelection => _isResumeCollection ? Controller.SelectionResume : Controller.Selection;

			public long Version { get; private set; }

			public void Add(Selection<TItem> newSelection)
			{
				if (Controller.MultipleSelection == false || newSelection.IsEmpty)
					return;

				var existIndex = -1;

				if (FindIndexOfKeyIndex(newSelection.Index, out var index))
					ListRemoveAt(index);

				foreach (var selection in List)
				{
					if (Controller.AreSelectionEquals(selection, newSelection))
					{
						if (existIndex == -1)
							existIndex = index;
						else
							_indicesToRemove.Add(index);
					}

					index++;
				}

				foreach (var removeIndex in _indicesToRemove)
					ListRemoveAt(removeIndex);

				_indicesToRemove.Clear();

				if (existIndex == -1)
					InsertSelection(newSelection);
			}

			public void Clear()
			{
				if (Controller.MultipleSelection == false)
					return;

				if (ListCount == 0)
					return;

				ListClear();
			}

			public void CommitDeferUnselect()
			{
				foreach (var selection in DeferList)
				{
					if (IsInverted)
						Add(selection);
					else
						Remove(selection);
				}

				DeferList.Clear();
			}

			public bool ContainsIndex(int index)
			{
				if (index == -1)
					return false;

				return Invert(FindByIndex(index, false, out var _));
			}

			public bool ContainsItem(TItem item)
			{
				if (item == null)
					return false;

				return Invert(FindByItem(item, false, out _));
			}

			public bool ContainsSelection(Selection<TItem> selection)
			{
				if (selection.IsEmpty)
					return false;

				return Invert(FindBySelection(selection, false, out _));
			}

			public bool ContainsSource(object source)
			{
				if (source == null)
					return false;

				return Invert(FindBySource(source, false, out _));
			}

			public bool ContainsValue(object value)
			{
				if (value == null)
					return false;

				return Invert(FindByValue(value, false, out _));
			}

			public void CopyFrom(SelectionCollectionImpl selectionCollection)
			{
				Clear();

				ListAddRange(selectionCollection.List);

				Version = selectionCollection.Version;
			}

			public void DeferRestoreCurrent()
			{
				foreach (var selection in DeferList)
					ListAdd(selection);

				DeferList.Clear();
			}

			public void DeferSaveCurrent()
			{
				foreach (var selection in List)
					DeferList.Add(selection);

				ListClear();
			}

			public void DeferUnselect(Selection<TItem> selection)
			{
				DeferList.Add(selection);
			}

			public bool FindByIndex(int index, bool inverted, out Selection<TItem> selection)
			{
				if (FindIndexOfKeyIndex(index, out var keyIndex))
				{
					if (inverted)
					{
						selection = Selection<TItem>.Empty;

						return false;
					}

					selection = ListGet(keyIndex);

					return true;
				}

				if (inverted)
				{
					Controller.Advisor.TryCreateSelection(index, false, out selection);

					return true;
				}

				selection = Selection<TItem>.Empty;

				return false;
			}

			public bool FindByItem(TItem item, bool inverted, out Selection<TItem> selection)
			{
				if (FindItemKeyIndex(item, out var keyIndex))
				{
					if (inverted)
					{
						selection = Selection<TItem>.Empty;

						return false;
					}

					selection = ListGet(keyIndex);

					return true;
				}

				if (inverted)
				{
					var index = Controller.GetIndexOfItem(item);
					var source = Controller.GetSource(item);
					var value = Controller.GetValue(item, source);

					selection = new Selection<TItem>(index, item, source, value);

					return true;
				}

				selection = Selection<TItem>.Empty;

				return false;
			}

			public bool FindBySelection(Selection<TItem> searchSelection, bool inverted, out Selection<TItem> selection)
			{
				if (FindSelectionKeyIndex(searchSelection, out var keyIndex))
				{
					if (inverted)
					{
						selection = Selection<TItem>.Empty;

						return false;
					}

					selection = ListGet(keyIndex);

					return true;
				}

				if (inverted)
				{
					selection = searchSelection;

					return true;
				}

				selection = Selection<TItem>.Empty;

				return false;
			}

			public bool FindBySource(object source, bool inverted, out Selection<TItem> selection)
			{
				if (FindSourceKeyIndex(source, out var keyIndex))
				{
					if (inverted)
					{
						selection = Selection<TItem>.Empty;

						return false;
					}

					selection = ListGet(keyIndex);

					return true;
				}

				if (inverted)
				{
					Controller.Advisor.TryCreateSelection(source, false, out selection);

					return true;
				}

				selection = Selection<TItem>.Empty;

				return false;
			}

			public bool FindByValue(object value, bool inverted, out Selection<TItem> selection)
			{
				if (FindValueKeyIndex(value, out var keyIndex))
				{
					if (inverted)
					{
						selection = Selection<TItem>.Empty;

						return false;
					}

					selection = ListGet(keyIndex);

					return true;
				}

				if (inverted)
				{
					Controller.Advisor.TryCreateSelection(Controller.GetIndexOfValue(value), false, out selection);

					return true;
				}

				selection = Selection<TItem>.Empty;

				return false;
			}

			private bool FindIndexOfKeyIndex(int selectionIndex, out int index)
			{
				for (var i = 0; i < ListCount; i++)
				{
					if (ListGet(i).Index == selectionIndex)
					{
						index = i;

						return true;
					}
				}

				index = -1;

				return false;
			}

			private int FindIndexOfKeyIndex(int selectionIndex)
			{
				FindIndexOfKeyIndex(selectionIndex, out var index);

				return index;
			}

			private bool FindItemKeyIndex(TItem item, out int keyIndex)
			{
				for (var index = 0; index < ListCount; index++)
				{
					var current = ListGet(index);

					if (ReferenceEquals(current.Item, item))
					{
						keyIndex = index;

						return true;
					}
				}

				keyIndex = -1;

				return false;
			}


			private bool FindSelectionKeyIndex(Selection<TItem> searchSelection, out int keyIndex)
			{
				for (var index = 0; index < ListCount; index++)
				{
					var current = ListGet(index);

					if (Controller.AreSelectionEquals(current, searchSelection))
					{
						keyIndex = index;

						return true;
					}
				}

				keyIndex = -1;

				return false;
			}

			private bool FindSourceKeyIndex(object source, out int keyIndex)
			{
				for (var index = 0; index < ListCount; index++)
				{
					var current = ListGet(index);

					if (ReferenceEquals(current.Source, source))
					{
						keyIndex = index;

						return true;
					}
				}

				keyIndex = -1;

				return false;
			}


			private bool FindValueKeyIndex(object value, out int keyIndex)
			{
				for (var index = 0; index < ListCount; index++)
				{
					var current = ListGet(index);

					if (Controller.CompareValues(current.Value, value))
					{
						keyIndex = index;

						return true;
					}
				}

				keyIndex = -1;

				return false;
			}

			public SelectionCollectionEnumerator GetEnumerator()
			{
				return new SelectionCollectionEnumerator(this);
			}

			private void IncrementVersion()
			{
				Version++;
			}

			private void InsertSelection(Selection<TItem> selection)
			{
				if (ListCount == 0)
				{
					ListAdd(selection);

					return;
				}

				// TODO Implement binary search
				for (var index = 0; index < ListCount; index++)
				{
					var current = ListGet(index);

					if (selection.Index < current.Index)
					{
						ListInsert(selection, index);

						return;
					}
				}

				ListAdd(selection);
			}

			private bool Invert(bool value)
			{
				return IsInverted == false ? value : !value;
			}

			private void ListAdd(Selection<TItem> selection)
			{
				List.Add(selection);

				IncrementVersion();
			}

			private void ListAddRange(IEnumerable<Selection<TItem>> collection)
			{
				List.AddRange(collection);

				IncrementVersion();
			}

			private void ListClear()
			{
				List.Clear();

				IncrementVersion();
			}

			private Selection<TItem> ListGet(int index)
			{
				return List[index];
			}

			private void ListInsert(Selection<TItem> selection, int index)
			{
				List.Insert(index, selection);

				IncrementVersion();
			}

			private void ListRemoveAt(int index)
			{
				List.RemoveAt(index);

				IncrementVersion();
			}

			private void ListSet(int index, Selection<TItem> item)
			{
				List[index] = item;

				IncrementVersion();
			}

			public void Remove(Selection<TItem> selection)
			{
				if (FindIndexOfKeyIndex(selection.Index, out var keyIndex))
					ListRemoveAt(keyIndex);
			}

			public void RemoveIndex(int index)
			{
				if (FindByIndex(index, false, out var selection))
					Remove(selection);
			}

			public void RemoveItem(TItem item)
			{
				if (FindByItem(item, false, out var selection))
					Remove(selection);
			}

			public void RemoveSource(object source)
			{
				if (FindBySource(source, false, out var selection))
					Remove(selection);
			}

			public void RemoveValue(object value)
			{
				if (FindByValue(value, false, out var selection))
					Remove(selection);
			}

			public void Select(Selection<TItem> selection)
			{
				if (Controller.MultipleSelection == false)
					return;

				if (IsInverted)
					Remove(selection);
				else
					Add(selection);
			}

			public bool TryRemoveSource(object source, out Selection<TItem> selection)
			{
				if (FindBySource(source, false, out selection))
				{
					Remove(selection);

					return true;
				}

				return false;
			}


			public void Unselect(Selection<TItem> selection)
			{
				if (Controller.MultipleSelection == false)
					return;

				if (IsInverted)
					Add(selection);
				else
					Remove(selection);
			}

			public void UpdateIndicesSources()
			{
				_indicesToUpdate.AddRange(Keys);

				for (var index = 0; index < ListCount; index++)
				{
					var selection = ListGet(index);
					var source = Controller.GetSource(selection.Index);

					if (ReferenceEquals(selection.Source, source) == false)
					{
						if (source == null)
							_indicesToRemove.Add(index);
						else
							ListSet(index, selection.WithSource(source));
					}
				}

				foreach (var index in _indicesToRemove)
					ListRemoveAt(index);

				_indicesToUpdate.Clear();
				_indicesToRemove.Clear();
			}

			public void UpdateSelectedItem(Selection<TItem> selection)
			{
				if (FindSourceKeyIndex(selection.Source, out var keyIndex))
					ListSet(keyIndex, selection);
			}

			public void UpdateSelection(int selectionIndex, Selection<TItem> selection)
			{
				if (selectionIndex != selection.Index)
					throw new InvalidOperationException();

				if (FindIndexOfKeyIndex(selectionIndex, out var index))
					ListSet(index, selection);
			}

			IEnumerator<Selection<TItem>> IEnumerable<Selection<TItem>>.GetEnumerator()
			{
				return GetEnumerator();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}

			internal struct SelectionCollectionEnumerator : IEnumerator<Selection<TItem>>, IIndexedEnumerableAdvisor
			{
				private SelectionCollectionImpl _collection;
				private readonly long _version;
				private readonly bool _inverted;
				private int _index;

				public SelectionCollectionEnumerator(SelectionCollectionImpl collection)
				{
					_collection = collection;
					_version = collection.Controller.Version;
					_inverted = collection.Controller.IsInverted;
					_index = -1;

					Current = Selection<TItem>.Empty;
				}

				private void Verify()
				{
					if (_collection == null)
						throw new InvalidOperationException("Enumerator has been disposed.");

					if (_version != _collection.Controller.Version)
						throw new InvalidOperationException("Collection has changed.");
				}

				public bool MoveNext()
				{
					Verify();

					if (_inverted)
						return MoveNextInverted();

					_index++;

					if (_collection.Controller.MultipleSelection)
					{
						var moveNext = _index < _collection.ListCount;

						Current = moveNext ? _collection.ListGet(_index) : Selection<TItem>.Empty;

						return moveNext;
					}

					if (_index == 0)
					{
						Current = _collection.SingleSelection;

						return Current.IsEmpty == false;
					}

					Current = Selection<TItem>.Empty;

					return false;
				}

				private bool MoveNextInverted()
				{
					var fullCount = _collection.Controller.Count;

					if (_index >= fullCount || fullCount - _collection.ListCount == 0)
						return false;

					var controller = _collection.Controller;

					Current = Selection<TItem>.Empty;

					while (_index + 1 < fullCount)
					{
						_index++;

						if (_collection.FindIndexOfKeyIndex(_index, out _))
							continue;

						controller.Advisor.TryCreateSelection(_index, false, out var selection);

						Current = selection;

						return true;
					}

					_index = fullCount;

					return false;
				}

				public void Reset()
				{
					Verify();

					Current = Selection<TItem>.Empty;
					_index = -1;
				}

				object IEnumerator.Current => Current;

				public Selection<TItem> Current { get; private set; }

				public void Dispose()
				{
					_collection = null;
				}

				public bool IsEnumeratorValid => _collection != null && _version == _collection.Controller.Version;
			}
		}
	}
}