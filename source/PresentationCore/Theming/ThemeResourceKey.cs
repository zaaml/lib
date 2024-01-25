// <copyright file="ThemeResourceKey.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;

namespace Zaaml.PresentationCore.Theming
{
	internal readonly struct ThemeResourceKey
	{
		private static readonly string[] EmptyKeyParts = Array.Empty<string>();
		private static readonly Dictionary<ThemeResourceKey, ThemeResourceKey> KeyInternDict = new();
		private static readonly Dictionary<string, ThemeResourceKey> KeyDict = new();
		public static readonly List<string> Builder = new();

		#region Static Fields and Constants

		private const string Separator = ".";

		#endregion

		#region Fields

		private readonly string _key;
		private readonly IReadOnlyList<string> _keyParts;
		private readonly int _hashCode;

		#endregion

		public static ThemeResourceKey Empty => new(null, null);

		#region Ctors

		public ThemeResourceKey(IReadOnlyList<string> keyParts) : this()
		{
			this = Intern(null, keyParts);
		}

		private ThemeResourceKey(string key, IReadOnlyList<string> keyParts) : this()
		{
			_key = key;
			_keyParts = keyParts ?? EmptyKeyParts;
			_hashCode = CalcHashCode(_keyParts);
		}

		public ThemeResourceKey(string key) : this()
		{
			this = Intern(key, null);
		}

		#endregion

		#region Properties

		public bool IsEmpty => string.IsNullOrEmpty(_key) && ReferenceEquals(_keyParts, EmptyKeyParts);

		public string Key => IsEmpty ? string.Empty : _key;

		public IReadOnlyList<string> KeyParts => _keyParts ?? EmptyKeyParts;

		#endregion

		#region Methods

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

		private static string BuildKey(IReadOnlyList<string> keyParts)
		{
			return string.Join(Separator, keyParts);
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

		private static readonly CharEnumerator Enumerator1 = new();
		private static readonly CharEnumerator Enumerator2 = new();

		private sealed class CharEnumerator
		{
			private int _charIndex;
			private string _keyPart;

			private IReadOnlyList<string> _keyParts;
			private int _partIndex;

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

			public void Reset(IReadOnlyList<string> keyParts)
			{
				_keyParts = keyParts;

				MoveNextKeyPart(0);
			}
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
				return false;

			return obj is ThemeResourceKey key && Equals(key);
		}

		private static int CalcHashCode(IReadOnlyList<string> keyParts)
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

		private static ThemeResourceKey Intern(string keyString, IReadOnlyList<string> keyParts)
		{
			var key = new ThemeResourceKey(keyString, keyParts);

			if (key.IsEmpty)
				return Empty;

			if (string.IsNullOrEmpty(keyString) == false && KeyDict.TryGetValue(keyString, out var internedKey))
				return internedKey;

			if (KeyInternDict.TryGetValue(key, out internedKey))
				return internedKey;

			internedKey = keyString != null ? new ThemeResourceKey(keyString, ParseKey(keyString)) : new ThemeResourceKey(BuildKey(keyParts), ReferenceEquals(keyParts, Builder) ? keyParts.ToList() : keyParts);

			KeyDict[internedKey.Key] = internedKey;
			KeyInternDict[internedKey] = internedKey;

			return internedKey;
		}

		#endregion
	}
}