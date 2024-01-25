// <copyright file="SkinDictionary.Logic.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using Zaaml.Core.Extensions;
using Zaaml.Core.Trees;

namespace Zaaml.PresentationCore.Theming
{
	public partial class SkinDictionary
	{
		internal static readonly DelegateTreeEnumeratorAdvisor<KeyValuePair<string, object>> ResourceTreeAdvisor = new(GetChildResourceEnumerator);
		internal static readonly DelegateTreeEnumeratorAdvisor<SkinDictionary> SkinDictionaryTreeAdvisor = new(GetChildSkinDictionaryEnumerator);

		public string DeferredKey { get; set; }

		internal bool IsAbsoluteKey => DeferredKey != null && IsPathAbsolute(DeferredKey);

		public bool IsDeferred => DeferredKey != null;

		private void AddCore(KeyValuePair<string, object> item)
		{
			AddCore(item.Key, item.Value);
		}

		private void AddCore(string key, object value)
		{
			var themeKey = new ThemeResourceKey(key);
			var current = EnsureKey(themeKey);

			key = themeKey.KeyParts[themeKey.KeyParts.Count - 1];

			current.SetDictionaryValue(key, value);
		}

		private void AddDictionaryValue(string key, object value)
		{
			Dictionary.Add(key, value);

			OnValueAdded(key, value);
		}

		private static bool AllowOverride(SkinDictionaryMergeFlags mergeFlags)
		{
			return mergeFlags == SkinDictionaryMergeFlags.Override;
		}

		private void ClearCore()
		{
			foreach (var skinValue in Values.OfType<SkinDictionary>())
			{
				skinValue.Key = null;
				skinValue.Parent = null;
			}

			DictionaryGeneric.Clear();
		}

		private bool ContainsCore(KeyValuePair<string, object> item)
		{
			return TryGetValueCore(item.Key, out var value) && Equals(value, item.Value);
		}

		private bool ContainsKeyCore(string key)
		{
			return TryGetValueCore(key, out _);
		}

		private SkinDictionary EnsureKey(ThemeResourceKey themeKey)
		{
			var keyParts = themeKey.KeyParts;
			var current = this;

			for (var i = 0; i < keyParts.Count - 1; i++)
			{
				var keyPart = keyParts[i];

				SkinDictionary skinDictionary;

				if (current.Dictionary.TryGetValue(keyPart, out var currentValue) == false)
				{
					skinDictionary = new SkinDictionary();

					current.AddDictionaryValue(keyPart, skinDictionary);
				}
				else
				{
					skinDictionary = currentValue as SkinDictionary;

					if (skinDictionary == null)
						throw new InvalidOperationException();
				}

				current = skinDictionary;
			}

			return current;
		}

		internal IEnumerable<KeyValuePair<string, object>> Flatten()
		{
			return TreeEnumerator.GetEnumerator(this, ResourceTreeAdvisor).Enumerate();
		}

		private static IEnumerator<KeyValuePair<string, object>> GetChildResourceEnumerator(KeyValuePair<string, object> keyValuePair)
		{
			var skinValue = keyValuePair.Value as SkinDictionary;

			return skinValue == null ? Enumerable.Empty<KeyValuePair<string, object>>().GetEnumerator() : skinValue.Select(kv => kv.WithParentKey(keyValuePair.Key)).GetEnumerator();
		}

		private static IEnumerator<SkinDictionary> GetChildSkinDictionaryEnumerator(SkinDictionary skinDictionary)
		{
			return skinDictionary.Dictionary.Values.OfType<SkinDictionary>().GetEnumerator();
		}

		private object GetCore(string key)
		{
			if (TryGetValueCore(key, out var value) == false)
				throw new ArgumentOutOfRangeException();

			return value;
		}

		private static bool IsPathAbsolute(string path)
		{
			var trimmedPath = path.TrimStart();

			return (trimmedPath.StartsWith("/") || trimmedPath.StartsWith("../")) == false;
		}

		internal void Merge(KeyValuePair<string, object> keyValuePair, SkinDictionaryMergeFlags mergeFlags)
		{
			Merge(keyValuePair.Key, keyValuePair.Value, mergeFlags);
		}

		internal void Merge(string key, object value, SkinDictionaryMergeFlags mergeFlags)
		{
			var skinValue = value as SkinDictionary;

			if (skinValue == null)
			{
				if (TryGetValueCore(key, out _) && AllowOverride(mergeFlags) == false)
					throw new InvalidOperationException("Duplicate key");

				SetCore(key, value);
			}
			else
			{
				var themeKey = new ThemeResourceKey(key);
				var current = EnsureKey(themeKey);

				key = themeKey.KeyParts[themeKey.KeyParts.Count - 1];

				if (current.Dictionary.TryGetValue(key, out var currentValue))
				{
					var currentSkinValue = currentValue as SkinDictionary;

					if (currentSkinValue == null)
					{
						if (AllowOverride(mergeFlags) == false)
							throw new InvalidOperationException("Duplicate key");

						current.SetDictionaryValue(key, value);

						return;
					}

					foreach (var basedOn in skinValue.BasedOn)
					{
						if (basedOn.IsDeferred && currentSkinValue.BasedOn.Any(b => string.Equals(b.DeferredKey, basedOn.DeferredKey)) == false)
							currentSkinValue.BasedOn.Add(basedOn);
						else if (currentSkinValue.BasedOn.Contains(basedOn) == false)
							currentSkinValue.BasedOn.Add(basedOn);
					}

					foreach (var keyValuePair in skinValue)
						currentSkinValue.Merge(keyValuePair, mergeFlags);
				}
				else
					current.SetDictionaryValue(key, value);
			}
		}

