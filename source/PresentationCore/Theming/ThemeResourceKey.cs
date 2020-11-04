// <copyright file="ThemeResourceKey.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;

namespace Zaaml.PresentationCore.Theming
{
	internal struct ThemeResourceKey
	{
		private static readonly Dictionary<ThemeResourceKey, ThemeResourceKey> KeyInternDict = new Dictionary<ThemeResourceKey, ThemeResourceKey>();
		private static readonly Dictionary<string, ThemeResourceKey> KeyDict = new Dictionary<string, ThemeResourceKey>();
		public static readonly List<string> Builder = new List<string>();

		#region Static Fields and Constants

		private const string Separator = ".";

		#endregion

		#region Fields

		private string _key;
		private List<string> _keyParts;
		private readonly int _hashCode;

		#endregion

		public static readonly ThemeResourceKey Empty = new ThemeResourceKey { _key = null, _keyParts = null };

		#region Ctors

		public ThemeResourceKey(List<string> keyParts) : this()
		{
			_keyParts = keyParts;
			_hashCode = CalcHashCode(_keyParts);

			this = Intern();
		}

		private ThemeResourceKey(string key, List<string> keyParts) : this()
		{
			_key = key;
			_keyParts = keyParts;
			_hashCode = CalcHashCode(_keyParts);
		}

		public ThemeResourceKey(string key) : this()
		{
			if (KeyDict.TryGetValue(key, out this))
				return;

			this = new ThemeResourceKey(key, ParseKey(key));

			this = Intern();

			KeyDict[key] = this;
		}

		#endregion

		#region Properties

		public bool IsEmpty => _key == null && _keyParts == null;

		public string Key => IsEmpty ? null : EnsureKey();

		public List<string> KeyParts => IsEmpty ? null : EnsureKeyParts();

		private List<string> EnsureKeyParts()
		{
			if (ReferenceEquals(Builder, _keyParts))
				return _keyParts = ParseKey(Key);

			return _keyParts ?? (_keyParts = ParseKey(_key));
		}

		#endregion

		#region  Methods

		internal ThemeResourceKey WithParent(string parentKey)
		{
			var parent = ParseKey(parentKey);

			parent.AddRange(KeyParts);

			return new ThemeResourceKey(parent);
		}

		public static explicit operator ThemeResourceKey(string key)
		{
			return new ThemeResourceKey(key);
		}

		public static explicit operator string(ThemeResourceKey key)
		{
			return key.Key;
		}

		private string EnsureKey()
		{
			return _key ?? (_key = BuildKey());
		}

		private static List<string> ParseKey(string key)
		{
			var keyParts = new List<string>();
			var currentIndex = 0;

			while (true)
			{
				var index = key.IndexOf(Separator, currentIndex, StringComparison.Ordinal);

				if (index == -1)
				{
					keyParts.Add(key.Substring(currentIndex));

					break;
				}

				keyParts.Add(key.Substring(currentIndex, index - currentIndex));

				currentIndex = index + 1;
			}

			return keyParts;
		}

		private string BuildKey()
		{
			return string.Join(Separator, _keyParts);
		}

		public bool Equals(ThemeResourceKey other)
		{
			Enumerator1.Reset(_keyParts);
			Enumerator2.Reset(other._keyParts);

			while (true)
			{
				var c1 = Enumerator1.MoveNext();
				var c2 = Enumerator2.MoveNext();

				if (c1 != c2)
					return false;

				if (c1 == 0)
					break;
			}

			return true;
		}

		private static readonly CharEnumerator Enumerator1 = new CharEnumerator();
		private static readonly CharEnumerator Enumerator2 = new CharEnumerator();

		private class CharEnumerator
		{
			#region Type: Fields

			private int _charIndex;
			private string _keyPart;

			private List<string> _keyParts;
			private int _partIndex;

			#endregion

			#region  Methods

			public char MoveNext()
			{
				if (_charIndex < _keyPart.Length)
					return _keyPart[_charIndex++];

				MoveNextKeyPart(1);

				if (_partIndex >= _keyParts.Count)
					return (char) 0;

				return '.';
			}

			private void MoveNextKeyPart(int inc)
			{
				_partIndex += inc;
				while (_partIndex < _keyParts.Count)
				{
					_charIndex = 0;

					_keyPart = _keyParts[_partIndex];

					if (string.IsNullOrEmpty(_keyPart))
						_partIndex++;
					else
						return;
				}
			}

			public void Reset(List<string> keyParts)
			{
				_keyParts = keyParts;
				MoveNextKeyPart(0);
			}

			#endregion
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
				return false;

			return obj is ThemeResourceKey && Equals((ThemeResourceKey) obj);
		}

		private static int CalcHashCode(List<string> keyParts)
		{
			unchecked
			{
				var hashCode = 0;
				// ReSharper disable ForCanBeConvertedToForeach
				for (var i = 0; i < keyParts.Count; i++)
				{
					var keyPart = keyParts[i];
					// ReSharper disable once LoopCanBeConvertedToQuery
					for (var j = 0; j < keyPart.Length; j++)
					{
						var c = keyPart[j];

						hashCode = ((hashCode << 5) + hashCode) ^ c;
					}

					hashCode = ((hashCode << 5) + hashCode) ^ '.';
				}
				// ReSharper restore ForCanBeConvertedToForeach

				return hashCode;
			}
		}

		public override int GetHashCode()
		{
			return _hashCode;
		}

		private ThemeResourceKey Intern()
		{
			if (IsEmpty)
				return this;

			ThemeResourceKey internedKey;

			if (KeyInternDict.TryGetValue(this, out internedKey))
				return internedKey;

			internedKey = this;

			internedKey.EnsureKey();
			internedKey.EnsureKeyParts();

			KeyInternDict[internedKey] = internedKey;

			return internedKey;
		}

		#endregion
	}
}