// <copyright file="Automata.DfaNode.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.Text
{
	internal partial class Lexer<TGrammar, TToken>
	{
		private protected partial class LexerAutomata
		{
			private sealed class LexerDfaNode : IEquatable<LexerDfaNode>
			{
				public readonly ExecutionPath ExecutionPath;
				public readonly Node Node;
				public readonly LexerDfaTransition Transition;

				public LexerDfaNode(Node node, LexerDfaTransition transition)
				{
					Node = node;
					Transition = transition;
				}

				public LexerDfaNode(Node node, LexerDfaTransition transition, ExecutionPath executionPath)
				{
					Node = node;
					Transition = transition;
					ExecutionPath = executionPath;
				}

				public bool Equals(LexerDfaNode other)
				{
					if (ReferenceEquals(null, other)) return false;
					if (ReferenceEquals(this, other)) return true;

					return Equals(Node, other.Node) && Equals(Transition, other.Transition) && Equals(ExecutionPath, other.ExecutionPath);
				}

				public override bool Equals(object obj)
				{
					return ReferenceEquals(this, obj) || obj is LexerDfaNode other && Equals(other);
				}

				public override int GetHashCode()
				{
#if NETCOREAPP
					return HashCode.Combine(Node, Transition, ExecutionPath);
#else
				unchecked
				{
					int hash = 17;

					hash = hash * 31 + Node.GetHashCode();
					hash = hash * 31 + Transition.GetHashCode();

					if (ExecutionPath != null)
						hash = hash * 31 + ExecutionPath.GetHashCode();

					return hash;
				}
#endif
				}

				public override string ToString()
				{
					return Node.ToString();
				}
			}
		}
	}
}