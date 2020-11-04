// <copyright file="Parser.Automata.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Zaaml.Core.Extensions;

namespace Zaaml.Text
{
	internal abstract partial class Parser<TGrammar, TToken> where TGrammar : Grammar<TToken> where TToken : unmanaged, Enum
	{
		#region Nested Types

		private sealed partial class ParserAutomata : ParserAutomataBase<TGrammar, TToken>
		{
			#region Fields

			//private readonly Parser<TGrammar, TToken> _parser;
			private readonly HashSet<Grammar<TToken>.ParserRule> _registeredRules = new HashSet<Grammar<TToken>.ParserRule>();
			private int _generateProductionNameCount;

			#endregion

			#region Ctors

			public ParserAutomata(AutomataManager manager) : base(manager)
			{
				//_parser = parser;

				var grammar = Grammar.Get<TGrammar, TToken>();

				foreach (var parserRule in grammar.ParserRules)
					RegisterParserRule(parserRule);
			}

			#endregion

			#region Properties

			protected override bool AllowParallelInstructionReader => false; //_parser.AllowParallel;

			private Dictionary<Grammar<TToken>.ParserRule, ParserState> ParserStateDictionary { get; } = new Dictionary<Grammar<TToken>.ParserRule, ParserState>();

			private List<ParserProduction> Productions { get; } = new List<ParserProduction>();

			#endregion

			#region Methods

			private static LeftRecursionClassifier Classify(ParserProduction production, ParserState parserState)
			{
				if (production.Entries.Length == 0)
					return LeftRecursionClassifier.Primary;

				var firstRecursion = IsRecursion(production.Entries[0], parserState);
				var lastRecursion = IsRecursion(production.Entries[production.Entries.Length - 1], parserState);

				if (production.Entries.Length == 1)
					return firstRecursion ? LeftRecursionClassifier.Generic : LeftRecursionClassifier.Primary;

				if (production.Entries.Length == 3 && IsRecursion(production.Entries[1], parserState) == false && firstRecursion && lastRecursion)
					return LeftRecursionClassifier.Binary;

				if (production.Entries.Length > 3 && firstRecursion && lastRecursion)
					return LeftRecursionClassifier.Ternary;

				if (firstRecursion && lastRecursion == false)
					return LeftRecursionClassifier.Suffix;

				if (firstRecursion == false && lastRecursion)
					return LeftRecursionClassifier.Prefix;

				return firstRecursion ? LeftRecursionClassifier.Generic : LeftRecursionClassifier.Primary;
			}

			private static Action<AutomataContext> CreateActionDelegate(Parser<TToken>.ActionEntry actionEntry)
			{
				return c => actionEntry.Action(((ParserAutomataContextState) ((ParserAutomataContext) c).ContextStateInternal)?.ParserContext);
			}

			private Grammar<TToken>.ParserRule CreateFragmentRule(Grammar<TToken>.ParserFragment parserFragment)
			{
				var rule = new Grammar<TToken>.ParserRule
				{
					IsInline = true
				};

				if (parserFragment.Productions.All(t => t.Entries.Length == 1 && t.Entries[0] is Grammar<TToken>.TokenRule))
				{
					var setEntry = new Grammar<TToken>.TokenRuleSet(parserFragment.Productions.Select(t => (Grammar<TToken>.TokenRule) t.Entries[0]).ToArray())
					{
						Name = parserFragment.Name
					};

					rule.Productions.Add(new Grammar<TToken>.ParserProduction(new Grammar<TToken>.ParserEntry[] {setEntry}));
				}
				else
				{
					foreach (var transition in parserFragment.Productions)
						rule.Productions.Add(new Grammar<TToken>.ParserProduction(transition.Entries));
				}

				RegisterParserRule(rule);

				return rule;
			}

			private static ParserSingleMatchEntry CreateLexerEntry(Grammar<TToken>.ParserTokenRuleEntry tokenEntry)
			{
				return new ParserSingleMatchEntry(EnsureName(tokenEntry), tokenEntry.TokenRule.Token);
			}

