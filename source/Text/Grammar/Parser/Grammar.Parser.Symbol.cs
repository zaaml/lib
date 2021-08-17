// <copyright file="Grammar.Parser.Symbol.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal abstract partial class Grammar<TGrammar, TToken>
	{
		public partial class ParserGrammar
		{
			protected internal abstract class Symbol : GrammarSymbol<Syntax, Production, Symbol>
			{
				public static implicit operator Symbol(NodeSyntax node)
				{
					return new NodeSymbol(node);
				}

				public static implicit operator Symbol(FragmentSyntax fragment)
				{
					return new FragmentSymbol(fragment);
				}

				public static implicit operator Symbol(LexerGrammar.TokenSyntax tokenSyntax)
				{
					return new TokenSymbol(tokenSyntax);
				}

				public static implicit operator Symbol(Parser<TToken>.PredicateEntry parserPredicateEntry)
				{
					return new PredicateSymbol(parserPredicateEntry);
				}

				public static implicit operator Symbol(Parser<TToken>.ActionEntry parserActionEntry)
				{
					return new ActionSymbol(parserActionEntry);
				}
			}
		}
	}
}