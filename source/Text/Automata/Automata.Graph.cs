// <copyright file="Automata.Graph.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Zaaml.Core;
using Zaaml.Core.Extensions;

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		#region Fields

		private bool _built;

		#endregion

		#region Properties

		private Dictionary<FiniteState, Graph> GraphDictionary { get; } = new Dictionary<FiniteState, Graph>();

		private Dictionary<FiniteState, EntryPointSubGraph> SubGraphDictionary { get; } = new Dictionary<FiniteState, EntryPointSubGraph>();

		#endregion

		#region Methods

		private void Build()
		{
			if (_built)
				return;

			lock (this)
			{
				if (_built)
					return;

				SyncBuild();

				_built = true;
			}
		}

		private Graph EnsureGraph(FiniteState state)
		{
			var graph = GraphDictionary.GetValueOrDefault(state);

			if (graph != null)
				return graph;

			graph = new Graph(state, this);

			GraphDictionary[state] = graph;

			graph.Build();

			return graph;
		}

		private EntryPointSubGraph EnsureSubGraph(FiniteState finiteState)
		{
			var subGraph = SubGraphDictionary.GetValueOrDefault(finiteState);

			if (subGraph != null)
				return subGraph;

			subGraph = SubGraphDictionary[finiteState] = new EntryPointSubGraph(this, finiteState);

			subGraph.BuildExecutionGraph();

			return subGraph;
		}

		private void EvaluateInlineStates()
		{
			var outboundCallsDictionary = new Dictionary<FiniteState, HashSet<FiniteState>>();
			var inboundCallsDictionary = new Dictionary<FiniteState, HashSet<FiniteState>>();
			var inlinedStates = new HashSet<FiniteState>();

			foreach (var state in States)
			{
				outboundCallsDictionary[state] = new HashSet<FiniteState>(state.Productions.SelectMany(transition => transition.Entries).Select(GetState).Where(s => s != null));
				inboundCallsDictionary[state] = new HashSet<FiniteState>();
			}

			foreach (var kv in outboundCallsDictionary)
			{
				var key = kv.Key;
				var value = kv.Value;

				foreach (var state in value)
					inboundCallsDictionary[state].Add(key);
			}

			while (true)
			{
				var inlineSates = outboundCallsDictionary.Where(kv => kv.Value.Count == 0 && kv.Key.Inline == false).Select(kv => kv.Key).ToList();

				if (inlineSates.Count == 0)
					break;

				foreach (var outState in inlineSates)
				{
					outState.Inline = true;

					inlinedStates.Add(outState);

					if (!inboundCallsDictionary.TryGetValue(outState, out var inboundCallsSet))
						continue;

					foreach (var inState in inboundCallsSet)
						outboundCallsDictionary[inState].Remove(outState);
				}
			}
		}

		private static FiniteState GetState(Entry entry)
		{
			return entry switch
			{
				StateEntry stateEntry => stateEntry.State,
				QuantifierEntry quantifier when quantifier.PrimitiveEntry is StateEntry quantifierStateEntry => quantifierStateEntry.State,
				_ => null
			};
		}

		private void SyncBuild()
		{
			//EvaluateInlineStates();

			foreach (var state in States)
			{
				if (state.Inline == false)
					EnsureGraph(state);
			}

			foreach (var state in States)
			{
				if (state.Inline == false)
					EnsureSubGraph(state);
			}

			EvalDfaGraph();
		}

		private void EvalDfaGraph()
		{
			var dfaBarriers = new HashSet<Graph>();

			foreach (var state in States)
			{
				if (state.Inline || state.Name == null)
					continue;

				var graph = EnsureGraph(state);

				graph.EvalDfaBarrier();

				if (graph.DfaBarrier)
					dfaBarriers.Add(graph);
			}
		}

		#endregion

		#region Nested Types

		[DebuggerDisplay("{" + nameof(DebuggerDisplay) + "}")]
		private sealed class Graph
		{
			#region Fields

			private int _innerCounter;

			#endregion

			#region Ctors

			public Graph(FiniteState state, Automata<TInstruction, TOperand> automata)
			{
				State = state;
				Automata = automata;

				BeginNode = new BeginStateNode(Automata, this);
				ReturnNode = new ReturnStateNode(Automata, this);
			}

			#endregion

			#region Properties

			private Automata<TInstruction, TOperand> Automata { get; }

			public Node BeginNode { get; }

			public bool CanSimulateDfa { get; private set; }

			private string DebuggerDisplay => State.Name;

			[UsedImplicitly] private IEnumerable<Edge> Edges => Nodes.SelectMany(n => n.OutEdges);

			public bool HasOperandNodes { get; private set; }

			[UsedImplicitly] public HashSet<Graph> InboundGraphCall { get; } = new HashSet<Graph>();

			[UsedImplicitly] public List<SubGraph> InboundSubGraphCall { get; } = new List<SubGraph>();

			public List<Node> Nodes { get; } = new List<Node>();

			[UsedImplicitly] public HashSet<Graph> OutboundDescendantsGraphCall { get; } = new HashSet<Graph>();

			[UsedImplicitly] public HashSet<Graph> OutboundGraphCall { get; } = new HashSet<Graph>();

			[UsedImplicitly] public List<SubGraph> OutboundSubGraphCall { get; } = new List<SubGraph>();

			public Node ReturnNode { get; }

			public FiniteState State { get; }

			public OperandNode SingleOperandBeforeReturn { get; private set; }

			public OperandNode SingleOperandAfterBegin { get; private set; }

			public bool DfaBarrier { get; private set; }

			#endregion

			#region Methods

			public void AddNode(Node node)
			{
				if (node is InnerNode)
					node.Index = _innerCounter++;

				HasOperandNodes |= node is OperandNode;
				Nodes.Add(node);
			}

			public void Build()
			{
				foreach (var transition in State.Productions)
					BuildProduction(transition);

				CanSimulateDfa = Nodes.OfType<EnterStateNode>().Any() == false;
			}

			private void EvalSingleOperandBeforeReturn()
			{
				var currentNode = ReturnNode;

				while (currentNode.InEdges.Count == 1)
				{
					var edge = currentNode.InEdges[0];

					currentNode = edge.SourceNode;

					if (currentNode is OperandNode operandNode)
					{
						SingleOperandBeforeReturn = operandNode;

						break;
					}
				}
			}

			private void EvalSingleOperandAfterBegin()
			{
				var currentNode = BeginNode;

				while (currentNode.OutEdges.Count == 1)
				{
					var edge = currentNode.OutEdges[0];

					currentNode = edge.TargetNode;

					if (currentNode is OperandNode operandNode)
					{
						SingleOperandAfterBegin = operandNode;

						break;
					}
				}
			}

			private void BuildProduction(Production production)
			{
				BuildProduction(production, BeginNode, ReturnNode);
			}

			private void BuildProduction(Production production, Node startNode, Node endNode)
			{
				var beginTransitionNode = (Node)new BeginProductionNode(Automata, this, production);
				var endTransitionNode = (Node)new EndProductionNode(Automata, this, production);

				production.Entries.Aggregate(startNode.Connect(beginTransitionNode), ConnectEntry).Connect(endTransitionNode).Connect(endNode);
			}

			private Node ConnectAction(Node source, ActionEntry actionEntry)
			{
				var outputNode = new ActionNode(Automata, this, actionEntry);

				source.Connect(outputNode);

				return outputNode;
			}

			private Node ConnectEntry(Node source, Entry entry)
			{
				return entry switch
				{
					MatchEntry matchEntry => ConnectMatch(source, matchEntry),
					StateEntry stateEntry => ConnectState(source, stateEntry, stateEntry.State.Inline || Automata.ForceInlineAll),
					QuantifierEntry quantifierEntry => ConnectQuantifier(source, quantifierEntry),
					PredicateEntryBase predicateEntry => ConnectPredicate(source, predicateEntry),
					ActionEntry actionEntry => ConnectAction(source, actionEntry),
					EpsilonEntry _ => source,
					_ => throw new ArgumentOutOfRangeException(nameof(entry))
				};
			}

			private Node ConnectMatch(Node source, MatchEntry operandEntry)
			{
				var outputNode = new OperandNode(Automata, this, operandEntry);

				source.Connect(outputNode, operandEntry);

				return outputNode;
			}

			private Node ConnectPredicate(Node source, PredicateEntryBase predicateEntry)
			{
				var outputNode = new PredicateNode(Automata, this, predicateEntry);

				source.Connect(outputNode, predicateEntry);

				return outputNode;
			}

			private Node ConnectQuantifier(Node source, QuantifierEntry quantifierEntry)
			{
				var actualMinimum = quantifierEntry.Minimum;
				var actualMaximum = quantifierEntry.Maximum;
				var output = new InnerNode(Automata, this);

				var leaveWeight = 0;
				var enterWeight = 1;

				if (quantifierEntry.Mode == QuantifierMode.Lazy)
				{
					leaveWeight = 1;
					enterWeight = 0;

					source = source.Connect(new LazyNode(Automata, this));
				}

				if (actualMinimum == actualMaximum)
				{
					for (var i = 0; i < actualMinimum; i++)
						source = ConnectEntry(source, quantifierEntry.PrimitiveEntry);

					source.Connect(output);

					return output;
				}

				for (var i = 0; i < actualMinimum; i++)
					source = ConnectEntry(source, quantifierEntry.PrimitiveEntry);

				source.Connect(output, leaveWeight);

				if (actualMaximum == int.MaxValue)
				{
					var loopEnter = new InnerNode(Automata, this);

					source.Connect(loopEnter, enterWeight);

					var loopLeave = ConnectEntry(loopEnter, quantifierEntry.PrimitiveEntry);

					loopLeave.Connect(output, leaveWeight);
					loopLeave.Connect(loopEnter, enterWeight);
				}
				else
				{
					for (var i = 0; i < actualMaximum - actualMinimum; i++)
					{
						var loopEnter = new InnerNode(Automata, this);

						source.Connect(loopEnter, enterWeight);

						var loopLeave = ConnectEntry(source, quantifierEntry.PrimitiveEntry);

						loopLeave.Connect(output, leaveWeight);

						source = loopLeave;
					}
				}

				return output;
			}

			private Node ConnectState(Node source, StateEntry stateEntry, bool inline)
			{
				var state = stateEntry.State;

				if (inline)
				{
					var inputNode = new InlineEnterStateNode(Automata, this, stateEntry);
					var outputNode = new InlineLeaveStateNode(Automata, this, stateEntry);

					source.Connect(inputNode);

					foreach (var transition in state.Productions)
						BuildProduction(transition, inputNode, outputNode);

					return outputNode;
				}

				var subGraph = CreateSubGraph(stateEntry);

				source.Connect(subGraph.EnterNode);

				return subGraph.LeaveNode;
			}

			private SubGraph CreateSubGraph(StateEntry stateEntry)
			{
				var subGraph = new SubGraph(Automata, stateEntry, this);

				subGraph.Graph.RegisterInboundCall(subGraph);
				RegisterOutboundCall(subGraph);

				return subGraph;
			}

			private void RegisterInboundCall(SubGraph subGraph)
			{
				InboundSubGraphCall.Add(subGraph);
				InboundGraphCall.Add(subGraph.Graph);
			}

			private void RegisterOutboundCall(SubGraph subGraph)
			{
				OutboundSubGraphCall.Add(subGraph);
				OutboundGraphCall.Add(subGraph.Graph);
			}

			public override string ToString()
			{
				return DebuggerDisplay;
			}

			#endregion

			private List<List<Graph>> EvalRoots(int maxCount = 100, int maxDepth = int.MaxValue)
			{
				var visited = new HashSet<Graph>();
				var stack = new Stack<(Graph, int)>();
				var rootLimit = maxCount;
				var roots = new List<List<Graph>>();

				stack.Push((this, -1));

				while (stack.Count > 0)
				{
					var edge = stack.Pop();

					edge.Item2++;

					if (edge.Item2 < edge.Item1.InboundSubGraphCall.Count)
					{
						var nextGraph = edge.Item1.InboundSubGraphCall[edge.Item2].InvokingGraph;

						stack.Push(edge);

						if (visited.Add(nextGraph))
						{
							if (nextGraph.InboundSubGraphCall.Count > 0 && stack.Count + 1 < maxDepth)
								stack.Push((nextGraph, -1));
							else
							{
								roots.Add(stack.Select(e => e.Item1).Prepend(nextGraph).ToList());

								if (roots.Count == rootLimit)
									break;

								visited.Remove(nextGraph);
							}
						}
						else
						{
							roots.Add(stack.Select(e => e.Item1).ToList());

							if (roots.Count == rootLimit)
								break;
						}
					}
					else
					{
						visited.Remove(edge.Item1);
					}
				}

				return roots;
			}

			public void EvalDfaBarrier()
			{
				EvalSingleOperandAfterBegin();
				EvalSingleOperandBeforeReturn();

				//if (State.Name == "type_declaration")
				//{
				//	var roots = EvalRoots().ToList();
				//}
				//DfaBarrier = InboundSubGraphCall.Count == 1 && SingleOperandBeforeReturn?.MatchEntry is SingleMatchEntry && SingleOperandAfterBegin?.MatchEntry is SingleMatchEntry;
				//DfaBarrier = InboundSubGraphCall.Count == 1 && SingleOperandBeforeReturn?.MatchEntry is SingleMatchEntry;

				var roots = EvalRoots(1000, 1000);

				if (roots.Count < 50)
				{
				}

				DfaBarrier = false;
			}
		}

		#endregion
	}
}