			private static ParserSingleMatchEntry CreateLexerEntry(Grammar<TToken>.TokenRule tokenRule)
			{
				return new ParserSingleMatchEntry(EnsureName(tokenRule), tokenRule.Token);
			}

			private static ParserSetMatchEntry CreateLexerSetEntry(Grammar<TToken>.TokenRuleSet tokenRuleEntrySet)
			{
				return new ParserSetMatchEntry(tokenRuleEntrySet);
			}

			private Entry CreateParserEntry(Grammar<TToken>.ParserEntry grammarParserEntry)
			{
				if (grammarParserEntry is Grammar<TToken>.ParserQuantifierEntry quantifierEntry)
				{
					var primitiveEntry = (PrimitiveEntry) CreateParserEntry(quantifierEntry.PrimitiveEntry);

					return new ParserQuantifierEntry(quantifierEntry, primitiveEntry, quantifierEntry.Range, quantifierEntry.Mode);
				}

				if (grammarParserEntry is Grammar<TToken>.TokenRuleSet tokenRuleEntrySet)
					return CreateLexerSetEntry(tokenRuleEntrySet);

				if (grammarParserEntry is Grammar<TToken>.TokenRule tokenRule)
					return CreateLexerEntry(new Grammar<TToken>.ParserTokenRuleEntry(tokenRule));

				if (grammarParserEntry is Grammar<TToken>.ParserTokenRuleEntry tokenRuleEntry)
					return CreateLexerEntry(tokenRuleEntry);

				if (grammarParserEntry is Grammar<TToken>.ParserRuleEntry parserRuleEntry)
				{
					var rule = parserRuleEntry.Rule;

					if (rule.IsInline)
					{
						var ruleClone = new Grammar<TToken>.ParserRule
						{
							IsInline = true
						};

						foreach (var transition in rule.Productions)
							ruleClone.Productions.Add(new Grammar<TToken>.ParserProduction(transition.Entries));

						RegisterParserRule(ruleClone);

						return new ParserStateEntry(EnsureName(parserRuleEntry), GetParserState(ruleClone), true, false);
					}

					return new ParserStateEntry(EnsureName(parserRuleEntry), GetParserState(rule), false, parserRuleEntry.IsReturn);
				}

				if (grammarParserEntry is Grammar<TToken>.ParserFragment parserFragment)
				{
					var fragmentRule = CreateFragmentRule(parserFragment);

					return new ParserStateEntry(EnsureName(parserFragment), GetParserState(fragmentRule), true, false);
				}

				if (grammarParserEntry is Grammar<TToken>.ParserPredicate parserPredicate)
					return new ParserPredicateEntry(parserPredicate);

				if (grammarParserEntry is Grammar<TToken>.ParserAction parserAction)
					return new ParserActionEntry(parserAction);

				if (grammarParserEntry is Grammar<TToken>.SubParserEntry subParserEntry)
					return CreateSubParserEntry(subParserEntry);

				if (grammarParserEntry is Grammar<TToken>.SubLexerEntry subLexerEntry)
					return CreateSubLexerEntry(subLexerEntry);

				throw new ArgumentOutOfRangeException(nameof(grammarParserEntry));
			}

			private static Func<AutomataContext, PredicateResult> CreatePredicateDelegate(Parser<TToken>.PredicateEntry predicateEntry)
			{
				return c => predicateEntry.Predicate(((ParserAutomataContextState) ((ParserAutomataContext) c).ContextStateInternal).ParserContext) ? PredicateResult.True : PredicateResult.False;
			}

			private Entry CreateSubLexerEntry(Grammar<TToken>.SubLexerEntry subLexerEntry)
			{
				var type = subLexerEntry.GetType();
				var typeArguments = type.GenericTypeArguments.Skip(1).ToArray();
				var subParserMethod = GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic).Single(m => m.Name == nameof(CreateSubLexerEntry) && m.GetGenericArguments().Length == typeArguments.Length);
				var subParserGenericMethod = subParserMethod.MakeGenericMethod(typeArguments);

				return (Entry) subParserGenericMethod.Invoke(this, new object[] {subLexerEntry});
			}

