// <copyright file="Parser.Automata.ParserActionEntry.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal partial class Parser<TGrammar, TToken>
	{
		private sealed partial class ParserAutomata
		{
			private sealed class ParserActionEntry : ActionEntry, IParserEntry
			{
				public ParserActionEntry(Grammar<TGrammar, TToken>.ParserGrammar.ActionSymbol grammarEntry) : base(CreateActionDelegate(grammarEntry.ActionEntry))
				{
					GrammarSymbol = grammarEntry;
				}

				public Grammar<TGrammar, TToken>.ParserGrammar.Symbol GrammarSymbol { get; }

				public ProductionArgument ProductionArgument { get; set; }

				public IParserEntry Source { get; private set; }

				public Entry Clone()
				{
					return new ParserActionEntry((Grammar<TGrammar, TToken>.ParserGrammar.ActionSymbol)GrammarSymbol)
					{
						Source = this
					};
				}
			}
		}
	}
}