// <copyright file="SelectionCollectionBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;

namespace Zaaml.UI.Controls.Core
{
	public abstract class SelectionCollectionBase<TItem> : IEnumerable<Selection<TItem>>, INotifyCollectionChanged, INotifyPropertyChanged where TItem : FrameworkElement
	{
		internal SelectionCollectionBase(SelectorController<TItem> selectorController)
		{
			SelectorController = selectorController;
			selectorController.SelectionCollectionChanged += SelectorControllerOnSelectionCollectionChanged;
			selectorController.SelectionCollectionPropertyChanged += SelectorControllerOnSelectionCollectionPropertyChanged;
		}

		public virtual int Count => SelectorController.SelectedCount;

		internal SelectorController<TItem> SelectorController { get; }

		public virtual SelectionCollectionEnumerator GetEnumerator()
		{
			return new SelectionCollectionEnumerator(this);
		}

		private void SelectorControllerOnSelectionCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			CollectionChanged?.Invoke(this, e);
		}

		private void SelectorControllerOnSelectionCollectionPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			PropertyChanged?.Invoke(this, e);
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

		public event PropertyChangedEventHandler PropertyChanged;

		public struct SelectionCollectionEnumerator : IEnumerator<Selection<TItem>>
		{
			private SelectorController<TItem>.SelectionCollectionImpl.SelectionCollectionEnumerator _enumerator;
			private readonly IEnumerator<Selection<TItem>> _alternativeEnumerator;

			internal SelectionCollectionEnumerator(SelectionCollectionBase<TItem> selectionCollection)
			{
				_alternativeEnumerator = null;
				_enumerator = selectionCollection.SelectorController.GetSelectionCollectionEnumerator();
			}

			internal SelectionCollectionEnumerator(IEnumerator<Selection<TItem>> enumerator)
			{
				_alternativeEnumerator = enumerator;
				_enumerator = default;
			}

			public bool MoveNext()
			{
				if (_alternativeEnumerator != null)
					return _alternativeEnumerator.MoveNext();

				return _enumerator.MoveNext();
			}

			public void Reset()
			{
				if (_alternativeEnumerator != null)
					_alternativeEnumerator.Reset();
				else
					_enumerator.Reset();
			}

			object IEnumerator.Current => _alternativeEnumerator?.Current ?? _enumerator.Current;

			public Selection<TItem> Current => _alternativeEnumerator?.Current ?? _enumerator.Current;

			public void Dispose()
			{
				if (_alternativeEnumerator != null)
					_alternativeEnumerator.Dispose();
				else
					_enumerator.Dispose();
			}
		}
	}
}