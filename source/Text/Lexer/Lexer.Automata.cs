// <copyright file="Lexer.Automata.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using Zaaml.Core.Extensions;

namespace Zaaml.Text
{
	internal partial class Lexer<TGrammar, TToken>
	{
		private LexerAutomata _automata;

		private protected LexerAutomata Automata => _automata ??= AutomataManager.Get<LexerAutomata>();

		private protected partial class LexerAutomata : Automata<char, int>
		{
			private readonly Dictionary<string, LexerRule> _lexerStateDictionary = new Dictionary<string, LexerRule>();
			private readonly HashSet<Grammar<TToken>.TokenFragment> _registeredFragments = new HashSet<Grammar<TToken>.TokenFragment>();
			private LexerDfaBuilder _dfaBuilder;
			private int _generatedLexerStateCount;

			static LexerAutomata()
			{
				Converter = c => c;
			}

			public LexerAutomata(AutomataManager manager) : base(manager)
			{
				var grammar = Grammar.Get<TGrammar, TToken>();

				foreach (var tokenFragment in grammar.TokenFragments)
					RegisterLexerFragment(tokenFragment);

				foreach (var tokenRule in grammar.TokenRules.OrderByDescending(r => r.TokenCode))
					RegisterLexerRule(tokenRule);

				BuildStates();
			}

			protected override bool ForceInlineAll => true;

			private void BuildStates()
			{
				_dfaBuilder = new LexerDfaBuilder(_lexerStateDictionary.Values.Where(s => s.TokenRule != null), this);
			}

			private static Action<AutomataContext> CreateActionDelegate(Lexer<TToken>.ActionEntry actionEntry)
			{
				return c => actionEntry.Action(((LexerAutomataContextState) ((LexerAutomataContext) c).ContextStateInternal).LexerContext);
			}

			private Entry CreateLexerEntry(Grammar<TToken>.TokenEntry tokenEntry)
			{
				if (tokenEntry is Grammar<TToken>.QuantifierEntry quantifierEntry)
					return new QuantifierEntry((PrimitiveEntry) CreateLexerEntry(quantifierEntry.PrimitiveEntry), quantifierEntry.Range, quantifierEntry.Mode);

				if (tokenEntry is Grammar<TToken>.MatchEntry matchEntry)
				{
					if (matchEntry is Grammar<TToken>.PrimitiveMatchEntry primitiveMatchEntry)
						return CreateLexerPrimitiveMatchEntry(primitiveMatchEntry);

					if (matchEntry is Grammar<TToken>.SetEntry charGroupEntry)
						return new SetMatchEntry(charGroupEntry.Matches.Select(CreateLexerPrimitiveMatchEntry).ToArray());

					throw new NotImplementedException();
				}

				if (tokenEntry is Grammar<TToken>.TokenFragment tokenFragment)
				{
					var lexerState = GetLexerState(tokenFragment);

					RegisterLexerFragment(tokenFragment);

					return new RuleEntry(lexerState);
				}

				if (tokenEntry is Grammar<TToken>.TokenFragmentEntry tokenFragmentEntry)
				{
					var lexerState = GetLexerState(tokenFragmentEntry.Fragment);

					RegisterLexerFragment(tokenFragmentEntry.Fragment);

					return new RuleEntry(lexerState);
				}

				if (tokenEntry is Grammar<TToken>.LexerPredicate lexerPredicate)
					return new LexerPredicateEntry(lexerPredicate);

				if (tokenEntry is Grammar<TToken>.LexerAction lexerAction)
					return new LexerActionEntry(lexerAction);

				throw new NotImplementedException();
			}

			private LexerRule CreateLexerFragmentState(Grammar<TToken>.TokenFragment tokenFragment)
			{
				return new LexerRule(tokenFragment.Name);
			}

			private static PrimitiveMatchEntry CreateLexerPrimitiveMatchEntry(Grammar<TToken>.PrimitiveMatchEntry match)
			{
				switch (match)
				{
					case Grammar<TToken>.CharEntry charMatch:
						return new SingleMatchEntry(charMatch.Char);

					case Grammar<TToken>.RangeEntry charRangeMatch:
						return new RangeMatchEntry(charRangeMatch.First, charRangeMatch.Last);

					default:
						throw new ArgumentOutOfRangeException();
				}
			}

			private LexerRule CreateLexerRuleState(Grammar<TToken>.TokenRule tokenRule)
			{
				return new LexerRule(tokenRule);
			}

			private static Func<AutomataContext, PredicateResult> CreatePredicateDelegate(Lexer<TToken>.PredicateEntry predicateEntry)
			{
				return c => predicateEntry.Predicate(((LexerAutomataContext) c).LexerContext) ? PredicateResult.True : PredicateResult.False;
			}

			private string GenerateLexerStateName()
			{
				return $"GeneratedLexerState{_generatedLexerStateCount++}";
			}

			private LexerRule GetLexerState(Grammar<TToken>.TokenRule tokenRule)
			{
				if (tokenRule.Name == null)
					tokenRule.Name = GenerateLexerStateName();

				return _lexerStateDictionary.GetValueOrCreate(tokenRule.Name, () => CreateLexerRuleState(tokenRule));
			}

			private LexerRule GetLexerState(Grammar<TToken>.TokenFragment tokenFragment)
			{
				if (tokenFragment.Name == null)
					tokenFragment.Name = GenerateLexerStateName();

				return _lexerStateDictionary.GetValueOrCreate(tokenFragment.Name, () => CreateLexerFragmentState(tokenFragment));
			}

			private void RegisterLexerFragment(Grammar<TToken>.TokenFragment tokenFragment)
			{
				if (_registeredFragments.Add(tokenFragment) == false)
					return;

				var lexerState = GetLexerState(tokenFragment);
				var transitions = tokenFragment.Pattern.Patterns.Select(pattern => new LexerProduction(pattern.Entries.Select(CreateLexerEntry))).ToList();

				AddState(lexerState, transitions);
			}

			private void RegisterLexerRule(Grammar<TToken>.TokenRule tokenRule)
			{
				var lexerState = GetLexerState(tokenRule);
				var transitions = tokenRule.Pattern.Patterns.Select(pattern => new LexerProduction(pattern.Entries.Select(CreateLexerEntry))).ToList();

				AddState(lexerState, transitions);
			}
		}
	}
}