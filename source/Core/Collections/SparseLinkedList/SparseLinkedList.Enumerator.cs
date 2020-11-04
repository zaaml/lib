// <copyright file="SparseLinkedList.Enumerator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;

namespace Zaaml.Core.Collections
{
	internal partial class SparseLinkedList<T>
	{
		#region  Methods

		[PublicAPI]
		public Enumerator GetEnumerator()
		{
			return new Enumerator(this);
		}

		#endregion

		#region Interface Implementations

		#region IEnumerable

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion

		#region IEnumerable<T>

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion

		#endregion

		#region  Nested Types

		public struct Enumerator : IEnumerator<T>
		{
			private SparseLinkedList<T> _list;
			private int _version;
			private Node _currentNode;
			private int _currentNodeIndex;
			private T _current;

			public Enumerator(SparseLinkedList<T> list)
			{
				_list = list;
				_version = _list.Version;
				_currentNode = _list.HeadNode;
				_currentNodeIndex = -1;
				_current = default(T);
			}

			public void Dispose()
			{
				_list = null;
				_version = -1;
			}

			private void Verify()
			{
				if (_list == null)
					throw new InvalidOperationException("Enumerator has been disposed.");

				if (_version != _list.Version)
					throw new InvalidOperationException("List has changed.");
			}

			public bool MoveNext()
			{
				Verify();

				_currentNodeIndex++;

				if (_currentNodeIndex >= _currentNode.Count)
				{
					_currentNode = _currentNode.Next;
					_currentNodeIndex = 0;
				}

				var moveNext = _currentNode != null && _currentNodeIndex < _currentNode.Count;

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

		#endregion
	}
}