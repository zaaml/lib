// <copyright file="Parser.Automata.ParserProduction.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Zaaml.Core.Extensions;
using Zaaml.Core.Utils;

// ReSharper disable StaticMemberInGenericType

namespace Zaaml.Text
{
	internal partial class Parser<TGrammar, TToken>
	{
		private sealed partial class ParserAutomata
		{
			private sealed class ParserProduction : Production
			{
				private static readonly Dictionary<Type, Type> ArrayTypeDictionary = new();
				public readonly ProductionBinder Binder;
				private ProductionArgumentCollection _arguments;
				private ProductionEntity[] _productionEntityPool;
				private int _productionEntityPoolTail = -1;

				public ParserProduction(ParserAutomata automata, Func<ParserProduction, ProductionBinder> binderFactory, IEnumerable<Entry> entries, ParserProduction sourceProduction, ParserProduction actualProduction) 
					: base(CloneEntries(automata, entries))
				{
					if (binderFactory == null)
						return;

					Binder = binderFactory(this);
					ProductionIndex = automata.RegisterProduction(this);
					SourceProduction = sourceProduction;
					ActualProduction = actualProduction;

					MapArgumentsBuilder = true;
				}

				public ParserProduction(ParserAutomata automata, ParserRule parserRule, Grammar<TGrammar, TToken>.ParserGrammar.Syntax parserSyntax, Grammar<TGrammar, TToken>.ParserGrammar.Production grammarParserProduction)
					: base(CreateEntries(automata, grammarParserProduction))
				{
					Name = grammarParserProduction.Name ?? automata.GenerateProductionName();
					ParserSyntax = parserSyntax;
					GrammarParserProduction = grammarParserProduction;
					ProductionIndex = automata.RegisterProduction(this);

					if (grammarParserProduction.ProductionBinding == null)
						return;

					Binder = CreateProductionBinder(grammarParserProduction);
				}

				private static IEnumerable<Entry> CloneEntries(ParserAutomata automata, IEnumerable<Entry> entries)
				{
					foreach (var sourceEntry in entries)
					{
						if (sourceEntry is IParserEntry parserEntry)
						{
							var cloneEntry = parserEntry.Clone();

							yield return cloneEntry;
						}
						else
							yield return sourceEntry;
					}
				}

				public ParserProduction ActualProduction { get; }

				public ProductionArgumentCollection Arguments => EnsureArguments();

				public string EntriesDebuggerDisplay => string.Join(" ", Entries.Select(e => e.ToString()));

				public Grammar<TGrammar, TToken>.ParserGrammar.Production GrammarParserProduction { get; }

				private bool MapArgumentsBuilder { get; }

				private string Name { get; }

				public Grammar<TGrammar, TToken>.ParserGrammar.Syntax ParserSyntax { get; }

				public int ProductionIndex { get; }

				internal ParserProduction SourceProduction { get; }

				private void BuildArgument(List<Entry> stack, ParserProduction parserProduction, ProductionArgumentCollection argumentCollection, Action<Entry, List<Entry>, ProductionArgumentCollection> argumentBuilder)
				{
					var entry = stack[stack.Count - 1];

					if (entry is ParserRuleEntry { Fragment: true } parserStateEntry)
					{
						stack.Add(parserStateEntry);

						foreach (var fragmentProduction in parserStateEntry.Rule.Productions)
							BuildArguments(parserProduction, fragmentProduction, stack, argumentCollection, argumentBuilder);

						stack.RemoveAt(stack.Count - 1);
					}
					else if (entry is ParserQuantifierEntry parserQuantifierEntry)
					{
						var parserPrimitiveEntry = parserQuantifierEntry.PrimitiveEntry;

						stack.Add(parserPrimitiveEntry);

						BuildArgument(stack, parserProduction, argumentCollection, argumentBuilder);

						stack.RemoveAt(stack.Count - 1);
					}
					else
						argumentBuilder(entry, stack, argumentCollection);
				}

				private ProductionArgumentCollection BuildArguments()
				{
					return BuildArguments(MapArgumentsBuilder ? MapArgument : CreateArgument);
				}

				private ProductionArgumentCollection BuildArguments(Action<Entry, List<Entry>, ProductionArgumentCollection> argumentBuilder)
				{
					var arguments = new ProductionArgumentCollection(this);

					BuildArguments(this, this, new List<Entry>(), arguments, argumentBuilder);

					return arguments;
				}

				private void BuildArguments(ParserProduction parserProduction, Production fragmentProduction, List<Entry> stack, ProductionArgumentCollection argumentCollection,
					Action<Entry, List<Entry>, ProductionArgumentCollection> argumentBuilder)
				{
					foreach (var entry in fragmentProduction.Entries)
					{
						stack.Add(entry);

						BuildArgument(stack, parserProduction, argumentCollection, argumentBuilder);

						stack.RemoveAt(stack.Count - 1);
					}
				}

