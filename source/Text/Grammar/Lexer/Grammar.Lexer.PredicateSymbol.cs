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
				public PredicateSymbol(Lexer<TToken>.PredicateEntry predicateEntry)
				{
					PredicateEntry = predicateEntry;
				}

				public Lexer<TToken>.PredicateEntry PredicateEntry { get; }
			}
		}
	}
}