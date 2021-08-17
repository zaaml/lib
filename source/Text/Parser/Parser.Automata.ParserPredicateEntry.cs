// <copyright file="Parser.Automata.ParserPredicateEntry.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.Text
{
	internal partial class Parser<TGrammar, TToken>
	{
		private sealed partial class ParserAutomata
		{
			private sealed class ParserPredicateEntry : PredicateEntry, IParserEntry, IParserPredicate
			{
				public ParserPredicateEntry(Grammar<TGrammar, TToken>.ParserGrammar.PredicateSymbol grammarEntry) : base(CreatePredicateDelegate(grammarEntry.Predicate))
				{
					GrammarEntry = grammarEntry;
				}

				public ParserPredicateEntry(Grammar<TGrammar, TToken>.ParserGrammar.Symbol grammarEntry, Func<AutomataContext, PredicateResult> predicate) : base(predicate)
				{
					GrammarEntry = grammarEntry;
				}

				public Grammar<TGrammar, TToken>.ParserGrammar.Symbol GrammarEntry { get; }

				public ProductionArgument ProductionArgument { get; set; }
				
				public Entry Clone()
				{
					return new ParserPredicateEntry(GrammarEntry, Predicate)
					{
						Source = this
					};
				}

				public ParserPredicateKind PredicateKind => ParserPredicateKind.Generic;

				public Type ResultType => null;

				public IParserEntry Source { get; private set; }
			}

			private sealed class ParserPredicateEntry<TResult> : PredicateEntry<TResult>, IParserEntry, IParserPredicate
			{
				public ParserPredicateEntry(Grammar<TGrammar, TToken>.ParserGrammar.Symbol grammarEntry, Func<AutomataContext, PredicateResult<TResult>> predicate, ParserPredicateKind predicateKind) : base(predicate)
				{
					GrammarEntry = grammarEntry;
					PredicateKind = predicateKind;
				}

				public Grammar<TGrammar, TToken>.ParserGrammar.Symbol GrammarEntry { get; }

				public ProductionArgument ProductionArgument { get; set; }
				
				public Entry Clone()
				{
					return new ParserPredicateEntry<TResult>(GrammarEntry, Predicate, PredicateKind)
					{
						Source = this
					};
				}

				public IParserEntry Source { get; private set; }

				public ParserPredicateKind PredicateKind { get; }

				public Type ResultType => typeof(TResult);
			}
		}
	}
}