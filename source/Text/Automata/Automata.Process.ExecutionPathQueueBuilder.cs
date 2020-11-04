// <copyright file="Automata.Process.ExecutionPathQueueBuilder.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		partial class Process
		{
			private sealed class ExecutionPathQueueBuilder
			{
				public readonly ExecutionPathGroupBuilder GroupBuilder;
				public ExecutionPath ExecutionPath;
				public ExecutionPathGroupBuilder NextBuilder;
				public int ReturnPathCount;
				public ExecutionPath[] ReturnPaths;

				public ExecutionPathQueueBuilder(ExecutionPathGroupBuilder groupBuilder)
				{
					GroupBuilder = groupBuilder;
					ReturnPaths = groupBuilder.ReturnPaths;
				}
			}
		}
	}
}