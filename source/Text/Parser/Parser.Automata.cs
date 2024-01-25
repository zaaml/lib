// <copyright file="Parser.Automata.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Zaaml.Core.Converters;

namespace Zaaml.Text
{
	internal partial class Parser<TGrammar, TToken>
	{
		private sealed partial class ParserAutomata : Automata<Lexeme<TToken>, TToken>
		{
			private readonly Dictionary<Grammar<TGrammar, TToken>.ParserGrammar.Syntax, int> _precedenceDictionary = new();
			private int _generateProductionNameCount;

			public ParserAutomata(AutomataManager manager) : base(manager)
			{
				var grammar = Grammar.Get<TGrammar, TToken>();

				grammar.ParserGrammarInstance.Seal();

				var undefinedTokenName = Enum.GetName(typeof(TToken), grammar.UndefinedToken);

				foreach (TToken token in Enum.GetValues(typeof(TToken)))
				{
					var tokenCode = (int)EnumConverter<TToken>.Convert(token);

					if (grammar.UndefinedToken.Equals(token) && Enum.GetName(typeof(TToken), token) == undefinedTokenName)
					{
						if (tokenCode != 0)
							throw new InvalidOperationException("Undefined token must have value of 0.");

						continue;
					}

					if (tokenCode == 0)
						throw new InvalidOperationException("No token except of Grammar.UndefinedToken may have value of 0.");
				}

				foreach (var parserSyntax in grammar.ParserSyntaxFragmentCollection)
					RegisterParserSyntax(parserSyntax);

				foreach (var parserSyntax in grammar.NodeCollection)
					RegisterParserSyntax(parserSyntax);

				foreach (var parserSyntax in ParserSyntaxDictionary.Values)
				{
					EliminateLeftRecursion(parserSyntax);
					EliminateLeftFactoring(parserSyntax);
				}

				foreach (var parserProduction in Productions)
					parserProduction.EnsureBinder();

				ProductionsArray = Productions.ToArray();

				Build();
			}

			protected override bool AllowParallelInstructionReader => false; //_parser.AllowParallel;

			protected override bool LookAheadEnabled => true;

			private Dictionary<Grammar<TGrammar, TToken>.ParserGrammar.Syntax, ParserSyntax> ParserSyntaxDictionary { get; } = new();

			private List<ParserProduction> Productions { get; } = new();

			private ParserProduction[] ProductionsArray { get; }

			private static Action<AutomataContext> CreateActionDelegate(Parser<TToken>.ActionEntry actionEntry)
			{
				return c => actionEntry.Action(((ParserAutomataContext)c).Parser);
			}

			private static ParserActionEntry CreateActionEntry(Grammar<TGrammar, TToken>.ParserGrammar.ActionSymbol actionSymbol)
			{
				return new ParserActionEntry(actionSymbol);
			}

			private static IEnumerable<Entry> CreateCompositeTokenEntries(Grammar<TGrammar, TToken>.ParserGrammar.TokenSymbol tokenSymbol)
			{
				var grammarSyntax = tokenSymbol.Token;

				if (grammarSyntax.Productions.Count > 1)
					throw new InvalidOperationException("Composite tokens can not have multiple productions.");

				var production = grammarSyntax.Productions.Single();

				foreach (var lexerSymbol in production.Symbols)
				{
					if (lexerSymbol is not Grammar<TGrammar, TToken>.LexerGrammar.TokenSymbol simpleTokenSymbol)
						throw new InvalidOperationException("Composite token can consist of simple token array.");

					var parserSymbol = new Grammar<TGrammar, TToken>.ParserGrammar.TokenSymbol(simpleTokenSymbol.Token);

					yield return new ParserOperandMatchEntry(parserSymbol)
					{
						ProductionArgument = NullArgument.Instance
					};
				}

				var compositeOperandPredicate = new CompositeOperandPredicate(tokenSymbol);

				yield return new PredicateEntry(compositeOperandPredicate.Predicate);
				yield return new CompositeOperandEntry(tokenSymbol);
			}

