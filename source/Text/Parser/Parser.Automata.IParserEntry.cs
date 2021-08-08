// <copyright file="Parser.Automata.IParserEntry.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal abstract partial class Parser<TGrammar, TToken>
	{
		private sealed partial class ParserAutomata
		{
			private interface IParserEntry
			{
				Grammar<TToken>.ParserEntry GrammarEntry { get; }

				ProductionArgument ProductionArgument { get; set; }
			}
		}
	}
}