			private Entry CreateSubLexerEntry<TSubToken>(Grammar<TToken>.SubLexerEntry<TSubToken> subLexerEntry) where TSubToken : unmanaged, Enum
			{
				var subLexerInvokeInfo = new SubLexerInvokeInfo<TSubToken>(subLexerEntry);

				return new ParserPredicateEntry<Lexeme<TSubToken>>(subLexerEntry, c => subLexerInvokeInfo.SubLex(c), ParserPredicateKind.SubLexer);
			}

			private Entry CreateSubParserEntry(Grammar<TToken>.SubParserEntry subParserEntry)
			{
				var type = subParserEntry.GetType();
				var typeArguments = type.GenericTypeArguments.Skip(1).ToList();

				typeArguments.Insert(0, subParserEntry.GrammarType);

				var subParserMethod = GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic).Single(m => m.Name == nameof(CreateSubParserEntry) && m.GetGenericArguments().Length == typeArguments.Count);
				var subParserGenericMethod = subParserMethod.MakeGenericMethod(typeArguments.ToArray());

				return (Entry) subParserGenericMethod.Invoke(this, new object[] {subParserEntry});
			}

			private Entry CreateSubParserEntry<TSubGrammar, TSubToken, TSubNode, TSubNodeBase>(Grammar<TToken>.SubParserEntry<TSubToken, TSubNode, TSubNodeBase> subParserEntry)
				where TSubGrammar : Grammar<TSubToken, TSubNodeBase> where TSubToken : unmanaged, Enum where TSubNode : TSubNodeBase where TSubNodeBase : class
			{
				return new SubParserInvokeInfo<TSubGrammar, TSubToken, TSubNode, TSubNodeBase>(subParserEntry).PredicateEntry;
			}

			private Entry CreateSubParserEntry<TSubGrammar, TSubToken>(Grammar<TToken>.SubParserEntry<TSubToken> subParserEntry)
				where TSubGrammar : Grammar<TSubToken> where TSubToken : unmanaged, Enum
			{
				return new SubParserInvokeInfo<TSubGrammar, TSubToken>(subParserEntry).PredicateEntry;
			}

			private static void EliminateLeftRecursion(ParserState parserState, List<ParserProduction> productions)
			{
				var hasLeftRecursion = false;

				foreach (var stateProduction in productions)
				{
					stateProduction.LeftRecursionClassifier = Classify(stateProduction, parserState);

					hasLeftRecursion |= IsLeftRecursion(stateProduction.LeftRecursionClassifier);
				}

				if (hasLeftRecursion == false)
					return;

				var count = productions.Count;
				var recursionState = new ParserState(null, true);
				var index = 0;

				foreach (var production in productions)
				{
					if (production.LeftRecursionClassifier == LeftRecursionClassifier.Binary ||
					    production.LeftRecursionClassifier == LeftRecursionClassifier.Ternary ||
					    production.LeftRecursionClassifier == LeftRecursionClassifier.Prefix)
					{
						var assoc = production.IsRightAssoc ? 0 : 1;
						var nextPriority = new PriorityStateEntryContext(count - index + assoc);

						foreach (var parserStateEntry in production.Entries.Skip(1).OfType<ParserStateEntry>().Where(e => ReferenceEquals(e.State, parserState)))
							parserStateEntry.StateEntryContext = nextPriority;
					}

					if (IsLeftRecursion(production.LeftRecursionClassifier) == false)
						continue;

					production.LeftRecursionEntry = (ParserStateEntry) production.Entries[0];
					production.Entries[0] = ShouldPrefixPredicate(production.LeftRecursionClassifier) ? (Entry) new LeftRecursionPredicate(parserState, count - index).Entry : EpsilonEntry.Instance;

					recursionState.Productions.Add(production);

					index++;
				}

				var quantifierEntry = new QuantifierEntry(new StateEntry(recursionState) { SkipStack = true }, QuantifierKind.ZeroOrMore, QuantifierMode.Greedy);
				var primaryState = new ParserState(null, true);

				primaryState.Productions.AddRange(productions.Where(production => IsLeftRecursion(production.LeftRecursionClassifier) == false));

				productions.Clear();
				productions.Add(new ParserProduction(new Entry[] { new StateEntry(primaryState) { SkipStack = true }, quantifierEntry }));
			}

