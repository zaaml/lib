// <copyright file="Automata.SubGraph.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using Zaaml.Core;

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		private protected partial class SubGraph : IEquatable<SubGraph>
		{
			[UsedImplicitly] public static readonly SubGraph Empty = new();

			public readonly EnterRuleNode EnterNode;
			public readonly Graph Graph;
			public readonly int Id;
			public readonly Graph InvokingGraph;
			public readonly LeaveRuleNode LeaveNode;
			public readonly Rule Rule;
			public readonly RuleEntry RuleEntry;
			public int RId = -1;

			private SubGraph()
			{
				Rule = null;
			}

			protected SubGraph(Automata<TInstruction, TOperand> automata, Rule rule, Graph invokingGraph)
			{
				Automata = automata;
				Rule = rule;
				Id = automata.RegisterSubGraph(this);
				Graph = automata.EnsureGraph(rule);
				InvokingGraph = invokingGraph;
				EnterNode = new EnterRuleNode(automata, invokingGraph, this);
				LeaveNode = new LeaveRuleNode(automata, invokingGraph, this);
			}

			public SubGraph(Automata<TInstruction, TOperand> automata, RuleEntry ruleEntry, Graph invokingGraph) : this(automata, ruleEntry.Rule, invokingGraph)
			{
				RuleEntry = ruleEntry;
			}

			public Automata<TInstruction, TOperand> Automata { get; }
			
			public override string ToString()
			{
				return Rule.Name;
			}

			public bool Equals(SubGraph other)
			{
				return ReferenceEquals(this, other);
			}
		}
	}
}