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
			private void EliminateLeftFactoring(ParserRule parserRule, List<ParserProduction> productions, bool subfactor = false)
			{
				if (parserRule.Name != "CallExpr" && subfactor == false)
					return;

				var dictionary = new Dictionary<LeftFactoringPrefix, List<Tuple<int, ParserProduction>>>();

				for (var index = 0; index < productions.Count; index++)
				{
					var production = productions[index];

					if (production.Entries.Length == 0)
						continue;

					LeftFactoringPrefix prefix = null;

					for (var i = 0; i < production.Entries.Length; i++)
					{
						var entry = production.Entries[i];

						if (LeftFactoringEntry.IsPrefixEntry(entry) == false)
							break;

						prefix ??= new LeftFactoringPrefix(i);

						prefix.Entries.Add(new LeftFactoringEntry(entry));

						if (entry is ParserRuleEntry)
							break;
					}

					if (prefix?.Entries[prefix.Entries.Count - 1].ParserEntry is not ParserRuleEntry)
						continue;

					if (prefix.Entries.Count < 2)
						continue;

					if (dictionary.TryGetValue(prefix, out var list) == false)
						dictionary[prefix] = list = new List<Tuple<int, ParserProduction>>();

					list.Add(new Tuple<int, ParserProduction>(index, production));
				}

				foreach (var kv in dictionary)
				{
					if (kv.Value.Count < 2)
						continue;

					var prefixLength = kv.Key.Entries.Count;
					var baseProduction = productions[kv.Key.Index];
					var parserStateFactorProductions = new List<ParserProduction>();

					foreach (var tuple in kv.Value.OrderBy(k => k.Item1))
					{
						var parserProduction = tuple.Item2;
						var factorProduction = new ParserProduction(this, p => new LeftFactorProductionFactory(p, parserRule, parserProduction, false), parserProduction.Entries.Skip(prefixLength))
						{
							LeftFactorProduction = parserProduction
						};

						productions[tuple.Item1] = null;

						parserStateFactorProductions.Add(factorProduction);
					}

					var factorRule = new ParserRule($"{parserRule.Name}Factor", false);
					var factorRuleEntry = new ParserRuleEntry(new Grammar<TToken>.ParserRule { Name = factorRule.Name }, factorRule, false, false) { SkipStack = true };

					var production = new ParserProduction(this, p => new LeftFactorProductionFactory(p, parserRule, baseProduction, true), kv.Key.Entries.Take(prefixLength).Select(e => e.ParserEntry).Append(factorRuleEntry))
					{
						LeftFactorEntry = factorRuleEntry
					};

					productions[kv.Key.Index] = production;

					EliminateLeftFactoring(factorRule, parserStateFactorProductions, true);

					foreach (var factorProduction in parserStateFactorProductions) 
						factorRule.Productions.Add(factorProduction);
				}

				for (var i = 0; i < productions.Count; i++)
				{
					if (productions[i] != null) 
						continue;

					productions.RemoveAt(i);
					i--;
				}
			}

			private sealed class LeftFactoringPrefix
			{
				public int Index { get; }

				public List<LeftFactoringEntry> Entries { get; } = new List<LeftFactoringEntry>();

				public LeftFactoringPrefix(int index)
				{
					Index = index;
				}

				private bool Equals(LeftFactoringPrefix other)
				{
					return Entries.SequenceEqual(other.Entries);
				}

				public override bool Equals(object obj)
				{
					return ReferenceEquals(this, obj) || obj is LeftFactoringPrefix other && Equals(other);
				}

				public override int GetHashCode()
				{
					var hashCode = 0;

					foreach (var entry in Entries)
						hashCode = (hashCode * 397) ^ entry.GetHashCode();

					return hashCode;
				}
			}

			private sealed class LeftFactorProductionFactory : ProductionBinder
			{
				public ParserRule OriginalRule { get; }

				public LeftFactorProductionFactory(ParserProduction parserProduction, ParserRule originalRule, ParserProduction baseProduction, bool prefix)
				{
					throw new NotImplementedException();

					//OriginalRule = originalRule;
					//ArgumentFactories = new ProductionEntityArgumentFactory[baseProduction.EntityFactory.ArgumentFactories.Length];

					//foreach (var argument in baseProduction.Arguments)
					//{
					//	var argumentFactory = baseProduction.EntityFactory.ArgumentFactories[argument.Index];

					//	ArgumentFactories[argument.Index] = argumentFactory;
					//}
				}
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

					return Equals((LeftFactoringEntry) obj);
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

					if (first.RuleEntryContext != second.RuleEntryContext)
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