			private static string EnsureName(Grammar<TToken>.ParserEntry parserEntry)
			{
				if (parserEntry == null)
					return null;

				if (string.IsNullOrEmpty(parserEntry.Name) == false)
					return parserEntry.Name;

				return parserEntry switch
				{
					Grammar<TToken>.ParserRuleEntry ruleEntry => ruleEntry.Rule.Name,
					Grammar<TToken>.TokenRule tokenEntry => tokenEntry.Name,
					Grammar<TToken>.ParserTokenRuleEntry tokenRuleEntry => tokenRuleEntry.Name ?? tokenRuleEntry.TokenRule.Name,
					Grammar<TToken>.ParserQuantifierEntry quantifierEntry => quantifierEntry.Name ?? EnsureName(quantifierEntry.PrimitiveEntry),
					Grammar<TToken>.SubParserEntry subParserEntry => subParserEntry.Name,
					Grammar<TToken>.SubLexerEntry subLexerEntry => subLexerEntry.Name,
					_ => null
				};
			}

			private string GenerateProductionName()
			{
				return $"GeneratedTransition{_generateProductionNameCount++}";
			}

			private static ParserEntryData GetParserEntryData(Entry entry)
			{
				return ((IParserEntry) entry).ParserEntryData;
			}

			private ParserState GetParserState(Grammar<TToken>.ParserRule parserRule)
			{
				return ParserStateDictionary.GetValueOrCreate(parserRule, pr => new ParserState(pr.Name, parserRule.IsInline));
			}

			private static bool IsLeftRecursion(LeftRecursionClassifier classifier)
			{
				return (classifier == LeftRecursionClassifier.Primary || classifier == LeftRecursionClassifier.Prefix) == false;
			}

			private static bool IsRecursion(Entry entry, ParserState state)
			{
				return entry is ParserStateEntry parserStateEntry && ReferenceEquals(parserStateEntry.State, state);
			}

			public TResult Parse<TResult>(Grammar<TToken>.ParserRule parserRule, LexemeSource<TToken> lexemeSource, ParserContext parserContext, Parser<TGrammar, TToken> parser)
			{
				using var context = GetParserState(parserRule).MountSyntaxTreeContext(this, lexemeSource, parserContext, parser);

				RunCore(lexemeSource, context);

				return context.GetResult<TResult>();
			}

			public void Parse(Grammar<TToken>.ParserRule parserRule, LexemeSource<TToken> lexemeSource, ParserContext parserContext, Parser<TGrammar, TToken> parser)
			{
				using var context = GetParserState(parserRule).MountProcessContext(this, lexemeSource, parserContext, parser);

				RunCore(lexemeSource, context);
			}

			public TResult Parse<TResult>(Visitor<TResult> visitor, Grammar<TToken>.ParserRule parserRule, LexemeSource<TToken> lexemeSource, ParserContext parserContext, Parser<TGrammar, TToken> parser)
			{
				using var context = GetParserState(parserRule).MountVisitorContext(visitor, this, lexemeSource, parserContext, parser);

				RunCore(lexemeSource, context);

				return context.GetResult<TResult>();
			}

			private void RegisterParserRule(Grammar<TToken>.ParserRule parserRule)
			{
				if (_registeredRules.Add(parserRule) == false)
					throw new InvalidOperationException();

				var parserState = GetParserState(parserRule);
				var parserProductions = parserRule.Productions.Select(t => new ParserProduction(this, parserRule, t, Productions)).ToList();

				//parserState.Inline |= parserRule.AggressiveInlining;

				EliminateLeftRecursion(parserState, parserProductions);

				AddState(parserState, parserProductions);
			}

			private static bool ShouldPrefixPredicate(LeftRecursionClassifier classifier)
			{
				return classifier == LeftRecursionClassifier.Binary ||
				       classifier == LeftRecursionClassifier.Ternary ||
				       classifier == LeftRecursionClassifier.Suffix;
			}

			#endregion
		}

		#endregion
	}
}