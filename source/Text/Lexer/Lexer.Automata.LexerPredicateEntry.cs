// <copyright file="Lexer.Automata.LexerPredicateEntry.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>


using System;

namespace Zaaml.Text
{
	internal partial class Lexer<TGrammar, TToken>
	{
		private protected partial class LexerAutomata
		{
			private sealed class LexerPredicateEntry : PredicateEntry
			{
				public LexerPredicateEntry(Grammar<TGrammar, TToken>.LexerGrammar.PredicateSymbol symbol) 
					: base(CreatePredicateDelegate(symbol.PredicateEntry))
				{
				}
			}

			private sealed class LexerPredicateEntry<TResult> : PredicateEntry<TResult>
			{
				public LexerPredicateEntry(Grammar<TGrammar, TToken>.LexerGrammar.Symbol symbol, Func<AutomataContext, PredicateResult<TResult>> predicate) : base(predicate)
				{
				}
			}
		}
	}
}