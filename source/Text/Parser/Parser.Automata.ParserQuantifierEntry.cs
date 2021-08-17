// <copyright file="Parser.Automata.ParserQuantifierEntry.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.Core;

namespace Zaaml.Text
{
	internal partial class Parser<TGrammar, TToken>
	{
		private sealed partial class ParserAutomata
		{
			private sealed class ParserQuantifierEntry : QuantifierEntry, IParserEntry
			{
				public ParserQuantifierEntry(Grammar<TGrammar, TToken>.ParserGrammar.QuantifierSymbol grammarEntry, PrimitiveEntry primitiveEntry, Interval<int> range, QuantifierMode mode)
					: base(primitiveEntry, range, mode)
				{
					GrammarEntry = grammarEntry;
				}

				public Grammar<TGrammar, TToken>.ParserGrammar.Symbol GrammarEntry { get; }

				public ProductionArgument ProductionArgument { get; set; }
				
				public Entry Clone()
				{
					var primitiveEntryClone = PrimitiveEntry is IParserEntry pe ? (PrimitiveEntry)pe.Clone() : PrimitiveEntry;

					return new ParserQuantifierEntry((Grammar<TGrammar, TToken>.ParserGrammar.QuantifierSymbol)GrammarEntry, primitiveEntryClone, Interval, Mode)
					{
						Source = this
					};
				}

				public IParserEntry Source { get; private set; }
			}
		}
	}
}