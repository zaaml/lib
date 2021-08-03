// <copyright file="Automata.SubGraph.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
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
		#region Fields

		private readonly List<SubGraph> _subGraphRegistry = new List<SubGraph>();

		#endregion

		#region Methods

		private void RegisterSubGraph(SubGraph subGraph)
		{
			subGraph.Id = _subGraphRegistry.Count;

			_subGraphRegistry.Add(subGraph);
		}

		#endregion

		#region Nested Types

		private protected class SubGraphBase
		{
			#region Fields

			public readonly Rule State;

			#endregion

			#region Ctors

			private protected SubGraphBase(Rule state)
			{
				State = state;
			}

			#endregion
		}

		private class SubGraph : SubGraphBase, IEquatable<SubGraph>
		{
			#region Static Fields and Constants

			[UsedImplicitly] public static readonly SubGraph Empty = new SubGraph();

			#endregion

			#region Fields

			public readonly EnterRuleNode EnterNode;
			public readonly Graph Graph;
			public readonly Graph InvokingGraph;
			public int Id;
			public readonly LeaveRuleNode LeaveNode;
			public readonly RuleEntry RuleEntry;
			public readonly bool DfaBarrier;

			#endregion

			#region Ctors

			private SubGraph() : base(null)
			{
			}

			protected SubGraph(Automata<TInstruction, TOperand> automata, Rule state, Graph invokingGraph) : base(state)
			{
				Automata = automata;
				automata.RegisterSubGraph(this);
				Graph = automata.EnsureGraph(state);
				InvokingGraph = invokingGraph;
				EnterNode = new EnterRuleNode(automata, invokingGraph, this);
				LeaveNode = new LeaveRuleNode(automata, invokingGraph, this);
				DfaBarrier = Graph.DfaBarrier;
			}

			public SubGraph(Automata<TInstruction, TOperand> automata, RuleEntry ruleEntry, Graph invokingGraph) : this(automata, ruleEntry.Rule, invokingGraph)
			{
				RuleEntry = ruleEntry;
			}

			#endregion

			#region Properties

			public Automata<TInstruction, TOperand> Automata { get; }

			#endregion

			#region Methods

			public bool Equals(SubGraph other)
			{
				return ReferenceEquals(this, other);
			}

			public override string ToString()
			{
				return State.Name;
			}

			#endregion
		}

		private sealed class EntryPointSubGraph : SubGraph
		{
			#region Fields

			public readonly EndRuleNode EndNode;
			public readonly ExecutionPath EndPath;
			public readonly InitRuleNode InitNode;
			private bool _executionGraphBuilt;
			public ExecutionPath EmptyPath = ExecutionPath.Invalid;

			#endregion

			#region Ctors

			public EntryPointSubGraph(Automata<TInstruction, TOperand> automata, Rule state) : base(automata, state, null)
			{
				InitNode = new InitRuleNode(automata, this);
				EndNode = new EndRuleNode(automata, this);

				InitNode.OutEdges.Add(new Edge(InitNode, EnterNode));
				LeaveNode.OutEdges.Add(new Edge(LeaveNode, EndNode));
				EndPath = new ExecutionPath(LeaveNode, new Node[] {EndNode}, -1);
				Automata.RegisterExecutionPath(EndPath);

				//EndNode.EnsureReady();
			}

			#endregion

			#region Methods

			public void BuildExecutionGraph()
			{
				if (_executionGraphBuilt)
					return;

				_executionGraphBuilt = true;

				//InitNode.EnsureReady();
				//EnterNode.SubGraph.Graph.BeginNode.EnsureReady();

				if (Graph.BeginNode.ReturnPath.IsInvalid)
					return;

				EmptyPath = new ExecutionPath(InitNode, new[]
				{
					EnterNode,
					Graph.BeginNode,
				}.Concat(Graph.BeginNode.ReturnPath.Nodes).ToArray(), -1);

				Automata.RegisterExecutionPath(EmptyPath);
			}

			#endregion
		}

		#endregion
	}
}