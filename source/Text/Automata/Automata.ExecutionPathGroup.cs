// <copyright file="Automata.ExecutionPathGroup.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		private readonly List<ExecutionPathGroup> _executionPathGroupRegistry = new();

		private int RegisterExecutionPathGroup(ExecutionPathGroup executionPathGroup)
		{
			lock (_executionPathGroupRegistry)
			{
				var id = _executionPathGroupRegistry.Count;

				_executionPathGroupRegistry.Add(executionPathGroup);

				return id;
			}
		}

		private protected sealed class ExecutionPathGroup
		{
			public static readonly ExecutionPathGroup Empty = new();
			public readonly ExecutionPath[] ExecutionPaths;

			public readonly int Id;
			public readonly PrimitiveMatchEntry Match;

			private ExecutionPathGroup()
			{
				Match = null;
				ExecutionPaths = Array.Empty<ExecutionPath>();
			}

			public ExecutionPathGroup(Automata<TInstruction, TOperand> automata, IEnumerable<ExecutionPath> executionPaths)
			{
				Id = automata.RegisterExecutionPathGroup(this);
				ExecutionPaths = BuildPathArray(executionPaths);
			}

			public ExecutionPathGroup(Automata<TInstruction, TOperand> automata, TOperand operand, IEnumerable<ExecutionPath> executionPaths)
			{
				Id = automata.RegisterExecutionPathGroup(this);
				Match = new SingleMatchEntry(operand);
				ExecutionPaths = BuildPathArray(executionPaths);
			}

			public ExecutionPathGroup(Automata<TInstruction, TOperand> automata, PrimitiveMatchEntry match, IEnumerable<ExecutionPath> executionPaths)
			{
				Id = automata.RegisterExecutionPathGroup(this);
				Match = match;
				ExecutionPaths = BuildPathArray(executionPaths);
			}

			private static ExecutionPath[] BuildPathArray(IEnumerable<ExecutionPath> executionPaths)
			{
				return executionPaths.ToList().Distinct(NodesEqualityComparer.Instance).OrderBy(e => e.PriorityIndex).ThenByDescending(e => e.Weight).ToArray();
			}
		}
	}
}