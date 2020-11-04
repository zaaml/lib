// <copyright file="Parser.Automata.ParserPredicateEntry.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.Text
{
	internal abstract partial class Parser<TGrammar, TToken>
	{
		#region Nested Types

		private sealed partial class ParserAutomata
		{
			#region Nested Types

			private sealed class ParserPredicateEntry : PredicateEntry, IParserEntry, IParserPredicate
			{
				#region Ctors

				public ParserPredicateEntry(Grammar<TToken>.ParserPredicate grammarEntry) : base(CreatePredicateDelegate(grammarEntry.PredicateEntry))
				{
					ParserEntryData = new ParserEntryData(EnsureName(grammarEntry), this);
				}

				public ParserPredicateEntry(Grammar<TToken>.ParserEntry grammarEntry, Func<AutomataContext, PredicateResult> predicate) : base(predicate)
				{
					ParserEntryData = new ParserEntryData(EnsureName(grammarEntry), this);
				}

				#endregion

				#region Interface Implementations

				#region Parser<TGrammar,TToken>.ParserAutomata.IParserEntry

				public ParserEntryData ParserEntryData { get; }

				#endregion

				#region Parser<TGrammar,TToken>.ParserAutomata.IParserPredicate

				public ParserPredicateKind PredicateKind => ParserPredicateKind.Generic;

				#endregion

				#endregion
			}

			private sealed class ParserPredicateEntry<TResult> : PredicateEntry<TResult>, IParserEntry, IParserPredicate
			{
				#region Ctors

				public ParserPredicateEntry(Grammar<TToken>.ParserEntry grammarEntry, Func<AutomataContext, PredicateResult<TResult>> predicate, ParserPredicateKind predicateKind) : base(predicate)
				{
					PredicateKind = predicateKind;
					ParserEntryData = new ParserEntryData(EnsureName(grammarEntry), this);
				}

				#endregion

				#region Interface Implementations

				#region Parser<TGrammar,TToken>.ParserAutomata.IParserEntry

				public ParserEntryData ParserEntryData { get; }

				#endregion

				#region Parser<TGrammar,TToken>.ParserAutomata.IParserPredicate

				public ParserPredicateKind PredicateKind { get; }

				#endregion

				#endregion
			}

			#endregion
		}

		#endregion
	}
}