			private Entry CreateEnterPrecedenceEntry(Grammar<TGrammar, TToken>.ParserGrammar.EnterPrecedenceSymbol enterPrecedenceSymbol)
			{
				var productionPrecedence = CreateProductionPrecedence(enterPrecedenceSymbol.Syntax, enterPrecedenceSymbol.Value, enterPrecedenceSymbol.Level);
				var precedenceEntry = new EnterPrecedenceEntry(productionPrecedence);

				return precedenceEntry;
			}

			private static IEnumerable<Grammar<TGrammar, TToken>.ParserGrammar.Symbol> RenameFragmentSymbols(Grammar<TGrammar, TToken>.ParserGrammar.Production production, string name)
			{
				foreach (var childSymbol in production.Symbols)
				{
					switch (childSymbol)
					{
						case Grammar<TGrammar, TToken>.ParserGrammar.TokenSymbol childTokenSymbol:
							yield return new Grammar<TGrammar, TToken>.ParserGrammar.TokenSymbol(childTokenSymbol.Token)
							{
								ArgumentName = name
							};
							break;
						case Grammar<TGrammar, TToken>.ParserGrammar.TokenSetSymbol childTokenSetSymbol:
							yield return new Grammar<TGrammar, TToken>.ParserGrammar.TokenSetSymbol(childTokenSetSymbol.Tokens)
							{
								ArgumentName = name
							};
							break;
						case Grammar<TGrammar, TToken>.ParserGrammar.NodeSymbol nodeSymbol:
							yield return new Grammar<TGrammar, TToken>.ParserGrammar.NodeSymbol(nodeSymbol.Node)
							{
								ArgumentName = name
							};
							break;
						default:
							yield return childSymbol;
							break;
					}
				}
			}

			private IEnumerable<Entry> CreateParserEntries(Grammar<TGrammar, TToken>.ParserGrammar.Symbol symbol)
			{
				switch (symbol)
				{
					case Grammar<TGrammar, TToken>.ParserGrammar.FragmentSymbol fragmentSymbol:

						if (fragmentSymbol.Fragment.Productions.Count == 1)
						{
							var production = fragmentSymbol.Fragment.Productions[0];

							if (production.Symbols.Any(s => s is Grammar<TGrammar, TToken>.ParserGrammar.FragmentSymbol recursiveFragmentSymbol && ReferenceEquals(recursiveFragmentSymbol.Fragment, fragmentSymbol.Fragment)))
								yield return CreateParserEntry(symbol);
							else
							{
								if (fragmentSymbol.ArgumentName != null)
									foreach (var entry in RenameFragmentSymbols(production, fragmentSymbol.ArgumentName).SelectMany(CreateParserEntries))
										yield return entry;
								else
									foreach (var entry in production.Symbols.SelectMany(CreateParserEntries))
										yield return entry;
							}
						}
						else
							yield return CreateParserEntry(symbol);

						break;

					case Grammar<TGrammar, TToken>.ParserGrammar.NodeSymbol { Node.Precedences.Count: > 0 } nodeSymbol:

						foreach (var entry in CreatePrecedenceNode(nodeSymbol))
							yield return entry;

						break;
					case Grammar<TGrammar, TToken>.ParserGrammar.TokenSymbol { Token.Composite: true } tokenSymbol:
						foreach (var entry in CreateCompositeTokenEntries(tokenSymbol))
							yield return entry;

						break;

					default:

						yield return CreateParserEntry(symbol);

						break;
				}
			}

			private IEnumerable<Entry> CreatePrecedenceNode(Grammar<TGrammar, TToken>.ParserGrammar.NodeSymbol nodeSymbol)
			{
				if (nodeSymbol.Node.Precedences.Count > 1)
					throw new NotImplementedException();

				var precedence = nodeSymbol.Node.Precedences[0];

				var productionPrecedence = CreateProductionPrecedence(precedence.Syntax, precedence.Level, false);
				var enterPrecedenceEntry = new EnterPrecedenceEntry(productionPrecedence);
				var leavePrecedenceEntry = new LeavePrecedenceEntry(productionPrecedence);

				yield return enterPrecedenceEntry;
				yield return CreateParserEntry(nodeSymbol);
				yield return leavePrecedenceEntry;
			}

