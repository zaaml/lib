// <copyright file="Parser.Automata.ExternalParserInvokeInfo.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Linq;

namespace Zaaml.Text
{
	internal abstract partial class Parser<TGrammar, TToken>
	{
		private sealed partial class ParserAutomata
		{
			private class ExternalParserInvokeInfo<TExternalGrammar, TExternalToken> where TExternalGrammar : Grammar<TExternalToken> where TExternalToken : unmanaged, Enum
			{
				public readonly Lexer<TExternalToken> Lexer;
				public readonly Parser<TExternalGrammar, TExternalToken> Parser;
				public readonly ParserPredicateEntry PredicateEntry;
				public readonly Grammar<TExternalToken>.ParserRule Rule;

				public ExternalParserInvokeInfo(Grammar<TToken>.ExternalParserEntry<TExternalToken> parserSubAutomataEntry)
				{
					Rule = parserSubAutomataEntry.ExternalParserRule;
					Parser = (Parser<TExternalGrammar, TExternalToken>)Rule.Grammar.GetType().GetProperties().SingleOrDefault(p => typeof(Parser<TExternalGrammar, TExternalToken>).IsAssignableFrom(p.PropertyType))?.GetValue(null);
					Lexer = (Lexer<TExternalToken>)Rule.Grammar.GetType().GetProperties().SingleOrDefault(p => typeof(Lexer<TExternalToken>).IsAssignableFrom(p.PropertyType))?.GetValue(null);

					if (Parser == null)
						throw new InvalidOperationException("Parser instance is null.");

					if (Lexer == null)
						throw new InvalidOperationException("Lexer instance is null.");

					PredicateEntry = new ParserPredicateEntry(parserSubAutomataEntry, Parse);
				}

				private PredicateResult Parse(AutomataContext automataContext)
				{
					var parserContext = (ParserAutomataContext)automataContext;

					return parserContext.CallExternalParser(this);
				}
			}

			private class ExternalParserInvokeInfo<TExternalGrammar, TExternalToken, TExternalNode, TExternalNodeBase> where TExternalGrammar : Grammar<TExternalToken, TExternalNodeBase>
				where TExternalToken : unmanaged, Enum
				where TExternalNode : TExternalNodeBase
				where TExternalNodeBase : class
			{
				public readonly Lexer<TExternalToken> Lexer;
				public readonly Parser<TExternalGrammar, TExternalToken, TExternalNodeBase> Parser;
				public readonly ParserPredicateEntry<TExternalNode> PredicateEntry;
				public readonly Grammar<TExternalToken>.ParserRule Rule;

				public ExternalParserInvokeInfo(Grammar<TToken>.ExternalParserEntry<TExternalToken, TExternalNode, TExternalNodeBase> externalGrammarEntry)
				{
					Rule = externalGrammarEntry.ExternalParserRule;
					Parser = (Parser<TExternalGrammar, TExternalToken, TExternalNodeBase>)Rule.Grammar.GetType().GetProperties()
						.SingleOrDefault(p => typeof(Parser<TExternalGrammar, TExternalToken, TExternalNodeBase>).IsAssignableFrom(p.PropertyType))?.GetValue(null);

					Lexer = (Lexer<TExternalToken>)Rule.Grammar.GetType().GetProperties().SingleOrDefault(p => typeof(Lexer<TExternalToken>).IsAssignableFrom(p.PropertyType))?.GetValue(null);

					if (Parser == null)
						throw new InvalidOperationException("Parser instance is null.");

					if (Lexer == null)
						throw new InvalidOperationException("Lexer instance is null.");

					PredicateEntry = new ParserPredicateEntry<TExternalNode>(externalGrammarEntry, Parse, ParserPredicateKind.ExternalParser);
				}

				private PredicateResult<TExternalNode> Parse(AutomataContext automataContext)
				{
					var parserContext = (ParserAutomataContext)automataContext;

					return parserContext.CallValueExternalParser(this);
				}
			}
		}
	}
}