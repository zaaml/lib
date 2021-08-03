// <copyright file="Parser.Automata.ParserQuantifierEntry.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.Core;

namespace Zaaml.Text
{
	internal abstract partial class Parser<TGrammar, TToken>
	{
		private sealed partial class ParserAutomata
		{
			private sealed class ParserQuantifierEntry : QuantifierEntry, IParserEntry
			{
				public ParserQuantifierEntry(Grammar<TToken>.ParserQuantifierEntry grammarEntry, PrimitiveEntry primitiveEntry, Interval<int> range, QuantifierMode mode) : base(primitiveEntry, range, mode)
				{
					GrammarEntry = grammarEntry;
				}

				public Grammar<TToken>.ParserEntry GrammarEntry { get; }

				public ProductionArgument ProductionArgument { get; set; }
			}
		}
	}
}