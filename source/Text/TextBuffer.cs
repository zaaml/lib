// <copyright file="TextBuffer.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.IO;

namespace Zaaml.Text
{
	internal sealed class TextBuffer
	{
		#region Fields

		private readonly char[] _charBuffer;
		private readonly TextReader _textReader;
		private int _bufferSize;
		private int _position;

		#endregion

		#region Ctors

		public TextBuffer(TextReader textReader, int bufferSize = 64)
		{
			_textReader = textReader;
			_charBuffer = new char[bufferSize];
			_position = -1;
		}

		#endregion

		#region Methods

		private bool EnsureBuffer()
		{
			if (_position >= 0 && _position < _bufferSize)
				return true;

			if (_position == -2)
				return false;

			_bufferSize = _textReader.ReadBlock(_charBuffer, 0, _charBuffer.Length);
			_position = 0;

			if (_bufferSize == 0)
				_position = -2;

			return _bufferSize > 0;
		}

		public char Peek()
		{
			if (EnsureBuffer() == false)
				return (char) 0;

			return _charBuffer[_position];
		}

		public char Read()
		{
			if (EnsureBuffer() == false)
				return (char) 0;

			return _charBuffer[_position++];
		}

		#endregion
	}
}