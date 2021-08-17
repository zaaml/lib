// <copyright file="Parser.Automata.ParserRuleEntry.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Linq;

namespace Zaaml.Text
{
	internal partial class Parser<TGrammar, TToken>
	{
		private sealed partial class ParserAutomata
		{
			private sealed class ParserRuleEntry : RuleEntry, IParserEntry
			{
				public ParserRuleEntry(Grammar<TGrammar, TToken>.ParserGrammar.NodeSymbol grammarEntry, Rule rule) : base(rule)
				{
					GrammarEntry = grammarEntry;
					Fragment = false;
				}

				public ParserRuleEntry(Grammar<TGrammar, TToken>.ParserGrammar.FragmentSymbol grammarEntry, Rule rule) : base(rule)
				{
					GrammarEntry = grammarEntry;
					Fragment = true;
				}

				public bool Fragment { get; }

				public Grammar<TGrammar, TToken>.ParserGrammar.Symbol GrammarEntry { get; }

				public ProductionArgument ProductionArgument { get; set; }
				
				public Entry Clone()
				{
					if (Fragment)
						return new ParserRuleEntry((Grammar<TGrammar, TToken>.ParserGrammar.FragmentSymbol)GrammarEntry, Rule)
						{
							Source = this
						};

					return new ParserRuleEntry((Grammar<TGrammar, TToken>.ParserGrammar.NodeSymbol)GrammarEntry, Rule)
					{
						Source = this
					};
				}

				public IParserEntry Source { get; private set; }

				protected override string DebuggerDisplay
				{
					get
					{
						if (Fragment)
							return $"({string.Join(" | ", Rule.Productions.Cast<ParserProduction>().Select(p => p.EntriesDebuggerDisplay))})";

						return base.DebuggerDisplay;
					}
				}
			}
		}
	}
}