// <copyright file="HierarchyNodeViewCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Zaaml.UI.Data.Hierarchy
{
	internal class HierarchyNodeViewCollection<THierarchy, TNodeCollection, TNode> : IEnumerable<TNode>
		where THierarchy : HierarchyView<THierarchy, TNodeCollection, TNode>
		where TNodeCollection : HierarchyNodeViewCollection<THierarchy, TNodeCollection, TNode>
		where TNode : HierarchyNodeView<THierarchy, TNodeCollection, TNode>
	{
		private static readonly IEnumerator<TNode> EmptyEnumerator = Enumerable.Empty<TNode>().GetEnumerator();
		private static readonly List<TNode> EmptyCollection = new List<TNode>();

		private readonly Func<object, TNode> _nodeFactory;
		private List<TNode> _collection;
		private List<TNode> _filteredCollection;
		private long _refreshFilterVersion;
		private IEnumerable _source;

		protected HierarchyNodeViewCollection(THierarchy hierarchy, TNode parentViewItemData, Func<object, TNode> nodeFactory)
		{
			Hierarchy = hierarchy;
			ParentViewItemData = parentViewItemData;
			_nodeFactory = nodeFactory;
		}

		private List<TNode> ActualCollection => IsFiltered ? _filteredCollection : _collection;

		public int Count => ActualCollection?.Count ?? 0;

		internal IReadOnlyList<TNode> FilteredCollection => _filteredCollection;

		private THierarchy Hierarchy { get; }

		private bool IsFiltered => Hierarchy.IsFilteredInternal;

		public TNode this[int index]
		{
			get
			{
				var collection = ActualCollection;

				if (collection == null)
					throw new ArgumentOutOfRangeException(nameof(index));

				return collection[index];
			}
		}

		private TNode ParentViewItemData { get; }

		public IEnumerable Source
		{
			get => _source;
			set
			{
				if (ReferenceEquals(_source, value))
					return;

				if (_source is INotifyCollectionChanged oldNotifyCollectionChanged)
					oldNotifyCollectionChanged.CollectionChanged -= OnSourceCollectionChanged;

				_source = value;

				if (_source is INotifyCollectionChanged newNotifyCollectionChanged)
					newNotifyCollectionChanged.CollectionChanged += OnSourceCollectionChanged;

				_collection ??= new List<TNode>();
				_collection.Clear();
				_collection.AddRange(_source.OfType<object>().Select(_nodeFactory));
			}
		}

		internal IReadOnlyList<TNode> SourceCollection => _collection ?? EmptyCollection;

		internal void HandleAdd(int newIndex, IList newItems)
		{
			var insertIndex = newIndex;

			foreach (var item in newItems)
				_collection.Insert(insertIndex++, _nodeFactory(item));
		}

		internal void HandleMove(int oldIndex, int newIndex)
		{
			var oldItem = _collection[oldIndex];

			_collection[oldIndex] = _collection[newIndex];
			_collection[newIndex] = oldItem;
		}

		internal void HandleRemove(int oldIndex, IList oldItems)
		{
			_collection.RemoveRange(oldIndex, oldItems.Count);
		}

		internal void HandleReset()
		{
			_collection.Clear();
			_collection.AddRange(_source.OfType<object>().Select(_nodeFactory));
		}

		private void OnSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (ParentViewItemData != null)
				ParentViewItemData.OnSourceCollectionChangedInternal(e);
			else
				Hierarchy.OnSourceCollectionChangedInternal(e);
		}

		internal void RefreshFilter(FilteringStrategy<THierarchy, TNodeCollection, TNode> filteringStrategy)
		{
			if (_refreshFilterVersion == Hierarchy.RefreshFilterVersion)
				return;

			try
			{
				if (IsFiltered == false)
					return;

				_filteredCollection ??= new List<TNode>();

				var filter = Hierarchy.FilterInternal;

				foreach (var node in SourceCollection)
				{
					node.PassedFilterField = filter.Pass(node);
					node.VisibleField = false;

					node.LoadNodes();
					node.Nodes.RefreshFilter(filteringStrategy);
				}

				var filteredItemsEnumerator = filteringStrategy.Filter((TNodeCollection) this).GetEnumerator();

				try
				{
					UpdateFilterCollection(filteredItemsEnumerator);
				}
				finally
				{
					filteredItemsEnumerator.Dispose();
				}
			}
			finally
			{
				_refreshFilterVersion = Hierarchy.RefreshFilterVersion;
			}
		}

		[SuppressMessage("ReSharper", "PossibleNullReferenceException")]
		private void UpdateFilterCollection(IEnumerator<TNode> filteredItemsEnumerator)
		{
			if (_filteredCollection.Count > 0)
			{
				var filterItemIndex = 0;
				var filterCollectionChanged = false;

				while (filteredItemsEnumerator.MoveNext())
				{
					var node = filteredItemsEnumerator.Current;

					node.VisibleField = true;

					if (filterItemIndex < _filteredCollection.Count)
					{
						if (ReferenceEquals(_filteredCollection[filterItemIndex], node))
							filterItemIndex++;
						else
						{
							if (filterItemIndex < _filteredCollection.Count)
								_filteredCollection.RemoveRange(filterItemIndex, _filteredCollection.Count - filterItemIndex);

							_filteredCollection.Add(node);
							filterItemIndex++;

							filterCollectionChanged = true;

							break;
						}
					}
					else
					{
						_filteredCollection.Add(node);
						filterItemIndex++;
					}
				}

				if (filterItemIndex < _filteredCollection.Count)
					_filteredCollection.RemoveRange(filterItemIndex, _filteredCollection.Count - filterItemIndex);

				if (filterCollectionChanged)
				{
					while (filteredItemsEnumerator.MoveNext())
					{
						var node = filteredItemsEnumerator.Current;

						node.VisibleField = true;

						_filteredCollection.Add(node);
					}
				}
			}
			else
			{
				while (filteredItemsEnumerator.MoveNext())
				{
					var node = filteredItemsEnumerator.Current;

					node.VisibleField = true;

					_filteredCollection.Add(filteredItemsEnumerator.Current);
				}
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public IEnumerator<TNode> GetEnumerator()
		{
			return ActualCollection?.GetEnumerator() ?? EmptyEnumerator;
		}
	}
}