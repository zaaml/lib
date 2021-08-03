// <copyright file="Parser.Automata.ParserProduction.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Zaaml.Core.Utils;

// ReSharper disable StaticMemberInGenericType

namespace Zaaml.Text
{
	internal abstract partial class Parser<TGrammar, TToken>
	{
		private sealed partial class ParserAutomata
		{
			private sealed class ParserProduction : Production
			{
				private static readonly Dictionary<Type, Type> ArrayTypeDictionary = new();
				public readonly ProductionBinder Binder;
				private ProductionEntity[] _productionEntityPool;
				private int _productionEntityPoolTail;

				private ParserProduction(IEnumerable<Entry> entries) : base(entries)
				{
					_productionEntityPool = new ProductionEntity[8];
					_productionEntityPoolTail = -1;
				}

				public ParserProduction(ParserAutomata automata, Func<ParserProduction, ProductionBinder> binderFactory, IEnumerable<Entry> entries) : this(entries)
				{
					if (binderFactory == null)
					{
						IsInline = true;

						return;
					}

					IsInline = false;
					Binder = binderFactory(this);
					ProductionIndex = automata.RegisterProduction(this);
				}

				public ParserProduction(ParserAutomata automata, Grammar<TToken>.ParserRule grammarParserRule, Grammar<TToken>.ParserProduction grammarParserProduction)
					: this(grammarParserProduction.Entries.Select(automata.CreateParserEntry))
				{
					Name = grammarParserProduction.Name ?? automata.GenerateProductionName();

					IsInline = grammarParserRule.IsInline;
					GrammarParserProduction = grammarParserProduction;
					ProductionIndex = automata.RegisterProduction(this);

					if (IsInline)
						return;

					BuildArguments();

					Binder = grammarParserProduction.ProductionBinding switch
					{
						Grammar<TToken>.ConstructorBinding ctorBinding => new ConstructorBinder(ctorBinding.NodeType, this),
						Grammar<TToken>.SyntaxFactoryBinding _ => new SyntaxFactoryBinder(this),
						null => null,
						_ => throw new InvalidOperationException("Unknown binder")
					};
				}

				public List<ProductionArgument> Arguments { get; } = new();

				public Grammar<TToken>.ParserProduction GrammarParserProduction { get; }

				public bool IsInline { get; }

				internal bool IsRightAssoc { get; set; }

				internal ParserRuleEntry LeftFactorEntry { get; set; }

				internal ParserProduction LeftFactorProduction { get; set; }

				internal LeftRecursionClassifier LeftRecursionClassifier { get; set; }

				internal ParserRuleEntry LeftRecursionEntry { get; set; }

				private string Name { get; }

				public int ProductionIndex { get; }

				private void BuildArgument(List<Entry> stack, ParserProduction parserProduction)
				{
					var entry = stack[stack.Count - 1];

					if (entry is ParserRuleEntry {Fragment: true} parserStateEntry)
					{
						stack.Add(parserStateEntry);

						foreach (var fragmentProduction in parserStateEntry.Rule.Productions)
							BuildArguments(parserProduction, fragmentProduction, stack);

						stack.RemoveAt(stack.Count - 1);
					}
					else if (entry is ParserQuantifierEntry parserQuantifierEntry)
					{
						var parserPrimitiveEntry = parserQuantifierEntry.PrimitiveEntry;

						stack.Add(parserPrimitiveEntry);

						BuildArgument(stack, parserProduction);

						stack.RemoveAt(stack.Count - 1);
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

						CreateArgument(entry, stack);
					}
					else
						CreateArgument(entry, stack);
				}

				private void BuildArguments()
				{
					BuildArguments(this, this, new List<Entry>());
				}

