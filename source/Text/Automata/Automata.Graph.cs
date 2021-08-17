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
		private const int SubGraphReturnMask = 0x10000000;
		private const int SubGraphIdMask = 0x10000000 - 1;
		private bool _built;

		private Dictionary<Rule, Graph> GraphDictionary { get; } = new();

		private Dictionary<Rule, EntryPointSubGraph> SubGraphDictionary { get; } = new();

		protected void Build()
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
			if (GraphDictionary.TryGetValue(rule, out var graph))
				return graph;

			graph = new Graph(rule, this);

			GraphDictionary[rule] = graph;

			graph.Build();

			return graph;
		}

		private EntryPointSubGraph EnsureSubGraph(Rule rule)
		{
			if (SubGraphDictionary.TryGetValue(rule, out var subGraph))
				return subGraph;

			subGraph = SubGraphDictionary[rule] = new EntryPointSubGraph(this, rule);

			subGraph.BuildExecutionGraph();

			return subGraph;
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
			//EvaluateInlineRules();

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

			foreach (var subGraph in _subGraphRegistry)
			{
				subGraph.LeaveNode.EnsureSafe();
				subGraph.RId = subGraph.LeaveNode.HasReturnPathSafe ? SubGraphReturnMask | subGraph.Id : subGraph.Id;
			}

			foreach (var node in _nodeRegistry)
				node.ReCalcRId(this);
		}

		[DebuggerDisplay("{" + nameof(DebuggerDisplay) + "}")]
		private protected sealed class Graph
		{
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
				var beginProductionNode = (Node)new BeginProductionNode(Automata, this, production);
				var endProductionNode = (Node)new EndProductionNode(Automata, this, production);
				//var precedence = production.PrecedencePredicates.ToArray();

				//if (precedence.Length > 0)
				//{
				//	var precedenceEnterNode = new PrecedenceEnterNode(Automata, this, production, precedence);
				//	var precedenceLeaveNode = new PrecedenceLeaveNode(Automata, this, production, precedence);
					
				//	production.Entries.Aggregate(startNode.Connect(precedenceEnterNode).Connect(beginProductionNode), ConnectEntry).Connect(endProductionNode).Connect(precedenceLeaveNode).Connect(endNode);
				//}

				production.Entries.Aggregate(startNode.Connect(beginProductionNode), ConnectEntry).Connect(endProductionNode).Connect(endNode);
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
					EnterPrecedenceEntry enterPrecedenceEntry => ConnectEnterPrecedence(source, enterPrecedenceEntry),
					LeavePrecedenceEntry leavePrecedenceEntry => ConnectLeavePrecedence(source, leavePrecedenceEntry),
					_ => throw new ArgumentOutOfRangeException(nameof(entry))
				};
			}

			private Node ConnectLeavePrecedence(Node source, LeavePrecedenceEntry leavePrecedenceEntry)
			{
				var precedenceLeaveNode = new PrecedenceLeaveNode(Automata, this, leavePrecedenceEntry.PrecedencePredicate);

				return source.Connect(precedenceLeaveNode);
			}

			private Node ConnectEnterPrecedence(Node source, EnterPrecedenceEntry enterPrecedenceEntry)
			{
				var precedenceEnterNode = new PrecedenceEnterNode(Automata, this, enterPrecedenceEntry.PrecedencePredicate);

				return source.Connect(precedenceEnterNode);
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