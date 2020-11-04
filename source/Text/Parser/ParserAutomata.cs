// <copyright file="ParserAutomata.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.Text
{
	internal abstract class ParserAutomataBase<TGrammar, TToken> : Automata<Lexeme<TToken>, TToken> where TGrammar : Grammar<TToken> where TToken : unmanaged, Enum
	{
		#region Ctors

		protected ParserAutomataBase(AutomataManager manager) : base(manager)
		{
		}

		#endregion

		#region Properties

		protected override bool LookAheadEnabled => false;

		#endregion

		#region Methods

		private protected override Pool<InstructionStream> CreateInstructionQueuePool()
		{
			return new Pool<InstructionStream>(p => new LexemeStream(p));
		}

		private protected AutomataResult PartialRunCore<TContext>(LexemeSource<TToken> lexemeSource, TContext context) where TContext : AutomataContext
		{
			using var instructionReader = new LexemeStreamInstructionReader(lexemeSource);

			return PartialRunCore(instructionReader, context);
		}

		protected void RunCore<TContext>(LexemeSource<TToken> lexemeSource, TContext context) where TContext : AutomataContext
		{
			using var instructionReader = new LexemeStreamInstructionReader(lexemeSource);

			RunCore(instructionReader, context);
		}

		#endregion

		#region Nested Types

		private sealed class LexemeStream : InstructionStream
		{
			#region Fields

			private int _instructionReaderPosition;

			#endregion

			#region Ctors

			public LexemeStream(Pool<InstructionStream> pool) : base(pool)
			{
			}

			#endregion

			#region Properties

			private ISeekableInstructionReader SeekableInstructionReader => (ISeekableInstructionReader) InstructionReader;

			#endregion

			#region Methods

			public override InstructionStream Advance(int instructionPointer, Automata<Lexeme<TToken>, TToken> automata)
			{
				var copy = (LexemeStream) Pool.Get().Mount(InstructionReader, automata);

				copy.AdvanceInstructionPosition(this, instructionPointer);

				return copy;
			}

			private void AdvanceInstructionPosition(LexemeStream lexemeStream, int instructionPointer)
			{
				StartPosition = SeekableInstructionReader.Position;
				StartInstructionPointer = instructionPointer;

				var lexemeIndex = instructionPointer & LocalIndexMask;
				var lexemePageIndex = instructionPointer >> PageIndexShift;

				EnsurePagesCapacity(lexemePageIndex + 1);

				for (var iPage = lexemeStream.HeadPage; iPage <= lexemePageIndex; iPage++)
				{
					InstructionReader.RentBuffers(PageSize, out var instructionsBuffer, out var operandsBuffer);

					var pageSource = lexemeStream.Pages[iPage];
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

				HeadPage = lexemeStream.HeadPage;
				PageCount = lexemePageIndex + 1;

				ReadOperand(ref instructionPointer);
			}

			protected override void OnReleased()
			{
				_instructionReaderPosition = -1;

				base.OnReleased();
			}

			public override int GetPosition(int instructionPointer)
			{
				if (instructionPointer == 0 || instructionPointer == StartInstructionPointer)
					return StartPosition;

				instructionPointer--;

				var localInstructionPointer = instructionPointer;

				ReadOperand(ref instructionPointer);

				return PeekInstruction(localInstructionPointer).End;
			}

			protected override void LoadPosition()
			{
				if (SeekableInstructionReader.Position != _instructionReaderPosition)
					SeekableInstructionReader.Position = _instructionReaderPosition;
			}

			public override InstructionStream Mount(IInstructionReader instructionReader, Automata<Lexeme<TToken>, TToken> automata)
			{
				var instructionQueue = base.Mount(instructionReader, automata);

				_instructionReaderPosition = SeekableInstructionReader.Position;

				return instructionQueue;
			}

			protected override void SavePosition()
			{
				_instructionReaderPosition = SeekableInstructionReader.Position;
			}

			#endregion
		}

		private sealed class LexemeStreamInstructionReader : ISeekableInstructionReader
		{
			#region Fields

			private readonly LexemeSource<TToken> _lexemeSource;

			#endregion

			#region Ctors

			public LexemeStreamInstructionReader(LexemeSource<TToken> lexemeSource)
			{
				_lexemeSource = lexemeSource;
			}

			#endregion

			#region Interface Implementations

			#region Automata<Lexeme<TToken>,TToken>.IInstructionReader

			public int ReadPage(int bufferLength, out Lexeme<TToken>[] lexemesBuffer, out int[] operandsBuffer)
			{
				RentBuffers(bufferLength, out var localLexemesBuffer, out var localOperandsBuffer);

				lexemesBuffer = localLexemesBuffer;
				operandsBuffer = localOperandsBuffer;

				return _lexemeSource.Read(localLexemesBuffer, localOperandsBuffer, 0, bufferLength, true);
			}

			public int ReadPage(int bufferOffset, int bufferLength, Lexeme<TToken>[] lexemesBuffer, int[] operandsBuffer)
			{
				return _lexemeSource.Read(lexemesBuffer, operandsBuffer, bufferOffset, bufferLength, true);
			}

			public void ReleaseBuffers(Lexeme<TToken>[] instructionsBuffer, int[] operandsBuffer)
			{
				Lexer<TGrammar, TToken>.ReturnLexemeBuffers(instructionsBuffer, operandsBuffer);
			}

			public void RentBuffers(int bufferLength, out Lexeme<TToken>[] instructionsBuffer, out int[] operandsBuffer)
			{
				Lexer<TGrammar, TToken>.RentLexemeBuffers(bufferLength, out instructionsBuffer, out operandsBuffer);
			}

			#endregion

			#region Automata<Lexeme<TToken>,TToken>.ISeekableInstructionReader

			public int Position
			{
				get => _lexemeSource.Position;
				set => _lexemeSource.Position = value;
			}

			#endregion

			#region IDisposable

			public void Dispose()
			{
			}

			#endregion

			#endregion
		}

		#endregion
	}
}