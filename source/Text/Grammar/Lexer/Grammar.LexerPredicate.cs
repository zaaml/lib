// <copyright file="Grammar.LexerPredicate.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal partial class Grammar<TToken>
	{
		protected internal sealed partial class LexerPredicate : TokenEntry
		{
			public LexerPredicate(Lexer<TToken>.PredicateEntry predicateEntry)
			{
				PredicateEntry = predicateEntry;
			}

			public Lexer<TToken>.PredicateEntry PredicateEntry { get; }
		}
	}
}