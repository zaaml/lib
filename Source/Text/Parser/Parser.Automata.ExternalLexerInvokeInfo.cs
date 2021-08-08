// <copyright file="Parser.Automata.ExternalLexerInvokeInfo.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
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
			private sealed class ExternalLexerInvokeInfo<TExternalToken> where TExternalToken : unmanaged, Enum
			{
				public readonly Lexer<TExternalToken> Lexer;
				public readonly Grammar<TExternalToken>.TokenRule Rule;

				public ExternalLexerInvokeInfo(Grammar<TToken>.ExternalLexerEntry<TExternalToken> externalGrammarEntry)
				{
					Rule = externalGrammarEntry.ExternalLexerRule;
					Lexer = (Lexer<TExternalToken>)Rule.Grammar.GetType().GetProperties().SingleOrDefault(p => typeof(Lexer<TExternalToken>).IsAssignableFrom(p.PropertyType))?.GetValue(null);

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