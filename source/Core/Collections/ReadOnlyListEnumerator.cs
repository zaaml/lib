// <copyright file="ReadOnlyListEnumerator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;

namespace Zaaml.Core.Collections
{
	public struct ReadOnlyListEnumerator<T> : IEnumerator<T>
	{
		private const int Initial = 0;
		private const int Started = 1;
		private const int Finished = 2;
		private const int Disposed = 3;

		private IReadOnlyList<T> _readOnlyList;
		private int _currentIndex;
		private int _status;

		public ReadOnlyListEnumerator(IReadOnlyList<T> readOnlyList)
		{
			_readOnlyList = readOnlyList;
			_currentIndex = -1;
			_status = Initial;
		}

		private void Verify(bool starting)
		{
			if (starting == false && _status == Initial)
				throw new InvalidOperationException("Enumeration not started.");

			if (_status == Finished)
				throw new InvalidOperationException("Enumeration finished.");

			if (_status == Disposed)
				throw new InvalidOperationException("Enumerator disposed");
		}

		public bool MoveNext()
		{
			Verify(true);

			if (_status == Initial)
			{
				_status = Started;

				_currentIndex = 0;

				if (_currentIndex == _readOnlyList.Count)
				{
					_status = Finished;

					return false;
				}

				return true;
			}

			_currentIndex++;

			if (_currentIndex == _readOnlyList.Count)
			{
				_status = Finished;

				return false;
			}

			return true;
		}

		public void Reset()
		{
			if (_status == Disposed)
				throw new InvalidOperationException("Enumerator disposed");

			_status = Initial;
			_currentIndex = -1;
		}

		public T Current
		{
			get
			{
				Verify(false);

				return _readOnlyList[_currentIndex];
			}
		}

		object IEnumerator.Current => Current;

		public void Dispose()
		{
			_status = Disposed;
			_currentIndex = -1;
			_readOnlyList = null;
		}
	}
}