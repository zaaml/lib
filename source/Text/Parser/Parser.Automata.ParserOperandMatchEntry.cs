// <copyright file="Parser.Automata.ParserOperandMatchEntry.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Linq;

namespace Zaaml.Text
{
	internal partial class Parser<TGrammar, TToken>
	{
		private sealed partial class ParserAutomata
		{
			private sealed class ParserOperandMatchEntry : OperandMatchEntry, IParserEntry
			{
				public ParserOperandMatchEntry(Grammar<TGrammar, TToken>.ParserGrammar.TokenSymbol grammarEntry)
					: base(grammarEntry.Token.TokenGroups.Single().Token)
				{
					GrammarSymbol = grammarEntry;
				}

				public Grammar<TGrammar, TToken>.ParserGrammar.Symbol GrammarSymbol { get; }

				public ProductionArgument ProductionArgument { get; set; }

				public Entry Clone()
				{
					return new ParserOperandMatchEntry((Grammar<TGrammar, TToken>.ParserGrammar.TokenSymbol)GrammarSymbol)
					{
						Source = this
					};
				}

				public IParserEntry Source { get; private set; }
			}
		}
	}
}