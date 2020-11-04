// <copyright file="Automata.InstructionPointer.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		#region Nested Types

		private interface IInstructionPointer
		{
			#region Properties

			int Index { get; }

			TInstruction Instruction { get; }

			#endregion
		}

		private sealed class DummyInstructionPointer : IInstructionPointer
		{
			#region Static Fields and Constants

			public static readonly IInstructionPointer Instance = new DummyInstructionPointer();

			#endregion

			#region Ctors

			private DummyInstructionPointer()
			{
			}

			#endregion

			#region Interface Implementations

			#region Automata<TInstruction,TOperand>.IInstructionPointer

			public int Index => -1;

			public TInstruction Instruction => default;

			#endregion

			#endregion
		}

		#endregion
	}
}