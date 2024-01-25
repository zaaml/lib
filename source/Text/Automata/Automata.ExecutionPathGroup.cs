// <copyright file="Automata.ExecutionPathGroup.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using Zaaml.Core;

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		private readonly List<ExecutionPathGroup> _executionPathGroupRegistry = new();
		private int _maxGroupLength;

		private int RegisterExecutionPathGroup(ExecutionPathGroup executionPathGroup)
		{
			lock (_executionPathGroupRegistry)
			{
				var id = _executionPathGroupRegistry.Count;

				_executionPathGroupRegistry.Add(executionPathGroup);

				if (executionPathGroup.ExecutionPaths.Length > _maxGroupLength)
					_maxGroupLength = executionPathGroup.ExecutionPaths.Length;

				return id;
			}
		}

		private protected sealed class ExecutionPathGroup
		{
			public static readonly ExecutionPathGroup Empty = new();
			public readonly ExecutionPath[] ExecutionPaths;

			public readonly int Id;
			public readonly Range<int> Range;

			private ExecutionPathGroup()
			{
				Range = Range<int>.Empty;
				ExecutionPaths = Array.Empty<ExecutionPath>();
			}

			public ExecutionPathGroup(Automata<TInstruction, TOperand> automata, IEnumerable<ExecutionPath> executionPaths)
			{
				ExecutionPaths = BuildPathArray(executionPaths);
				Id = automata.RegisterExecutionPathGroup(this);
			}

			public ExecutionPathGroup(Automata<TInstruction, TOperand> automata, TOperand operand, IEnumerable<ExecutionPath> executionPaths)
			{
				Range = new Range<int>(ConvertFromOperand(operand), ConvertFromOperand(operand));
				ExecutionPaths = BuildPathArray(executionPaths);
				Id = automata.RegisterExecutionPathGroup(this);
			}

			public ExecutionPathGroup(Automata<TInstruction, TOperand> automata, Range<int> range, IEnumerable<ExecutionPath> executionPaths)
			{
				Range = range;
				ExecutionPaths = BuildPathArray(executionPaths);
				Id = automata.RegisterExecutionPathGroup(this);
			}

			private static ExecutionPath[] BuildPathArray(IEnumerable<ExecutionPath> executionPaths)
			{
				return executionPaths.ToList().Distinct(NodesEqualityComparer.Instance).OrderBy(e => e.PriorityIndex).ToArray();
			}
		}
	}
}