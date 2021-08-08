// <copyright file="Automata.InstructionStream.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Runtime.CompilerServices;
using Zaaml.Core;
using Zaaml.Core.Utils;

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		private protected virtual Pool<InstructionStream> CreateInstructionStreamPool()
		{
			return new Pool<InstructionStream>(p => new InstructionStream(p));
		}

		private protected partial class InstructionStream : PoolSharedObject<InstructionStream>
		{
			protected const int PageIndexShift = 12;
			protected const int PageSize = 1 << PageIndexShift;
			protected const int LocalIndexMask = PageSize - 1;
			protected const int InvalidInstructionOperand = 0;

			protected static readonly InstructionPage NullPage;

			protected Exception Exception;
			protected bool Finished;
			protected int HeadPage;
			protected IInstructionReader InstructionReader;
			protected int PageCount;
			protected InstructionPage[] Pages;

			static InstructionStream()
			{
				var instructions = new TInstruction[PageSize];
				var operands = new int[PageSize];

				Array.Fill(operands, 0, PageSize, InvalidInstructionOperand);

				NullPage = new InstructionPage(instructions, operands, 0, PageSize)
				{
					ReferenceCount = int.MaxValue
				};
			}

			public InstructionStream(Pool<InstructionStream> pool) : base(pool)
			{
				Pages = new InstructionPage[64];

				ArrayUtils.Fill(Pages, NullPage);
			}

			public InstructionStream() : base(null)
			{
				Pages = new InstructionPage[64];

				ArrayUtils.Fill(Pages, NullPage);
			}

			public int StartInstructionPointer { get; set; }

			public int StartPosition { get; set; }

			public virtual InstructionStream Advance(int instructionPointer, Automata<TInstruction, TOperand> automata)
			{
				throw new NotImplementedException();
			}

			private void CleanHeadPage()
			{
				for (var iPage = HeadPage; iPage < PageCount; iPage++)
				{
					var page = Pages[iPage];

					if (page.ReferenceCount > 0 || page.PageLength < PageSize)
						break;

					HeadPage = iPage + 1;
					Pages[iPage] = NullPage;

					InstructionReader.ReleaseBuffers(page.InstructionsBuffer, page.OperandsBuffer);
				}
			}

			private void CleanPages()
			{
				for (var i = HeadPage; i < PageCount; i++)
				{
					var page = Pages[i];

					if (ReferenceEquals(NullPage, page))
						continue;

					Pages[i] = NullPage;
					InstructionReader.ReleaseBuffers(page.InstructionsBuffer, page.OperandsBuffer);
				}
			}

			protected void EnsurePagesCapacity(int pageIndex)
			{
				if (pageIndex < Pages.Length)
					return;

				var pageLength = Pages.Length;

				// TODO Pages array from pool. May we resize it ?
				Array.Resize(ref Pages, Pages.Length * 2);
				ArrayUtils.Fill(Pages, NullPage, pageLength, Pages.Length - pageLength);
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public void FetchOperand(int instructionPointer)
			{
				var pageIndex = instructionPointer >> PageIndexShift;
				var pageOffset = instructionPointer & LocalIndexMask;
				var operand = Pages[pageIndex].OperandsBuffer[pageOffset];

				if (operand == InvalidInstructionOperand)
					LoadPage(pageIndex);
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public int FetchPeekOperand(int instructionPointer)
			{
				var pageIndex = instructionPointer >> PageIndexShift;
				var pageOffset = instructionPointer & LocalIndexMask;
				var operand = Pages[pageIndex].OperandsBuffer[pageOffset];

				if (operand > 0)
					return operand;

				operand = LoadPage(pageIndex).OperandsBuffer[pageOffset];

				return operand > 0 ? operand : InvalidInstructionOperand;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public int FetchReadOperand(ref int instructionPointer)
			{
				var pageIndex = instructionPointer >> PageIndexShift;
				var pageOffset = instructionPointer & LocalIndexMask;
				var operand = Pages[pageIndex].OperandsBuffer[pageOffset];

				instructionPointer++;

				if (operand > 0)
					return operand;

				operand = LoadPage(pageIndex).OperandsBuffer[pageOffset];

				if (operand > 0)
					return operand;

				instructionPointer--;

				return InvalidInstructionOperand;
			}

			public virtual int GetPosition(int instructionPointer)
			{
				return instructionPointer;
			}

			public static int InitializeInstructionPointer()
			{
				return 0;
			}

			private InstructionPage LoadPage(int pageIndex)
			{
				EnsurePagesCapacity(pageIndex + 1);
				CleanHeadPage();

				InstructionPage page = null;

				if (pageIndex < PageCount)
				{
					page = Pages[pageIndex];

					if (page.PageLength < PageSize)
					{
						LoadPosition();

						page.PageLength += InstructionReader.ReadPage(page.PageLength, PageSize, page.InstructionsBuffer, page.OperandsBuffer);

						SavePosition();
					}

					return page;
				}

				if (Finished)
					return NullPage;

				for (var iPage = PageCount - 1; iPage < pageIndex; iPage++)
				{
					page = ReadNextPage(PageCount);

					Pages[page.PageIndex] = page;
					PageCount++;

					if (Finished)
						break;
				}

				return page;
			}

			protected virtual void LoadPosition()
			{
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public void LockPointer(int instructionPointer)
			{
				Pages[instructionPointer >> PageIndexShift].ReferenceCount++;
			}

			public virtual InstructionStream Mount(IInstructionReader instructionReader, Automata<TInstruction, TOperand> automata)
			{
				//Pages = ArrayPool<InstructionPage>.Shared.Rent(PageCapacity);

				ArrayUtils.Fill(Pages, NullPage);

				InstructionReader = instructionReader;

				MountParallel(automata);

				return this;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public void Move(ref int instructionPointer)
			{
				var pageIndex = instructionPointer >> PageIndexShift;
				var pageOffset = instructionPointer & LocalIndexMask;
				var operand = Pages[pageIndex].OperandsBuffer[pageOffset];

				instructionPointer++;

				if (operand != InvalidInstructionOperand)
					return;

				operand = LoadPage(pageIndex).OperandsBuffer[pageOffset];

				if (operand == InvalidInstructionOperand)
					instructionPointer--;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public void Move(ref int instructionPointer, int count)
			{
#if AUTOMATA_VAR_INSTRUCTION_SIZE
				for (var i = 0; i < count; i++)
				{
					var pageIndex = instructionPointer >> PageIndexShift;
					var pageOffset = instructionPointer & LocalIndexMask;
					var operand = Pages[pageIndex].OperandsBuffer[pageOffset];

					instructionPointer++;

					if (operand > 0)
						continue;

					operand = LoadPage(pageIndex).OperandsBuffer[pageOffset];

					if (operand > 0)
						continue;

					instructionPointer--;

					return;
				}
#else
				instructionPointer += count;
#endif
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public int Move(int instructionPointer, int count)
			{
				Move(ref instructionPointer, count);

				return instructionPointer;
			}

			protected override void OnReleased()
			{
				CleanPages();
				DisposeParallel();
				InstructionReader.Dispose();
				//ArrayPool<InstructionPage>.Shared.Return(Pages, true);
				//Pages = null;
				HeadPage = 0;
				PageCount = 0;
				Finished = false;
				Exception = null;
				StartPosition = 0;
				InstructionReader = null;
				StartInstructionPointer = 0;
				NullPage.ReferenceCount = int.MaxValue;

				base.OnReleased();
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public ref TInstruction PeekInstruction(int instructionPointer)
			{
				return ref Pages[instructionPointer >> PageIndexShift].InstructionsBuffer[instructionPointer & LocalIndexMask];
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public ref TInstruction PeekInstructionOperand(int instructionPointer, out int operand)
			{
				var page = Pages[instructionPointer >> PageIndexShift];
				var localIndex = instructionPointer & LocalIndexMask;

				operand = page.OperandsBuffer[localIndex];

				return ref page.InstructionsBuffer[localIndex];
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public int PeekOperand(int instructionPointer)
			{
				return Pages[instructionPointer >> PageIndexShift].OperandsBuffer[instructionPointer & LocalIndexMask];
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public int PrefetchReadOperand(ref int instructionPointer, int fetchLength)
			{
				var fetchEndPointer = instructionPointer + fetchLength;
				var fetchPageIndex = fetchEndPointer >> PageIndexShift;
				var fetchPageOffset = fetchEndPointer & LocalIndexMask;
				var operand = Pages[fetchPageIndex].OperandsBuffer[fetchPageOffset];
				var pageIndex = instructionPointer >> PageIndexShift;
				var pageOffset = instructionPointer & LocalIndexMask;

				if (operand == InvalidInstructionOperand)
					LoadPage(fetchPageIndex);

				operand = Pages[pageIndex].OperandsBuffer[pageOffset];

				if (operand == InvalidInstructionOperand)
					return InvalidInstructionOperand;

				instructionPointer++;

				return operand;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public ReadOnlySpan<int> PrefetchReadOperandSpan(int instructionPointer, int fetchLength)
			{
				var fetchEndPointer = instructionPointer + fetchLength;
				var fetchPageIndex = fetchEndPointer >> PageIndexShift;
				var fetchPageOffset = fetchEndPointer & LocalIndexMask;
				var fetchOperand = Pages[fetchPageIndex].OperandsBuffer[fetchPageOffset];
				var pageIndex = instructionPointer >> PageIndexShift;
				var pageOffset = instructionPointer & LocalIndexMask;

				if (fetchOperand == InvalidInstructionOperand)
					LoadPage(fetchPageIndex);

				var span = Pages[pageIndex].OperandsBuffer.AsSpan(pageOffset);

				return fetchLength < span.Length ? span.Slice(0, fetchLength) : span;
			}

			private InstructionPage ReadNextPage(int pageIndex)
			{
				var pageLength = ReadNextPage(out var instructions, out var operands);

				ArrayUtils.Fill(operands, InvalidInstructionOperand, pageLength, PageSize - pageLength);

				return new InstructionPage(instructions, operands, pageIndex, pageLength);
			}

			private int ReadNextPage(out TInstruction[] instructionsBuffer, out int[] operandsBuffer)
			{
				if (Finished)
				{
					instructionsBuffer = null;
					operandsBuffer = null;

					return 0;
				}

				if (ReadNextPageParallel(out instructionsBuffer, out operandsBuffer, out var instructionsCount))
					return instructionsCount;

				var lexemesPage = ReadPageCore();

				if (Exception != null)
					throw Exception;

				TryRunParallel();

				operandsBuffer = lexemesPage.OperandsBuffer;
				instructionsBuffer = lexemesPage.InstructionsBuffer;

				Finished = lexemesPage.InstructionsCount < PageSize;

				return lexemesPage.InstructionsCount;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public int ReadOperand(ref int instructionPointer)
			{
				var pageIndex = instructionPointer >> PageIndexShift;
				var pageOffset = instructionPointer & LocalIndexMask;
				var operand = Pages[pageIndex].OperandsBuffer[pageOffset];

				if (operand == InvalidInstructionOperand)
					return InvalidInstructionOperand;

				instructionPointer++;

				return operand;
			}

			protected InstructionPageStruct ReadPageCore()
			{
				try
				{
					LoadPosition();

					var count = InstructionReader.ReadPage(PageSize, out var instructions, out var operands);
					var lexemesPage = new InstructionPageStruct(instructions, operands, count);

					if (count < PageSize)
						_readerFinished = true;

					return lexemesPage;
				}
				catch (Exception e)
				{
					Exception = e;
					_readerFinished = true;

					return new InstructionPageStruct(null, null, 0);
				}
				finally
				{
					SavePosition();
				}
			}

			protected virtual void SavePosition()
			{
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public void UnlockPointer(int instructionPointer)
			{
				Pages[instructionPointer >> PageIndexShift].ReferenceCount--;
			}

			protected sealed class InstructionPage
			{
				public readonly TInstruction[] InstructionsBuffer;
				public readonly int[] OperandsBuffer;
				public readonly int PageIndex;
				public int PageLength;
				public int ReferenceCount;

				public InstructionPage(TInstruction[] instructionsBuffer, int[] operandsBuffer, int pageIndex, int pageLength)
				{
					InstructionsBuffer = instructionsBuffer;
					OperandsBuffer = operandsBuffer;
					PageIndex = pageIndex;
					PageLength = pageLength;
					ReferenceCount = 1;
				}
			}

			protected readonly struct InstructionPageStruct
			{
				public InstructionPageStruct(TInstruction[] instructionsBuffer, int[] operandsBuffer, int instructionsCount)
				{
					InstructionsBuffer = instructionsBuffer;
					OperandsBuffer = operandsBuffer;
					InstructionsCount = instructionsCount;
				}

				public readonly TInstruction[] InstructionsBuffer;
				public readonly int[] OperandsBuffer;
				public readonly int InstructionsCount;
			}
		}
	}
}