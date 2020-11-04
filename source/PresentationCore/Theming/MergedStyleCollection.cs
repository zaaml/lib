// <copyright file="MergedStyleCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;

namespace Zaaml.PresentationCore.Theming
{
	public sealed class MergedStyleCollection : IList<StyleBase>, IList
	{
		#region Fields

		private readonly List<StyleBase> _innerCollection = new List<StyleBase>();

		#endregion

		#region Properties

		internal bool IsReadOnly { get; set; }

		private IList ListImplementation => _innerCollection;

		#endregion

		#region  Methods

		private void Clear()
		{
			_innerCollection.Clear();
		}

		private void RemoveAt(int index)
		{
			_innerCollection.RemoveAt(index);
		}

		#endregion

		#region Interface Implementations

		#region ICollection

		void ICollection.CopyTo(Array array, int index)
		{
			ListImplementation.CopyTo(array, index);
		}

		public object SyncRoot => ListImplementation.SyncRoot;

		public bool IsSynchronized => ListImplementation.IsSynchronized;

		#endregion

		#region ICollection<StyleBase>

		void ICollection<StyleBase>.Add(StyleBase item)
		{
			_innerCollection.Add(item);
		}

		void ICollection<StyleBase>.Clear()
		{
			Clear();
		}

		bool ICollection<StyleBase>.Contains(StyleBase item)
		{
			return _innerCollection.Contains(item);
		}

		void ICollection<StyleBase>.CopyTo(StyleBase[] array, int arrayIndex)
		{
			_innerCollection.CopyTo(array, arrayIndex);
		}

		bool ICollection<StyleBase>.Remove(StyleBase item)
		{
			return _innerCollection.Remove(item);
		}

		public int Count => _innerCollection.Count;
		bool ICollection<StyleBase>.IsReadOnly => IsReadOnly;

		#endregion

		#region IEnumerable

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable) _innerCollection).GetEnumerator();
		}

		#endregion

		#region IEnumerable<StyleBase>

		IEnumerator<StyleBase> IEnumerable<StyleBase>.GetEnumerator()
		{
			return _innerCollection.GetEnumerator();
		}

		#endregion

		#region IList

		void IList.Clear()
		{
			Clear();
		}

		bool IList.IsReadOnly => false;

		int IList.Add(object value)
		{
			return ListImplementation.Add(value);
		}

		bool IList.Contains(object value)
		{
			return ListImplementation.Contains(value);
		}

		int IList.IndexOf(object value)
		{
			return ListImplementation.IndexOf(value);
		}

		void IList.Insert(int index, object value)
		{
			ListImplementation.Insert(index, value);
		}

		void IList.Remove(object value)
		{
			ListImplementation.Remove(value);
		}

		bool IList.IsFixedSize => ListImplementation.IsFixedSize;

		object IList.this[int index]
		{
			get => ListImplementation[index];
			set => ListImplementation[index] = value;
		}

		void IList.RemoveAt(int index)
		{
			RemoveAt(index);
		}

		#endregion

		#region IList<StyleBase>

		int IList<StyleBase>.IndexOf(StyleBase item)
		{
			return _innerCollection.IndexOf(item);
		}

		void IList<StyleBase>.Insert(int index, StyleBase item)
		{
			_innerCollection.Insert(index, item);
		}

		void IList<StyleBase>.RemoveAt(int index)
		{
			RemoveAt(index);
		}

		StyleBase IList<StyleBase>.this[int index]
		{
			get => _innerCollection[index];
			set => _innerCollection[index] = value;
		}

		#endregion

		#endregion
	}
}