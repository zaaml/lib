// <copyright file="Parser.Automata.ExternalParserDelegate.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.Text
{
	internal partial class Parser<TGrammar, TToken>
	{
		private sealed partial class ParserAutomata
		{
			private sealed class ExternalParserDelegate<TExternalGrammar, TExternalToken, TExternalNode>
				where TExternalGrammar : Grammar<TExternalGrammar, TExternalToken>
				where TExternalToken : unmanaged, Enum
				where TExternalNode : class
			{
				public ExternalParserDelegate(Grammar<TGrammar, TToken>.ParserGrammar.ExternalNodeSymbol<TExternalGrammar, TExternalToken, TExternalNode> symbol)
				{
					Symbol = symbol;
					PredicateEntry = new ParserPredicateEntry<TExternalNode>(Symbol, Parse, ParserPredicateKind.ExternalParser);
				}

				public ParserPredicateEntry<TExternalNode> PredicateEntry { get; }

				public Grammar<TGrammar, TToken>.ParserGrammar.ExternalNodeSymbol<TExternalGrammar, TExternalToken, TExternalNode> Symbol { get; }

				private PredicateResult<TExternalNode> Parse(AutomataContext automataContext)
				{
					return ((ParserAutomataContext)automataContext).CallValueExternalParser(this);
				}
			}

			private sealed class ExternalParserDelegate<TExternalGrammar, TExternalToken>
				where TExternalGrammar : Grammar<TExternalGrammar, TExternalToken>
				where TExternalToken : unmanaged, Enum
			{
				public ExternalParserDelegate(Grammar<TGrammar, TToken>.ParserGrammar.ExternalNodeSymbol<TExternalGrammar, TExternalToken> symbol)
				{
					Symbol = symbol;
					PredicateEntry = new ParserPredicateEntry(Symbol, Parse);
				}

				public ParserPredicateEntry PredicateEntry { get; }

				public Grammar<TGrammar, TToken>.ParserGrammar.ExternalNodeSymbol<TExternalGrammar, TExternalToken> Symbol { get; }

				private PredicateResult Parse(AutomataContext automataContext)
				{
					var parserContext = (ParserAutomataContext)automataContext;

					return parserContext.CallExternalParser(this);
				}
			}
		}
	}
}