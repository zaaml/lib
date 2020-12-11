// <copyright file="SparseLinkedListBase.Enumerator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;

namespace Zaaml.Core.Collections
{
	internal partial class SparseLinkedListBase<T>
	{
		public struct Enumerator : IEnumerator<T>
		{
			private SparseLinkedListBase<T> _list;
			private int _structureVersion;
			private NodeBase _currentNode;
			private int _currentNodeIndex;
			private T _current;

			public Enumerator(SparseLinkedListBase<T> list)
			{
				_list = list;
				_current = default;
				_currentNode = _list.HeadNode;
				_currentNodeIndex = -1;
				_structureVersion = _list.StructureVersion;
			}

			public void Dispose()
			{
				_list = null;
				_structureVersion = -1;
			}

			private void Verify()
			{
				if (_list == null)
					throw new InvalidOperationException("Enumerator has been disposed.");

				if (_structureVersion != _list.StructureVersion)
					throw new InvalidOperationException("List has changed.");
			}

			public bool MoveNext()
			{
				Verify();

				_currentNodeIndex++;

				if (_currentNodeIndex >= _currentNode.Size)
				{
					_currentNode = _currentNode.Next;
					_currentNodeIndex = 0;
				}

				var moveNext = _currentNode != null && _currentNodeIndex < _currentNode.Size;

				if (moveNext)
					_current = _currentNode.GetLocalItem(_currentNodeIndex);

				return moveNext;
			}

			public void Reset()
			{
				Verify();

				_currentNode = _list.HeadNode;
				_currentNodeIndex = -1;
			}

			public T Current
			{
				get
				{
					Verify();

					return _current;
				}
			}

			object IEnumerator.Current => Current;
		}
	}
}