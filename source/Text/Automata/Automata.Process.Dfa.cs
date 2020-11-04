// <copyright file="Automata.Process.Dfa.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

// ReSharper disable ForCanBeConvertedToForeach

using Zaaml.Core;

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		#region Nested Types

		partial class Process
		{
			#region Methods

			private bool RunDfa()
			{
				throw Error.Refactoring;

				//DfaState dfaState;
				//SubGraphBase successSubGraph = null;
				//var instructionPointer = _instructionPointer;
				//var successSubGraphInstructionPointer = new InstructionPointerStruct();
				//instructionPointer.AddRef();

				//if (instructionPointer.Page == null)
				//	dfaState = _dfaEntryPoint;
				//else
				//{
				//	var dfaStateBuilder = _automata.DfaBuilderInstance;

				//	dfaState = dfaStateBuilder.Build(instructionPointer.InstructionOperand, _dfaEntryPoint);

				//	int operand;

				//	while (dfaState != null && (operand = instructionPointer.MoveNext()) >= 0)
				//	{
				//		if (dfaState.SuccessTransition != null)
				//		{
				//			successSubGraph = dfaState.SuccessTransition;
				//			successSubGraphInstructionPointer.Sync(ref instructionPointer);
				//		}

				//		dfaState = dfaStateBuilder.Build(operand, dfaState);
				//	}
				//}

				//if (dfaState == null)
				//{
				//	if (instructionPointer.Page != null && _runMode != RunMode.Continuous)
				//		throw _automata.Error(this);
				//}
				//else if (dfaState.SuccessTransition != null)
				//{
				//	successSubGraph = dfaState.SuccessTransition;
				//	successSubGraphInstructionPointer.Sync(ref instructionPointer);
				//}

				//if (successSubGraph == null)
				//	throw _automata.Error(this);

				//successSubGraph.State.EnterState(_context);

				//_instructionPointer.ReleaseRef();
				//_instructionPointer = successSubGraphInstructionPointer;

				//successSubGraph.State.LeaveState(_context);

				//instructionPointer.ReleaseRef();

				//return _instructionPointer.Page == null;
			}

			#endregion

			//private DfaState _dfaEntryPoint;
		}

		#endregion
	}
}