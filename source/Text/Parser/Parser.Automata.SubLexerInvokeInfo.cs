// <copyright file="Parser.Automata.SubLexerInvokeInfo.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Linq;

namespace Zaaml.Text
{
	internal abstract partial class Parser<TGrammar, TToken>
	{
		#region Nested Types

		private sealed partial class ParserAutomata
		{
			#region Nested Types

			private sealed class SubLexerInvokeInfo<TSubToken> where TSubToken : unmanaged, Enum
			{
				#region Fields

				public readonly Lexer<TSubToken> Lexer;
				public readonly Grammar<TSubToken>.TokenRule Rule;

				#endregion

				#region Ctors

				public SubLexerInvokeInfo(Grammar<TToken>.SubLexerEntry<TSubToken> lexerSubAutomataEntry)
				{
					Rule = lexerSubAutomataEntry.SubLexerRule;
					Lexer = (Lexer<TSubToken>) Rule.Grammar.GetType().GetProperties().SingleOrDefault(p => typeof(Lexer<TSubToken>).IsAssignableFrom(p.PropertyType))?.GetValue(null);

					if (Lexer == null)
						throw new InvalidOperationException("Lexer instance is null.");
				}

				#endregion

				#region Methods

				public PredicateResult<Lexeme<TSubToken>> SubLex(AutomataContext automataContext)
				{
					var parserContext = (ParserAutomataContext) automataContext;

					return parserContext.CallSubLexer(this, out var parseResult) ? new LocalPredicateResult<Lexeme<TSubToken>>(parseResult) : null;
				}

				#endregion
			}

			#endregion
		}

		#endregion
	}
}