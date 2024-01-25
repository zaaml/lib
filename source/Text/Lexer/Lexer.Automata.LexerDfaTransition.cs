// <copyright file="Automata.DfaTransition.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.Text
{
	internal partial class Lexer<TGrammar, TToken>
	{
		private protected partial class LexerAutomata
		{
			private sealed class LexerDfaTransition : IEquatable<LexerDfaTransition>
			{
				public readonly int LazyIndex;
				public readonly SubGraph SubGraph;

				public LexerDfaTransition(SubGraph subGraph, int lazyIndex)
				{
					SubGraph = subGraph;
					LazyIndex = lazyIndex;
				}

				public bool Equals(LexerDfaTransition transition)
				{
					return ReferenceEquals(SubGraph, transition.SubGraph);
				}

				public override string ToString()
				{
					return SubGraph.ToString();
				}
			}
		}
	}
}