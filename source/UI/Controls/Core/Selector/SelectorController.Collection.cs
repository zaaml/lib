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

		internal sealed class SelectionCollectionImpl : IEnumerable<Selection<TItem>>
		{
			// ReSharper disable once StaticMemberInGenericType
			private readonly List<int> _indicesToRemove = new List<int>();
			private readonly List<int> _indicesToUpdate = new List<int>();

			public SelectionCollectionImpl(SelectorController<TItem> controller)
			{
				Controller = controller;
			}

			public SelectorController<TItem> Controller { get; }

			public int Count => Controller.MultipleSelection ? (Controller.IsInverted ? Controller.Count - List.Count : List.Count) : 0;

			public bool IsInverted => Controller.IsInverted;

			private IEnumerable<int> Keys => List.Select(s => s.Index);

			private List<Selection<TItem>> List { get; } = new List<Selection<TItem>>();

			public void Add(Selection<TItem> newSelection, bool raiseEvent = true)
			{
				if (Controller.MultipleSelection == false)
				{
					if (raiseEvent)
						RaiseCollectionChanged();

					return;
				}

				var existIndex = -1;

				if (FindIndexOfKeyIndex(newSelection.Index, out var index))
					List.RemoveAt(index);

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

				var raiseChange = _indicesToRemove.Count > 0 || existIndex == -1;

				foreach (var removeIndex in _indicesToRemove)
					List.RemoveAt(removeIndex);

				_indicesToRemove.Clear();

				if (existIndex == -1)
					InsertSelection(newSelection);

				if (raiseChange && raiseEvent)
					RaiseCollectionChanged();
			}

			public void Clear(bool raiseCollectionChanged = true)
			{
				if (Controller.MultipleSelection == false)
				{
					if (raiseCollectionChanged)
						RaiseCollectionChanged();

					return;
				}

				if (List.Count == 0)
					return;

				List.Clear();

				if (raiseCollectionChanged)
					RaiseCollectionChanged();
			}

			public bool ContainsIndex(int index)
			{
				return Invert(FindByIndex(index, false, out var _));
			}

			public bool ContainsItem(TItem item)
			{
				return Invert(FindByItem(item, false, out _));
			}
			
			private bool Invert(bool value)
			{
				return IsInverted == false ? value : !value;
			}

			public bool ContainsSource(object source)
			{
				return Invert(FindBySource(source, false, out _));
			}

			public bool ContainsSelection(Selection<TItem> selection)
			{
				return Invert(FindBySelection(selection, false, out _));
			}

			public bool ContainsValue(object value)
			{
				return Invert(FindByValue(value, false, out _));
			}

			public void CopyFrom(SelectionCollectionImpl selectionCollection)
			{
				Clear(false);

				List.AddRange(selectionCollection.List);
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
					
					selection = List[keyIndex];

					return true;
				}

				if (inverted)
				{
					Controller.Advisor.TryGetSelection(index, false, out selection);

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
					
					selection = List[keyIndex];

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

			public bool FindBySource(object source, bool inverted, out Selection<TItem> selection)
			{
				if (FindSourceKeyIndex(source, out var keyIndex))
				{
					if (inverted)
					{
						selection = Selection<TItem>.Empty;

						return false;
					}
					
					selection = List[keyIndex];

					return true;
				}

				if (inverted)
				{
					Controller.Advisor.TryGetSelection(source, false, out selection);

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
					
					selection = List[keyIndex];

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

			private bool FindIndexOfKeyIndex(int selectionIndex, out int index)
			{
				for (var i = 0; i < List.Count; i++)
				{
					if (List[i].Index == selectionIndex)
					{
						index = i;

						return true;
					}
				}

				index = -1;

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
					
					selection = List[keyIndex];

					return true;
				}

				if (inverted)
				{
					Controller.Advisor.TryGetSelection(Controller.GetIndexOfValue(value), false, out selection);

					return true;
				}

				selection = Selection<TItem>.Empty;

				return false;
			}

			private int FindIndexOfKeyIndex(int selectionIndex)
			{
				FindIndexOfKeyIndex(selectionIndex, out var index);

				return index;
			}

			private bool FindItemKeyIndex(TItem item, out int keyIndex)
			{
				for (var index = 0; index < List.Count; index++)
				{
					var current = List[index];

					if (ReferenceEquals(current.Item, item))
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
				for (var index = 0; index < List.Count; index++)
				{
					var current = List[index];

					if (ReferenceEquals(current.Source, source))
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
				for (var index = 0; index < List.Count; index++)
				{
					var current = List[index];

					if (Controller.AreSelectionEquals(current, searchSelection))
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
				for (var index = 0; index < List.Count; index++)
				{
					var current = List[index];

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

			private void InsertSelection(Selection<TItem> selection)
			{
				if (List.Count == 0)
				{
					List.Add(selection);

					return;
				}

				// TODO Implement binary search
				for (var index = 0; index < List.Count; index++)
				{
					var current = List[index];

					if (selection.Index < current.Index)
					{
						List.Insert(index, selection);

						return;
					}
				}

				List.Add(selection);
			}

			private void RaiseCollectionChanged()
			{
				Controller.RaiseSelectionCollectionChanged(ResetNotifyCollectionChangedEventArgs);
			}

			public void RemoveIndex(int index)
			{
				if (FindByIndex(index, false, out var selection))
					RemoveSelectionImpl(selection);
			}

			public void RemoveItem(TItem item)
			{
				if (FindByItem(item, false, out var selection))
					RemoveSelectionImpl(selection);
			}

			public void RemoveSource(object source)
			{
				if (FindBySource(source, false, out var selection))
					RemoveSelectionImpl(selection);
			}

			public bool TryRemoveSource(object source, out Selection<TItem> selection)
			{
				if (FindBySource(source, false, out selection))
				{
					RemoveSelectionImpl(selection);

					return true;
				}
				
				return false;
			}

			private void RemoveSelectionImpl(Selection<TItem> selection)
			{
				if (FindIndexOfKeyIndex(selection.Index, out var keyIndex))
					List.RemoveAt(keyIndex);

				RaiseCollectionChanged();
			}

			public void RemoveValue(object value)
			{
				if (FindByValue(value, false, out var selection))
					RemoveSelectionImpl(selection);
			}

			public void UpdateIndicesSources()
			{
				_indicesToUpdate.AddRange(Keys);

				for (var index = 0; index < List.Count; index++)
				{
					var selection = List[index];
					var source = Controller.GetSource(selection.Index);

					if (ReferenceEquals(selection.Source, source) == false)
					{
						if (source == null)
							_indicesToRemove.Add(index);
						else
						{
							List[index] = selection.WithSource(source);

							Controller.RaiseSelectionCollectionPropertyChanged(new PropertyChangedEventArgs("Item[]"));
							RaiseCollectionChanged();
						}
					}
				}

				foreach (var index in _indicesToRemove)
					List.RemoveAt(index);

				_indicesToUpdate.Clear();
				_indicesToRemove.Clear();
			}

			public void UpdateSelectedItem(Selection<TItem> selection)
			{
				if (FindSourceKeyIndex(selection.Source, out var keyIndex))
					List[keyIndex] = selection;
			}

			public void UpdateSelection(int selectionIndex, Selection<TItem> selection)
			{
				if (selectionIndex != selection.Index)
					throw new InvalidOperationException();

				if (FindIndexOfKeyIndex(selectionIndex, out var index))
					List[index] = selection;

				Controller.RaiseSelectionCollectionPropertyChanged(new PropertyChangedEventArgs("Item[]"));
				RaiseCollectionChanged();
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
					{
						return MoveNextInverted();
					}	
					
					_index++;

					if (_collection.Controller.MultipleSelection)
					{
						var moveNext = _index < _collection.List.Count;

						Current = moveNext ? _collection.List[_index] : Selection<TItem>.Empty;

						return moveNext;
					}

					if (_index == 0)
					{
						Current = _collection.Controller.CurrentSelection;

						return Current.IsEmpty == false;
					}

					Current = Selection<TItem>.Empty;

					return false;
				}

				private bool MoveNextInverted()
				{
					var fullCount = _collection.Controller.Count;
					
					if (_index >= fullCount || fullCount - _collection.List.Count == 0)
						return false;

					var controller = _collection.Controller;
					
					Current = Selection<TItem>.Empty;
					
					while (_index + 1 < fullCount)
					{
						_index++;

						if (_collection.FindIndexOfKeyIndex(_index, out _))
							continue;

						controller.Advisor.TryGetSelection(_index, false, out var selection);

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