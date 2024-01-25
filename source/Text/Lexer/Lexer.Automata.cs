// <copyright file="Lexer.Automata.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Zaaml.Core.Extensions;

namespace Zaaml.Text
{
	internal partial class Lexer<TGrammar, TToken>
	{
		private LexerAutomata _automata;

		private protected LexerAutomata Automata => _automata ??= AutomataManager.Get<LexerAutomata>();

		private protected partial class LexerAutomata : Automata<char, int>
		{
			private readonly Dictionary<Grammar<TGrammar, TToken>.LexerGrammar.Syntax, LexerSyntax> _lexerFragmentDictionary = new();
			private readonly Dictionary<Grammar<TGrammar, TToken>.LexerGrammar.Syntax, List<LexerSyntax>> _lexerSyntaxDictionary = new();

			private LexerDfaBuilder _dfaBuilder;

			static LexerAutomata()
			{
				FromConverter = ch => ch + 1;
				ToConverter = code => code - 1;
			}

			public LexerAutomata(AutomataManager manager) : base(manager)
			{
				var grammar = Grammar.Get<TGrammar, TToken>();

				grammar.LexerGrammarInstance.Seal();

				foreach (var fragment in grammar.LexerSyntaxFragmentCollection)
					RegisterLexerSyntax(fragment);

				foreach (var trivia in grammar.LexerSyntaxTriviaCollection)
					RegisterLexerSyntax(trivia);

				foreach (var token in grammar.LexerSyntaxTokenCollection)
				{
					if (token.Composite == false) 
						RegisterLexerSyntax(token);
				}

				Build();
				BuildStates();
			}

			protected override bool ForceInlineAll => true;

			private protected override Pool<InstructionStream> CreateInstructionStreamPool()
			{
				throw new NotImplementedException();
			}

			private void BuildStates()
			{
				var syntaxes = _lexerSyntaxDictionary.Values.SelectMany(l => l).OrderByDescending(p => p.TokenCode);

				_dfaBuilder = new LexerDfaBuilder(syntaxes, this);
			}

			private static Action<AutomataContext> CreateActionDelegate(Lexer<TToken>.ActionEntry actionEntry)
			{
				return c => actionEntry.Action(((LexerAutomataContext)c).Lexer);
			}

			private Entry CreateLexerEntry(Grammar<TGrammar, TToken>.LexerGrammar.Symbol symbol)
			{
				return symbol switch
				{
					Grammar<TGrammar, TToken>.LexerGrammar.QuantifierSymbol quantifierSymbol => CreateQuantifierEntry(quantifierSymbol),
					Grammar<TGrammar, TToken>.LexerGrammar.PrimitiveMatchSymbol primitiveMatchSymbol => CreateLexerPrimitiveMatchEntry(primitiveMatchSymbol),
					Grammar<TGrammar, TToken>.LexerGrammar.FragmentSymbol fragmentSymbol => CreateSyntaxEntry(fragmentSymbol),
					Grammar<TGrammar, TToken>.LexerGrammar.TokenSymbol tokenSymbol => CreateSyntaxEntry(tokenSymbol),
					Grammar<TGrammar, TToken>.LexerGrammar.ExternalNodeSymbol externalNodeSymbol => CreateExternalParserEntry(externalNodeSymbol),
					Grammar<TGrammar, TToken>.LexerGrammar.PredicateSymbol predicateSymbol => CreateLexerPredicateEntry(predicateSymbol),
					Grammar<TGrammar, TToken>.LexerGrammar.ActionSymbol actionSymbol => CreateLexerActionEntry(actionSymbol),
					_ => throw new NotImplementedException()
				};
			}

			private Entry CreateExternalParserEntry(Grammar<TGrammar, TToken>.LexerGrammar.ExternalNodeSymbol externalNodeSymbol)
			{
				var externalGrammarType = externalNodeSymbol.ExternalGrammarType;
				var externalTokenType = externalNodeSymbol.ExternalTokenType;

				var typeArguments = externalNodeSymbol.ExternalNodeType == null ? new[] { externalGrammarType, externalTokenType } : new[] { externalGrammarType, externalTokenType, externalNodeSymbol.ExternalNodeType };

				var externalParserMethod = GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic).Single(m => m.Name == nameof(CreateExternalParserEntry) && m.GetGenericArguments().Length == typeArguments.Length);
				var externalParserGenericMethod = externalParserMethod.MakeGenericMethod(typeArguments.ToArray());

				return (Entry)externalParserGenericMethod.Invoke(this, new object[] { externalNodeSymbol });
			}

