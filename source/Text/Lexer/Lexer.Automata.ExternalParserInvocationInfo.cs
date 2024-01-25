// <copyright file="Lexer.Automata.LexerActionEntry.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.Text
{
	internal partial class Lexer<TGrammar, TToken>
	{
		private protected partial class LexerAutomata
		{
			private class ExternalParserDelegate<TExternalGrammar, TExternalToken, TExternalNode>
				where TExternalGrammar : Grammar<TExternalGrammar, TExternalToken>
				where TExternalToken : unmanaged, Enum
				where TExternalNode : class
			{
				public ExternalParserDelegate(Grammar<TGrammar, TToken>.LexerGrammar.ExternalNodeSymbol<TExternalGrammar, TExternalToken, TExternalNode> symbol)
				{
					Symbol = symbol;
					PredicateEntry = new LexerPredicateEntry<TExternalNode>(symbol, Parse);
				}

				public Grammar<TGrammar, TToken>.LexerGrammar.ExternalNodeSymbol<TExternalGrammar, TExternalToken, TExternalNode> Symbol { get; }

				public LexerPredicateEntry<TExternalNode> PredicateEntry { get; }

				private PredicateResult<TExternalNode> Parse(AutomataContext automataContext)
				{
					var lexerContext = (LexerAutomataContext)automataContext;
					var grammar = Symbol.ExternalParserNode.Grammar;
					var textSource = lexerContext.Text.Slice(lexerContext.Position);
					var nodeParser = new Parser<TExternalGrammar, TExternalToken>.SyntaxNodeParser<TExternalNode>(Symbol.ExternalParserNode, grammar.GetLexerFactory(), grammar.GetParserFactory());
					var result = nodeParser.ParseExternal(textSource, lexerContext.ServiceProvider);

					if (result is Parser<TExternalGrammar, TExternalToken>.SuccessExternalParseResult<TExternalNode> successResult)
					{
						lexerContext.Position += successResult.TextPosition;

						return new LocalPredicateResult<TExternalNode>(successResult.Value);
					}

					return null;
				}
			}
		}
	}
}