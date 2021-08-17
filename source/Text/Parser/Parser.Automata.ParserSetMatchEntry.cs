﻿// <copyright file="Parser.Automata.ParserSetMatchEntry.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Zaaml.Text
{
	internal partial class Parser<TGrammar, TToken>
	{
		private sealed partial class ParserAutomata
		{
			private sealed class ParserSetMatchEntry : SetMatchEntry, IParserEntry
			{
				public ParserSetMatchEntry(Grammar<TGrammar, TToken>.ParserGrammar.TokenSymbol grammarEntry)
					: base(CreateMatches(grammarEntry.Token))
				{
					GrammarEntry = grammarEntry;
				}


				public ParserSetMatchEntry(Grammar<TGrammar, TToken>.ParserGrammar.TokenSetSymbol grammarEntry)
					: base(CreateMatches(grammarEntry.Tokens))
				{
					GrammarEntry = grammarEntry;
				}

				private static IEnumerable<PrimitiveMatchEntry> CreateMatches(Grammar<TGrammar, TToken>.LexerGrammar.TokenSyntax tokenSyntax)
				{
					foreach (var tokenGroup in tokenSyntax.TokenGroups)
						yield return new SingleMatchEntry(tokenGroup.Token);
				}

				private static IEnumerable<PrimitiveMatchEntry> CreateMatches(Grammar<TGrammar, TToken>.LexerGrammar.TokenSyntax[] tokenSyntaxCollection)
				{
					foreach (var tokenSyntax in tokenSyntaxCollection)
					foreach (var tokenGroup in tokenSyntax.TokenGroups)
						yield return new SingleMatchEntry(tokenGroup.Token);
				}

				public Grammar<TGrammar, TToken>.ParserGrammar.Symbol GrammarEntry { get; }

				public ProductionArgument ProductionArgument { get; set; }
				
				public Entry Clone()
				{
					if (GrammarEntry is Grammar<TGrammar, TToken>.ParserGrammar.TokenSymbol symbol)
						return new ParserSetMatchEntry(symbol)
						{
							Source = this
						};

					return new ParserSetMatchEntry((Grammar<TGrammar, TToken>.ParserGrammar.TokenSetSymbol)GrammarEntry)
					{
						Source = this
					};
				}

				public IParserEntry Source { get; private set; }
			}
		}
	}
}