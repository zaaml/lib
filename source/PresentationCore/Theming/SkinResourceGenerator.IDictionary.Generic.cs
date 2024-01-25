// <copyright file="SkinResourceGenerator.IDictionary.Generic.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Zaaml.PresentationCore.Theming
{
	public abstract partial class SkinResourceGenerator : IDictionary<string, SkinResourceTemplate>
	{
		private IDictionary<string, SkinResourceTemplate> DictionaryGeneric => Dictionary;

		public void Add(KeyValuePair<string, SkinResourceTemplate> item)
		{
			DictionaryGeneric.Add(item);
		}

		public void Clear()
		{
			DictionaryGeneric.Clear();
		}

		public bool Contains(KeyValuePair<string, SkinResourceTemplate> item)
		{
			return DictionaryGeneric.Contains(item);
		}

		public void CopyTo(KeyValuePair<string, SkinResourceTemplate>[] array, int arrayIndex)
		{
			DictionaryGeneric.CopyTo(array, arrayIndex);
		}

		public bool Remove(KeyValuePair<string, SkinResourceTemplate> item)
		{
			return DictionaryGeneric.Remove(item);
		}

		public int Count => DictionaryGeneric.Count;

		public bool IsReadOnly => DictionaryGeneric.IsReadOnly;

		public bool ContainsKey(string key)
		{
			return DictionaryGeneric.ContainsKey(key);
		}

		public void Add(string key, SkinResourceTemplate value)
		{
			DictionaryGeneric.Add(key, value);
		}

		public bool Remove(string key)
		{
			return DictionaryGeneric.Remove(key);
		}

		public bool TryGetValue(string key, out SkinResourceTemplate value)
		{
			return DictionaryGeneric.TryGetValue(key, out value);
		}

		public SkinResourceTemplate this[string key]
		{
			get => DictionaryGeneric[key];
			set => DictionaryGeneric[key] = value;
		}

		public ICollection<string> Keys => DictionaryGeneric.Keys;

		public ICollection<SkinResourceTemplate> Values => DictionaryGeneric.Values;

		public IEnumerator<KeyValuePair<string, SkinResourceTemplate>> GetEnumerator()
		{
			return DictionaryGeneric.GetEnumerator();
		}
	}
}