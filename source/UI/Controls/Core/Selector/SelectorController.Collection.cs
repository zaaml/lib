// <copyright file="SelectorController.Collection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Zaaml.UI.Controls.Core
{
	internal abstract partial class SelectorController<TItem>
	{
		internal sealed class SelectionCollectionImpl : IEnumerable<Selection<TItem>>, INotifyCollectionChanged
		{
			// ReSharper disable once StaticMemberInGenericType
			private static readonly NotifyCollectionChangedEventArgs ResetNotifyCollectionChangedEventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
			private readonly List<int> _indicesToRemove = new List<int>();

			public SelectionCollectionImpl(SelectorController<TItem> controller)
			{
				Controller = controller;
			}

			public SelectorController<TItem> Controller { get; }

			public bool Include { get; set; } = true;

			private SortedList<int, Selection<TItem>> List { get; } = new SortedList<int, Selection<TItem>>();

			public void Add(Selection<TItem> newSelection)
			{
				if (Controller.MultipleSelection == false)
				{
					Controller.RaiseSelectionCollectionChanged();

					return;
				}

				var existIndex = -1;
				var index = 0;

				foreach (var selection in this)
				{
					if (Controller.AreSelectionEquals(selection, newSelection))
					{
						if (existIndex == -1)
							existIndex = index;
						else
							_indicesToRemove.Add(index);
					}
					else if (ReferenceEquals(selection.Item, newSelection.Item))
						_indicesToRemove.Add(index);
					else if (ReferenceEquals(selection.ItemSource, newSelection.ItemSource))
						_indicesToRemove.Add(index);
					else if (Controller.CompareValues(selection.Value, newSelection.Value))
						_indicesToRemove.Add(index);

					index++;
				}

				var raiseChange = _indicesToRemove.Count > 0 || existIndex == -1;

				foreach (var removeIndex in _indicesToRemove)
					List.RemoveAt(removeIndex);

				_indicesToRemove.Clear();

				if (existIndex == -1)
					List.Add(newSelection.Index, newSelection);

				if (raiseChange)
					Controller.RaiseSelectionCollectionChanged();
			}

			public void Clear()
			{
				if (Controller.MultipleSelection == false)
				{
					Controller.RaiseSelectionCollectionChanged();

					return;
				}

				if (List.Count == 0)
					return;

				List.Clear();

				RaiseCollectionChanged();
			}

			public bool ContainsIndex(int index)
			{
				foreach (var selection in this)
				{
					if (selection.Index == index)
						return Include;
				}

				return Include == false;
			}

			public bool ContainsItem(TItem item)
			{
				foreach (var selection in this)
				{
					if (ReferenceEquals(selection.Item, item))
						return Include;
				}

				return Include == false;
			}

			public bool ContainsItemSource(object itemSource)
			{
				foreach (var selection in this)
				{
					if (ReferenceEquals(selection.ItemSource, itemSource))
						return Include;
				}

				return Include == false;
			}

			public bool ContainsSelection(Selection<TItem> selection)
			{
				foreach (var current in this)
				{
					if (Controller.AreSelectionEquals(current, selection))
						return Include;
				}

				return Include == false;
			}

			public bool ContainsValue(object value)
			{
				foreach (var selection in this)
				{
					if (Controller.CompareValues(selection.Value, value))
						return Include;
				}

				return Include == false;
			}

			public SelectionCollectionEnumerator GetEnumerator()
			{
				return new SelectionCollectionEnumerator(this);
			}

			private void RaiseCollectionChanged()
			{
				Controller.RaiseSelectionCollectionChanged();

				CollectionChanged?.Invoke(this, ResetNotifyCollectionChangedEventArgs);
			}

			public void UpdateSelectedItem(in Selection<TItem> selection)
			{
			}

			IEnumerator<Selection<TItem>> IEnumerable<Selection<TItem>>.GetEnumerator()
			{
				return GetEnumerator();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}

			public event NotifyCollectionChangedEventHandler CollectionChanged;

			internal struct SelectionCollectionEnumerator : IEnumerator<Selection<TItem>>
			{
				private SelectionCollectionImpl _collection;
				private readonly long _version;
				private int _index;

				public SelectionCollectionEnumerator(SelectionCollectionImpl collection)
				{
					_collection = collection;
					_version = collection.Controller.Version;
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

					_index++;

					if (_collection.Controller.MultipleSelection)
					{
						var moveNext = _index < _collection.List.Count;

						Current = moveNext ? _collection.List[_collection.List.Keys[_index]] : Selection<TItem>.Empty;

						return moveNext;
					}

					Current = _index == 0 ? _collection.Controller.CurrentSelection : Selection<TItem>.Empty;

					return _index == 0;
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
			}
		}
	}
}