			private Entry CreateExternalLexerEntry(Grammar<TGrammar, TToken>.ParserGrammar.ExternalTokenSymbol externalTokenSymbol)
			{
				var externalGrammarType = externalTokenSymbol.ExternalGrammarType;
				var externalTokenType = externalTokenSymbol.ExternalTokenType;
				var typeArguments = new[] { externalGrammarType, externalTokenType };

				var externalLexerMethod = GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic).Single(m => m.Name == nameof(CreateExternalLexerEntry) && m.GetGenericArguments().Length == typeArguments.Length);
				var externalLexerGenericMethod = externalLexerMethod.MakeGenericMethod(typeArguments);

				return (Entry)externalLexerGenericMethod.Invoke(this, new object[] { externalTokenSymbol });
			}

			private Entry CreateExternalLexerEntry<TExternalGrammar, TExternalToken>(Grammar<TGrammar, TToken>.ParserGrammar.ExternalTokenSymbol<TExternalGrammar, TExternalToken> externalTokenSymbol)
				where TExternalGrammar : Grammar<TExternalGrammar, TExternalToken>
				where TExternalToken : unmanaged, Enum
			{
				var externalLexerInvokeInfo = new ExternalLexerInvokeInfo<TExternalGrammar, TExternalToken>(externalTokenSymbol);

				return new ParserPredicateEntry<Lexeme<TExternalToken>>(externalTokenSymbol, c => externalLexerInvokeInfo.ExternalLex(c), ParserPredicateKind.ExternalLexer);
			}

			private Entry CreateExternalParserEntry(Grammar<TGrammar, TToken>.ParserGrammar.ExternalNodeSymbol externalNodeSymbol)
			{
				var externalGrammarType = externalNodeSymbol.ExternalGrammarType;
				var externalTokenType = externalNodeSymbol.ExternalTokenType;

				var typeArguments = externalNodeSymbol.ExternalNodeType == null ? new[] { externalGrammarType, externalTokenType } : new[] { externalGrammarType, externalTokenType, externalNodeSymbol.ExternalNodeType };

				var externalParserMethod = GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic).Single(m => m.Name == nameof(CreateExternalParserEntry) && m.GetGenericArguments().Length == typeArguments.Length);
				var externalParserGenericMethod = externalParserMethod.MakeGenericMethod(typeArguments.ToArray());

				return (Entry)externalParserGenericMethod.Invoke(this, new object[] { externalNodeSymbol });
			}

			private Entry CreateExternalParserEntry<TExternalGrammar, TExternalToken, TExternalNode>(Grammar<TGrammar, TToken>.ParserGrammar.ExternalNodeSymbol<TExternalGrammar, TExternalToken, TExternalNode> externalNodeSymbol)
				where TExternalGrammar : Grammar<TExternalGrammar, TExternalToken>
				where TExternalToken : unmanaged, Enum
				where TExternalNode : class
			{
				return new ExternalParserDelegate<TExternalGrammar, TExternalToken, TExternalNode>(externalNodeSymbol).PredicateEntry;
			}

			private Entry CreateExternalParserEntry<TExternalGrammar, TExternalToken>(Grammar<TGrammar, TToken>.ParserGrammar.ExternalNodeSymbol<TExternalGrammar, TExternalToken> externalNodeSymbol)
				where TExternalGrammar : Grammar<TExternalGrammar, TExternalToken> where TExternalToken : unmanaged, Enum
			{
				return new ExternalParserDelegate<TExternalGrammar, TExternalToken>(externalNodeSymbol).PredicateEntry;
			}

