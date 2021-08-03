// <copyright file="Parser.Automata.LeftRecursion.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable UseIndexFromEndExpression

namespace Zaaml.Text
{
	internal abstract partial class Parser<TGrammar, TToken> where TGrammar : Grammar<TToken> where TToken : unmanaged, Enum
	{
		private sealed partial class ParserAutomata
		{
			private static LeftRecursionClassifier ClassifyLeftRecursion(ParserProduction production, ParserRule parserRule)
			{
				if (production.Entries.Length == 0)
					return LeftRecursionClassifier.Primary;

				var firstRecursion = IsRecursion(production.Entries[0], parserRule);
				var lastRecursion = IsRecursion(production.Entries[production.Entries.Length - 1], parserRule);

				if (production.Entries.Length == 1)
					return firstRecursion ? LeftRecursionClassifier.Generic : LeftRecursionClassifier.Primary;

				if (production.Entries.Length == 3 && IsRecursion(production.Entries[1], parserRule) == false && firstRecursion && lastRecursion)
					return LeftRecursionClassifier.Binary;

				if (production.Entries.Length > 3 && firstRecursion && lastRecursion)
					return LeftRecursionClassifier.Ternary;

				if (firstRecursion && lastRecursion == false)
					return LeftRecursionClassifier.Suffix;

				if (firstRecursion == false && lastRecursion)
					return LeftRecursionClassifier.Prefix;

				return firstRecursion ? LeftRecursionClassifier.Generic : LeftRecursionClassifier.Primary;
			}

			private void EliminateLeftRecursion(ParserRule parserRule, List<ParserProduction> productions)
			{
				var hasLeftRecursion = false;

				foreach (var stateProduction in productions)
				{
					stateProduction.LeftRecursionClassifier = ClassifyLeftRecursion(stateProduction, parserRule);

					hasLeftRecursion |= IsLeftRecursion(stateProduction.LeftRecursionClassifier);
				}

				if (hasLeftRecursion == false)
					return;

				var count = productions.Count;
				var recursionState = new ParserRule(null, true);
				var index = 0;

				foreach (var production in productions)
				{
					if (production.LeftRecursionClassifier is LeftRecursionClassifier.Binary or LeftRecursionClassifier.Ternary or LeftRecursionClassifier.Prefix)
					{
						var assoc = production.IsRightAssoc ? 0 : 1;
						var nextPriority = new PriorityRuleEntryContext(count - index + assoc);

						foreach (var parserStateEntry in production.Entries.Skip(1).OfType<ParserRuleEntry>().Where(e => ReferenceEquals(e.Rule, parserRule)))
							parserStateEntry.RuleEntryContext = nextPriority;
					}

					if (IsLeftRecursion(production.LeftRecursionClassifier) == false)
						continue;

					production.LeftRecursionEntry = (ParserRuleEntry) production.Entries[0];
					production.Entries[0] = ShouldPrefixLeftRecursionPredicate(production.LeftRecursionClassifier) ? new LeftRecursionPredicate(parserRule, count - index).Entry : EpsilonEntry.Instance;

					recursionState.Productions.Add(production);

					index++;
				}

				var quantifierEntry = new QuantifierEntry(new RuleEntry(recursionState) {SkipStack = true}, QuantifierKind.ZeroOrMore, QuantifierMode.Greedy);
				var primaryState = new ParserRule(null, true);

				primaryState.Productions.AddRange(productions.Where(production => IsLeftRecursion(production.LeftRecursionClassifier) == false));

				productions.Clear();
				productions.Add(new ParserProduction(this, null, new Entry[] {new RuleEntry(primaryState) {SkipStack = true}, quantifierEntry}));
			}

			private static bool IsLeftRecursion(LeftRecursionClassifier classifier)
			{
				return classifier is LeftRecursionClassifier.Primary or LeftRecursionClassifier.Prefix == false;
			}

			private static bool IsRecursion(Entry entry, ParserRule rule)
			{
				return entry is ParserRuleEntry parserStateEntry && ReferenceEquals(parserStateEntry.Rule, rule);
			}

			private static bool ShouldPrefixLeftRecursionPredicate(LeftRecursionClassifier classifier)
			{
				return classifier is LeftRecursionClassifier.Binary or LeftRecursionClassifier.Ternary or LeftRecursionClassifier.Suffix;
			}
		}
	}
}
