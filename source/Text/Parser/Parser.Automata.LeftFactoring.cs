// <copyright file="Parser.Automata.LeftFactoring.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

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
			private void EliminateLeftFactoring(ParserRule parserRule)
			{
				if (parserRule.Inline)
					return;

				var dictionary = new Dictionary<Entry, List<Tuple<int, ParserProduction>>>(EntryEqualityComparer.Instance);
				var productions = parserRule.Productions;

				for (var index = 0; index < productions.Count; index++)
				{
					var production = (ParserProduction)productions[index];

					if (production.Entries.Length == 0)
						continue;

					var entry = production.Entries[0];

					if (IsLeftFactoringPrefixEntry(entry) == false)
						continue;

					if (dictionary.TryGetValue(entry, out var list) == false)
						dictionary[entry] = list = new List<Tuple<int, ParserProduction>>();

					list.Add(new Tuple<int, ParserProduction>(index, production));
				}

				foreach (var kv in dictionary)
				{
					if (kv.Value.Count < 2)
						continue;

					var prefixEntry = kv.Key;
					var factorPostfixProductions = new List<ParserProduction>();
					var parserSyntaxNode = new Grammar<TGrammar, TToken>.ParserGrammar.NodeSyntax($"{parserRule.Name}Factor", true);
					var parserSyntaxNodeEntry = new Grammar<TGrammar, TToken>.ParserGrammar.NodeSymbol(parserSyntaxNode);
					var factorPrefixRule = new ParserRule(parserSyntaxNode);
					var factorPrefixRuleEntry = new ParserRuleEntry(parserSyntaxNodeEntry, factorPrefixRule);

					foreach (var tuple in kv.Value.OrderBy(k => k.Item1))
					{
						var parserProduction = tuple.Item2;
						var postfixEntries = parserProduction.Entries.Skip(1).ToArray();
						var factorPostfixProduction = new ParserProduction(this, p => new LeftFactoringBinder(p), postfixEntries, parserProduction, parserProduction);

						if (factorPostfixProductions.Count == 0)
							productions[tuple.Item1] = new ParserProduction(this, p => new LeftFactoringBinder(p), new[] { prefixEntry, factorPrefixRuleEntry }, parserProduction, null);
						else
							productions[tuple.Item1] = null;

						factorPostfixProductions.Add(factorPostfixProduction);
					}

					foreach (var factorPostfixProduction in factorPostfixProductions)
						factorPrefixRule.Productions.Add(factorPostfixProduction);

					EliminateLeftFactoring(factorPrefixRule);
				}

				for (var i = 0; i < productions.Count; i++)
				{
					if (productions[i] != null)
						continue;

					productions.RemoveAt(i);
					i--;
				}
			}

			private sealed class LeftFactoringBinder : ProductionBinder
			{
				private ParserProduction ParserProduction { get; }

				public LeftFactoringBinder(ParserProduction parserProduction)
				{
					ParserProduction = parserProduction;
				}

				protected override void BuildCore()
				{
					ParserProduction.EnsureArguments();
				}
			}

			private static bool IsLeftFactoringPrefixEntry(Entry parserEntry)
			{
				return parserEntry switch
				{
					PrimitiveMatchEntry => true,
					QuantifierEntry => true,
					RuleEntry => true,
					_ => false
				};
			}
		}
	}
}