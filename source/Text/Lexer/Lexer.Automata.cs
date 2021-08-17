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
			private readonly Dictionary<Grammar<TGrammar, TToken>.LexerGrammar.Syntax, LexerRule> _lexerFragmentDictionary = new();
			private readonly Dictionary<Grammar<TGrammar, TToken>.LexerGrammar.Syntax, List<LexerRule>> _lexerRuleDictionary = new();

			private LexerDfaBuilder _dfaBuilder;

			static LexerAutomata()
			{
				FromConverter = ch => ch + 1;
				ToConverter = code => code - 1;
			}

			public LexerAutomata(AutomataManager manager) : base(manager)
			{
				var grammar = Grammar.Get<TGrammar, TToken>();

				foreach (var fragment in grammar.LexerSyntaxFragmentCollection)
					RegisterLexerSyntax(fragment);

				foreach (var trivia in grammar.LexerSyntaxTriviaCollection)
					RegisterLexerSyntax(trivia);

				foreach (var token in grammar.LexerSyntaxTokenCollection)
					RegisterLexerSyntax(token);

				Build();
				BuildStates();
			}

			protected override bool ForceInlineAll => true;

			private void BuildStates()
			{
				var rules = _lexerRuleDictionary.Values.SelectMany(l => l).OrderByDescending(p => p.TokenCode);

				_dfaBuilder = new LexerDfaBuilder(rules, this);
			}

			private static Action<AutomataContext> CreateActionDelegate(Lexer<TToken>.ActionEntry actionEntry)
			{
				return c => actionEntry.Action(((LexerAutomataContextState)((LexerAutomataContext)c).ContextStateInternal).LexerContext);
			}

			private Entry CreateLexerEntry(Grammar<TGrammar, TToken>.LexerGrammar.Symbol lexerEntry)
			{
				if (lexerEntry is Grammar<TGrammar, TToken>.LexerGrammar.QuantifierSymbol quantifierSymbol)
					return new QuantifierEntry((PrimitiveEntry)CreateLexerEntry(quantifierSymbol.Symbol), quantifierSymbol.Range, quantifierSymbol.Mode);

				if (lexerEntry is Grammar<TGrammar, TToken>.LexerGrammar.MatchSymbol matchSymbol)
				{
					if (matchSymbol is Grammar<TGrammar, TToken>.LexerGrammar.PrimitiveMatchSymbol primitiveMatchEntry)
						return CreateLexerPrimitiveMatchEntry(primitiveMatchEntry);

					if (matchSymbol is Grammar<TGrammar, TToken>.LexerGrammar.CharSetSymbol charSetSymbol)
						return new SetMatchEntry(charSetSymbol.Matches.Select(CreateLexerPrimitiveMatchEntry).ToArray());

					throw new NotImplementedException();
				}

				if (lexerEntry is Grammar<TGrammar, TToken>.LexerGrammar.FragmentSymbol fragmentSymbol)
					return new RuleEntry(GetLexerRule(fragmentSymbol.Fragment));

				if (lexerEntry is Grammar<TGrammar, TToken>.LexerGrammar.PredicateSymbol predicateSymbol)
					return new LexerPredicateEntry(predicateSymbol);

				if (lexerEntry is Grammar<TGrammar, TToken>.LexerGrammar.ActionSymbol actionSymbol)
					return new LexerActionEntry(actionSymbol);

				throw new NotImplementedException();
			}

			private static PrimitiveMatchEntry CreateLexerPrimitiveMatchEntry(Grammar<TGrammar, TToken>.LexerGrammar.PrimitiveMatchSymbol match)
			{
				switch (match)
				{
					case Grammar<TGrammar, TToken>.LexerGrammar.CharSymbol charMatch:
						return new SingleMatchEntry(charMatch.Char);

					case Grammar<TGrammar, TToken>.LexerGrammar.CharRangeSymbol charRangeMatch:
						return new RangeMatchEntry(charRangeMatch.First, charRangeMatch.Last);

					default:
						throw new ArgumentOutOfRangeException();
				}
			}

			private LexerRule CreateLexerRule(Grammar<TGrammar, TToken>.LexerGrammar.Syntax syntax)
			{
				var lexerRule = new LexerRule(syntax);

				if (syntax is not Grammar<TGrammar, TToken>.LexerGrammar.FragmentSyntax)
					throw new InvalidOperationException();

				var productions = syntax.Productions.Select(production => new LexerProduction(production.Symbols.Select(CreateLexerEntry))).ToList();

				AddRule(lexerRule, productions);

				return lexerRule;
			}

			private static Func<AutomataContext, PredicateResult> CreatePredicateDelegate(Lexer<TToken>.PredicateEntry predicateEntry)
			{
				return c => predicateEntry.Predicate(((LexerAutomataContext)c).LexerContext) ? PredicateResult.True : PredicateResult.False;
			}

			private LexerRule GetLexerRule(Grammar<TGrammar, TToken>.LexerGrammar.Syntax syntax)
			{
				return _lexerFragmentDictionary.GetValueOrCreate(syntax, CreateLexerRule);
			}

			private void RegisterLexerSyntax(Grammar<TGrammar, TToken>.LexerGrammar.Syntax syntax)
			{
				if (syntax is not Grammar<TGrammar, TToken>.LexerGrammar.TokenBaseSyntax syntaxTokenBase)
					return;

				var list = new List<LexerRule>();

				foreach (var tokenGroup in syntaxTokenBase.TokenGroups)
				{
					foreach (var production in tokenGroup.Productions)
					{
						var lexerRule = new LexerRule(syntax, production);
						var productions = new List<LexerProduction> { new(production.Symbols.Select(CreateLexerEntry)) };

						list.Add(lexerRule);

						AddRule(lexerRule, productions);
					}
				}

				_lexerRuleDictionary.Add(syntax, list);
			}
		}
	}
}