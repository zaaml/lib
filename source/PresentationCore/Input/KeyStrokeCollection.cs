// <copyright file="KeyStrokeCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace Zaaml.PresentationCore.Input
{
	[TypeConverter(typeof(KeyStrokeCollectionConverter))]
	public class KeyStrokeCollection : ICollection<KeyStroke>
	{
		private static readonly char[] Separator = {','};
		private readonly List<KeyStroke> _keyStrokeCollectionInternal = new();

		public KeyStroke this[int index] => _keyStrokeCollectionInternal[index];

		public void Seal()
		{
			IsReadOnly = true;
		}

		public override string ToString()
		{
			return string.Join(",", _keyStrokeCollectionInternal);
		}

		public static bool TryParse(string str, out KeyStrokeCollection keyStrokes)
		{
			keyStrokes = new KeyStrokeCollection();

			var delimited = str.Split(Separator, StringSplitOptions.RemoveEmptyEntries);

			foreach (var keyStrokeStr in delimited)
			{
				if (KeyStroke.TryParse(keyStrokeStr, out var keyStroke))
					keyStrokes._keyStrokeCollectionInternal.Add(keyStroke);
				else
				{
					keyStrokes = null;

					return false;
				}
			}

			return true;
		}

		private void VerifyWritable()
		{
			if (IsReadOnly)
				throw new InvalidOperationException();
		}

		public IEnumerator<KeyStroke> GetEnumerator()
		{
			return _keyStrokeCollectionInternal.GetEnumerator();
		}

		public void Add(KeyStroke item)
		{
			VerifyWritable();

			_keyStrokeCollectionInternal.Add(item);
		}

		public void Clear()
		{
			VerifyWritable();

			_keyStrokeCollectionInternal.Clear();
		}

		public bool Contains(KeyStroke item)
		{
			return _keyStrokeCollectionInternal.Contains(item);
		}

		public void CopyTo(KeyStroke[] array, int arrayIndex)
		{
			_keyStrokeCollectionInternal.CopyTo(array, arrayIndex);
		}

		public bool Remove(KeyStroke item)
		{
			VerifyWritable();

			return _keyStrokeCollectionInternal.Remove(item);
		}

		public int Count => _keyStrokeCollectionInternal.Count;

		public bool IsReadOnly { get; private set; }

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}