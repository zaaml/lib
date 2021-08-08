// <copyright file="Automata.Graph.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Zaaml.Core;

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		private bool _built;

		private Dictionary<Rule, Graph> GraphDictionary { get; } = new();

		private Dictionary<Rule, EntryPointSubGraph> SubGraphDictionary { get; } = new();

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

		private Graph EnsureGraph(Rule rule)
		{
			var graph = GraphDictionary.GetValueOrDefault(rule);

			if (graph != null)
				return graph;

			graph = new Graph(rule, this);

			GraphDictionary[rule] = graph;

			graph.Build();

			return graph;
		}

		private EntryPointSubGraph EnsureSubGraph(Rule rule)
		{
			var subGraph = SubGraphDictionary.GetValueOrDefault(rule);

			if (subGraph != null)
				return subGraph;

			subGraph = SubGraphDictionary[rule] = new EntryPointSubGraph(this, rule);

			subGraph.BuildExecutionGraph();

			return subGraph;
		}

		private void EvalDfaGraph()
		{
			//var dfaBarriers = new HashSet<Graph>();

			//foreach (var state in States)
			//{
			//	if (state.Inline || state.Name == null)
			//		continue;

			//	var graph = EnsureGraph(state);

			//	graph.EvalDfaBarrier();

			//	if (graph.DfaBarrier)
			//		dfaBarriers.Add(graph);
			//}
		}

		private void EvaluateInlineRules()
		{
			var outboundCallsDictionary = new Dictionary<Rule, HashSet<Rule>>();
			var inboundCallsDictionary = new Dictionary<Rule, HashSet<Rule>>();
			var inlinedRules = new HashSet<Rule>();

			foreach (var rule in Rules)
			{
				outboundCallsDictionary[rule] = new HashSet<Rule>(rule.Productions.SelectMany(production => production.Entries).Select(GetRule).Where(s => s != null));
				inboundCallsDictionary[rule] = new HashSet<Rule>();
			}

			foreach (var kv in outboundCallsDictionary)
			{
				var key = kv.Key;
				var value = kv.Value;

				foreach (var rule in value)
				{
					if (inboundCallsDictionary.TryGetValue(rule, out var set))
						set.Add(key);
				}
			}

			while (true)
			{
				var inlineRules = outboundCallsDictionary.Where(kv => kv.Value.Count == 0 && kv.Key.Inline == false).Select(kv => kv.Key).ToList();

				if (inlineRules.Count == 0)
					break;

				foreach (var rule in inlineRules)
				{
					rule.Inline = true;

					inlinedRules.Add(rule);

					if (inboundCallsDictionary.TryGetValue(rule, out var inboundCallsSet) == false)
						continue;

					foreach (var inState in inboundCallsSet)
						outboundCallsDictionary[inState].Remove(rule);
				}
			}
		}

		private static Rule GetRule(Entry entry)
		{
			return entry switch
			{
				RuleEntry ruleEntry => ruleEntry.Rule,
				QuantifierEntry { PrimitiveEntry: RuleEntry quantifierRuleEntry } => quantifierRuleEntry.Rule,
				_ => null
			};
		}

		private void SyncBuild()
		{
			EvaluateInlineRules();

			foreach (var rule in Rules)
			{
				if (rule.Inline == false)
					EnsureGraph(rule);
			}

			foreach (var rule in Rules)
			{
				if (rule.Inline == false)
					EnsureSubGraph(rule);
			}

			//EvalDfaGraph();
		}

		[DebuggerDisplay("{" + nameof(DebuggerDisplay) + "}")]
		private protected sealed class Graph
		{
			private int _innerCounter;

			public Graph(Rule rule, Automata<TInstruction, TOperand> automata)
			{
				Rule = rule;
				Automata = automata;

				BeginNode = new BeginRuleNode(Automata, this);
				ReturnNode = new ReturnRuleNode(Automata, this);
			}

			private Automata<TInstruction, TOperand> Automata { get; }

			public Node BeginNode { get; }

			public bool CanSimulateDfa { get; private set; }

			private string DebuggerDisplay => Rule.Name;

			[UsedImplicitly]
			private IEnumerable<Edge> Edges => Nodes.SelectMany(n => n.OutEdges);

			public bool HasOperandNodes { get; private set; }

			[UsedImplicitly]
			public HashSet<Graph> InboundGraphCall { get; } = new();

			[UsedImplicitly]
			public List<SubGraph> InboundSubGraphCall { get; } = new();

			public List<Node> Nodes { get; } = new();

			[UsedImplicitly]
			public HashSet<Graph> OutboundDescendantsGraphCall { get; } = new();

			[UsedImplicitly]
			public HashSet<Graph> OutboundGraphCall { get; } = new();

			[UsedImplicitly]
			public List<SubGraph> OutboundSubGraphCall { get; } = new();

			public Node ReturnNode { get; }

			public Rule Rule { get; }

			public OperandNode SingleOperandAfterBegin { get; private set; }

			public OperandNode SingleOperandBeforeReturn { get; private set; }

			public void AddNode(Node node)
			{
				if (node is InnerNode)
					node.Index = _innerCounter++;

				HasOperandNodes |= node is OperandNode;
				Nodes.Add(node);
			}

			public void Build()
			{
				foreach (var production in Rule.Productions)
					BuildProduction(production);

				CanSimulateDfa = Nodes.OfType<EnterRuleNode>().Any() == false;
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

			private Node ConnectActionEntry(Node source, ActionEntry actionEntry)
			{
				var outputNode = new ActionNode(Automata, this, actionEntry);

				source.Connect(outputNode);

				return outputNode;
			}

			private Node ConnectEntry(Node source, Entry entry)
			{
				return entry switch
				{
					MatchEntry matchEntry => ConnectMatchEntry(source, matchEntry),
					RuleEntry stateEntry => ConnectRuleEntry(source, stateEntry, stateEntry.Rule.Inline || Automata.ForceInlineAll),
					QuantifierEntry quantifierEntry => ConnectQuantifierEntry(source, quantifierEntry),
					PredicateEntryBase predicateEntry => ConnectPredicateEntry(source, predicateEntry),
					ActionEntry actionEntry => ConnectActionEntry(source, actionEntry),
					EpsilonEntry _ => source,
					_ => throw new ArgumentOutOfRangeException(nameof(entry))
				};
			}

			private Node ConnectMatchEntry(Node source, MatchEntry operandEntry)
			{
				var outputNode = new OperandNode(Automata, this, operandEntry);

				source.Connect(outputNode, operandEntry);

				return outputNode;
			}

			private Node ConnectPredicateEntry(Node source, PredicateEntryBase predicateEntry)
			{
				var outputNode = new PredicateNode(Automata, this, predicateEntry);

				source.Connect(outputNode, predicateEntry);

				return outputNode;
			}

			private Node ConnectQuantifierEntry(Node source, QuantifierEntry quantifierEntry)
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

			private Node ConnectRuleEntry(Node source, RuleEntry ruleEntry, bool inline)
			{
				var state = ruleEntry.Rule;

				if (inline)
				{
					var inputNode = new InlineEnterRuleNode(Automata, this, ruleEntry);
					var outputNode = new InlineLeaveRuleNode(Automata, this, ruleEntry);

					source.Connect(inputNode);

					foreach (var transition in state.Productions)
						BuildProduction(transition, inputNode, outputNode);

					return outputNode;
				}

				var subGraph = CreateSubGraph(ruleEntry);

				source.Connect(subGraph.EnterNode);

				return subGraph.LeaveNode;
			}

			private SubGraph CreateSubGraph(RuleEntry ruleEntry)
			{
				var subGraph = new SubGraph(Automata, ruleEntry, this);

				subGraph.Graph.RegisterInboundCall(subGraph);
				RegisterOutboundCall(subGraph);

				return subGraph;
			}

			public void EvalDfaBarrier()
			{
				//EvalSingleOperandAfterBegin();
				//EvalSingleOperandBeforeReturn();

				//if (State.Name == "type_declaration")
				//{
				//	var roots = EvalRoots().ToList();
				//}
				//DfaBarrier = InboundSubGraphCall.Count == 1 && SingleOperandBeforeReturn?.MatchEntry is SingleMatchEntry && SingleOperandAfterBegin?.MatchEntry is SingleMatchEntry;
				//DfaBarrier = InboundSubGraphCall.Count == 1 && SingleOperandBeforeReturn?.MatchEntry is SingleMatchEntry;

				//var roots = EvalRoots(1000, 1000);

				//if (roots.Count < 50)
				//{
				//}

				//DfaBarrier = false;
			}

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
		}
	}
}