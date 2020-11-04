// <copyright file="LexemeEnumerator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;

namespace Zaaml.Text
{
	internal struct LexemeEnumerator<TToken> : IEnumerator<Lexeme<TToken>> where TToken : unmanaged, Enum
	{
		private const int BufferSize = 256;
		private readonly LexemeSource<TToken> _lexemeSource;
		private int _pointer;
		private readonly Lexeme<TToken>[] _lexemeBuffer;
		private readonly int[] _operandBuffer;
		private int _bufferSize;

		internal LexemeEnumerator(LexemeSource<TToken> lexemeSource)
		{
			_lexemeSource = lexemeSource;
			_pointer = BufferSize - 1;
			_lexemeBuffer = new Lexeme<TToken>[BufferSize];
			_operandBuffer = new int[BufferSize];
			_bufferSize = BufferSize;
			Current = default;
		}

		public bool MoveNext()
		{
			_pointer++;

			if (_pointer == _bufferSize)
			{
				if (_bufferSize < BufferSize)
				{
					Current = default;

					return false;
				}

				_pointer = 0;

				for (var i = 0; i < BufferSize; i++)
					_lexemeBuffer[i] = new Lexeme<TToken>();

				_bufferSize = _lexemeSource.Read(_lexemeBuffer, _operandBuffer, 0, BufferSize, true);

				if (_bufferSize == 0)
				{
					Current = default;

					return false;
				}
			}

			Current = _lexemeBuffer[_pointer];

			return true;
		}

		public void Reset()
		{
			throw new NotSupportedException();
		}

		public Lexeme<TToken> Current { get; private set; }

		object IEnumerator.Current => Current;

		public void Dispose()
		{
		}
	}
}