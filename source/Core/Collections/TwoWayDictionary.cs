// <copyright file="TwoWayDictionary.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using Zaaml.Core.Extensions;

namespace Zaaml.Core.Collections
{
	internal class TwoWayDictionary<TKey, TValue>
	{
		#region Fields

		private readonly Dictionary<TKey, TValue> _key2ValueDictionary;
		private readonly Dictionary<TValue, TKey> _value2KeyDictionary;

		#endregion

		#region Ctors

		public TwoWayDictionary()
		{
			_key2ValueDictionary = new Dictionary<TKey, TValue>();
			_value2KeyDictionary = new Dictionary<TValue, TKey>();
		}

		public TwoWayDictionary(IEqualityComparer<TKey> keyComparer, IEqualityComparer<TValue> valueComparer)
		{
			_key2ValueDictionary = new Dictionary<TKey, TValue>(keyComparer ?? EqualityComparer<TKey>.Default);
			_value2KeyDictionary = new Dictionary<TValue, TKey>(valueComparer ?? EqualityComparer<TValue>.Default);
		}

		#endregion

		#region Methods

		public void Add(TKey key, TValue value)
		{
			_key2ValueDictionary.Add(key, value);
			_value2KeyDictionary.Add(value, key);
		}

		public bool ContainsKey(TKey key)
		{
			return _key2ValueDictionary.ContainsKey(key);
		}

		public bool ContainsValue(TValue value)
		{
			return _value2KeyDictionary.ContainsKey(value);
		}

		public TKey GetKeyOrCreate(TValue value, Func<TValue, TKey> factory)
		{
			return IDictionaryExtensions.GetValueOrCreate(_value2KeyDictionary, value, factory);
		}

		public TKey GetKeyOrDefault(TValue value, TKey defaultKey = default(TKey))
		{
			return IDictionaryExtensions.GetValueOrDefault(_value2KeyDictionary, value, defaultKey);
		}

		public TValue GetValueOrCreate(TKey key, Func<TKey, TValue> factory)
		{
			return IDictionaryExtensions.GetValueOrCreate(_key2ValueDictionary, key, factory);
		}

		public TValue GetValueOrDefault(TKey key, TValue defaultValue = default(TValue))
		{
			return IDictionaryExtensions.GetValueOrDefault(_key2ValueDictionary, key, defaultValue);
		}

		public void Remove(TKey key, TValue value)
		{
			_key2ValueDictionary.Remove(key);
			_value2KeyDictionary.Remove(value);
		}

		public void RemoveByKey(TKey key)
		{
			TValue value;
			if (_key2ValueDictionary.TryGetValue(key, out value))
				Remove(key, value);
		}

		public void RemoveByValue(TValue value)
		{
			TKey key;
			if (_value2KeyDictionary.TryGetValue(value, out key))
				Remove(key, value);
		}

		public bool TryGetKey(TValue value, out TKey key)
		{
			return _value2KeyDictionary.TryGetValue(value, out key);
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			return _key2ValueDictionary.TryGetValue(key, out value);
		}

		#endregion
	}
}