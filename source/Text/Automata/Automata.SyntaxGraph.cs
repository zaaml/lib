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

		private Dictionary<Syntax, SyntaxGraph> GraphDictionary { get; } = new();

		private Dictionary<Syntax, EntryPointSubGraph> SubGraphDictionary { get; } = new();

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

		private SyntaxGraph EnsureGraph(Syntax rule)
		{
			if (GraphDictionary.TryGetValue(rule, out var graph))
				return graph;

			graph = new SyntaxGraph(rule, this);

			GraphDictionary[rule] = graph;

			graph.Build();

			return graph;
		}

		private EntryPointSubGraph EnsureSubGraph(Syntax rule)
		{
			if (SubGraphDictionary.TryGetValue(rule, out var subGraph))
				return subGraph;

			subGraph = SubGraphDictionary[rule] = new EntryPointSubGraph(this, rule);

			subGraph.BuildExecutionGraph();

			return subGraph;
		}

		private protected SubGraph EnsureSubGraphProtected(Syntax rule)
		{
			return EnsureSubGraph(rule);
		}

		private void EvaluateInlineRules()
		{
			var outboundCallsDictionary = new Dictionary<Syntax, HashSet<Syntax>>();
			var inboundCallsDictionary = new Dictionary<Syntax, HashSet<Syntax>>();
			var inlinedRules = new HashSet<Syntax>();

			foreach (var rule in Syntaxes)
			{
				outboundCallsDictionary[rule] = new HashSet<Syntax>(rule.Productions.SelectMany(production => production.Entries).Select(GetRule).Where(s => s != null));
				inboundCallsDictionary[rule] = new HashSet<Syntax>();
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

		private static Syntax GetRule(Entry entry)
		{
			return entry switch
			{
				SyntaxEntry ruleEntry => ruleEntry.Syntax,
				QuantifierEntry { PrimitiveEntry: SyntaxEntry quantifierRuleEntry } => quantifierRuleEntry.Syntax,
				_ => null
			};
		}

		private void SyncBuild()
		{
			//EvaluateInlineRules();

			foreach (var syntax in Syntaxes)
			{
				if (syntax.Inline == false)
					EnsureGraph(syntax);
			}

			foreach (var syntax in Syntaxes)
			{
				if (syntax.Inline == false)
					EnsureSubGraph(syntax);
			}

			foreach (var subGraph in _subGraphRegistry)
			{
				subGraph.LeaveNode.EnsureSafe();
				subGraph.RId = subGraph.LeaveNode.HasReturnPathSafe ? SubGraphReturnMask | subGraph.Id : subGraph.Id;
			}

			foreach (var node in _nodeRegistry)
			{
				node.ReCalcRId(this);
			}
		}

		[DebuggerDisplay("{" + nameof(DebuggerDisplay) + "}")]
		private protected sealed class SyntaxGraph
		{
			public SyntaxGraph(Syntax syntax, Automata<TInstruction, TOperand> automata)
			{
				Syntax = syntax;
				Automata = automata;

				BeginNode = new BeginSyntaxNode(Automata, this);
				ReturnNode = new ReturnSyntaxNode(Automata, this);
			}

			private Automata<TInstruction, TOperand> Automata { get; }

			public Node BeginNode { get; }

			public bool CanSimulateDfa { get; private set; }

			private string DebuggerDisplay => Syntax.Name;

			[UsedImplicitly]
			private IEnumerable<Edge> Edges => Nodes.SelectMany(n => n.OutEdges);

			public bool HasOperandNodes { get; private set; }

			[UsedImplicitly]
			public HashSet<SyntaxGraph> InboundGraphCall { get; } = new();

			[UsedImplicitly]
			public List<SubGraph> InboundSubGraphCall { get; } = new();

			public List<Node> Nodes { get; } = new();

			[UsedImplicitly]
			public HashSet<SyntaxGraph> OutboundDescendantsGraphCall { get; } = new();

			[UsedImplicitly]
			public HashSet<SyntaxGraph> OutboundGraphCall { get; } = new();

			[UsedImplicitly]
			public List<SubGraph> OutboundSubGraphCall { get; } = new();

			public Node ReturnNode { get; }

			public Syntax Syntax { get; }

			public OperandNode SingleOperandAfterBegin { get; private set; }

			public OperandNode SingleOperandBeforeReturn { get; private set; }

			public void AddNode(Node node)
			{
				HasOperandNodes |= node is OperandNode;
				Nodes.Add(node);
			}

			public void Build()
			{
				foreach (var production in Syntax.Productions)
					BuildProduction(production);

				foreach (var node in Nodes)
					node.SortEdges();

				CanSimulateDfa = Nodes.OfType<EnterSyntaxNode>().Any() == false;
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
					SyntaxEntry stateEntry => ConnectRuleEntry(source, stateEntry, stateEntry.Syntax.Inline || Automata.ForceInlineAll),
					QuantifierEntry quantifierEntry => ConnectQuantifierEntry(source, quantifierEntry),
					PredicateEntryBase predicateEntry => ConnectPredicateEntry(source, predicateEntry),
					ActionEntry actionEntry => ConnectActionEntry(source, actionEntry),
					EpsilonEntry _ => source,
					EnterPrecedenceEntry enterPrecedenceEntry => ConnectEnterPrecedence(source, enterPrecedenceEntry),
					LeavePrecedenceEntry leavePrecedenceEntry => ConnectLeavePrecedence(source, leavePrecedenceEntry),
					ValueEntry valueEntry => ConnectValue(source, valueEntry),
					_ => throw new ArgumentOutOfRangeException(nameof(entry))
				};
			}

			private Node ConnectValue(Node source, ValueEntry valueEntry)
			{
				var valueNode = new ValueNode(Automata, this, valueEntry);

				return source.Connect(valueNode);
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
				var operandNode = new OperandNode(Automata, this, operandEntry);

				source.Connect(operandNode, operandEntry);

				return operandNode;
			}

			private Node ConnectPredicateEntry(Node source, PredicateEntryBase predicateEntry)
			{
				var predicateNode = new PredicateNode(Automata, this, predicateEntry);
				var innerNode = new InnerNode(Automata, this);

				source.Connect(predicateNode, predicateEntry);
				predicateNode.Connect(innerNode);

				return innerNode;
			}

			private Node ConnectQuantifierEntry(Node source, QuantifierEntry quantifierEntry)
			{
				var actualMinimum = quantifierEntry.Minimum;
				var actualMaximum = quantifierEntry.Maximum;
				var output = new InnerNode(Automata, this);

				var leaveWeight = -1;
				var enterWeight = 1;

				if (quantifierEntry.Mode == QuantifierMode.Lazy)
				{
					leaveWeight = 1;
					enterWeight = -1;

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

			private Node ConnectRuleEntry(Node source, SyntaxEntry ruleEntry, bool inline)
			{
				var state = ruleEntry.Syntax;

				if (inline)
				{
					var inputNode = new InlineEnterSyntaxNode(Automata, this, ruleEntry);
					var outputNode = new InlineLeaveSyntaxNode(Automata, this, ruleEntry);

					source.Connect(inputNode);

					foreach (var transition in state.Productions)
						BuildProduction(transition, inputNode, outputNode);

					return outputNode;
				}

				var subGraph = CreateSubGraph(ruleEntry);

				source.Connect(subGraph.EnterNode);

				return subGraph.LeaveNode;
			}

			private SubGraph CreateSubGraph(SyntaxEntry ruleEntry)
			{
				var subGraph = new SubGraph(Automata, ruleEntry, this);

				subGraph.SyntaxGraph.RegisterInboundCall(subGraph);
				RegisterOutboundCall(subGraph);

				return subGraph;
			}

			private void RegisterInboundCall(SubGraph subGraph)
			{
				InboundSubGraphCall.Add(subGraph);
				InboundGraphCall.Add(subGraph.SyntaxGraph);
			}

			private void RegisterOutboundCall(SubGraph subGraph)
			{
				OutboundSubGraphCall.Add(subGraph);
				OutboundGraphCall.Add(subGraph.SyntaxGraph);
			}

			public override string ToString()
			{
				return DebuggerDisplay;
			}
		}
	}
}