// <copyright file="CompositeCollectionContainer.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Windows;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.PresentationCore.Data
{
	public sealed class CompositeCollectionContainer : InheritanceContextObject
	{
		public static readonly DependencyProperty SourceCollectionProperty = DPM.Register<IEnumerable, CompositeCollectionContainer>
			("SourceCollection", d => d.OnSourceCollectionPropertyChangedPrivate);

		public static readonly DependencyProperty SourceItemProperty = DPM.Register<object, CompositeCollectionContainer>
			("SourceItem", d => d.OnSourceItemPropertyChangedPrivate);

		private CompositeCollection _compositeCollection;
		private IndexedEnumerable _indexedEnumerable;
		private Mode _mode;

		internal int Count
		{
			get
			{
				return _mode switch
				{
					Mode.Item => 1,
					Mode.Collection => _indexedEnumerable.Count,
					_ => 0
				};
			}
		}

		internal object this[int index]
		{
			get
			{
				return _mode switch
				{
					Mode.Item => SourceItem,
					Mode.Collection => _indexedEnumerable[index],
					_ => throw new ArgumentOutOfRangeException()
				};
			}
		}

		public IEnumerable SourceCollection
		{
			get => (IEnumerable)GetValue(SourceCollectionProperty);
			set => SetValue(SourceCollectionProperty, value);
		}

		public object SourceItem
		{
			get => GetValue(SourceItemProperty);
			set => SetValue(SourceItemProperty, value);
		}

		private void IndexedEnumerableOnInnerCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			_compositeCollection?.OnSourceChangedInternal(this);
		}

		private void OnSourceCollectionPropertyChangedPrivate(IEnumerable oldValue, IEnumerable newValue)
		{
			UpdateMode();

			if (oldValue != null)
			{
				_indexedEnumerable.InnerCollectionChanged -= IndexedEnumerableOnInnerCollectionChanged;
				_indexedEnumerable = null;
			}

			if (newValue != null)
			{
				_indexedEnumerable = new IndexedEnumerable(newValue);
				_indexedEnumerable.InnerCollectionChanged += IndexedEnumerableOnInnerCollectionChanged;
			}

			_compositeCollection?.OnSourceChangedInternal(this);
		}

		private void OnSourceItemPropertyChangedPrivate(object oldValue, object newValue)
		{
			UpdateMode();

			_compositeCollection?.OnSourceChangedInternal(this);
		}

		private Mode ResolveMode()
		{
			var isItem = DependencyPropertyUtils.GetValueSource(this, SourceItemProperty) != PropertyValueSource.Default;
			var isCollection = DependencyPropertyUtils.GetValueSource(this, SourceCollectionProperty) != PropertyValueSource.Default;

			if (isItem && isCollection)
				throw new InvalidOperationException("SourceItem and SourceCollection can not be used simultaneously.");

			if (isItem)
				return Mode.Item;

			if (isCollection)
				return Mode.Collection;

			return Mode.None;
		}

		internal void SetCollectionInternal(CompositeCollection collection)
		{
			_compositeCollection = collection;
		}

		private void UpdateMode()
		{
			_mode = ResolveMode();
		}

		private enum Mode
		{
			None,
			Item,
			Collection
		}
	}
}