				private void CreateArgument(Entry entry, List<Entry> stack, ProductionArgumentCollection argumentCollection)
				{
					if (entry is not IParserEntry parserEntry)
						return;

					var argumentType = GetArgumentType(entry);

					if (argumentType == null)
					{
						argumentCollection.AddArgument(new NullArgument(EnsureEntryName(entry, stack), entry, argumentCollection.Count, this));

						return;
					}

					var array = stack.Any(e => e is ParserQuantifierEntry parentQuantifierEntry && parentQuantifierEntry.Maximum - parentQuantifierEntry.Minimum > 1);
					var argumentName = EnsureEntryName(entry, stack);
					var argumentIndex = argumentCollection.Count;
					var argument = entry switch
					{
						IParserPredicate parserPredicateEntry => CreatePredicateArgument(argumentName, parserPredicateEntry, argumentIndex, argumentType, array),
						ParserSetMatchEntry _ => CreateLexerArgument(argumentName, entry, argumentIndex, argumentType, array),
						ParserSingleMatchEntry _ => CreateLexerArgument(argumentName, entry, argumentIndex, argumentType, array),
						ParserRuleEntry _ => CreateParserArgument(argumentName, entry, argumentIndex, argumentType, array),
						_ => null
					};

					if (argument == null)
						return;

					argumentCollection.AddArgument(argument);
				}

				private static IEnumerable<Entry> CreateEntries(ParserAutomata automata, Grammar<TGrammar, TToken>.ParserGrammar.Production grammarParserProduction)
				{
					var entries = new List<Entry>();

					entries.AddRange(grammarParserProduction.Symbols.SelectMany(symbol => InlineFragment(automata, symbol)));

					return entries;
				}

				private ProductionArgument CreateLexerArgument(string name, Entry parserEntry, int argumentIndex, Type argumentType, bool array)
				{
					return array ? new LexerValueArrayArgument(name, parserEntry, argumentIndex, GetArrayType(argumentType), this) : new LexerValueArgument(name, parserEntry, argumentIndex, argumentType, this);
				}

				private ProductionArgument CreateParserArgument(string name, Entry parserEntry, int argumentIndex, Type argumentType, bool array)
				{
					var parserArgumentGenericType = array ? typeof(ParserValueArrayArgument<>) : typeof(ParserValueArgument<>);
					var parserArgumentType = parserArgumentGenericType.MakeGenericType(typeof(TGrammar), typeof(TToken), argumentType);

					return (ProductionArgument)Activator.CreateInstance(parserArgumentType, name, parserEntry, argumentIndex, array ? GetArrayType(argumentType) : argumentType, this);
				}

				private ProductionArgument CreatePredicateArgument(string name, IParserPredicate parserPredicateEntry, int argumentIndex, Type argumentType, bool array)
				{
					return parserPredicateEntry.PredicateKind switch
					{
						ParserPredicateKind.ExternalParser => CreateParserArgument(name, (Entry)parserPredicateEntry, argumentIndex, argumentType, array),
						//ParserPredicateKind.SubLexer => CreateSubLexerArgumentFactory(parserPredicateEntry),
						_ => null
					};
				}

				private ProductionBinder CreateProductionBinder(Grammar<TGrammar, TToken>.ParserGrammar.Production grammarParserProduction)
				{
					if (grammarParserProduction.ProductionBinding.ConstValue != null)
						return new ConstValueBinder(this);

					return grammarParserProduction.ProductionBinding switch
					{
						Grammar<TGrammar, TToken>.ParserGrammar.ReturnNodeBinding _ => new ReturnValueBinder(this),
						Grammar<TGrammar, TToken>.ParserGrammar.ConstructorNodeBinding _ => new ConstructorBinder(this),
						Grammar<TGrammar, TToken>.ParserGrammar.SyntaxNodeFactoryBinding _ => new SyntaxNodeFactoryBinder(this),
						_ => null
					};
				}

				public ProductionArgumentCollection EnsureArguments()
				{
					return _arguments ??= BuildArguments();
				}

				public void EnsureBinder()
				{
					Binder?.Build();
				}

				private static string EnsureEntryName(Entry entry, List<Entry> stack)
				{
					if (entry is not IParserEntry parserEntry)
						return null;

					var name = EnsureName(parserEntry.GrammarEntry);

					if (stack.Count > 1 && stack[stack.Count - 2] is ParserQuantifierEntry parserQuantifierEntry)
						return EnsureName(parserQuantifierEntry.GrammarEntry) ?? name;

					return name;
				}

