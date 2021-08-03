// <copyright file="Parser.Automata.ParserActionEntry.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal abstract partial class Parser<TGrammar, TToken>
	{
		private sealed partial class ParserAutomata
		{
			private sealed class ParserActionEntry : ActionEntry, IParserEntry
			{
				public ParserActionEntry(Grammar<TToken>.ParserAction grammarEntry) : base(CreateActionDelegate(grammarEntry.ActionEntry))
				{
					GrammarEntry = grammarEntry;
				}

				public Grammar<TToken>.ParserEntry GrammarEntry { get; }

				public ProductionArgument ProductionArgument { get; set; }
			}
		}
	}
}