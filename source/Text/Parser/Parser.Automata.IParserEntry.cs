// <copyright file="Parser.Automata.IParserEntry.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal partial class Parser<TGrammar, TToken>
	{
		private sealed partial class ParserAutomata
		{
			private interface IParserEntry
			{
				Grammar<TGrammar, TToken>.ParserGrammar.Symbol GrammarSymbol { get; }

				ProductionArgument ProductionArgument { get; set; }

				IParserEntry Source { get; }

				Entry Clone();
			}
		}
	}
}