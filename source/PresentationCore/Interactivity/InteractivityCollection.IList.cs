// <copyright file="InteractivityCollection.IList.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using Zaaml.Core.Collections;

#pragma warning disable 108

namespace Zaaml.PresentationCore.Interactivity
{
	public abstract class InteractivityCollection
	{
		internal abstract IEnumerable<InteractivityObject> Items { get; }

		internal void WalkTree(IInteractivityVisitor visitor)
		{
			foreach (var interactivityObject in Items)
				interactivityObject.WalkTree(visitor);
		}
	}

	public abstract partial class InteractivityCollection<T> : InteractivityCollection, IList where T : InteractivityObject
	{
		internal override IEnumerable<InteractivityObject> Items => this;

		private IList ListImplementation => _innerCollection ?? (IList)EmptyReadOnlyList<T>.Instance;

		void ICollection.CopyTo(Array array, int index)
		{
			ListImplementation.CopyTo(array, index);
		}

		int ICollection.Count => ListImplementation.Count;

		object ICollection.SyncRoot => ListImplementation.SyncRoot;

		bool ICollection.IsSynchronized => ListImplementation.IsSynchronized;

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ListImplementation.GetEnumerator();
		}

		int IList.Add(object value)
		{
			return AddImpl((T)value);
		}

		bool IList.Contains(object value)
		{
			return ListImplementation.Contains(value);
		}

		void IList.Clear()
		{
			ClearImpl();
		}

		int IList.IndexOf(object value)
		{
			return ListImplementation.IndexOf(value);
		}


		void IList.Insert(int index, object value)
		{
			InsertImpl(index, (T)value);
		}

		void IList.Remove(object value)
		{
			RemoveImpl((T)value, -1);
		}

		void IList.RemoveAt(int index)
		{
			RemoveAtImpl(index);
		}

		object IList.this[int index]
		{
			get => ListImplementation[index];
			set => SetItemImpl(index, (T)value);
		}

		bool IList.IsReadOnly => ListImplementation.IsReadOnly;

		bool IList.IsFixedSize => ListImplementation.IsFixedSize;

		int ICollection<T>.Count => ListImplementation.Count;

		void ICollection<T>.Clear()
		{
			ClearImpl();
		}

		bool ICollection<T>.IsReadOnly => ListImplementation.IsReadOnly;

		void IList<T>.RemoveAt(int index)
		{
			RemoveAtImpl(index);
		}
	}
}