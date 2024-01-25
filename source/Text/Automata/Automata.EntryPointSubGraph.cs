// <copyright file="Automata.EntryPointSubGraph.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Linq;

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		private sealed class EntryPointSubGraph : SubGraph
		{
			public readonly ExitSyntaxNode ExitNode;
			public readonly ExecutionPath EndPath;
			public readonly InitSyntaxNode InitNode;
			private bool _executionGraphBuilt;
			public ExecutionPath EmptyPath = ExecutionPath.Invalid;

			public EntryPointSubGraph(Automata<TInstruction, TOperand> automata, Syntax syntax) : base(automata, syntax, null)
			{
				InitNode = new InitSyntaxNode(automata, this);
				ExitNode = new ExitSyntaxNode(automata, this);

				InitNode.OutEdges.Add(new Edge(InitNode, EnterNode));
				LeaveNode.OutEdges.Add(new Edge(LeaveNode, ExitNode));
				
				LeaveNode.EnsureSafe();
				EndPath = LeaveNode.ExecutionPaths.Single();
				
				LeaveNode.EnsureSafe();
			}

			public void BuildExecutionGraph()
			{
				if (_executionGraphBuilt)
					return;

				_executionGraphBuilt = true;

				if (SyntaxGraph.BeginNode.HasReturn == false)
					return;

				EmptyPath = Automata.CreateExecutionPath(InitNode, new[]
				{
					EnterNode,
					SyntaxGraph.BeginNode,
				}.Concat(SyntaxGraph.BeginNode.ReturnPaths[0].Nodes).ToArray());

			}
		}
	}
}