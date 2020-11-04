// <copyright file="AncestorsEnumerator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;

namespace Zaaml.Core.Trees
{
	internal struct AncestorsEnumerator<T>
	{
		private readonly TreeEnumerator<T> _treeEnumerator;
		private int _index;
		private readonly int _version;

		public AncestorsEnumerator(TreeEnumerator<T> treeEnumerator)
		{
			_index = -1;
			_version = treeEnumerator.Version;
			_treeEnumerator = treeEnumerator;
		}

		public void Dispose()
		{
			Reset();
		}

		public bool MoveNext()
		{
			if (_treeEnumerator.Version != _version)
				throw new InvalidOperationException("Tree enumerator has changed");

			if (_treeEnumerator.Stack == null)
				return false;

			if (_index == -1)
			{
				var distance = _treeEnumerator.AncestorsIncludesSelf ? 2 : 1;

				if (_treeEnumerator.Stack.Count < distance)
					return false;

				_index = _treeEnumerator.Stack.Count - distance;

				return true;
			}

			_index--;

			return _index >= 0;
		}

		public void Reset()
		{
			_index = -1;
		}

		public T Current => _index == -1 ? default : _treeEnumerator.Stack[_index].Node;

		public IEnumerable<T> Enumerate()
		{
			while (MoveNext())
				yield return Current;
		}
	}
}