// <copyright file="Parser.Automata.ExternalLexerInvokeInfo.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Linq;

namespace Zaaml.Text
{
	internal partial class Parser<TGrammar, TToken>
	{
		private sealed partial class ParserAutomata
		{
			private sealed class ExternalLexerInvokeInfo<TEternalGrammar, TExternalToken>
				where TEternalGrammar : Grammar<TEternalGrammar, TExternalToken>
				where TExternalToken : unmanaged, Enum 
			{
				public readonly Lexer<TEternalGrammar, TExternalToken> Lexer;
				public readonly Grammar<TEternalGrammar, TExternalToken>.LexerGrammar.TokenSyntax Token;

				public ExternalLexerInvokeInfo(Grammar<TGrammar, TToken>.ParserGrammar.ExternalTokenSymbol<TEternalGrammar, TExternalToken> externalGrammarEntry)
				{
					Token = externalGrammarEntry.ExternalToken;
					Lexer = (Lexer<TEternalGrammar, TExternalToken>)(typeof(TEternalGrammar).GetProperties().SingleOrDefault(p => typeof(Lexer<TEternalGrammar, TExternalToken>).IsAssignableFrom(p.PropertyType))?.GetValue(null));

					if (Lexer == null)
						throw new InvalidOperationException("Lexer instance is null.");
				}

				public PredicateResult<Lexeme<TExternalToken>> ExternalLex(AutomataContext automataContext)
				{
					var parserContext = (ParserAutomataContext)automataContext;

					return parserContext.CallExternalLexer(this, out var parseResult) ? new LocalPredicateResult<Lexeme<TExternalToken>>(parseResult) : null;
				}
			}
		}
	}
}