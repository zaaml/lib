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

			public readonly EnterSyntaxNode EnterNode;
			public readonly SyntaxGraph SyntaxGraph;
			public readonly int Id;
			public readonly SyntaxGraph InvokingSyntaxGraph;
			public readonly LeaveSyntaxNode LeaveNode;
			public readonly Syntax Syntax;
			public readonly SyntaxEntry SyntaxEntry;
			public int RId = -1;

			private SubGraph()
			{
				Syntax = null;
			}

			protected SubGraph(Automata<TInstruction, TOperand> automata, Syntax syntax, SyntaxGraph invokingSyntaxGraph)
			{
				Automata = automata;
				Syntax = syntax;
				Id = automata.RegisterSubGraph(this);
				SyntaxGraph = automata.EnsureGraph(syntax);
				InvokingSyntaxGraph = invokingSyntaxGraph;
				EnterNode = new EnterSyntaxNode(automata, invokingSyntaxGraph, this);
				LeaveNode = new LeaveSyntaxNode(automata, invokingSyntaxGraph, this);
			}

			public SubGraph(Automata<TInstruction, TOperand> automata, SyntaxEntry syntaxEntry, SyntaxGraph invokingSyntaxGraph) : this(automata, syntaxEntry.Syntax, invokingSyntaxGraph)
			{
				SyntaxEntry = syntaxEntry;
			}

			public Automata<TInstruction, TOperand> Automata { get; }
			
			public override string ToString()
			{
				return Syntax.Name;
			}

			public bool Equals(SubGraph other)
			{
				return ReferenceEquals(this, other);
			}
		}
	}
}