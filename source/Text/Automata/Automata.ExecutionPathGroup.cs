// <copyright file="Automata.ExecutionPathGroup.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Linq;

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		#region Nested Types

		private sealed class ExecutionPathGroup
		{
			#region Static Fields and Constants

			public static readonly ExecutionPathGroup Empty = new ExecutionPathGroup();

			#endregion

			#region Fields

			public readonly ExecutionPath[] ExecutionPaths;
			public readonly PrimitiveMatchEntry Match;

			#endregion

			#region Ctors

			private ExecutionPathGroup()
			{
				Match = null;
				ExecutionPaths = new ExecutionPath[0];
			}

			public ExecutionPathGroup(IEnumerable<ExecutionPath> executionPaths)
			{
				ExecutionPaths = BuildPathArray(executionPaths);
			}

			public ExecutionPathGroup(TOperand operand, IEnumerable<ExecutionPath> executionPaths)
			{
				Match = new SingleMatchEntry(operand);
				ExecutionPaths = BuildPathArray(executionPaths);
			}

			public ExecutionPathGroup(PrimitiveMatchEntry match, IEnumerable<ExecutionPath> executionPaths)
			{
				Match = match;
				ExecutionPaths = BuildPathArray(executionPaths);
			}

			#endregion

			#region Methods

			private static ExecutionPath[] BuildPathArray(IEnumerable<ExecutionPath> executionPaths)
			{
				return executionPaths.ToList().Distinct(NodesEqualityComparer.Instance).OrderBy(e => e.PriorityIndex).ThenByDescending(e => e.Weight).ToArray();
			}

			#endregion
		}

		#endregion
	}
}