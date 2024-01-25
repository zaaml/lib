// <copyright file="MultiMap.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections;
using System.Collections.Generic;
using Zaaml.Core.Extensions;

namespace Zaaml.Core.Collections
{
	internal class MultiMap<TKey, TValue> : IDictionary<TKey, List<TValue>>
	{
		private readonly Dictionary<TKey, List<TValue>> _dictionary;

		public MultiMap()
		{
			_dictionary = new Dictionary<TKey, List<TValue>>();
		}

		public MultiMap(IEqualityComparer<TKey> equalityComparer)
		{
			_dictionary = new Dictionary<TKey, List<TValue>>(equalityComparer);
		}

		public void AddValue(TKey key, TValue value)
		{
			GetOrCreateValues(key).Add(value);
		}

		public List<TValue> GetOrCreateValues(TKey key)
		{
			return _dictionary.GetValueOrCreate(key, () => new List<TValue>());
		}

		public bool RemoveValue(TKey key, TValue value, bool disposeList = true)
		{
			var list = _dictionary.GetValueOrDefault(key);
			if (list == null)
				return false;

			var result = list.Remove(value);

			if (disposeList && list.Count == 0)
				_dictionary.Remove(key);

			return result;
		}

		void ICollection<KeyValuePair<TKey, List<TValue>>>.Add(KeyValuePair<TKey, List<TValue>> item)
		{
			((ICollection<KeyValuePair<TKey, List<TValue>>>)_dictionary).Add(item);
		}

		public void Clear()
		{
			_dictionary.Clear();
		}

		bool ICollection<KeyValuePair<TKey, List<TValue>>>.Contains(KeyValuePair<TKey, List<TValue>> item)
		{
			return ((ICollection<KeyValuePair<TKey, List<TValue>>>)_dictionary).Contains(item);
		}

		void ICollection<KeyValuePair<TKey, List<TValue>>>.CopyTo(KeyValuePair<TKey, List<TValue>>[] array, int arrayIndex)
		{
			((ICollection<KeyValuePair<TKey, List<TValue>>>)_dictionary).CopyTo(array, arrayIndex);
		}

		public int Count => _dictionary.Count;

		public bool IsReadOnly => false;

		bool ICollection<KeyValuePair<TKey, List<TValue>>>.Remove(KeyValuePair<TKey, List<TValue>> item)
		{
			return ((ICollection<KeyValuePair<TKey, List<TValue>>>)_dictionary).Remove(item);
		}

		public void Add(TKey key, List<TValue> value)
		{
			_dictionary.Add(key, value);
		}

		public bool ContainsKey(TKey key)
		{
			return _dictionary.ContainsKey(key);
		}

		public List<TValue> this[TKey key]
		{
			get => _dictionary[key];
			set => _dictionary[key] = value;
		}

		public ICollection<TKey> Keys => _dictionary.Keys;

		public bool Remove(TKey key)
		{
			return _dictionary.Remove(key);
		}

		public bool TryGetValue(TKey key, out List<TValue> value)
		{
			return _dictionary.TryGetValue(key, out value);
		}

		public ICollection<List<TValue>> Values => _dictionary.Values;

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public IEnumerator<KeyValuePair<TKey, List<TValue>>> GetEnumerator()
		{
			return _dictionary.GetEnumerator();
		}
	}
}