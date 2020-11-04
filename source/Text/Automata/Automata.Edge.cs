// <copyright file="Automata.Edge.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Zaaml.Text
{
	[SuppressMessage("ReSharper", "MemberCanBePrivate.Local")]
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		#region Nested Types

		[DebuggerDisplay("{" + nameof(DebuggerDisplay) + "}")]
		private sealed class Edge
		{
			#region Ctors

			public Edge(Node sourceNode, Node targetNode, int weight = 0)
			{
				SourceNode = sourceNode;
				TargetNode = targetNode;

				IsMatch = false;
				Weight = weight;
			}

			public Edge(Node sourceNode, Node targetNode, MatchEntry operandMatch, int weight = 0) : this(sourceNode, targetNode, weight)
			{
				OperandMatch = operandMatch;
				IsMatch = true;
			}

			public Edge(Node sourceNode, Node targetNode, PredicateEntryBase predicateMatch, int weight = 0) : this(sourceNode, targetNode, weight)
			{
				PredicateMatch = predicateMatch;
				IsMatch = true;
			}

			#endregion

			#region Properties

			private string DebuggerDisplay => SourceNode + " - " + ValueString + " - " + TargetNode;

			public bool IsMatch { get; }

			public MatchEntry OperandMatch { get; }

			public PredicateEntryBase PredicateMatch { get; }

			public Node SourceNode { get; }

			public Node TargetNode { get; }

			private string ValueString => IsMatch ? OperandMatch?.ToString() ?? PredicateMatch?.ToString() : "?";

			public int Weight { get; }

			#endregion

			#region Methods

			public override string ToString()
			{
				return DebuggerDisplay;
			}

			#endregion
		}

		#endregion
	}
}