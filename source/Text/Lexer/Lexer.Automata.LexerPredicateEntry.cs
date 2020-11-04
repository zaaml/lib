// <copyright file="Lexer.Automata.LexerPredicateEntry.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal partial class Lexer<TGrammar, TToken>
	{
		private protected partial class LexerAutomata
		{
			private sealed class LexerPredicateEntry : PredicateEntry
			{
				public LexerPredicateEntry(Grammar<TToken>.LexerPredicate grammarEntry) : base(CreatePredicateDelegate(grammarEntry.PredicateEntry))
				{
				}
			}
		}
	}
}