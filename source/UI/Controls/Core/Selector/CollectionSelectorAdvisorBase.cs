// <copyright file="CollectionSelectorAdvisorBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Windows;
using Zaaml.PresentationCore;

namespace Zaaml.UI.Controls.Core
{
	internal abstract class CollectionSelectorAdvisorBase<T> : ISelectorAdvisor<T> where T : FrameworkElement
	{
		public event EventHandler<ItemSelectionChangedEventArgs<T>> ItemSelectionChanged;

		protected CollectionSelectorAdvisorBase(ISelector<T> selector, IList<T> collection)
		{
			SelectorCore = selector;
			Collection = collection;
		}

		protected IList<T> Collection { get; }

		private protected ISelector<T> SelectorCore { get; }

		protected virtual void OnItemSelectionChanged(ItemSelectionChangedEventArgs<T> e)
		{
			ItemSelectionChanged?.Invoke(this, e);
		}

		public virtual void Dispose()
		{
		}

		public abstract object GetValue(T item, object source);

		public abstract object GetSource(T item);

		public abstract void Unlock(T item);

		public virtual bool CanSelectItem(T item)
		{
			return true;
		}

		public virtual bool CanSelectSource(object source)
		{
			return true;
		}

		public virtual bool CanSelectValue(object value)
		{
			return true;
		}

		public virtual bool CanSelectIndex(int index)
		{
			return true;
		}

		public abstract bool GetItemSelected(T item);

		public abstract void SetItemSelected(T item, bool value);

		public abstract bool GetSourceSelected(object source);

		public abstract void SetSourceSelected(object source, bool value);

		public abstract void Lock(T item);

		public SelectorController<T> Controller { get; set; }

		public virtual int Count => Collection.Count;

		public abstract bool HasSource { get; }

		public abstract bool IsVirtualizing { get; }

		public virtual int GetIndexOfItem(T item)
		{
			return Collection.FindIndex(t => ReferenceEquals(t, item));
		}

		public abstract int GetIndexOfSource(object source);

		public virtual bool TryGetItemByIndex(int index, bool ensure, out T item)
		{
			if (index >= 0 && index < Collection.Count && Collection.Count > 0)
			{
				item = Collection[index];

				return true;
			}

			item = default;

			return false;
		}

		public abstract bool TryCreateSelection(int index, bool ensure, out Selection<T> selection);

		public abstract bool TryCreateSelection(object source, bool ensure, out Selection<T> selection);

		public abstract bool TryGetItemBySource(object source, bool ensure, out T item);

		public abstract bool TryGetItemByValue(object value, bool ensure, out T item);

		public abstract object GetSource(int index);

		public bool EqualValues(object value1, object value2)
		{
			return XamlValueComparer.AreEqual(value1, value2);
		}

		public virtual int GetIndexOfValue(object value)
		{
			var count = Count;

			for (var i = 0; i < count; i++)
			{
				TryGetItemByIndex(i, false, out var item);

				var source = GetSource(i);
				var itemValue = GetValue(item, source);

				if (EqualValues(itemValue, value))
					return i;
			}

			return -1;
		}
	}
}