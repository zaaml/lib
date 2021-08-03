// <copyright file="Parser.Automata.ParserPredicateEntry.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.Text
{
	internal abstract partial class Parser<TGrammar, TToken>
	{
		private sealed partial class ParserAutomata
		{
			private sealed class ParserPredicateEntry : PredicateEntry, IParserEntry, IParserPredicate
			{
				public ParserPredicateEntry(Grammar<TToken>.ParserPredicate grammarEntry) : base(CreatePredicateDelegate(grammarEntry.PredicateEntry))
				{
					GrammarEntry = grammarEntry;
				}

				public ParserPredicateEntry(Grammar<TToken>.ParserEntry grammarEntry, Func<AutomataContext, PredicateResult> predicate) : base(predicate)
				{
					GrammarEntry = grammarEntry;
				}

				public Grammar<TToken>.ParserEntry GrammarEntry { get; }

				public ProductionArgument ProductionArgument { get; set; }

				public ParserPredicateKind PredicateKind => ParserPredicateKind.Generic;
			}

			private sealed class ParserPredicateEntry<TResult> : PredicateEntry<TResult>, IParserEntry, IParserPredicate
			{
				public ParserPredicateEntry(Grammar<TToken>.ParserEntry grammarEntry, Func<AutomataContext, PredicateResult<TResult>> predicate, ParserPredicateKind predicateKind) : base(predicate)
				{
					GrammarEntry = grammarEntry;
					PredicateKind = predicateKind;
				}

				public Grammar<TToken>.ParserEntry GrammarEntry { get; }

				public ProductionArgument ProductionArgument { get; set; }

				public ParserPredicateKind PredicateKind { get; }
			}
		}
	}
}