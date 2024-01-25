// <copyright file="Grammar.Lexer.PredicateSymbol.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal abstract partial class Grammar<TGrammar, TToken>
	{
		public partial class LexerGrammar
		{
			protected internal sealed class PredicateSymbol : Symbol
			{
				public PredicateSyntax PredicateSyntax { get; }

				public PredicateSymbol(PredicateSyntax predicateSyntax)
				{
					PredicateSyntax = predicateSyntax;
				}

				public Lexer<TToken>.PredicateEntry PredicateEntry => PredicateSyntax.PredicateEntry;
			}
		}
	}
}