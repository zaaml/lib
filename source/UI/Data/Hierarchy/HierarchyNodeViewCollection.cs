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
		private IEnumerable _source;

		protected HierarchyNodeViewCollection(THierarchy hierarchy, TNode parentViewItemData, Func<object, TNode> nodeFactory)
		{
			Hierarchy = hierarchy;
			ParentViewItemData = parentViewItemData;
			NodeFactory = nodeFactory;
		}

		private List<TNode> ActualCollection => IsFiltered ? FilteredCollection : Collection;

		private List<TNode> Collection { get; set; }

		public int Count => ActualCollection?.Count ?? 0;

		private List<TNode> FilteredCollection { get; set; }

		internal IReadOnlyList<TNode> FilteredCollectionInternal => FilteredCollection;

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

		private Func<object, TNode> NodeFactory { get; }

		private TNode ParentViewItemData { get; }
		private long RefreshFilterVersion { get; set; }

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

				Collection ??= new List<TNode>();

				HandleReset();
			}
		}

		internal IReadOnlyList<TNode> SourceCollection => Collection ?? EmptyCollection;

		internal void HandleAdd(int newIndex, IList newItems)
		{
			var insertIndex = newIndex;

			foreach (var item in newItems)
				Collection.Insert(insertIndex++, NodeFactory(item));
		}

		internal void HandleMove(int oldIndex, int newIndex)
		{
			var oldItem = Collection[oldIndex];

			Collection[oldIndex] = Collection[newIndex];
			Collection[newIndex] = oldItem;
		}

		internal void HandleRemove(int oldIndex, IList oldItems)
		{
			for (var i = oldIndex; i < oldIndex + oldItems.Count; i++) 
				Collection[i].Dispose();

			Collection.RemoveRange(oldIndex, oldItems.Count);
		}

		internal void HandleReset()
		{
			foreach (var node in Collection)
				node.Dispose();

			Collection.Clear();

			if (Source != null)
				Collection.AddRange(Source.OfType<object>().Select(NodeFactory));
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
			if (RefreshFilterVersion == Hierarchy.RefreshFilterVersion)
				return;

			try
			{
				if (IsFiltered == false)
					return;

				FilteredCollection ??= new List<TNode>();

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
				RefreshFilterVersion = Hierarchy.RefreshFilterVersion;
			}
		}

		[SuppressMessage("ReSharper", "PossibleNullReferenceException")]
		private void UpdateFilterCollection(IEnumerator<TNode> filteredItemsEnumerator)
		{
			if (FilteredCollection.Count > 0)
			{
				var filterItemIndex = 0;
				var filterCollectionChanged = false;

				while (filteredItemsEnumerator.MoveNext())
				{
					var node = filteredItemsEnumerator.Current;

					node.VisibleField = true;

					if (filterItemIndex < FilteredCollection.Count)
					{
						if (ReferenceEquals(FilteredCollection[filterItemIndex], node))
							filterItemIndex++;
						else
						{
							if (filterItemIndex < FilteredCollection.Count)
								FilteredCollection.RemoveRange(filterItemIndex, FilteredCollection.Count - filterItemIndex);

							FilteredCollection.Add(node);
							filterItemIndex++;

							filterCollectionChanged = true;

							break;
						}
					}
					else
					{
						FilteredCollection.Add(node);
						filterItemIndex++;
					}
				}

				if (filterItemIndex < FilteredCollection.Count)
					FilteredCollection.RemoveRange(filterItemIndex, FilteredCollection.Count - filterItemIndex);

				if (filterCollectionChanged)
				{
					while (filteredItemsEnumerator.MoveNext())
					{
						var node = filteredItemsEnumerator.Current;

						node.VisibleField = true;

						FilteredCollection.Add(node);
					}
				}
			}
			else
			{
				while (filteredItemsEnumerator.MoveNext())
				{
					var node = filteredItemsEnumerator.Current;

					node.VisibleField = true;

					FilteredCollection.Add(filteredItemsEnumerator.Current);
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