			private protected override Pool<InstructionStream> CreateInstructionStreamPool()
			{
				return new Pool<InstructionStream>(p => new LexemeInstructionStream(p));
			}

			private Entry CreateLeavePrecedenceEntry(Grammar<TGrammar, TToken>.ParserGrammar.LeavePrecedenceSymbol leavePrecedenceSymbol)
			{
				var productionPrecedence = CreateProductionPrecedence(leavePrecedenceSymbol.Syntax, leavePrecedenceSymbol.Value, leavePrecedenceSymbol.Level);
				var precedenceEntry = new LeavePrecedenceEntry(productionPrecedence);

				return precedenceEntry;
			}

			private Entry CreateParserEntry(Grammar<TGrammar, TToken>.ParserGrammar.Symbol symbol)
			{
				return symbol switch
				{
					Grammar<TGrammar, TToken>.ParserGrammar.QuantifierSymbol quantifierEntry => CreateQuantifierEntry(quantifierEntry),
					Grammar<TGrammar, TToken>.ParserGrammar.TokenSymbol syntaxTokenEntry => CreateTokenEntry(syntaxTokenEntry),
					Grammar<TGrammar, TToken>.ParserGrammar.TokenSetSymbol syntaxSetTokenEntry => CreateTokenSetEntry(syntaxSetTokenEntry),
					Grammar<TGrammar, TToken>.ParserGrammar.NodeSymbol syntaxNodeEntry => CreateParserSyntaxEntry(syntaxNodeEntry),
					Grammar<TGrammar, TToken>.ParserGrammar.FragmentSymbol syntaxFragmentEntry => CreateParserSyntaxEntry(syntaxFragmentEntry),

					Grammar<TGrammar, TToken>.ParserGrammar.PredicateSymbol parserPredicate => CreatePredicateEntry(parserPredicate),
					Grammar<TGrammar, TToken>.ParserGrammar.ActionSymbol parserAction => CreateActionEntry(parserAction),

					Grammar<TGrammar, TToken>.ParserGrammar.ExternalNodeSymbol externalParserEntry => CreateExternalParserEntry(externalParserEntry),
					Grammar<TGrammar, TToken>.ParserGrammar.ExternalTokenSymbol externalLexerEntry => CreateExternalLexerEntry(externalLexerEntry),
					Grammar<TGrammar, TToken>.ParserGrammar.EnterPrecedenceSymbol enterPrecedenceSymbol => CreateEnterPrecedenceEntry(enterPrecedenceSymbol),
					Grammar<TGrammar, TToken>.ParserGrammar.LeavePrecedenceSymbol leavePrecedenceSymbol => CreateLeavePrecedenceEntry(leavePrecedenceSymbol),

					_ => throw new ArgumentOutOfRangeException(nameof(symbol))
				};
			}

			private ParserSyntax CreateParserSyntax(Grammar<TGrammar, TToken>.ParserGrammar.Syntax grammarParserSyntax)
			{
				var parserSyntax = new ParserSyntax(grammarParserSyntax);

				ParserSyntaxDictionary.Add(grammarParserSyntax, parserSyntax);
				AddSyntax(parserSyntax, GetSyntaxProductions(grammarParserSyntax).Select(t => new ParserProduction(this, parserSyntax, grammarParserSyntax, t)).ToList());

				return parserSyntax;
			}

			private ParserSyntaxEntry CreateParserSyntaxEntry(Grammar<TGrammar, TToken>.ParserGrammar.FragmentSymbol fragmentSymbol)
			{
				var originalFragment = fragmentSymbol.Fragment;
				var fragment = new Grammar<TGrammar, TToken>.ParserGrammar.FragmentSyntax(originalFragment.Name, true);

				foreach (var production in originalFragment.Productions)
					fragment.AddProduction(new Grammar<TGrammar, TToken>.ParserGrammar.Production(production.Symbols));

				return new ParserSyntaxEntry(fragmentSymbol, GetParserSyntax(fragment));
			}

