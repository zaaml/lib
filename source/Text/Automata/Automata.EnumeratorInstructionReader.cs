// <copyright file="Automata.EnumeratorInstructionReader.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Buffers;
using System.Collections.Generic;

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		#region Nested Types

		protected sealed class EnumeratorInstructionReader : IInstructionReader
		{
			#region Fields

			private readonly Func<TInstruction, int> _decoder;
			private readonly IEnumerator<TInstruction> _enumerator;

			#endregion

			#region Ctors

			public EnumeratorInstructionReader(IEnumerator<TInstruction> enumerator, Func<TInstruction, int> decoder)
			{
				_enumerator = enumerator;
				_decoder = decoder;
			}

			#endregion

			#region Properties

			private int ReaderPosition { get; set; }

			#endregion

			#region Interface Implementations

			#region Automata<TInstruction,TOperand>.IInstructionReader

			public int ReadPage(int bufferOffset, int bufferLength, TInstruction[] instructionsBuffer, int[] operandsBuffer)
			{
				var count = bufferOffset;
				var decoder = _decoder;

				while (count < bufferLength && _enumerator.MoveNext())
				{
					var instruction = _enumerator.Current;

					ReaderPosition++;

					instructionsBuffer[count] = instruction;
					operandsBuffer[count] = decoder(instruction);

					count++;
				}

				return count - bufferOffset;
			}

			public int ReadPage(int bufferLength, out TInstruction[] instructionsBuffer, out int[] operandsBuffer)
			{
				RentBuffers(bufferLength, out instructionsBuffer, out operandsBuffer);

				return ReadPage(0, bufferLength, instructionsBuffer, operandsBuffer);
			}

			public void ReleaseBuffers(TInstruction[] instructionsBuffer, int[] operandsBuffer)
			{
				ArrayPool<TInstruction>.Shared.Return(instructionsBuffer);
				ArrayPool<int>.Shared.Return(operandsBuffer);
			}

			public void RentBuffers(int bufferLength, out TInstruction[] instructionsBuffer, out int[] operandsBuffer)
			{
				instructionsBuffer = ArrayPool<TInstruction>.Shared.Rent(bufferLength);
				operandsBuffer = ArrayPool<int>.Shared.Rent(bufferLength);
			}

			#endregion

			#region IDisposable

			public void Dispose()
			{
				_enumerator.Dispose();
			}

			#endregion

			#endregion
		}

		#endregion
	}
}