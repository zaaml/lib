// <copyright file="Lexer.TextInstructionReader.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Buffers;
using System.IO;

// ReSharper disable StaticMemberInGenericType

namespace Zaaml.Text
{
	internal partial class Lexer<TGrammar, TToken>
	{
		private protected sealed partial class LexerAutomata
		{
			private sealed class TextInstructionReader : IInstructionReader
			{
				private static readonly ArrayPool<char> CharArrayPool = ArrayPool<char>.Shared;
				private static readonly ArrayPool<int> IntArrayPool = ArrayPool<int>.Shared;

				private readonly TextReader _textReader;
				private bool _isHighSurrogate;
				private char _prevChar;

				public TextInstructionReader(TextReader textReader)
				{
					_textReader = textReader;
				}

				public int ReaderPosition => throw new NotImplementedException();

				public int ReadPage(int bufferOffset, int bufferLength, char[] instructionsBuffer, int[] operandsBuffer)
				{
					var isHighSurrogate = _isHighSurrogate;
					var prevChar = _prevChar;

					try
					{
						var readBufferLength = _textReader.ReadBlock(instructionsBuffer, 0, bufferLength - bufferOffset);

						if (readBufferLength == 0)
							return 0;

						var i = bufferOffset;

						foreach (var c in instructionsBuffer)
						{
							if (c >= 0xD800 && c <= 0xDBFF)
							{
								instructionsBuffer[i] = c;
								operandsBuffer[i] = -1;
								isHighSurrogate = true;
							}
							else if (isHighSurrogate)
							{
								instructionsBuffer[i] = c;
								operandsBuffer[i] = char.ConvertToUtf32(prevChar, c);
								isHighSurrogate = false;
							}
							else
							{
								instructionsBuffer[i] = c;
								operandsBuffer[i] = c;
							}

							prevChar = c;
							i++;
						}

						return i - readBufferLength;
					}
					finally
					{
						_isHighSurrogate = isHighSurrogate;
						_prevChar = prevChar;
					}
				}

				public int ReadPage(int bufferLength, out char[] instructionsBuffer, out int[] operandsBuffer)
				{
					RentBuffers(bufferLength, out instructionsBuffer, out operandsBuffer);

					return ReadPage(0, bufferLength, instructionsBuffer, operandsBuffer);
				}

				public void ReleaseBuffers(char[] instructionsBuffer, int[] operandsBuffer)
				{
					CharArrayPool.Return(instructionsBuffer, true);
					IntArrayPool.Return(operandsBuffer, true);
				}

				public void RentBuffers(int bufferLength, out char[] instructionsBuffer, out int[] operandsBuffer)
				{
					instructionsBuffer = CharArrayPool.Rent(bufferLength);
					operandsBuffer = IntArrayPool.Rent(bufferLength);
				}

				public void Dispose()
				{
					_textReader.Dispose();
				}
			}
		}
	}
}