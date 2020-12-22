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
	internal abstract class CollectionSelectorAdvisorBase<T> : ISelectorAdvisor<T> where T : FrameworkElement
	{
		#region Fields

		public event EventHandler<ItemSelectionChangedEventArgs<T>> ItemSelectionChanged;

		#endregion

		#region Ctors

		protected CollectionSelectorAdvisorBase(ISelector<T> selector, IList<T> collection)
		{
			SelectorCore = selector;
			Collection = collection;
		}

		#endregion

		#region Properties

		protected IList<T> Collection { get; }

		private protected ISelector<T> SelectorCore { get; }

		#endregion

		#region  Methods

		protected virtual void OnItemSelectionChanged(ItemSelectionChangedEventArgs<T> e)
		{
			ItemSelectionChanged?.Invoke(this, e);
		}

		#endregion

		#region Interface Implementations

		#region IDisposable

		public virtual void Dispose()
		{
		}

		#endregion

		#region ISelectorAdvisor<T>

		public abstract object GetSource(T item);

		public virtual object GetValue(int index)
		{
			return null;
		}

		public abstract void Unlock(T item);
		
		public virtual bool CanSelect(T item)
		{
			return true;
		}

		public abstract bool GetItemSelected(T item);
		
		public abstract void SetItemSelected(T item, bool value);
		
		public abstract bool GetSourceSelected(T item);
		
		public abstract void SetSourceSelected(T item, bool value);

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

		public virtual bool TryGetItem(int index, bool ensure, out T item)
		{
			if (index >= 0 && index < Collection.Count && Collection.Count > 0)
			{
				item = Collection[index];

				return true;
			}

			item = default;

			return false;
		}

		public abstract bool TryGetSelection(int index, bool ensure, out Selection<T> selection);
		
		public abstract bool TryGetSelection(object source, bool ensure, out Selection<T> selection);

		public abstract bool TryGetItemBySource(object source, bool ensure, out T item);

		public abstract object GetSource(int index);

		public bool CompareValues(object value1, object value2)
		{
			return XamlValueComparer.AreEqual(value1, value2);
		}

		public virtual int GetIndexOfValue(object value)
		{
			var count = Count;

			for (var i = 0; i < count; i++)
			{
				TryGetItem(i, false, out var item);

				var source = GetSource(i);
				var itemValue = SelectorCore.GetValue(item, source);

				if (CompareValues(itemValue, value))
					return i;
			}

			return -1;
		}

		#endregion

		#endregion
	}
}