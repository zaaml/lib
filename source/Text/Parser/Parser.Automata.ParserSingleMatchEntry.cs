// <copyright file="Parser.Automata.ParserSingleMatchEntry.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Linq;

namespace Zaaml.Text
{
	internal partial class Parser<TGrammar, TToken>
	{
		private sealed partial class ParserAutomata
		{
			private sealed class ParserSingleMatchEntry : SingleMatchEntry, IParserEntry
			{
				public ParserSingleMatchEntry(Grammar<TGrammar, TToken>.ParserGrammar.TokenSymbol grammarEntry) 
					: base(grammarEntry.Token.TokenGroups.Single().Token)
				{
					GrammarEntry = grammarEntry;
				}

				public Grammar<TGrammar, TToken>.ParserGrammar.Symbol GrammarEntry { get; }

				public ProductionArgument ProductionArgument { get; set; }
				
				public Entry Clone()
				{
					return new ParserSingleMatchEntry((Grammar<TGrammar, TToken>.ParserGrammar.TokenSymbol)GrammarEntry)
					{
						Source = this
					};
				}

				public IParserEntry Source { get; private set; }
			}
		}
	}
}