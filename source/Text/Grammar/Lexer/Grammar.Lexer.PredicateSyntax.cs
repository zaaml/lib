// <copyright file="Grammar.Lexer.PredicateSyntax.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Runtime.CompilerServices;

namespace Zaaml.Text
{
	internal abstract partial class Grammar<TGrammar, TToken>
	{
		public partial class LexerGrammar
		{
			protected internal sealed class PredicateSyntax : Syntax
			{
				public PredicateSyntax([CallerMemberName] string name = null) : base(name)
				{
				}

				public Lexer<TToken>.PredicateEntry PredicateEntry { get; private set; }

				public void Bind<TLexer>(Func<TLexer, Lexer<TGrammar, TToken>.LexerPredicate> predicateFactory) where TLexer : Lexer<TGrammar, TToken>
				{
					PredicateEntry = new Lexer<TToken>.PredicateEntry(l =>
					{
						var predicate = predicateFactory((TLexer)l);

						return predicate();
					});
				}
			}
		}
	}
}