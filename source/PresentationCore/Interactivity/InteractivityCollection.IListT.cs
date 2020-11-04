// <copyright file="InteractivityCollection.IListT.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using Zaaml.Core.Collections;

#pragma warning disable 108

namespace Zaaml.PresentationCore.Interactivity
{
	public abstract partial class InteractivityCollection<T> : IList<T> where T : InteractivityObject
	{
		#region Properties

		public int Count => ListTImplementation.Count;

		public bool IsReadOnly => ListTImplementation.IsReadOnly;

		private IList<T> ListTImplementation => _innerCollection ?? EmptyReadOnlyList<T>.Instance;

		#endregion

		#region Interface Implementations

		#region ICollection<T>

		public void Add(T item)
		{
			AddImpl(item);
		}

		public bool Contains(T item)
		{
			return ListTImplementation.Contains(item);
		}

		void ICollection<T>.CopyTo(T[] array, int arrayIndex)
		{
			ListTImplementation.CopyTo(array, arrayIndex);
		}

		public bool Remove(T item)
		{
			return RemoveImpl(item, -1);
		}

		#endregion

		#region IEnumerable<T>

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return ListTImplementation.GetEnumerator();
		}

		#endregion

		#region IList<T>

		int IList<T>.IndexOf(T item)
		{
			return ListTImplementation.IndexOf(item);
		}

		public void Insert(int index, T item)
		{
			InsertImpl(index, item);
		}

		public T this[int index]
		{
			get => ListTImplementation[index];
			set => SetItemImpl(index, value);
		}

		#endregion

		#endregion
	}
}