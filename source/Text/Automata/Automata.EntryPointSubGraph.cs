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
			public readonly EndRuleNode EndNode;
			public readonly ExecutionPath EndPath;
			public readonly InitRuleNode InitNode;
			private bool _executionGraphBuilt;
			public ExecutionPath EmptyPath = ExecutionPath.Invalid;

			public EntryPointSubGraph(Automata<TInstruction, TOperand> automata, Rule rule) : base(automata, rule, null)
			{
				InitNode = new InitRuleNode(automata, this);
				EndNode = new EndRuleNode(automata, this);

				InitNode.OutEdges.Add(new Edge(InitNode, EnterNode));
				LeaveNode.OutEdges.Add(new Edge(LeaveNode, EndNode));
				EndPath = new ExecutionPath(LeaveNode, new Node[] { EndNode }, 0);
				Automata.RegisterExecutionPath(EndPath);

				LeaveNode.EnsureSafe();
			}

			public void BuildExecutionGraph()
			{
				if (_executionGraphBuilt)
					return;

				_executionGraphBuilt = true;

				if (Graph.BeginNode.HasReturn == false)
					return;

				EmptyPath = new ExecutionPath(InitNode, new[]
				{
					EnterNode,
					Graph.BeginNode,
				}.Concat(Graph.BeginNode.ReturnPaths[0].Nodes).ToArray(), -1);

				Automata.RegisterExecutionPath(EmptyPath);
			}
		}
	}
}