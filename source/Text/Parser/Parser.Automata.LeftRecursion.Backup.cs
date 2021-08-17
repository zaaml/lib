// <copyright file="Parser.Automata.LeftRecursion.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

#if false

using System;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable UseIndexFromEndExpression

namespace Zaaml.Text
{
	internal partial class Parser<TGrammar, TToken>
	{
		private sealed partial class ParserAutomata
		{
			private IEnumerable<ParserProduction> DeriveLeftRecursionProductions(ParserRule parserRule, ParserProduction parserProduction)
			{
				if (IsLeftRecursion(parserRule, parserProduction, out var entryOffset))
				{
					yield return parserProduction;
					yield break;
				}

				if (parserProduction.Entries[entryOffset] is not ParserRuleEntry derivedRuleEntry)
					yield break;

				var derivedRule = derivedRuleEntry.Rule;

				foreach (var production in derivedRule.Productions)
				{
					var derivedProduction = (ParserProduction)production;
					var replaceProductionEntries = new List<Entry>();

					for (var i = 0; i < parserProduction.Entries.Length; i++)
					{
						if (i == entryOffset)
						{
							replaceProductionEntries.AddRange(derivedProduction.Entries);

							continue;
						}

						replaceProductionEntries.Add(parserProduction.Entries[i]);
					}

					var replaceProduction = new ParserProduction(this, p => new LeftRecursionBinder(p, LeftRecursionBinderKind.Indirect), replaceProductionEntries, derivedProduction, parserProduction);

					foreach (var deriveLeftRecursionProduction in DeriveLeftRecursionProductions(parserRule, replaceProduction))
						yield return deriveLeftRecursionProduction;
				}
			}

			private void EliminateLeftRecursion(ParserRule parserRule)
			{
				HandleIndirectLeftRecursion(parserRule);

				var hasLeftRecursion = false;
				var productions = parserRule.Productions;

				foreach (var parserProduction in productions.Cast<ParserProduction>())
					hasLeftRecursion |= IsLeftRecursion(parserRule, parserProduction, out _);

				if (hasLeftRecursion == false)
					return;

				var tailGrammarNode = new Grammar<TGrammar, TToken>.ParserGrammar.NodeSyntax($"{parserRule.Name}Tail", true);
				var tailGrammarNodeEntry = new Grammar<TGrammar, TToken>.ParserGrammar.NodeSymbol(tailGrammarNode);
				var tailRule = new ParserRule(tailGrammarNode);
				var tailRuleProductions = new List<ParserProduction>();

				for (var index = 0; index < productions.Count; index++)
				{
					var parserProduction = (ParserProduction)productions[index];

					productions[index] = null;

					if (IsLeftRecursion(parserRule, parserProduction, out var entryOffset))
					{
						var entries = new List<Entry>();

						for (var i = 0; i < parserProduction.Entries.Length; i++)
						{
							if (i == entryOffset)
								continue;

							entries.Add(parserProduction.Entries[i]);
						}

						tailRuleProductions.Add(new ParserProduction(this, p => new LeftRecursionBinder(p, LeftRecursionBinderKind.Recurse), entries, parserProduction, parserProduction));
					}
					else
					{
						var tailRuleEntry = new ParserRuleEntry(tailGrammarNodeEntry, tailRule);

						productions[index] = new ParserProduction(this, p => new LeftRecursionBinder(p, LeftRecursionBinderKind.Tail), parserProduction.Entries.Append(tailRuleEntry), parserProduction, parserProduction);
					}
				}
				
				var tailEpsilonProduction = new ParserProduction(this, p => new LeftRecursionBinder(p, LeftRecursionBinderKind.Tail), Array.Empty<Entry>(), null, null);

				tailRuleProductions.Add(tailEpsilonProduction);

				foreach (var tailProduction in tailRuleProductions)
					tailRule.Productions.Add(tailProduction);

				EliminateLeftRecursion(tailRule);

				for (var i = 0; i < productions.Count; i++)
				{
					if (productions[i] != null)
						continue;

					productions.RemoveAt(i);
					i--;
				}
			}

			private void HandleIndirectLeftRecursion(ParserRule parserRule)
			{
				var hashSet = new HashSet<Rule>();

				for (var index = 0; index < parserRule.Productions.Count; index++)
				{
					var parserProduction = (ParserProduction)parserRule.Productions[index];

					if (IsLeftRecursion(parserRule, parserProduction, out _))
						continue;

					var indirectRecursive = IsIndirectLeftRecursive(parserRule, parserProduction, hashSet);

					if (indirectRecursive == false)
						continue;

					parserRule.Productions.RemoveAt(index);

					foreach (var deriveLeftRecursionProduction in DeriveLeftRecursionProductions(parserRule, parserProduction))
						parserRule.Productions.Insert(index++, deriveLeftRecursionProduction);

					index--;
				}
			}

			private static bool IsIndirectLeftRecursive(ParserRule parserRule, Rule derivedRule, HashSet<Rule> rules)
			{
				return ReferenceEquals(parserRule, derivedRule) || derivedRule.Productions.Cast<ParserProduction>().Any(p => IsIndirectLeftRecursive(parserRule, p, rules));
			}

			private static bool IsIndirectLeftRecursive(ParserRule parserRule, ParserProduction parserProduction, HashSet<Rule> rules)
			{
				if (parserProduction.Entries.Length == 0)
					return false;

				var entryOffset = 0;

				while (IsPredicateEntry(parserProduction.Entries[entryOffset]))
					entryOffset++;

				var entry = parserProduction.Entries[entryOffset];

				if (entry is not ParserRuleEntry parserRuleEntry)
					return false;

				var derivedRule = parserRuleEntry.Rule;

				if (rules.Add(derivedRule) == false)
					return false;

				var isIndirectLeftRecursive = IsIndirectLeftRecursive(parserRule, derivedRule, rules);

				rules.Remove(derivedRule);

				return isIndirectLeftRecursive;
			}

			private static bool IsLeftRecursion(ParserRule parserRule, ParserProduction parserProduction, out int entryOffset)
			{
				entryOffset = 0;

				while (entryOffset < parserProduction.Entries.Length && IsPredicateEntry(parserProduction.Entries[entryOffset]))
					entryOffset++;

				if (entryOffset == parserProduction.Entries.Length)
					return false;

				return IsRecursion(parserRule, parserProduction.Entries[entryOffset]);
			}

			private static bool IsPredicateEntry(Entry entry)
			{
				return entry switch
				{
					PrecedenceEntry => true,
					_ => false
				};
			}

			private static bool IsRecursion(ParserRule rule, Entry entry)
			{
				return entry is ParserRuleEntry parserStateEntry && ReferenceEquals(parserStateEntry.Rule, rule);
			}

			private enum LeftRecursionBinderKind
			{
				Tail,
				Recurse,
				Indirect
			}

			private sealed class LeftRecursionBinder : ProductionBinder
			{
				public LeftRecursionBinder(ParserProduction parserProduction, LeftRecursionBinderKind kind)
				{
					ParserProduction = parserProduction;
					Kind = kind;
				}

				private ParserProduction ParserProduction { get; }

				public LeftRecursionBinderKind Kind { get; }

				protected override void BuildCore()
				{
					ParserProduction.EnsureArguments();
				}
			}
		}
	}
}

#endif