			private ParserSyntaxEntry CreateParserSyntaxEntry(Grammar<TGrammar, TToken>.ParserGrammar.NodeSymbol syntaxNode)
			{
				return new ParserSyntaxEntry(syntaxNode, GetParserSyntax(syntaxNode.Node));
			}

			private static Func<AutomataContext, PredicateResult> CreatePredicateDelegate(Parser<TToken>.PredicateEntry predicateEntry)
			{
				return c => predicateEntry.Predicate(((ParserAutomataContext)c).Parser) ? PredicateResult.True : PredicateResult.False;
			}

			private static ParserPredicateEntry CreatePredicateEntry(Grammar<TGrammar, TToken>.ParserGrammar.PredicateSymbol predicateSymbol)
			{
				return new ParserPredicateEntry(predicateSymbol);
			}

			private ProcessAutomataContext CreateProcessContext(Grammar<TGrammar, TToken>.ParserGrammar.NodeSyntax parserRule, LexemeSource<TToken> lexemeSource, ProcessKind processKind,
				Parser<TGrammar, TToken> parser)
			{
				return new ProcessAutomataContext(GetParserSyntax(parserRule), lexemeSource, processKind, parser, this);
			}

			private PrecedencePredicate CreateProductionPrecedence(Grammar<TGrammar, TToken>.ParserGrammar.Syntax syntax, int precedence, bool level)
			{
				if (_precedenceDictionary.TryGetValue(syntax, out var precedenceId) == false)
				{
					precedenceId = CreatePrecedencePredicateId();

					_precedenceDictionary.Add(syntax, precedenceId);
				}

				return new PrecedencePredicate(precedenceId, precedence, level);
			}

			private Entry CreateQuantifierEntry(Grammar<TGrammar, TToken>.ParserGrammar.QuantifierSymbol quantifierEntry)
			{
				var primitiveEntry = (PrimitiveEntry)CreateParserEntry(quantifierEntry.Symbol);

				return new ParserQuantifierEntry(quantifierEntry, primitiveEntry, quantifierEntry.Range, quantifierEntry.Mode);
			}

			private SyntaxTreeAutomataContext CreateSyntaxTreeContext(Grammar<TGrammar, TToken>.ParserGrammar.NodeSyntax parserRule, LexemeSource<TToken> lexemeSource, ProcessKind processKind,
				Parser<TGrammar, TToken> parser)
			{
				return new SyntaxTreeAutomataContext(GetParserSyntax(parserRule), lexemeSource, processKind, parser, this);
			}

			private static Entry CreateTokenEntry(Grammar<TGrammar, TToken>.ParserGrammar.TokenSymbol tokenSymbol)
			{
				var grammarSyntax = tokenSymbol.Token;

				if (grammarSyntax.Composite)
					throw new InvalidOperationException();

				if (grammarSyntax.TokenGroups.Count == 1)
					return new ParserOperandMatchEntry(tokenSymbol);

				return new ParserSetMatchEntry(tokenSymbol);
			}

			private Entry CreateTokenSetEntry(Grammar<TGrammar, TToken>.ParserGrammar.TokenSetSymbol tokenSetSymbol)
			{
				if (tokenSetSymbol.Tokens.Any(t => t.Composite) == false)
					return new ParserSetMatchEntry(tokenSetSymbol);

				var tokens = tokenSetSymbol.Tokens;

				tokenSetSymbol = new Grammar<TGrammar, TToken>.ParserGrammar.TokenSetSymbol(tokens.Where(t => t.Composite == false).ToArray())
				{
					ArgumentName = tokenSetSymbol.ArgumentName
				};

				var fragmentSyntax = new Grammar<TGrammar, TToken>.ParserGrammar.FragmentSyntax("Internal", true);

				fragmentSyntax.AddProduction(new Grammar<TGrammar, TToken>.ParserGrammar.Production(tokenSetSymbol));

				foreach (var tokenSyntax in tokens.Where(t => t.Composite))
				{
					var compositeTokenSymbol = new Grammar<TGrammar, TToken>.ParserGrammar.TokenSymbol(tokenSyntax)
					{
						ArgumentName = tokenSetSymbol.ArgumentName
					};

					fragmentSyntax.AddProduction(new Grammar<TGrammar, TToken>.ParserGrammar.Production(compositeTokenSymbol));
				}

				var fragment = new Grammar<TGrammar, TToken>.ParserGrammar.FragmentSymbol(fragmentSyntax);

				return CreateParserSyntaxEntry(fragment);
			}

