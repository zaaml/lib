// <copyright file="Parser.Automata.LeftRecursion.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Linq;

// ReSharper disable UseIndexFromEndExpression

namespace Zaaml.Text
{
	internal partial class Parser<TGrammar, TToken>
	{
		private sealed partial class ParserAutomata
		{
			private IEnumerable<ParserProduction> DeriveLeftRecursionProductions(ParserSyntax parserRule, ParserProduction parserProduction, HashSet<Syntax> hashSet)
			{
				if (IsLeftRecursion(parserRule, parserProduction, out var entryOffset))
				{
					yield return parserProduction;
					yield break;
				}

				if (IsIndirectLeftRecursive(parserRule, parserProduction, hashSet) == false)
				{
					yield return parserProduction;
					yield break;
				}

				if (parserProduction.Entries[entryOffset] is not ParserSyntaxEntry derivedRuleEntry)
					yield break;

				var derivedRule = derivedRuleEntry.Syntax;

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

					foreach (var deriveLeftRecursionProduction in DeriveLeftRecursionProductions(parserRule, replaceProduction, hashSet))
						yield return deriveLeftRecursionProduction;
				}
			}

			private void EliminateLeftRecursion(ParserSyntax parserSyntax)
			{
				HandleIndirectLeftRecursion(parserSyntax);

				var hasLeftRecursion = false;
				var productions = parserSyntax.Productions;

				foreach (var parserProduction in productions.Cast<ParserProduction>())
					hasLeftRecursion |= IsLeftRecursion(parserSyntax, parserProduction, out _);

				if (hasLeftRecursion == false)
					return;

				var tailGrammarNode = new Grammar<TGrammar, TToken>.ParserGrammar.NodeSyntax($"{parserSyntax.Name}Tail", true);
				var tailGrammarNodeEntry = new Grammar<TGrammar, TToken>.ParserGrammar.NodeSymbol(tailGrammarNode);
				var tailGrammarQuantifierSymbol = new Grammar<TGrammar, TToken>.ParserGrammar.QuantifierSymbol(tailGrammarNodeEntry, QuantifierKind.ZeroOrMore, QuantifierMode.Greedy);
				var tailSyntax = new ParserSyntax(tailGrammarNode);
				var tailSyntaxProductions = new List<ParserProduction>();

				var tailProductionAdded = false;

				for (var index = 0; index < productions.Count; index++)
				{
					var parserProduction = (ParserProduction)productions[index];

					productions[index] = null;

					if (IsLeftRecursion(parserSyntax, parserProduction, out var entryOffset))
					{
						var entries = new List<Entry>();

						for (var i = 0; i < parserProduction.Entries.Length; i++)
						{
							if (i == entryOffset)
								continue;

							entries.Add(parserProduction.Entries[i]);
						}

						var recurseProduction = new ParserProduction(this, p => new LeftRecursionBinder(p, LeftRecursionBinderKind.Recurse), entries, parserProduction, parserProduction);

						tailSyntaxProductions.Add(recurseProduction);
					}
					else
					{
						var tailSyntaxEntry = new ParserSyntaxEntry(tailGrammarNodeEntry, tailSyntax);
						var tailQuantifier = new ParserQuantifierEntry(tailGrammarQuantifierSymbol, tailSyntaxEntry, QuantifierHelper.GetRange(QuantifierKind.ZeroOrMore), QuantifierMode.Greedy);
						var tailProduction = new ParserProduction(this, p => new LeftRecursionBinder(p, LeftRecursionBinderKind.Tail), parserProduction.Entries.Append(tailQuantifier), parserProduction, parserProduction);

						productions[index] = tailProduction;

						tailProductionAdded = true;
					}
				}

				foreach (var tailProduction in tailSyntaxProductions)
					tailSyntax.Productions.Add(tailProduction);

				EliminateLeftRecursion(tailSyntax);

				for (var i = 0; i < productions.Count; i++)
				{
					if (productions[i] != null)
						continue;

					productions.RemoveAt(i);
					i--;
				}

				if (tailProductionAdded == false)
				{
					var tailSyntaxEntry = new ParserSyntaxEntry(tailGrammarNodeEntry, tailSyntax);
					var tailQuantifier = new ParserQuantifierEntry(tailGrammarQuantifierSymbol, tailSyntaxEntry, QuantifierHelper.GetRange(QuantifierKind.ZeroOrMore), QuantifierMode.Greedy);
					var tailProduction = new ParserProduction(this, p => new LeftRecursionBinder(p, LeftRecursionBinderKind.Tail), new Entry[] { tailQuantifier }, null, null);

					productions.Add(tailProduction);
				}
			}

			private void HandleIndirectLeftRecursion(ParserSyntax parserRule)
			{
				var hashSet = new HashSet<Syntax>();

				for (var index = 0; index < parserRule.Productions.Count; index++)
				{
					var parserProduction = (ParserProduction)parserRule.Productions[index];

					if (IsLeftRecursion(parserRule, parserProduction, out _))
						continue;

					var indirectRecursive = IsIndirectLeftRecursive(parserRule, parserProduction, hashSet);

					if (indirectRecursive == false)
						continue;

					parserRule.Productions.RemoveAt(index);

					foreach (var deriveLeftRecursionProduction in DeriveLeftRecursionProductions(parserRule, parserProduction, hashSet))
						parserRule.Productions.Insert(index++, deriveLeftRecursionProduction);

					index--;
				}
			}

			private static bool IsIndirectLeftRecursive(ParserSyntax parserRule, Syntax derivedRule, HashSet<Syntax> rules)
			{
				return ReferenceEquals(parserRule, derivedRule) || derivedRule.Productions.Cast<ParserProduction>().Any(p => IsIndirectLeftRecursive(parserRule, p, rules));
			}

			private static bool IsIndirectLeftRecursive(ParserSyntax parserRule, ParserProduction parserProduction, HashSet<Syntax> rules)
			{
				if (parserProduction.Entries.Length == 0)
					return false;

				var entryOffset = 0;

				while (entryOffset < parserProduction.Entries.Length && IsPredicateEntry(parserProduction.Entries[entryOffset]))
					entryOffset++;

				if (entryOffset == parserProduction.Entries.Length)
					return false;

				var entry = parserProduction.Entries[entryOffset];

				if (entry is not ParserSyntaxEntry parserRuleEntry)
					return false;

				var derivedRule = parserRuleEntry.Syntax;

				if (rules.Add(derivedRule) == false)
					return false;

				var isIndirectLeftRecursive = IsIndirectLeftRecursive(parserRule, derivedRule, rules);

				rules.Remove(derivedRule);

				return isIndirectLeftRecursive;
			}

			private static bool IsLeftRecursion(ParserSyntax parserRule, ParserProduction parserProduction, out int entryOffset)
			{
				entryOffset = 0;

				if (parserProduction.Entries.Length == 0)
					return false;

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

			private static bool IsRecursion(ParserSyntax rule, Entry entry)
			{
				return entry is ParserSyntaxEntry parserStateEntry && ReferenceEquals(parserStateEntry.Syntax, rule);
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