				public ProductionNamedArgumentCollection GetArguments(string name, StringComparison comparisonType = StringComparison.OrdinalIgnoreCase)
				{
					return new(name, this, comparisonType);
				}

				private static Type GetArgumentType(Entry parserEntry)
				{
					return parserEntry switch
					{
						IParserPredicate parserPredicateEntry => GetPredicateArgumentType(parserPredicateEntry),
						ParserRuleEntry parserRuleEntry => ((Grammar<TGrammar, TToken>.ParserGrammar.NodeSymbol)parserRuleEntry.GrammarEntry).Node.NodeType,
						ParserSetMatchEntry _ => typeof(Lexeme<TToken>),
						ParserSingleMatchEntry _ => typeof(Lexeme<TToken>),
						_ => null
					};
				}

				private static Type GetArrayType(Type elementType)
				{
					if (ArrayTypeDictionary.TryGetValue(elementType, out var arrayType) == false)
						ArrayTypeDictionary[elementType] = arrayType = Array.CreateInstance(elementType, 0).GetType();

					return arrayType;
				}

				private static Type GetExternalTokenType(IParserPredicate parserPredicateEntry)
				{
					return parserPredicateEntry.GetType().GetGenericArguments().Last().GetGenericArguments().Single();
				}

				private static Type GetPredicateArgumentType(IParserPredicate parserPredicateEntry)
				{
					return parserPredicateEntry.PredicateKind switch
					{
						ParserPredicateKind.ExternalParser => GetExternalNodeType(parserPredicateEntry),
						ParserPredicateKind.ExternalLexer => GetExternalTokenType(parserPredicateEntry),
						_ => null
					};
				}

				private static Type GetExternalNodeType(IParserPredicate parserPredicateEntry)
				{
					return parserPredicateEntry.ResultType;
				}

				private void InitPool()
				{
					_productionEntityPool = new ProductionEntity[8];

					var length = _productionEntityPool.Length;

					for (var i = 0; i < length; i++)
						_productionEntityPool[i] = new ProductionEntity(this);

					_productionEntityPoolTail = length - 1;
				}

				private static IEnumerable<Entry> InlineFragment(ParserAutomata automata, Grammar<TGrammar, TToken>.ParserGrammar.Symbol symbol)
				{
					switch (symbol)
					{
						case Grammar<TGrammar, TToken>.ParserGrammar.FragmentSymbol fragmentSymbol:

							if (fragmentSymbol.Fragment.Productions.Count == 1)
							{
								var production = fragmentSymbol.Fragment.Productions[0];

								if (production.Symbols.Any(s => s is Grammar<TGrammar, TToken>.ParserGrammar.FragmentSymbol recursiveFragmentSymbol && ReferenceEquals(recursiveFragmentSymbol.Fragment, fragmentSymbol.Fragment)))
									yield return automata.CreateParserEntry(symbol);
								else
								{
									foreach (var entry in production.Symbols.SelectMany(ps => InlineFragment(automata, ps)))
										yield return entry;
								}
							}
							else
								yield return automata.CreateParserEntry(symbol);

							break;

						default:

							yield return automata.CreateParserEntry(symbol);

							break;
					}
				}

				private static Entry GetEntrySource(Entry entry)
				{
					if (entry is IParserEntry parserEntry)
						return (Entry)parserEntry.Source ?? entry;

					return entry;
				}

				private void MapArgument(Entry entry, List<Entry> stack, ProductionArgumentCollection argumentCollection)
				{
					if (SourceProduction == null || SourceProduction.Arguments.TryGetArgument(GetEntrySource(entry), out var originalArgument) == false)
						CreateArgument(entry, stack, argumentCollection);
					else
						argumentCollection.AddArgument(originalArgument.MapArgument(argumentCollection.Count, entry, this));
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public ProductionEntity RentEntity()
				{
					if (_productionEntityPoolTail == -1)
						Resize();

					var productionEntity = _productionEntityPool[_productionEntityPoolTail--];

					//_productionEntityPool[_productionEntityPoolTail + 1] = null;

					productionEntity.Busy = true;

					return productionEntity;
				}

				private void Resize()
				{
					if (_productionEntityPool == null)
					{
						InitPool();

						return;
					}

					var length = _productionEntityPool.Length;

					ArrayUtils.ExpandArrayLength(ref _productionEntityPool, false);

					for (var i = 0; i < length; i++)
						_productionEntityPool[i] = new ProductionEntity(this);

					_productionEntityPoolTail = length - 1;
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public void ReturnEntity(ProductionEntity productionEntity)
				{
					productionEntity.Busy = false;

					_productionEntityPool[++_productionEntityPoolTail] = productionEntity;
				}

				public override string ToString()
				{
					return $"{Name} => {EntriesDebuggerDisplay}";
				}
			}
		}
	}
}