				private void BuildArguments(ParserProduction parserProduction, Production fragmentProduction, List<Entry> stack)
				{
					foreach (var entry in fragmentProduction.Entries)
					{
						stack.Add(entry);

						BuildArgument(stack, parserProduction);

						stack.RemoveAt(stack.Count - 1);
					}
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

				private void CreateArgument(Entry entry, List<Entry> stack)
				{
					if (entry is not IParserEntry parserEntry)
						return;

					var array = stack.Any(e => e is ParserQuantifierEntry parentQuantifierEntry && parentQuantifierEntry.Maximum - parentQuantifierEntry.Minimum > 1);
					var argumentName = EnsureEntryName(entry, stack);
					var argumentType = GetArgumentType(entry);
					var argumentIndex = Arguments.Count;
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

					parserEntry.ProductionArgument = argument;

					Arguments.Add(argument);
				}

				private ProductionArgument CreateLexerArgument(string name, Entry parserEntry, int argumentIndex, Type argumentType, bool array)
				{
					return array ? new LexerValueArrayArgument(name, parserEntry, argumentIndex, GetArrayType(argumentType), this) : new LexerValueArgument(name, parserEntry, argumentIndex, argumentType, this);
				}

				private ProductionArgument CreateParserArgument(string name, Entry parserEntry, int argumentIndex, Type argumentType, bool array)
				{
					var parserArgumentGenericType = array ? typeof(ParserValueArrayArgument<>) : typeof(ParserValueArgument<>);
					var parserArgumentType = parserArgumentGenericType.MakeGenericType(typeof(TGrammar), typeof(TToken), argumentType);

					return (ProductionArgument) Activator.CreateInstance(parserArgumentType, name, parserEntry, argumentIndex, array ? GetArrayType(argumentType) : argumentType, this);
				}

				private ProductionArgument CreatePredicateArgument(string name, IParserPredicate parserPredicateEntry, int argumentIndex, Type argumentType, bool array)
				{
					return parserPredicateEntry.PredicateKind switch
					{
						ParserPredicateKind.SubParser => CreateParserArgument(name, (Entry) parserPredicateEntry, argumentIndex, argumentType, array),
						//ParserPredicateKind.SubLexer => CreateSubLexerArgumentFactory(parserPredicateEntry),
						_ => null
					};
				}

				public ProductionArgumentCollection GetArguments(string name, StringComparison comparisonType = StringComparison.OrdinalIgnoreCase)
				{
					return new(name, this, comparisonType);
				}

				private static Type GetArgumentType(Entry parserEntry)
				{
					return parserEntry switch
					{
						IParserPredicate parserPredicateEntry => GetPredicateArgumentType(parserPredicateEntry),
						ParserRuleEntry parserRuleEntry => ((Grammar<TToken>.ParserRuleEntry) parserRuleEntry.GrammarEntry).Rule.NodeType,
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

				private static Type GetPredicateArgumentType(IParserPredicate parserPredicateEntry)
				{
					return parserPredicateEntry.PredicateKind switch
					{
						ParserPredicateKind.SubParser => null,
						ParserPredicateKind.SubLexer => GetSubTokenType(parserPredicateEntry),
						_ => null
					};
				}

				//private ProductionArgument CreateSubLexerArgumentFactory(IParserPredicate parserPredicateEntry)
				//{
				//	var tokenType = GetSubTokenType(parserPredicateEntry);
				//	var argumentBuilderType = typeof(ExternalLexerEntityArgumentFactory<>).MakeGenericType(typeof(TGrammar), typeof(TToken), tokenType);
				//	var argumentBuilderCtor = argumentBuilderType.GetConstructors().Single();
				//	var argumentBuilder = (ProductionArgument)argumentBuilderCtor.Invoke(new object[] { this });

				//	return argumentBuilder;
				//}

				private static Type GetSubTokenType(IParserPredicate parserPredicateEntry)
				{
					return parserPredicateEntry.GetType().GetGenericArguments().Last().GetGenericArguments().Single();
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public ProductionEntity RentEntity()
				{
					if (_productionEntityPoolTail == -1)
						Resize();

					var productionInstanceBuilder = _productionEntityPool[_productionEntityPoolTail--];

					productionInstanceBuilder.Busy = true;

					return productionInstanceBuilder;
				}

				private void Resize()
				{
					ArrayUtils.ExpandArrayLength(ref _productionEntityPool, false);

					var length = _productionEntityPool.Length;

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
					return $"{Name} => {string.Join(" ", Entries.Select(e => e.ToString()))}";
				}
			}
		}
	}
}