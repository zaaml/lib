// <copyright file="CollectionSelectorAdvisorBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Windows;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore;

namespace Zaaml.UI.Controls.Core
{
	internal abstract class CollectionSelectorAdvisorBase<T> : ISelectorAdvisor<T> where T : FrameworkElement, ISelectable
	{
		#region Fields

		public event EventHandler<ItemSelectionChangedEventArgs<T>> ItemSelectionChanged;

		#endregion

		#region Ctors

		protected CollectionSelectorAdvisorBase(ISelector<T> selector, IList<T> collection)
		{
			Selector = selector;
			Collection = collection;
		}

		#endregion

		#region Properties

		protected IList<T> Collection { get; }

		private ISelector<T> Selector { get; }

		#endregion

		#region  Methods

		public virtual bool GetIsSelected(T item)
		{
			return item.IsSelected;
		}

		protected virtual void OnItemSelectionChanged(ItemSelectionChangedEventArgs<T> e)
		{
			ItemSelectionChanged?.Invoke(this, e);
		}

		public virtual void SetIsSelected(T item, bool value)
		{
			item.IsSelected = value;
		}

		#endregion

		#region Interface Implementations

		#region IDisposable

		public virtual void Dispose()
		{
		}

		#endregion

		#region ISelectorAdvisor<T>

		public abstract object GetItemSource(T item);

		public virtual object GetValue(int index)
		{
			return null;
		}

		public abstract void Unlock(T item);

		public abstract void Lock(T item);

		public SelectorController<T> Controller { get; set; }

		public virtual int Count => Collection.Count;

		public abstract bool HasSource { get; }

		public virtual int GetIndexOfItem(T item)
		{
			return Collection.FindIndex(t => ReferenceEquals(t, item));
		}

		public abstract int GetIndexOfItemSource(object itemSource);

		public virtual bool TryGetItem(int index, out T item)
		{
			if (index >= 0 && index < Collection.Count && Collection.Count > 0)
			{
				item = Collection[index];

				return true;
			}

			item = default;

			return false;
		}

		public abstract bool TryGetItemBySource(object itemSource, out T item);

		public abstract object GetItemSource(int index);

		public bool CompareValues(object value1, object value2)
		{
			return XamlValueComparer.AreEqual(value1, value2);
		}

		public virtual int GetIndexOfValue(object value)
		{
			var count = Count;

			for (var i = 0; i < count; i++)
			{
				TryGetItem(i, out var item);

				var itemSource = GetItemSource(i);
				var itemValue = Selector.GetValue(item, itemSource);

				if (CompareValues(itemValue, value))
					return i;
			}

			return -1;
		}

		#endregion

		#endregion
	}
}