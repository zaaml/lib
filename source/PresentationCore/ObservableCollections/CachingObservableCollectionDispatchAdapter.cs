// <copyright file="CachingObservableCollectionDispatchAdapter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;

namespace Zaaml.PresentationCore.ObservableCollections
{
	internal class CachingObservableCollectionDispatchAdapter<T>
	{
		private readonly List<T> _collectionCache = new();
		private readonly Action<T> _onItemAdded;
		private readonly Action<T> _onItemRemoved;
		private readonly ICollection _sourceCollection;

		public CachingObservableCollectionDispatchAdapter(ICollection sourceCollection, Action<T> onItemAdded, Action<T> onItemRemoved)
		{
			_sourceCollection = sourceCollection;
			_onItemAdded = onItemAdded;
			_onItemRemoved = onItemRemoved;
		}

		public void OnItemAdded(T item)
		{
			_collectionCache.Add(item);
			_onItemAdded(item);
		}

		public void OnItemRemoved(T item)
		{
			_collectionCache.Remove(item);
			_onItemRemoved(item);
		}

		public void OnReset()
		{
			foreach (var item in _collectionCache)
				_onItemRemoved(item);

			_collectionCache.Clear();

			foreach (T item in _sourceCollection)
				OnItemAdded(item);
		}
	}
}