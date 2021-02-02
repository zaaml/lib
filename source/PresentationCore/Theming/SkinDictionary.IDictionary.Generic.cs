// <copyright file="SkinDictionary.IDictionary.Generic.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Zaaml.PresentationCore.Theming
{
	public partial class SkinDictionary : IDictionary<string, object>
	{
		private IDictionary<string, object> DictionaryGeneric => Dictionary;

		public void Add(KeyValuePair<string, object> item)
		{
			AddCore(item);
		}

		public void Clear()
		{
			ClearCore();
		}

		public bool Contains(KeyValuePair<string, object> item)
		{
			return ContainsCore(item);
		}

		public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
		{
			DictionaryGeneric.CopyTo(array, arrayIndex);
		}

		public bool Remove(KeyValuePair<string, object> item)
		{
			return RemoveCore(item);
		}

		public int Count => DictionaryGeneric.Count;

		public bool IsReadOnly => DictionaryGeneric.IsReadOnly;

		public bool ContainsKey(string key)
		{
			return ContainsKeyCore(key);
		}

		public void Add(string key, object value)
		{
			AddCore(key, value);
		}

		public bool Remove(string key)
		{
			return RemoveCore(key);
		}

		public bool TryGetValue(string key, out object value)
		{
			return TryGetValueCore(key, out value);
		}

		public object this[string key]
		{
			get => GetCore(key);
			set => SetCore(key, value);
		}

		public ICollection<string> Keys => DictionaryGeneric.Keys;

		public ICollection<object> Values => DictionaryGeneric.Values;

		public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
		{
			return DictionaryGeneric.GetEnumerator();
		}
	}
}