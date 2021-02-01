// <copyright file="SkinExpressionResourceGenerator.IDictionary.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections;

namespace Zaaml.PresentationCore.Theming
{
	public abstract partial class SkinResourceGenerator : IDictionary
	{
		private IDictionary DictionaryInterface => Dictionary;

		void ICollection.CopyTo(Array array, int index)
		{
			DictionaryInterface.CopyTo(array, index);
		}

		int ICollection.Count => DictionaryInterface.Count;

		object ICollection.SyncRoot => DictionaryInterface.SyncRoot;

		bool ICollection.IsSynchronized => DictionaryInterface.IsSynchronized;

		bool IDictionary.Contains(object key)
		{
			return DictionaryInterface.Contains(key);
		}

		void IDictionary.Add(object key, object value)
		{
			DictionaryInterface.Add(key, value);
		}

		void IDictionary.Clear()
		{
			DictionaryInterface.Clear();
		}

		IDictionaryEnumerator IDictionary.GetEnumerator()
		{
			return DictionaryInterface.GetEnumerator();
		}

		void IDictionary.Remove(object key)
		{
			DictionaryInterface.Remove(key);
		}

		object IDictionary.this[object key]
		{
			get => DictionaryInterface[key];
			set => DictionaryInterface[key] = value;
		}

		ICollection IDictionary.Keys => DictionaryInterface.Keys;

		ICollection IDictionary.Values => DictionaryInterface.Values;

		bool IDictionary.IsReadOnly => DictionaryInterface.IsReadOnly;

		bool IDictionary.IsFixedSize => DictionaryInterface.IsFixedSize;

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable) DictionaryInterface).GetEnumerator();
		}
	}
}