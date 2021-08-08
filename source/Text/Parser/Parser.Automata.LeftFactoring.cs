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
					var prefixEntries = kv.Key.Entries.Take(prefixLength).Select(e => e.ParserEntry).ToArray();
					var factorPostfixProductions = new List<ParserProduction>();

					foreach (var tuple in kv.Value.OrderBy(k => k.Item1))
					{
						var parserProduction = tuple.Item2;
						var postfixEntries = parserProduction.Entries.Skip(prefixLength).ToArray();
						var factorPostfixProduction = new ParserProduction(this, _ => LeftFactoringBinder.Instance, postfixEntries)
						{
							OriginalProduction = parserProduction
						};

						for (var j = 0; j < postfixEntries.Length; j++)
						{
							var postfixEntry = postfixEntries[j];

							if (postfixEntry is not IParserEntry postfixParserEntry)
								continue;

							var postfixArgument = postfixParserEntry.ProductionArgument;

							if (postfixArgument == null)
								continue;

							var factorPostfixEntry = factorPostfixProduction.Entries[j];
							var factorPostfixArgument = (postfixArgument.OriginalArgument ?? postfixArgument).MapArgument(j, factorPostfixEntry, factorPostfixProduction);
							var factorPostfixParserEntry = (IParserEntry)factorPostfixEntry;

							factorPostfixParserEntry.ProductionArgument = factorPostfixArgument;
							factorPostfixProduction.Arguments.Add(factorPostfixArgument);
							factorPostfixArgument.Bind(postfixArgument.Binder);
						}

						productions[tuple.Item1] = null;

						factorPostfixProductions.Add(factorPostfixProduction);
					}

					var factorPrefixRule = new ParserRule($"{parserRule.Name}Factor", false);
					var factorPrefixRuleEntry = new ParserRuleEntry(new Grammar<TToken>.ParserRule { Name = factorPrefixRule.Name }, factorPrefixRule, false, false) { SkipStack = false };
					var factorPrefixProduction = new ParserProduction(this, _ => LeftFactoringBinder.Instance, prefixEntries.Append(factorPrefixRuleEntry)) { LeftFactorProduction = true };
					var factorRuleEntryArgument = new NullProductionArgument(factorPrefixRule.Name, factorPrefixRuleEntry, prefixLength, factorPrefixProduction);

					for (var i = 0; i < prefixEntries.Length; i++)
					{
						var prefixEntry = prefixEntries[i];

						if (prefixEntry is not IParserEntry prefixParserEntry)
							continue;

						var prefixArgument = prefixParserEntry.ProductionArgument;

						if (prefixArgument == null)
							continue;

						var factorPrefixEntry = factorPrefixProduction.Entries[i];
						var factorPrefixArgument = (prefixArgument.OriginalArgument ?? prefixArgument).MapArgument(i, factorPrefixEntry, factorPrefixProduction);
						var factorPrefixParserEntry = (IParserEntry)factorPrefixEntry;

						factorPrefixParserEntry.ProductionArgument = factorPrefixArgument;
						factorPrefixProduction.Arguments.Add(factorPrefixArgument);
						factorPrefixArgument.Bind(prefixArgument.Binder);
					}

					factorPrefixRuleEntry.ProductionArgument = factorRuleEntryArgument;

					productions[kv.Key.Index] = factorPrefixProduction;

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
				public static readonly LeftFactoringBinder Instance = new LeftFactoringBinder();

				private LeftFactoringBinder()
				{
				}

				public override bool IsFactoryBinder => false;
			}

			private sealed class LeftFactoringPrefix
			{
				public LeftFactoringPrefix(int index)
				{
					Index = index;
				}

				public List<LeftFactoringEntry> Entries { get; } = new List<LeftFactoringEntry>();
				public int Index { get; }

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