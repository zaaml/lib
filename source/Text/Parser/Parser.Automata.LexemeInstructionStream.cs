// <copyright file="Parser.Automata.LexemeInstructionStream.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.Text
{
	internal partial class Parser<TGrammar, TToken>
	{
		private sealed partial class ParserAutomata
		{
			private sealed class LexemeInstructionStream : InstructionStream
			{
				public LexemeInstructionStream(Pool<InstructionStream> pool) : base(pool)
				{
				}

				public override InstructionStream Advance(int position, int instructionPointer, Automata<Lexeme<TToken>, TToken> automata)
				{
					var copy = (LexemeInstructionStream)Pool.Rent().Mount(InstructionReader, automata);

					copy.AdvanceInstructionPosition(this, position, instructionPointer);

					return copy;
				}

				private void AdvanceInstructionPosition(LexemeInstructionStream lexemeInstructionStream, int textPosition, int instructionPointer)
				{
					Position = textPosition;
					StartPosition = textPosition;
					StartInstructionPointer = instructionPointer;

					var lexemeIndex = instructionPointer & LocalIndexMask;
					var lexemePageIndex = instructionPointer >> PageIndexShift;

					EnsurePagesCapacity(lexemePageIndex + 1);

					for (var iPage = lexemeInstructionStream.HeadPage; iPage <= lexemePageIndex; iPage++)
					{
						InstructionReader.RentBuffers(PageSize, out var instructionsBuffer, out var operandsBuffer);

						var pageSource = lexemeInstructionStream.Pages[iPage];
						var pageCopy = new InstructionPage(instructionsBuffer, operandsBuffer, pageSource.PageIndex, pageSource.PageLength);

						Array.Copy(pageSource.InstructionsBuffer, 0, pageCopy.InstructionsBuffer, 0, pageCopy.PageLength);
						Array.Copy(pageSource.OperandsBuffer, 0, pageCopy.OperandsBuffer, 0, pageCopy.PageLength);

						pageCopy.ReferenceCount = pageSource.ReferenceCount;

						Pages[pageCopy.PageIndex] = pageCopy;
					}

					var lexemePage = Pages[lexemePageIndex];

					lexemePage.PageLength = lexemeIndex;

					for (var i = lexemeIndex; i < PageSize; i++)
					{
						lexemePage.OperandsBuffer[i] = int.MinValue;
						lexemePage.InstructionsBuffer[i] = default;
					}

					HeadPage = lexemeInstructionStream.HeadPage;
					PageCount = lexemePageIndex + 1;

					FetchReadOperand(ref instructionPointer);
				}

				public override string Dump(int position, int length)
				{
					var lexemeInstructionReader = (LexemeStreamInstructionReader)InstructionReader;

					return lexemeInstructionReader.LexemeSource.TextSourceSpan.GetText(position, length);
				}

				public override string Dump(int position)
				{
					var lexemeInstructionReader = (LexemeStreamInstructionReader)InstructionReader;

					return lexemeInstructionReader.LexemeSource.TextSourceSpan.GetText(position);
				}

				public override int GetPosition(int instructionPointer)
				{
					if (instructionPointer == 0 || instructionPointer == StartInstructionPointer)
						return StartPosition;

					instructionPointer--;

					var localInstructionPointer = instructionPointer;

					FetchReadOperand(ref instructionPointer);

					return PeekInstructionRef(localInstructionPointer).End;
				}
			}
		}
	}
}