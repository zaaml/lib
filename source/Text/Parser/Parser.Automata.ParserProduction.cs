// <copyright file="Parser.Automata.ParserProduction.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

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
				private ProductionEntity _productionEntityPoolHead;

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

				public ParserProduction(ParserAutomata automata, ParserSyntax parserSyntax, Grammar<TGrammar, TToken>.ParserGrammar.Syntax grammarParserSyntax, Grammar<TGrammar, TToken>.ParserGrammar.Production grammarParserProduction)
					: base(grammarParserProduction.Symbols.SelectMany(automata.CreateParserEntries))
				{
					Name = grammarParserProduction.Name ?? automata.GenerateProductionName();
					GrammarParserSyntax = grammarParserSyntax;
					GrammarParserProduction = grammarParserProduction;
					ProductionIndex = automata.RegisterProduction(this);

					if (grammarParserProduction.ProductionBinding == null)
						return;

					Binder = CreateProductionBinder(grammarParserProduction);
				}

				public ParserProduction ActualProduction { get; }

				public ProductionArgumentCollection Arguments => EnsureArguments();

				public string EntriesDebuggerDisplay => string.Join(" ", Entries.Select(e => e.ToString()));

				public Grammar<TGrammar, TToken>.ParserGrammar.Production GrammarParserProduction { get; }

				public Grammar<TGrammar, TToken>.ParserGrammar.Syntax GrammarParserSyntax { get; }

				private bool MapArgumentsBuilder { get; }

				public string Name { get; }

				public int ProductionIndex { get; }

				internal ParserProduction SourceProduction { get; }

				private void BuildArgument(List<Entry> stack, ParserProduction parserProduction, ProductionArgumentCollection argumentCollection, Action<Entry, List<Entry>, ProductionArgumentCollection> argumentFactory)
				{
					var entry = stack[stack.Count - 1];

					if (entry is ParserSyntaxEntry { Fragment: true } parserSyntaxEntry)
					{
						stack.Add(parserSyntaxEntry);

						foreach (var fragmentProduction in parserSyntaxEntry.Syntax.Productions)
							BuildArguments(parserProduction, fragmentProduction, stack, argumentCollection, argumentFactory);

						stack.RemoveAt(stack.Count - 1);
					}
					else if (entry is ParserQuantifierEntry parserQuantifierEntry)
					{
						var parserPrimitiveEntry = parserQuantifierEntry.PrimitiveEntry;

						stack.Add(parserPrimitiveEntry);

						BuildArgument(stack, parserProduction, argumentCollection, argumentFactory);

						stack.RemoveAt(stack.Count - 1);
					}
					else
						argumentFactory(entry, stack, argumentCollection);
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
					Action<Entry, List<Entry>, ProductionArgumentCollection> argumentFactory)
				{
					foreach (var entry in fragmentProduction.Entries)
					{
						stack.Add(entry);

						BuildArgument(stack, parserProduction, argumentCollection, argumentFactory);

						stack.RemoveAt(stack.Count - 1);
					}
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

				private void CreateArgument(Entry entry, List<Entry> stack, ProductionArgumentCollection argumentCollection)
				{
					if (entry is not IParserEntry parserEntry)
						return;

					var argumentType = GetArgumentType(entry);

					if (argumentType == null || ReferenceEquals(parserEntry.ProductionArgument, NullArgument.Instance))
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
						ParserOperandMatchEntry _ => CreateLexerArgument(argumentName, entry, argumentIndex, argumentType, array),
						CompositeOperandEntry _ => CreateLexerArgument(argumentName, entry,argumentIndex, argumentType, array),
						ParserSyntaxEntry _ => CreateParserArgument(argumentName, entry, argumentIndex, argumentType, array),
						_ => null
					};

					if (argument == null)
						return;

					argumentCollection.AddArgument(argument);
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

				private SourceEntityFactory _sourceEntityFactory;

				public SourceEntityFactory SourceEntityFactory => _sourceEntityFactory ??= new SourceEntityFactory(this);

				public void EnsureBinder()
				{
					Binder?.Build();
				}

				private static string EnsureEntryName(Entry entry, List<Entry> stack)
				{
					if (entry is not IParserEntry parserEntry)
						return null;

					var name = EnsureName(parserEntry.GrammarSymbol);

					if (stack.Count > 1 && stack[stack.Count - 2] is ParserQuantifierEntry parserQuantifierEntry)
						return EnsureName(parserQuantifierEntry.GrammarSymbol) ?? name;

					return name;
				}

				public ProductionNamedArgumentCollection GetArguments(string name, StringComparison comparisonType = StringComparison.OrdinalIgnoreCase)
				{
					return new ProductionNamedArgumentCollection(name, this, comparisonType);
				}

				private static Type GetArgumentType(Entry parserEntry)
				{
					return parserEntry switch
					{
						IParserPredicate parserPredicateEntry => GetPredicateArgumentType(parserPredicateEntry),
						ParserSyntaxEntry parserSyntaxEntry => ((Grammar<TGrammar, TToken>.ParserGrammar.NodeSymbol)parserSyntaxEntry.GrammarSymbol).Node.NodeType,
						ParserSetMatchEntry _ => typeof(Lexeme<TToken>),
						ParserOperandMatchEntry _ => typeof(Lexeme<TToken>),
						CompositeOperandEntry _ => typeof(Lexeme<TToken>),
						_ => null
					};
				}

				private static Type GetArrayType(Type elementType)
				{
					if (ArrayTypeDictionary.TryGetValue(elementType, out var arrayType) == false)
						ArrayTypeDictionary[elementType] = arrayType = Array.CreateInstance(elementType, 0).GetType();

					return arrayType;
				}

				private static Entry GetEntrySource(Entry entry)
				{
					if (entry is IParserEntry parserEntry)
						return (Entry)parserEntry.Source ?? entry;

					return entry;
				}

				private static Type GetExternalNodeType(IParserPredicate parserPredicateEntry)
				{
					return parserPredicateEntry.ResultType;
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
						ParserPredicateKind.GenericValue => parserPredicateEntry.ResultType,
						_ => null
					};
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
					if (_productionEntityPoolHead == null)
						return new ProductionEntity(this) { Busy = true };

					var productionEntity = _productionEntityPoolHead;

					_productionEntityPoolHead = productionEntity.PoolNext;

					productionEntity.Busy = true;

					return productionEntity;
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public void ReturnEntity(ProductionEntity productionEntity)
				{
					Debug.Assert(productionEntity.Busy);

					productionEntity.Busy = false;
					productionEntity.SpanStart = 0;
					productionEntity.PoolNext = _productionEntityPoolHead;

					_productionEntityPoolHead = productionEntity;
				}

				public override string ToString()
				{
					return $"{Name} => {EntriesDebuggerDisplay}";
				}
			}
		}
	}
}