﻿// <copyright file="Automata.Edge.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Zaaml.Text
{
	[SuppressMessage("ReSharper", "MemberCanBePrivate.Local")]
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		[DebuggerDisplay("{" + nameof(DebuggerDisplay) + "}")]
		private protected sealed class Edge
		{
			public Edge(Node sourceNode, Node targetNode, int weight = 0)
			{
				SourceNode = sourceNode;
				TargetNode = targetNode;

				Weight = weight;
				Terminal = targetNode is EndRuleNode;
			}

			public Edge(Node sourceNode, Node targetNode, MatchEntry operandMatch, int weight = 0) : this(sourceNode, targetNode, weight)
			{
				OperandMatch = operandMatch;
				Terminal = true;
			}

			public Edge(Node sourceNode, Node targetNode, PredicateEntryBase predicateMatch, int weight = 0) : this(sourceNode, targetNode, weight)
			{
				PredicateMatch = predicateMatch;
				Terminal = true;
			}

			private string DebuggerDisplay => SourceNode + " - " + ValueString + " - " + TargetNode;

			public MatchEntry OperandMatch { get; }

			public PredicateEntryBase PredicateMatch { get; }

			public Node SourceNode { get; }

			public Node TargetNode { get; }

			public bool Terminal { get; }

			private string ValueString => Terminal ? OperandMatch?.ToString() ?? PredicateMatch?.ToString() : "?";

			public int Weight { get; }

			public override string ToString()
			{
				return DebuggerDisplay;
			}
		}
	}
}