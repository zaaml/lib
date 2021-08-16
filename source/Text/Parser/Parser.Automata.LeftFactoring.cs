// <copyright file="Parser.Automata.LeftFactoring.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
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
			private void EliminateLeftFactoring(ParserRule parserRule, List<ParserProduction> productions)
			{
				var dictionary = new Dictionary<LeftFactoringEntry, List<Tuple<int, ParserProduction>>>();

				for (var index = 0; index < productions.Count; index++)
				{
					var production = productions[index];

					if (production.Entries.Length == 0)
						continue;

					var entry = production.Entries[0];

					if (LeftFactoringEntry.IsPrefixEntry(entry) == false)
						continue;

					var leftFactoringEntry = new LeftFactoringEntry(entry);

					if (dictionary.TryGetValue(leftFactoringEntry, out var list) == false)
						dictionary[leftFactoringEntry] = list = new List<Tuple<int, ParserProduction>>();

					list.Add(new Tuple<int, ParserProduction>(index, production));
				}

				foreach (var kv in dictionary)
				{
					if (kv.Value.Count < 2)
						continue;

					var prefixEntry = kv.Key.ParserEntry;
					var factorPostfixProductions = new List<ParserProduction>();
					var factorPrefixRule = new ParserRule($"{parserRule.Name}Factor", false);
					var factorPrefixRuleEntry = new ParserRuleEntry(new Grammar<TToken>.ParserRule { Name = factorPrefixRule.Name }, factorPrefixRule, false, false) { SkipStack = false };

					foreach (var tuple in kv.Value.OrderBy(k => k.Item1))
					{
						var parserProduction = tuple.Item2;
						var postfixEntries = parserProduction.Entries.Skip(1).ToArray();
						var factorPostfixProduction = new ParserProduction(this, _ => LeftFactoringBinder.Instance, postfixEntries, parserProduction);

						if (factorPostfixProductions.Count == 0)
							productions[tuple.Item1] = new ParserProduction(this, _ => LeftFactoringBinder.Instance, new[] { prefixEntry, factorPrefixRuleEntry }, tuple.Item2);
						else
							productions[tuple.Item1] = null;

						factorPostfixProductions.Add(factorPostfixProduction);
					}

					EliminateLeftFactoring(factorPrefixRule, factorPostfixProductions);

					foreach (var factorPostfixProduction in factorPostfixProductions)
						factorPrefixRule.Productions.Add(factorPostfixProduction);
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
				public static readonly LeftFactoringBinder Instance = new();

				private LeftFactoringBinder()
				{
				}

				public override bool IsFactoryBinder => false;
			}

			private sealed class LeftFactoringEntry
			{
				public LeftFactoringEntry(Entry parserEntry)
				{
					ParserEntry = parserEntry;
				}

				public Entry ParserEntry { get; }

				private bool Equals(LeftFactoringEntry other)
				{
					if (ParserEntry.GetType() != other.ParserEntry.GetType())
						return false;

					return (ParserEntry, other.ParserEntry) switch
					{
						(PrimitiveMatchEntry f, PrimitiveMatchEntry s) => EqualsMatchEntry(f, s),
						(ParserRuleEntry f, ParserRuleEntry s) => EqualsParserRuleEntry(f, s),
						_ => throw new InvalidOperationException()
					};
				}

				public override bool Equals(object obj)
				{
					if (ReferenceEquals(null, obj))
						return false;

					if (ReferenceEquals(this, obj))
						return true;

					if (obj.GetType() != GetType())
						return false;

					return Equals((LeftFactoringEntry)obj);
				}

				private bool EqualsMatchEntry(PrimitiveMatchEntry first, PrimitiveMatchEntry second)
				{
					return first.Equals(second);
				}

				private bool EqualsParserRuleEntry(ParserRuleEntry first, ParserRuleEntry second)
				{
					if (first.Rule != second.Rule)
						return false;

					if (first.Fragment != second.Fragment)
						return false;

					if (first.TryReturn != second.TryReturn)
						return false;

					return true;
				}

				public override int GetHashCode()
				{
					return ParserEntry switch
					{
						PrimitiveMatchEntry matchEntry => matchEntry.GetHashCode(),
						ParserRuleEntry stateEntry => stateEntry.Rule.GetHashCode(),
						_ => 0
					};
				}

				public static bool IsPrefixEntry(Entry parserEntry)
				{
					return parserEntry switch
					{
						PrimitiveMatchEntry => true,
						ParserRuleEntry => true,
						_ => false
					};
				}
			}
		}
	}
}