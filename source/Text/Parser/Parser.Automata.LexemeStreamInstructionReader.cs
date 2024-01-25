// <copyright file="Parser.Automata.LexemeStreamInstructionReader.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal partial class Parser<TGrammar, TToken>
	{
		private sealed partial class ParserAutomata
		{
			private sealed class LexemeStreamInstructionReader : IInstructionReader
			{
				public readonly LexemeSource<TToken> LexemeSource;

				public LexemeStreamInstructionReader(LexemeSource<TToken> lexemeSource)
				{
					LexemeSource = lexemeSource;
				}

				public int ReadPage(ref int position, int bufferLength, out Lexeme<TToken>[] lexemesBuffer, out int[] operandsBuffer)
				{
					RentBuffers(bufferLength, out var localLexemesBuffer, out var localOperandsBuffer);

					lexemesBuffer = localLexemesBuffer;
					operandsBuffer = localOperandsBuffer;

					return LexemeSource.Read(ref position, localLexemesBuffer, localOperandsBuffer, 0, bufferLength);
				}

				public int ReadPage(ref int position, int bufferOffset, int bufferLength, Lexeme<TToken>[] lexemesBuffer, int[] operandsBuffer)
				{
					return LexemeSource.Read(ref position, lexemesBuffer, operandsBuffer, bufferOffset, bufferLength);
				}

				public void ReleaseBuffers(Lexeme<TToken>[] instructionsBuffer, int[] operandsBuffer)
				{
					Lexer<TGrammar, TToken>.ReturnLexemeBuffers(instructionsBuffer, operandsBuffer);
				}

				public void RentBuffers(int bufferLength, out Lexeme<TToken>[] instructionsBuffer, out int[] operandsBuffer)
				{
					Lexer<TGrammar, TToken>.RentLexemeBuffers(bufferLength, out instructionsBuffer, out operandsBuffer);
				}

				public void Dispose()
				{
				}
			}
		}
	}
}