		private void OnValueAdded(string key, object value)
		{
			if (value is SkinDictionary skinValue)
			{
				skinValue.Parent = this;
				skinValue.Key = key;
			}

			//if (value is SkinDictionaryProcessor skinProcessor)
			//	Processors.Add(skinProcessor);
			//else if (value is SkinResourceGenerator skinResourceGenerator)
			//	Generators.Add(skinResourceGenerator);
		}

		private void OnValueRemoved(string key, object value)
		{
			if (value is SkinDictionary skinValue)
			{
				skinValue.Parent = null;
				skinValue.Key = null;
			}

			//if (value is SkinDictionaryProcessor skinProcessor)
			//	Processors.Remove(skinProcessor);
			//else if (value is SkinResourceGenerator skinResourceGenerator)
			//	Generators.Remove(skinResourceGenerator);
		}

		private bool RemoveCore(KeyValuePair<string, object> item)
		{
			if (TryGetValueCore(item.Key, out var value) == false)
				return false;

			return Equals(item.Value, value) && RemoveCore(item.Key);
		}

		private bool RemoveCore(string key)
		{
			var themeKey = new ThemeResourceKey(key);
			var keyParts = themeKey.KeyParts;
			var current = this;

			for (var i = 0; i < keyParts.Count - 1; i++)
			{
				var keyPart = keyParts[i];

				if (current.Dictionary.TryGetValue(keyPart, out var currentValue) == false)
					return false;

				current = currentValue as SkinDictionary;

				if (current == null)
					return false;
			}

			key = keyParts[keyParts.Count - 1];

			if (current.Dictionary.ContainsKey(key) == false)
				return false;

			current.RemoveDictionaryValue(key);

			while (current != this)
			{
				var parent = current.Parent;

				if (current.Dictionary.Count == 0 && (current.BasedOnInternal == null || current.BasedOnInternal.Count == 0))
					current.Parent.RemoveDictionaryValue(current.Key);

				current = parent;
			}

			return true;
		}

		private void RemoveDictionaryValue(string key)
		{
			if (Dictionary.TryGetValue(key, out var value))
			{
				Dictionary.Remove(key);

				OnValueRemoved(key, value);
			}
		}

		private void SetCore(KeyValuePair<string, object> keyValuePair)
		{
			SetCore(keyValuePair.Key, keyValuePair.Value);
		}

		private void SetCore(string key, object value)
		{
			RemoveCore(key);
			AddCore(key, value);
		}

		private void SetDictionaryValue(string key, object value)
		{
			RemoveDictionaryValue(key);
			AddDictionaryValue(key, value);
		}

		private bool TryGetValueCore(string key, out object value)
		{
			if (IsPathAbsolute(key) == false)
				return TryGetValueRelative(key, out value);

			var themeKey = new ThemeResourceKey(key);
			var keyParts = themeKey.KeyParts;
			var current = Dictionary;

			for (var i = 0; i < keyParts.Count - 1; i++)
			{
				var keyPart = keyParts[i];

				if (current.TryGetValue(keyPart, out var currentValue) == false)
				{
					value = null;

					return false;
				}

				var skinDictionary = currentValue as SkinDictionary;

				if (skinDictionary == null)
				{
					value = null;

					return false;
				}

				current = skinDictionary.Dictionary;
			}

			return current.TryGetValue(keyParts[keyParts.Count - 1], out value);
		}

		private bool TryGetValueRelative(string key, out object value)
		{
			var current = this;
			var currentKey = key;

			if (currentKey.StartsWith("/"))
				currentKey = currentKey.Substring(1);

			while (currentKey.StartsWith("../") && current != null)
			{
				currentKey = currentKey.Substring(3);
				current = current.Parent;
			}

			if (current == null)
			{
				value = null;

				return false;
			}

			currentKey = currentKey.TrimEnd('/');

			var themeKey = new ThemeResourceKey(currentKey);
			var keyParts = themeKey.KeyParts;

			for (var iKeyPart = 0; iKeyPart < keyParts.Count - 1; iKeyPart++)
			{
				var keyPart = keyParts[iKeyPart];

				current = current.GetValueOrDefault(keyPart) as SkinDictionary;

				if (current != null)
					continue;

				value = null;

				return false;
			}

			currentKey = keyParts[keyParts.Count - 1];

			return current.TryGetValueCore(currentKey, out value);
		}
	}

	internal enum SkinDictionaryMergeFlags
	{
		Default,
		Override
	}
}