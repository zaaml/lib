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
			private void EliminateLeftRecursion(ParserRule parserRule, List<ParserProduction> productions)
			{
				var hasLeftRecursion = false;

				foreach (var parserProduction in productions)
					hasLeftRecursion |= IsLeftRecursion(parserRule, parserProduction);

				if (hasLeftRecursion == false)
					return;

				var tailRule = new ParserRule($"{parserRule.Name}Tail", false);
				var tailGrammarRule = new Grammar<TToken>.ParserRule { Name = tailRule.Name };
				var tailRuleProductions = new List<ParserProduction>();

				for (var index = 0; index < productions.Count; index++)
				{
					var parserProduction = productions[index];

					productions[index] = null;

					var tailRuleEntry = new ParserRuleEntry(tailGrammarRule, tailRule, false, false);

					if (IsLeftRecursion(parserRule, parserProduction))
					{
						var tailProduction = new ParserProduction(this, _ => LeftRecursionBinder.RecursiveInstance, parserProduction.Entries.Skip(1).Append(tailRuleEntry));
						var tailRuleEntryArgument = new NullProductionArgument(tailRule.Name, tailRuleEntry, tailProduction.Entries.Length - 1, tailProduction);

						tailProduction.OriginalProduction = parserProduction;

						tailRuleEntry.ProductionArgument = tailRuleEntryArgument;

						tailRuleProductions.Add(tailProduction);

						for (var j = 1; j < parserProduction.Entries.Length; j++)
							MapArgument(j, parserProduction, j - 1, tailProduction);
					}
					else
					{
						var headProduction = new ParserProduction(this, _ => LeftRecursionBinder.NonRecursiveInstance, parserProduction.Entries.Append(tailRuleEntry));
						var tailRuleEntryArgument = new NullProductionArgument(tailRule.Name, tailRuleEntry, headProduction.Entries.Length - 1, headProduction);

						headProduction.OriginalProduction = parserProduction;

						tailRuleEntry.ProductionArgument = tailRuleEntryArgument;

						productions[index] = headProduction;

						for (var j = 0; j < parserProduction.Entries.Length; j++)
							MapArgument(j, parserProduction, j, headProduction);
					}
				}

				var tailEpsilonProduction = new ParserProduction(this, _ => LeftRecursionBinder.NonRecursiveInstance, new[] { EpsilonEntry.Instance });

				tailRuleProductions.Add(tailEpsilonProduction);

				EliminateLeftRecursion(tailRule, tailRuleProductions);

				foreach (var tailProduction in tailRuleProductions)
					tailRule.Productions.Add(tailProduction);

				for (var i = 0; i < productions.Count; i++)
				{
					if (productions[i] != null)
						continue;

					productions.RemoveAt(i);
					i--;
				}
			}

			private static bool IsLeftRecursion(ParserRule parserRule, ParserProduction parserProduction)
			{
				return IsRecursion(parserProduction.Entries[0], parserRule);
			}

			private static bool IsRecursion(Entry entry, ParserRule rule)
			{
				return entry is ParserRuleEntry parserStateEntry && ReferenceEquals(parserStateEntry.Rule, rule);
			}

			private static void MapArgument(int sourceEntryIndex, ParserProduction sourceProduction, int targetEntryIndex, ParserProduction targetProduction)
			{
				var entry = sourceProduction.Entries[sourceEntryIndex];

				if (entry is not IParserEntry parserEntry)
					return;

				var entryArgument = parserEntry.ProductionArgument;

				if (entryArgument == null)
					return;

				var targetEntry = targetProduction.Entries[targetEntryIndex];
				var targetArgument = (entryArgument.OriginalArgument ?? entryArgument).MapArgument(targetEntryIndex, targetEntry, targetProduction);
				var targetParserEntry = (IParserEntry)targetEntry;

				targetParserEntry.ProductionArgument = targetArgument;
				targetProduction.Arguments.Add(targetArgument);
				targetArgument.Bind(entryArgument.Binder);
			}

			private sealed class LeftRecursionBinder : ProductionBinder
			{
				public static readonly LeftRecursionBinder RecursiveInstance = new(true);
				public static readonly LeftRecursionBinder NonRecursiveInstance = new(false);

				private LeftRecursionBinder(bool recursive)
				{
					Recursive = recursive;
				}

				public override bool IsFactoryBinder => false;

				public bool Recursive { get; }
			}
		}
	}
}