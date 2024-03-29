﻿// <copyright file="Automata.IInstructionReader.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		protected interface IInstructionReader : IDisposable
		{
			int ReadPage(ref int position, int bufferLength, out TInstruction[] instructions, out int[] operands);

			int ReadPage(ref int position, int bufferOffset, int bufferLength, TInstruction[] instructions, int[] operands);

			void ReleaseBuffers(TInstruction[] instructionsBuffer, int[] operandsBuffer);

			void RentBuffers(int bufferLength, out TInstruction[] instructionsBuffer, out int[] operandsBuffer);
		}
	}
}