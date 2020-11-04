// <copyright file="Parser.Automata.ParserProduction.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using Zaaml.Core.Extensions;

namespace Zaaml.Text
{
	internal abstract partial class Parser<TGrammar, TToken>
	{
		private sealed partial class ParserAutomata
		{
			private sealed class ParserProduction : Production
			{
				public readonly ParserProductionBinder Binder;

				public ParserProduction(IEnumerable<Entry> entries) : base(entries)
				{
					IsInline = true;
				}

				public ParserProduction(ParserAutomata automata, Grammar<TToken>.ParserRule parserRule, Grammar<TToken>.ParserProduction grammarParserProduction, List<ParserProduction> productions)
					: base(grammarParserProduction.Entries.Select(automata.CreateParserEntry))
				{
					Name = grammarParserProduction.Name ?? automata.GenerateProductionName();
					IsInline = parserRule.IsInline;
					GrammarParserProduction = grammarParserProduction;
					ProductionIndex = productions.Count;

					productions.Add(this);

					Flatten();

					if (IsInline)
						return;

					Binder = grammarParserProduction.Binding switch
					{
						Grammar<TToken>.ConstructorParserProductionBinding _ => new CtorParserProductionBinder(this),
						Grammar<TToken>.SyntaxFactoryParserProductionBinding _ => new SyntaxFactoryProductionBinder(this),
						null => null,
						_ => throw new InvalidOperationException("Unknown binder")
					};
				}

				public List<ParserEntryData> FlatEntries { get; } = new List<ParserEntryData>();

				private Dictionary<string, ParserEntryData> FlatEntriesDictionary { get; } = new Dictionary<string, ParserEntryData>(StringComparer.OrdinalIgnoreCase);

				public Grammar<TToken>.ParserProduction GrammarParserProduction { get; }

				public bool IsInline { get; }

				internal bool IsRightAssoc { get; set; }

				internal LeftRecursionClassifier LeftRecursionClassifier { get; set; }

				internal ParserStateEntry LeftRecursionEntry { get; set; }

				private string Name { get; }

				public int ProductionIndex { get; }

				private void Flatten()
				{
					if (IsInline)
						return;

					var flatEntries = new List<ParserEntryData>();

					FlattenProduction(this, this, flatEntries);

					foreach (var parserStateEntry in flatEntries)
					{
						var name = parserStateEntry.Name;

						if (string.IsNullOrEmpty(name))
							continue;

						if (FlatEntriesDictionary.TryGetValue(name, out var existing))
							parserStateEntry.FlatIndex = existing.FlatIndex;
						else
						{
							FlatEntries.Add(parserStateEntry);

							parserStateEntry.FlatIndex = FlatEntriesDictionary.Count;

							FlatEntriesDictionary[name] = parserStateEntry;
						}
					}
				}

				private static void FlattenEntry(Entry entry, ParserProduction owner, List<ParserEntryData> flatEntries)
				{
					while (true)
					{
						if (entry is ParserStateEntry parserStateEntry && parserStateEntry.Fragment)
						{
							foreach (var parserTransition in parserStateEntry.State.Productions)
								FlattenProduction(owner, parserTransition, flatEntries);
						}
						else if (entry is ParserQuantifierEntry parserQuantifierEntry)
						{
							var parserPrimitiveEntry = parserQuantifierEntry.PrimitiveEntry;

							entry = parserPrimitiveEntry;

							continue;
						}
						else if (entry is ParserSetMatchEntry)
						{
							//var set = (ParserSetMatchEntry) entry;

							//foreach (var primitiveMatchEntry in set.Matches)
							//{
							//	var setDataEntry = GetParserEntryData(primitiveMatchEntry);
							//	var setGrammarEntry = GetGrammarEntry(primitiveMatchEntry);

							//	setDataEntry.ParserTransition = owner;
							//	setDataEntry.Name = EnsureName(setGrammarEntry);

							//	flatEntries.Add(setDataEntry);
							//}

							var parserEntryData = GetParserEntryData(entry);

							parserEntryData.ParserProduction = owner;

							flatEntries.Add(parserEntryData);
						}
						else
						{
							var parserEntryData = GetParserEntryData(entry);

							parserEntryData.ParserProduction = owner;

							flatEntries.Add(parserEntryData);
						}

						break;
					}
				}

				private static void FlattenProduction(ParserProduction owner, Production production, List<ParserEntryData> flatEntries)
				{
					foreach (var entry in production.Entries)
						FlattenEntry(entry, owner, flatEntries);
				}

				public ParserEntryData GetEntry(string name)
				{
					return FlatEntriesDictionary.GetValueOrDefault(name);
				}

				public override string ToString()
				{
					return Name;
				}
			}
		}
	}
}