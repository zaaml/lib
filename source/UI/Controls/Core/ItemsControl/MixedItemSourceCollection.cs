// <copyright file="MixedItemSourceCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using Zaaml.PresentationCore;

namespace Zaaml.UI.Controls.Core
{
	[SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
	internal sealed class MixedItemSourceCollection<TItem> : IReadOnlyList<object>, IDisposable, INotifyCollectionChanged
		where TItem : FrameworkElement
	{
		public MixedItemSourceCollection(ItemCollectionBase<TItem> itemCollection, IEnumerable sourceCollection, int itemCollectionSplitIndex)
		{
			IndexedSourceCollection = new IndexedEnumerable(sourceCollection);
			SourceCollection = sourceCollection;
			ItemCollectionSplitIndex = itemCollectionSplitIndex >= 0 ? itemCollectionSplitIndex.Clamp(0, itemCollection.Count) : itemCollection.Count;
			ItemCollection = itemCollection;

			if (SourceCollection is INotifyCollectionChanged incc)
				incc.CollectionChanged += OnSourceCollectionChanged;

			((INotifyCollectionChanged) itemCollection).CollectionChanged += OnItemCollectionChanged;
		}

		public IndexedEnumerable IndexedSourceCollection { get; }

		public ItemCollectionBase<TItem> ItemCollection { get; }

		public int ItemCollectionSplitIndex { get; }

		public IEnumerable SourceCollection { get; }

		public Enumerator GetEnumerator()
		{
			return new Enumerator(this);
		}

		private void OnItemCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (IndexedSourceCollection.IsEmpty || IndexedSourceCollection.Count == 0)
				CollectionChanged?.Invoke(this, e);
			else
				CollectionChanged?.Invoke(this, Constants.NotifyCollectionChangedReset);
		}

		private void OnSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (ItemCollection.Count == 0)
				CollectionChanged?.Invoke(this, e);
			else
				CollectionChanged?.Invoke(this, Constants.NotifyCollectionChangedReset);
		}

		public void Dispose()
		{
			((INotifyCollectionChanged) ItemCollection).CollectionChanged -= OnItemCollectionChanged;

			if (SourceCollection is INotifyCollectionChanged incc)
				incc.CollectionChanged -= OnSourceCollectionChanged;
		}

		public event NotifyCollectionChangedEventHandler CollectionChanged;

		IEnumerator<object> IEnumerable<object>.GetEnumerator()
		{
			return GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public int Count => IndexedSourceCollection.Count + ItemCollection.Count;

		public object this[int index]
		{
			get
			{
				if (Count == 0)
					throw new ArgumentOutOfRangeException(nameof(index));

				var itemCount = ItemCollection.Count;

				if (itemCount > 0 && index >= 0 && index < ItemCollectionSplitIndex)
					return ItemCollection[index];

				var sourceCount = IndexedSourceCollection.Count;

				index -= ItemCollectionSplitIndex;

				if (index >= 0 && index < sourceCount)
					return IndexedSourceCollection[index];

				index -= sourceCount;
				index += ItemCollectionSplitIndex;

				if (index >= 0 && index < itemCount)
					return ItemCollection[index + ItemCollectionSplitIndex];

				throw new ArgumentOutOfRangeException(nameof(index));
			}
		}

		public struct Enumerator : IEnumerator<object>
		{
			private int _index;
			private object _current;

			public MixedItemSourceCollection<TItem> MixedCollection { get; }

			public Enumerator(MixedItemSourceCollection<TItem> mixedCollection)
			{
				_index = -1;
				_current = null;

				MixedCollection = mixedCollection;
			}

			public bool MoveNext()
			{
				if (_index == MixedCollection.Count)
					throw new InvalidOperationException();

				if (_index == -2)
					throw new InvalidOperationException();

				if (++_index == MixedCollection.Count)
				{
					Current = null;

					return false;
				}

				Current = MixedCollection[_index];

				return true;
			}

			public void Reset()
			{
				_index = -1;
			}

			private object Current
			{
				get
				{
					if (_index < 0)
						throw new InvalidOperationException();

					return _current;
				}
				set => _current = value;
			}

			object IEnumerator<object>.Current => Current;

			object IEnumerator.Current => Current;

			public void Dispose()
			{
				_index = -2;
			}
		}
	}
}