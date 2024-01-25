// <copyright file="Grammar.Parser.SyntaxFactory.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.Text
{
	internal abstract partial class Grammar<TGrammar, TToken>
	{
		public partial class ParserGrammar
		{
			protected static class SyntaxFactory
			{
				public static ExternalNodeSymbol<TExternalGrammar, TExternalToken, TExternalNode> External<TExternalGrammar, TExternalToken, TExternalNode>(Grammar<TExternalGrammar, TExternalToken>.ParserGrammar.NodeSyntax<TExternalNode> externalParserRule)
					where TExternalGrammar : Grammar<TExternalGrammar, TExternalToken>
					where TExternalToken : unmanaged, Enum
					where TExternalNode : class
				{
					return new ExternalNodeSymbol<TExternalGrammar, TExternalToken, TExternalNode>(externalParserRule);
				}

				public static ExternalNodeSymbol<TExternalGrammar, TExternalToken> External<TExternalGrammar, TExternalToken>(Grammar<TExternalGrammar, TExternalToken>.ParserGrammar.NodeSyntax externalParserRule)
					where TExternalGrammar : Grammar<TExternalGrammar, TExternalToken>
					where TExternalToken : unmanaged, Enum
				{
					return new ExternalNodeSymbol<TExternalGrammar, TExternalToken>(externalParserRule);
				}

				public static FragmentSyntax Fragment()
				{
					return new FragmentSyntax("Internal", true);
				}

				public static FragmentSyntax Fragment(Production production)
				{
					var parserSyntaxFragment = new FragmentSyntax("Internal", true);

					parserSyntaxFragment.AddProduction(production);

					return parserSyntaxFragment;
				}

				public static FragmentSyntax Fragment(Production production1, Production production2)
				{
					var parserSyntaxFragment = new FragmentSyntax("Internal", true);

					parserSyntaxFragment.AddProduction(production1);
					parserSyntaxFragment.AddProduction(production2);

					return parserSyntaxFragment;
				}

				public static FragmentSyntax Fragment(Production production1, Production production2, Production production3)
				{
					var parserSyntaxFragment = new FragmentSyntax("Internal", true);

					parserSyntaxFragment.AddProduction(production1);
					parserSyntaxFragment.AddProduction(production2);
					parserSyntaxFragment.AddProduction(production3);

					return parserSyntaxFragment;
				}

				public static FragmentSyntax Fragment(Production production1, Production production2, Production production3, Production production4)
				{
					var parserSyntaxFragment = new FragmentSyntax("Internal", true);

					parserSyntaxFragment.AddProduction(production1);
					parserSyntaxFragment.AddProduction(production2);
					parserSyntaxFragment.AddProduction(production3);
					parserSyntaxFragment.AddProduction(production4);

					return parserSyntaxFragment;
				}

				public static FragmentSyntax Fragment(params Production[] productions)
				{
					var parserSyntaxFragment = new FragmentSyntax("Internal", true);

					foreach (var production in productions) 
						parserSyntaxFragment.AddProduction(production);

					return parserSyntaxFragment;
				}

				public static Production Production(params Symbol[] parserEntries)
				{
					return new Production(parserEntries);
				}

				public static TokenSymbol Token(LexerGrammar.TokenSyntax token)
				{
					return new TokenSymbol(token);
				}

				public static TokenSetSymbol TokenSet(params LexerGrammar.TokenSyntax[] tokens)
				{
					return new TokenSetSymbol(tokens);
				}

				public static ActionSymbol Action(ActionSyntax actionSyntax)
				{
					return new ActionSymbol(actionSyntax);
				}

				public static PredicateSymbol Predicate(PredicateSyntax predicateSyntax)
				{
					return new PredicateSymbol(predicateSyntax);
				}
			}
		}
	}
}