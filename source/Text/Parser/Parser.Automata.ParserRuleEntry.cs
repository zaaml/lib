// <copyright file="Parser.Automata.ParserRuleEntry.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal abstract partial class Parser<TGrammar, TToken>
	{
		private sealed partial class ParserAutomata
		{
			private sealed class ParserRuleEntry : RuleEntry, IParserEntry
			{
				public ParserRuleEntry(Grammar<TToken>.ParserEntry grammarEntry, Rule rule, bool fragment, bool tryReturn) : base(rule)
				{
					GrammarEntry = grammarEntry;
					Fragment = fragment;
					TryReturn = tryReturn;
				}

				public bool Fragment { get; }

				public bool TryReturn { get; }

				public Grammar<TToken>.ParserEntry GrammarEntry { get; }

				public ProductionArgument ProductionArgument { get; set; }
			}
		}
	}
}