			private VisitorAutomataContext CreateVisitorContext<TResult>(Visitor<TResult> visitor, Grammar<TGrammar, TToken>.ParserGrammar.NodeSyntax nodeSyntax, LexemeSource<TToken> lexemeSource, ProcessKind processKind,
				Parser<TGrammar, TToken> parser)
			{
				return new VisitorAutomataContext(visitor, GetParserSyntax(nodeSyntax), lexemeSource, processKind, parser, this);
			}

			private static string EnsureName(Grammar<TGrammar, TToken>.ParserGrammar.Symbol parserEntry)
			{
				if (parserEntry == null)
					return null;

				if (string.IsNullOrEmpty(parserEntry.ArgumentName) == false)
					return parserEntry.ArgumentName;

				return parserEntry switch
				{
					Grammar<TGrammar, TToken>.ParserGrammar.NodeSymbol ruleEntry => ruleEntry.ArgumentName ?? ruleEntry.Node.Name,
					Grammar<TGrammar, TToken>.ParserGrammar.TokenSymbol tokenRuleEntry => tokenRuleEntry.ArgumentName ?? tokenRuleEntry.Token.Name,
					Grammar<TGrammar, TToken>.ParserGrammar.QuantifierSymbol quantifierEntry => quantifierEntry.ArgumentName ?? EnsureName(quantifierEntry.Symbol),
					Grammar<TGrammar, TToken>.ParserGrammar.ExternalNodeSymbol externalParserEntry => externalParserEntry.ArgumentName,
					Grammar<TGrammar, TToken>.ParserGrammar.ExternalTokenSymbol externalLexerEntry => externalLexerEntry.ArgumentName,
					_ => null
				};
			}

			private string GenerateProductionName()
			{
				return $"GeneratedProduction{_generateProductionNameCount++}";
			}

			private ExternalParseResult<TResult> GetForkResult<TResult>(ForkAutomataResult forkAutomataResult, SyntaxTreeAutomataContext context)
			{
				var firstResult = (SuccessAutomataResult)forkAutomataResult.RunFirst();
				//var secondResult = forkAutomataResult.RunSecond();
				var result = context.GetResult<TResult>();

				return new SuccessExternalParseResult<TResult>(result, firstResult.InstructionPosition);
			}

			private ParserSyntax GetParserSyntax(Grammar<TGrammar, TToken>.ParserGrammar.Syntax syntax)
			{
				return ParserSyntaxDictionary.TryGetValue(syntax, out var parserSyntax) == false ? CreateParserSyntax(syntax) : parserSyntax;
			}

			private static IEnumerable<Grammar<TGrammar, TToken>.ParserGrammar.Production> GetProductions(Grammar<TGrammar, TToken>.ParserGrammar.Syntax parserSyntax)
			{
				foreach (var production in parserSyntax.Productions)
				{
					if (production.ProductionBinding is Grammar<TGrammar, TToken>.ParserGrammar.ReturnNodeBinding)
					{
						var nodeSymbolIndex = 0;

						while (production.Symbols[nodeSymbolIndex] is not Grammar<TGrammar, TToken>.ParserGrammar.NodeSymbol)
							nodeSymbolIndex++;

						var nodeSymbol = (Grammar<TGrammar, TToken>.ParserGrammar.NodeSymbol)production.Symbols[nodeSymbolIndex];

						foreach (var replaceProduction in GetSyntaxProductions(nodeSymbol.Node))
						{
							var symbols = new List<Grammar<TGrammar, TToken>.ParserGrammar.Symbol>();

							for (var i = 0; i < production.Symbols.Count; i++)
							{
								if (i == nodeSymbolIndex)
									symbols.AddRange(replaceProduction.Symbols);
								else
									symbols.Add(production.Symbols[i]);
							}

							var replaceProductionCopy = replaceProduction.Clone(symbols);

							yield return replaceProductionCopy;
						}
					}
					else
						yield return production;
				}
			}

