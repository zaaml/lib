// <copyright file="Automata.InstructionPointer.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		private interface IInstructionPointer
		{
			int Index { get; }

			TInstruction Instruction { get; }
		}

		private sealed class DummyInstructionPointer : IInstructionPointer
		{
			public static readonly IInstructionPointer Instance = new DummyInstructionPointer();

			private DummyInstructionPointer()
			{
			}

			public int Index => -1;

			public TInstruction Instruction => default;
		}
	}
}