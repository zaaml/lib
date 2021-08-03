// <copyright file="Parser.Automata.ParserSetMatchEntry.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Linq;

namespace Zaaml.Text
{
	internal abstract partial class Parser<TGrammar, TToken>
	{
		private sealed partial class ParserAutomata
		{
			private sealed class ParserSetMatchEntry : SetMatchEntry, IParserEntry
			{
				public ParserSetMatchEntry(Grammar<TToken>.TokenRuleSet grammarEntry) : base(grammarEntry.TokenRules.Select(CreateLexerEntry))
				{
					GrammarEntry = grammarEntry;
				}

				public Grammar<TToken>.ParserEntry GrammarEntry { get; }

				public ProductionArgument ProductionArgument { get; set; }
			}
		}
	}
}