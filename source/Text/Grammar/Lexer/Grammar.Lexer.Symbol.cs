// <copyright file="Grammar.Lexer.Symbol.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal abstract partial class Grammar<TGrammar, TToken>
	{
		public partial class LexerGrammar
		{
			protected internal abstract class Symbol : GrammarSymbol<Syntax, Production, Symbol>
			{
				public static implicit operator Symbol(FragmentSyntax fragment)
				{
					return new FragmentSymbol(fragment);
				}

				public static implicit operator Symbol(TokenSyntax token)
				{
					return new TokenSymbol(token);
				}

				public static implicit operator Symbol(PredicateSyntax predicateSyntax)
				{
					return new PredicateSymbol(predicateSyntax);
				}
			}
		}
	}
}