			private static IEnumerable<Grammar<TGrammar, TToken>.ParserGrammar.Production> GetSyntaxProductions(Grammar<TGrammar, TToken>.ParserGrammar.Syntax parserSyntax)
			{
#if true
				if (parserSyntax.Productions.All(p => p.ProductionBinding is Grammar<TGrammar, TToken>.ParserGrammar.ReturnNodeBinding))
				{
					return GetProductions(parserSyntax);
				}
#endif

				return parserSyntax.Productions;
			}

			public TResult Parse<TResult>(Grammar<TGrammar, TToken>.ParserGrammar.NodeSyntax syntax, LexemeSource<TToken> lexemeSource, Parser<TGrammar, TToken> parser, CancellationToken cancellationToken = default)
			{
				using var context = CreateSyntaxTreeContext(syntax, lexemeSource, ProcessKind.Process, parser);
				using var result = context.Process.Run(cancellationToken).Verify();

				return context.GetResult<TResult>();
			}
			
			public void Parse(Grammar<TGrammar, TToken>.ParserGrammar.NodeSyntax syntax, LexemeSource<TToken> lexemeSource, Parser<TGrammar, TToken> parser, CancellationToken cancellationToken = default)
			{
				using var context = CreateProcessContext(syntax, lexemeSource, ProcessKind.Process, parser);
				using var result = context.Process.Run(cancellationToken).Verify();
			}

			public TResult Parse<TResult>(Visitor<TResult> visitor, Grammar<TGrammar, TToken>.ParserGrammar.NodeSyntax syntax, LexemeSource<TToken> lexemeSource, Parser<TGrammar, TToken> parser, CancellationToken cancellationToken = default)
			{
				using var context = CreateVisitorContext(visitor, syntax, lexemeSource, ProcessKind.Process, parser);
				using var result = context.Process.Run(cancellationToken).Verify();

				return context.GetResult<TResult>();
			}

			public ExternalParseResult<TResult> ParseExternal<TResult>(Grammar<TGrammar, TToken>.ParserGrammar.NodeSyntax<TResult> syntax, LexemeSource<TToken> lexemeSource, Parser<TGrammar, TToken> parser,
				CancellationToken cancellationToken = default)
				where TResult : class
			{
				using var context = CreateSyntaxTreeContext(syntax, lexemeSource, ProcessKind.SubProcess, parser);
				var externalResult = context.Process.Run(cancellationToken);

				try
				{
					return externalResult switch
					{
						SuccessAutomataResult successAutomataResult => new SuccessExternalParseResult<TResult>(context.GetResult<TResult>(), successAutomataResult.InstructionPosition),
						ExceptionAutomataResult exceptionAutomataResult => new ExceptionExternalParseResult<TResult>(exceptionAutomataResult.Exception),
						ForkAutomataResult forkAutomataResult => GetForkResult<TResult>(forkAutomataResult, context),
						_ => throw new ArgumentOutOfRangeException()
					};
				}
				finally
				{
					externalResult.Dispose();
				}
			}

			private void RegisterParserSyntax(Grammar<TGrammar, TToken>.ParserGrammar.Syntax parserSyntax)
			{
				GetParserSyntax(parserSyntax);
			}

			private int RegisterProduction(ParserProduction parserProduction)
			{
				var productionIndex = Productions.Count;

				Productions.Add(parserProduction);

				return productionIndex;
			}
		}
	}
}