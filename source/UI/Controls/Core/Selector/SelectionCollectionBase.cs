// <copyright file="SelectionCollectionBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections;
using System.Collections.Generic;
using System.Windows;

namespace Zaaml.UI.Controls.Core
{
	public abstract class SelectionCollectionBase<TItem> : IEnumerable<Selection<TItem>> where TItem : FrameworkElement, ISelectable
	{
		internal SelectionCollectionBase(SelectorController<TItem> selectorController)
		{
			SelectorController = selectorController;
		}

		internal SelectorController<TItem> SelectorController { get; }

		public SelectionCollectionEnumerator GetEnumerator()
		{
			return new SelectionCollectionEnumerator(SelectorController);
		}

		IEnumerator<Selection<TItem>> IEnumerable<Selection<TItem>>.GetEnumerator()
		{
			return GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public struct SelectionCollectionEnumerator : IEnumerator<Selection<TItem>>
		{
			private SelectorController<TItem>.SelectionCollectionImpl.SelectionCollectionEnumerator _enumerator;

			internal SelectionCollectionEnumerator(SelectorController<TItem> selectorController)
			{
				_enumerator = selectorController.GetSelectionCollectionEnumerator();
			}

			public bool MoveNext()
			{
				return _enumerator.MoveNext();
			}

			public void Reset()
			{
				_enumerator.Reset();
			}

			object IEnumerator.Current => _enumerator.Current;

			public Selection<TItem> Current => _enumerator.Current;

			public void Dispose()
			{
				_enumerator.Dispose();
			}
		}
	}
}