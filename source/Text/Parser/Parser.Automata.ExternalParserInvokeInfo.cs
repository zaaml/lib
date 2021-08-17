// <copyright file="Parser.Automata.ExternalParserInvokeInfo.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Linq;
using Zaaml.Core.Reflection;

namespace Zaaml.Text
{
	internal partial class Parser<TGrammar, TToken>
	{
		private sealed partial class ParserAutomata
		{
			private class ExternalParserInvokeInfo<TExternalGrammar, TExternalToken> 
				where TExternalGrammar : Grammar<TExternalGrammar, TExternalToken> where TExternalToken : unmanaged, Enum
			{
				public readonly Lexer<TExternalToken> Lexer;
				public readonly Parser<TExternalGrammar, TExternalToken> Parser;
				public readonly ParserPredicateEntry PredicateEntry;
				public readonly Grammar<TExternalGrammar, TExternalToken>.ParserGrammar.NodeSyntax SyntaxNode;

				public ExternalParserInvokeInfo(Grammar<TGrammar, TToken>.ParserGrammar.ExternalNodeSymbol<TExternalGrammar, TExternalToken> parserSubAutomataEntry)
				{
					SyntaxNode = parserSubAutomataEntry.ExternalParserNode;

					var grammarType = typeof(TGrammar);
					Parser = (Parser<TExternalGrammar, TExternalToken>)parserSubAutomataEntry.ExternalGrammarType.GetProperties(BF.SPNP).SingleOrDefault(p => typeof(Parser<TExternalGrammar, TExternalToken>).IsAssignableFrom(p.PropertyType))?.GetValue(null);
					Lexer = (Lexer<TExternalGrammar, TExternalToken>)parserSubAutomataEntry.ExternalGrammarType.GetProperties(BF.SPNP).SingleOrDefault(p => typeof(Lexer<TExternalGrammar, TExternalToken>).IsAssignableFrom(p.PropertyType))?.GetValue(null);

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

			private class ExternalParserInvokeInfo<TExternalGrammar, TExternalToken, TExternalNode> 
				where TExternalGrammar : Grammar<TExternalGrammar, TExternalToken>
				where TExternalToken : unmanaged, Enum
				where TExternalNode : class
			{
				public readonly Lexer<TExternalToken> Lexer;
				public readonly Parser<TExternalGrammar, TExternalToken> Parser;
				public readonly ParserPredicateEntry<TExternalNode> PredicateEntry;
				public readonly Grammar<TExternalGrammar, TExternalToken>.ParserGrammar.NodeSyntax SyntaxNode;

				public ExternalParserInvokeInfo(Grammar<TGrammar, TToken>.ParserGrammar.ExternalNodeSymbol<TExternalGrammar, TExternalToken, TExternalNode> externalGrammarEntry)
				{
					SyntaxNode = externalGrammarEntry.ExternalParserNode;
					Parser = (Parser<TExternalGrammar, TExternalToken>)SyntaxNode.Grammar.GetType().GetProperties(BF.SPNP).SingleOrDefault(p => typeof(Parser<TExternalGrammar, TExternalToken>).IsAssignableFrom(p.PropertyType))?.GetValue(null);
					Lexer = (Lexer<TExternalToken>)SyntaxNode.Grammar.GetType().GetProperties(BF.SPNP).SingleOrDefault(p => typeof(Lexer<TExternalToken>).IsAssignableFrom(p.PropertyType))?.GetValue(null);

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