			private Entry CreateExternalParserEntry<TExternalGrammar, TExternalToken, TExternalNode>(Grammar<TGrammar, TToken>.LexerGrammar.ExternalNodeSymbol<TExternalGrammar, TExternalToken, TExternalNode> externalNodeSymbol)
				where TExternalGrammar : Grammar<TExternalGrammar, TExternalToken>
				where TExternalToken : unmanaged, Enum
				where TExternalNode : class
			{
				return new ExternalParserDelegate<TExternalGrammar, TExternalToken, TExternalNode>(externalNodeSymbol).PredicateEntry;
			}

			private static Entry CreateLexerActionEntry(Grammar<TGrammar, TToken>.LexerGrammar.ActionSymbol actionSymbol)
			{
				return new LexerActionEntry(actionSymbol);
			}

			private static Entry CreateLexerPredicateEntry(Grammar<TGrammar, TToken>.LexerGrammar.PredicateSymbol predicateSymbol)
			{
				return new LexerPredicateEntry(predicateSymbol);
			}

			private Entry CreateSyntaxEntry(Grammar<TGrammar, TToken>.LexerGrammar.FragmentSymbol fragmentSymbol)
			{
				return new SyntaxEntry(GetLexerSyntax(fragmentSymbol.Fragment));
			}

			private Entry CreateSyntaxEntry(Grammar<TGrammar, TToken>.LexerGrammar.TokenSymbol tokenSymbol)
			{
				return new SyntaxEntry(GetLexerSyntax(tokenSymbol.Token));
			}

			private Entry CreateQuantifierEntry(Grammar<TGrammar, TToken>.LexerGrammar.QuantifierSymbol quantifierSymbol)
			{
				var primitiveEntry = (PrimitiveEntry)CreateLexerEntry(quantifierSymbol.Symbol);

				return new QuantifierEntry(primitiveEntry, quantifierSymbol.Range, quantifierSymbol.Mode);
			}

			private static PrimitiveMatchEntry CreateLexerPrimitiveMatchEntry(Grammar<TGrammar, TToken>.LexerGrammar.PrimitiveMatchSymbol match)
			{
				return match switch
				{
					Grammar<TGrammar, TToken>.LexerGrammar.CharSymbol charMatch => new OperandMatchEntry(charMatch.Char),
					Grammar<TGrammar, TToken>.LexerGrammar.CharRangeSymbol charRangeMatch => new RangeMatchEntry(charRangeMatch.First, charRangeMatch.Last),
					Grammar<TGrammar, TToken>.LexerGrammar.CharSetSymbol charSetSymbol => new SetMatchEntry(charSetSymbol.Matches.Select(CreateLexerPrimitiveMatchEntry).ToArray()),
					_ => throw new ArgumentOutOfRangeException()
				};
			}

			private LexerSyntax CreateLexerSyntax(Grammar<TGrammar, TToken>.LexerGrammar.Syntax grammarSyntax)
			{
				var lexerSyntax = new LexerSyntax(grammarSyntax);

				if (grammarSyntax is not Grammar<TGrammar, TToken>.LexerGrammar.FragmentSyntax)
					throw new InvalidOperationException();

				var productions = grammarSyntax.Productions.Select(production => new LexerProduction(production.Symbols.Select(CreateLexerEntry))).ToList();

				AddSyntax(lexerSyntax, productions);

				return lexerSyntax;
			}

			private static Func<AutomataContext, PredicateResult> CreatePredicateDelegate(Lexer<TToken>.PredicateEntry predicateEntry)
			{
				return c => predicateEntry.Predicate(((LexerAutomataContext)c).Lexer) ? PredicateResult.True : PredicateResult.False;
			}

			private LexerSyntax GetLexerSyntax(Grammar<TGrammar, TToken>.LexerGrammar.Syntax syntax)
			{
				return _lexerFragmentDictionary.GetValueOrCreate(syntax, CreateLexerSyntax);
			}

			private void RegisterLexerSyntax(Grammar<TGrammar, TToken>.LexerGrammar.Syntax syntax)
			{
				if (syntax is not Grammar<TGrammar, TToken>.LexerGrammar.TokenBaseSyntax syntaxTokenBase)
					return;

				var list = new List<LexerSyntax>();

				foreach (var tokenGroup in syntaxTokenBase.TokenGroups)
				{
					foreach (var production in tokenGroup.Productions)
					{
						var lexerSyntax = new LexerSyntax(syntax, production);
						var productions = new List<LexerProduction> { new(production.Symbols.Select(CreateLexerEntry)) };

						list.Add(lexerSyntax);

						AddSyntax(lexerSyntax, productions);
					}
				}

				_lexerSyntaxDictionary.Add(syntax, list);
			}
		}
	}
}