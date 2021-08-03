// <copyright file="Parser.Automata.ParserSingleMatchEntry.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal abstract partial class Parser<TGrammar, TToken>
	{
		private sealed partial class ParserAutomata
		{
			private sealed class ParserSingleMatchEntry : SingleMatchEntry, IParserEntry
			{
				public ParserSingleMatchEntry(Grammar<TToken>.ParserEntry grammarEntry, TToken operand) : base(operand)
				{
					GrammarEntry = grammarEntry;
				}

				public Grammar<TToken>.ParserEntry GrammarEntry { get; }

				public ProductionArgument ProductionArgument { get; set; }
			}
		}
	}
}