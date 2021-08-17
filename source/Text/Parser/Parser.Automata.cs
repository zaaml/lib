// <copyright file="Parser.Automata.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
					RegisterParserRule(parserSyntax);

				foreach (var parserSyntax in grammar.NodeCollection)
					RegisterParserRule(parserSyntax);

				foreach (var parserRule in ParserRuleDictionary.Values)
				{
					EliminateLeftRecursion(parserRule);
					EliminateLeftFactoring(parserRule);
				}

				foreach (var parserProduction in Productions)
					parserProduction.EnsureBinder();

				Build();
			}
			
			protected override bool AllowParallelInstructionReader => false; //_parser.AllowParallel;

			protected override bool LookAheadEnabled => true;

			private Dictionary<Grammar<TGrammar, TToken>.ParserGrammar.Syntax, ParserRule> ParserRuleDictionary { get; } = new();

			private List<ParserProduction> Productions { get; } = new();

			private static Action<AutomataContext> CreateActionDelegate(Parser<TToken>.ActionEntry actionEntry)
			{
				void ActionDelegate(AutomataContext c)
				{
					var parserAutomataContext = (ParserAutomataContext)c;
					var parserAutomataContextState = (ParserAutomataContextState)parserAutomataContext.ContextStateInternal;

					actionEntry.Action(parserAutomataContextState?.ParserContext);
				}

				return ActionDelegate;
			}

			private static ParserActionEntry CreateActionEntry(Grammar<TGrammar, TToken>.ParserGrammar.ActionSymbol actionSymbol)
			{
				return new ParserActionEntry(actionSymbol);
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
				return new ExternalParserInvokeInfo<TExternalGrammar, TExternalToken, TExternalNode>(externalNodeSymbol).PredicateEntry;
			}

			private Entry CreateExternalParserEntry<TExternalGrammar, TExternalToken>(Grammar<TGrammar, TToken>.ParserGrammar.ExternalNodeSymbol<TExternalGrammar, TExternalToken> externalNodeSymbol)
				where TExternalGrammar : Grammar<TExternalGrammar, TExternalToken> where TExternalToken : unmanaged, Enum
			{
				return new ExternalParserInvokeInfo<TExternalGrammar, TExternalToken>(externalNodeSymbol).PredicateEntry;
			}

			private protected override Pool<InstructionStream> CreateInstructionStreamPool()
			{
				return new Pool<InstructionStream>(p => new LexemeStream(p));
			}

			private Entry CreateParserEntry(Grammar<TGrammar, TToken>.ParserGrammar.Symbol grammarParserEntry)
			{
				return grammarParserEntry switch
				{
					Grammar<TGrammar, TToken>.ParserGrammar.QuantifierSymbol quantifierEntry => CreateQuantifierEntry(quantifierEntry),
					Grammar<TGrammar, TToken>.ParserGrammar.TokenSymbol syntaxTokenEntry => CreateTokenEntry(syntaxTokenEntry),
					Grammar<TGrammar, TToken>.ParserGrammar.TokenSetSymbol syntaxSetTokenEntry => CreateTokenSetEntry(syntaxSetTokenEntry),
					Grammar<TGrammar, TToken>.ParserGrammar.NodeSymbol syntaxNodeEntry => CreateParserRuleEntry(syntaxNodeEntry),
					Grammar<TGrammar, TToken>.ParserGrammar.FragmentSymbol syntaxFragmentEntry => CreateParserRuleEntry(syntaxFragmentEntry),
					Grammar<TGrammar, TToken>.ParserGrammar.PredicateSymbol parserPredicate => CreatePredicateEntry(parserPredicate),
					Grammar<TGrammar, TToken>.ParserGrammar.ActionSymbol parserAction => CreateActionEntry(parserAction),
					Grammar<TGrammar, TToken>.ParserGrammar.ExternalNodeSymbol externalParserEntry => CreateExternalParserEntry(externalParserEntry),
					Grammar<TGrammar, TToken>.ParserGrammar.ExternalTokenSymbol externalLexerEntry => CreateExternalLexerEntry(externalLexerEntry),
					Grammar<TGrammar, TToken>.ParserGrammar.EnterPrecedenceSymbol enterPrecedenceSymbol => CreateEnterPrecedenceEntry(enterPrecedenceSymbol),
					Grammar<TGrammar, TToken>.ParserGrammar.LeavePrecedenceSymbol leavePrecedenceSymbol => CreateLeavePrecedenceEntry(leavePrecedenceSymbol),
					_ => throw new ArgumentOutOfRangeException(nameof(grammarParserEntry))
				};
			}

			private Entry CreateLeavePrecedenceEntry(Grammar<TGrammar, TToken>.ParserGrammar.LeavePrecedenceSymbol leavePrecedenceSymbol)
			{
				var productionPrecedence = CreateProductionPrecedence(leavePrecedenceSymbol.Syntax, leavePrecedenceSymbol.Value, leavePrecedenceSymbol.Syntax.MaxPrecedenceValue + 1, leavePrecedenceSymbol.Level);
				var precedenceEntry = new LeavePrecedenceEntry(productionPrecedence);

				return precedenceEntry;
			}

			private Entry CreateEnterPrecedenceEntry(Grammar<TGrammar, TToken>.ParserGrammar.EnterPrecedenceSymbol enterPrecedenceSymbol)
			{
				var productionPrecedence = CreateProductionPrecedence(enterPrecedenceSymbol.Syntax, enterPrecedenceSymbol.Value, enterPrecedenceSymbol.Syntax.MaxPrecedenceValue + 1, enterPrecedenceSymbol.Level);
				var precedenceEntry = new EnterPrecedenceEntry(productionPrecedence);

				return precedenceEntry;
			}

			private ParserRule CreateParserRule(Grammar<TGrammar, TToken>.ParserGrammar.Syntax parserSyntax)
			{
				var parserRule = new ParserRule(parserSyntax);

				ParserRuleDictionary.Add(parserSyntax, parserRule);
				AddRule(parserRule, GetSyntaxProductions(parserSyntax).Select(t => new ParserProduction(this, parserRule, parserSyntax, t)).ToList());

				return parserRule;
			}

			private static IEnumerable<Grammar<TGrammar, TToken>.ParserGrammar.Production> GetSyntaxProductions(Grammar<TGrammar, TToken>.ParserGrammar.Syntax parserSyntax)
			{
#if true
				return parserSyntax.InlineReturnProductions ? GetProductions(parserSyntax) : parserSyntax.Productions;
#else
				return parserSyntax.Productions;
#endif
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

			private ParserRuleEntry CreateParserRuleEntry(Grammar<TGrammar, TToken>.ParserGrammar.FragmentSymbol fragmentEntry)
			{
				var originalFragment = fragmentEntry.Fragment;
				var fragment = new Grammar<TGrammar, TToken>.ParserGrammar.FragmentSyntax(originalFragment.Name, true);

				foreach (var parserSyntaxProduction in originalFragment.Productions)
					fragment.AddProduction(new Grammar<TGrammar, TToken>.ParserGrammar.Production(parserSyntaxProduction.Symbols));

				return new ParserRuleEntry(fragmentEntry, GetParserRule(fragment));
			}

			private ParserRuleEntry CreateParserRuleEntry(Grammar<TGrammar, TToken>.ParserGrammar.NodeSymbol syntaxNode)
			{
				return new ParserRuleEntry(syntaxNode, GetParserRule(syntaxNode.Node));
			}

			private static Func<AutomataContext, PredicateResult> CreatePredicateDelegate(Parser<TToken>.PredicateEntry predicateEntry)
			{
				return c => predicateEntry.Predicate(((ParserAutomataContextState)((ParserAutomataContext)c).ContextStateInternal).ParserContext) ? PredicateResult.True : PredicateResult.False;
			}

			private static ParserPredicateEntry CreatePredicateEntry(Grammar<TGrammar, TToken>.ParserGrammar.PredicateSymbol predicateSymbol)
			{
				return new ParserPredicateEntry(predicateSymbol);
			}

			private ProcessAutomataContext CreateProcessContext(Grammar<TGrammar, TToken>.ParserGrammar.NodeSyntax parserRule, LexemeSource<TToken> lexemeSource, ParserContext parserContext, ProcessKind processKind,
				Parser<TGrammar, TToken> parser)
			{
				return new ProcessAutomataContext(GetParserRule(parserRule), lexemeSource, parserContext, processKind, parser, this);
			}

			private PrecedencePredicate CreateProductionPrecedence(Grammar<TGrammar, TToken>.ParserGrammar.Syntax syntax, int precedence, int precedenceCount, bool level)
			{
				if (_precedenceDictionary.TryGetValue(syntax, out var precedenceId) == false)
				{
					precedenceId = CreatePrecedencePredicateId(precedenceCount);

					_precedenceDictionary.Add(syntax, precedenceId);
				}

				return new PrecedencePredicate(precedenceId, precedence, level);
			}

			private Entry CreateQuantifierEntry(Grammar<TGrammar, TToken>.ParserGrammar.QuantifierSymbol quantifierEntry)
			{
				var primitiveEntry = (PrimitiveEntry)CreateParserEntry(quantifierEntry.Symbol);

				return new ParserQuantifierEntry(quantifierEntry, primitiveEntry, quantifierEntry.Range, quantifierEntry.Mode);
			}

			private SyntaxTreeAutomataContext CreateSyntaxTreeContext(Grammar<TGrammar, TToken>.ParserGrammar.NodeSyntax parserRule, LexemeSource<TToken> lexemeSource, ParserContext parserContext, ProcessKind processKind,
				Parser<TGrammar, TToken> parser)
			{
				return new SyntaxTreeAutomataContext(GetParserRule(parserRule), lexemeSource, parserContext, processKind, parser, this);
			}

			private static Entry CreateTokenEntry(Grammar<TGrammar, TToken>.ParserGrammar.TokenSymbol tokenSymbol)
			{
				var syntaxToken = tokenSymbol.Token;

				if (syntaxToken.TokenGroups.Count == 1)
					return new ParserSingleMatchEntry(tokenSymbol);

				return new ParserSetMatchEntry(tokenSymbol);
			}

			private static Entry CreateTokenSetEntry(Grammar<TGrammar, TToken>.ParserGrammar.TokenSetSymbol tokenSetSymbol)
			{
				return new ParserSetMatchEntry(tokenSetSymbol);
			}

			private VisitorAutomataContext CreateVisitorContext<TResult>(Visitor<TResult> visitor, Grammar<TGrammar, TToken>.ParserGrammar.NodeSyntax parserRule, LexemeSource<TToken> lexemeSource, ParserContext parserContext,
				ProcessKind processKind,
				Parser<TGrammar, TToken> parser)
			{
				return new VisitorAutomataContext(visitor, GetParserRule(parserRule), lexemeSource, parserContext, processKind, parser, this);
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
				return $"GeneratedTransition{_generateProductionNameCount++}";
			}

			private ParserRule GetParserRule(Grammar<TGrammar, TToken>.ParserGrammar.Syntax parserSyntax)
			{
				return ParserRuleDictionary.TryGetValue(parserSyntax, out var parserRule) == false ? CreateParserRule(parserSyntax) : parserRule;
			}

			public TResult Parse<TResult>(Grammar<TGrammar, TToken>.ParserGrammar.NodeSyntax parserRule, LexemeSource<TToken> lexemeSource, ParserContext parserContext, Parser<TGrammar, TToken> parser)
			{
				using var context = CreateSyntaxTreeContext(parserRule, lexemeSource, parserContext, ProcessKind.Process, parser);
				using var result = context.Process.Run().Verify();

				return context.GetResult<TResult>();
			}

			public void Parse(Grammar<TGrammar, TToken>.ParserGrammar.NodeSyntax parserRule, LexemeSource<TToken> lexemeSource, ParserContext parserContext, Parser<TGrammar, TToken> parser)
			{
				using var context = CreateProcessContext(parserRule, lexemeSource, parserContext, ProcessKind.Process, parser);
				using var result = context.Process.Run().Verify();
			}

			public TResult Parse<TResult>(Visitor<TResult> visitor, Grammar<TGrammar, TToken>.ParserGrammar.NodeSyntax parserRule, LexemeSource<TToken> lexemeSource, ParserContext parserContext, Parser<TGrammar, TToken> parser)
			{
				using var context = CreateVisitorContext(visitor, parserRule, lexemeSource, parserContext, ProcessKind.Process, parser);
				using var result = context.Process.Run().Verify();

				return context.GetResult<TResult>();
			}

			private void RegisterParserRule(Grammar<TGrammar, TToken>.ParserGrammar.Syntax parserSyntax)
			{
				GetParserRule(parserSyntax);
			}

			private int RegisterProduction(ParserProduction parserProduction)
			{
				var productionIndex = Productions.Count;

				Productions.Add(parserProduction);

				return productionIndex;
			}

			private sealed class LexemeStream : InstructionStream
			{
				public LexemeStream(Pool<InstructionStream> pool) : base(pool)
				{
				}

				public override InstructionStream Advance(int position, int instructionPointer, Automata<Lexeme<TToken>, TToken> automata)
				{
					var copy = (LexemeStream)Pool.Get().Mount(InstructionReader, automata);

					copy.AdvanceInstructionPosition(this, position, instructionPointer);

					return copy;
				}

				private void AdvanceInstructionPosition(LexemeStream lexemeStream, int textPosition, int instructionPointer)
				{
					Position = textPosition;
					StartPosition = textPosition;
					StartInstructionPointer = instructionPointer;

					var lexemeIndex = instructionPointer & LocalIndexMask;
					var lexemePageIndex = instructionPointer >> PageIndexShift;

					EnsurePagesCapacity(lexemePageIndex + 1);

					for (var iPage = lexemeStream.HeadPage; iPage <= lexemePageIndex; iPage++)
					{
						InstructionReader.RentBuffers(PageSize, out var instructionsBuffer, out var operandsBuffer);

						var pageSource = lexemeStream.Pages[iPage];
						var pageCopy = new InstructionPage(instructionsBuffer, operandsBuffer, pageSource.PageIndex, pageSource.PageLength);

						Array.Copy(pageSource.InstructionsBuffer, 0, pageCopy.InstructionsBuffer, 0, pageCopy.PageLength);
						Array.Copy(pageSource.OperandsBuffer, 0, pageCopy.OperandsBuffer, 0, pageCopy.PageLength);

						pageCopy.ReferenceCount = pageSource.ReferenceCount;

						Pages[pageCopy.PageIndex] = pageCopy;
					}

					var lexemePage = Pages[lexemePageIndex];

					lexemePage.PageLength = lexemeIndex;

					for (var i = lexemeIndex; i < PageSize; i++)
					{
						lexemePage.OperandsBuffer[i] = int.MinValue;
						lexemePage.InstructionsBuffer[i] = default;
					}

					HeadPage = lexemeStream.HeadPage;
					PageCount = lexemePageIndex + 1;

					FetchReadOperand(ref instructionPointer);
				}

				public override int GetPosition(int instructionPointer)
				{
					if (instructionPointer == 0 || instructionPointer == StartInstructionPointer)
						return StartPosition;

					instructionPointer--;

					var localInstructionPointer = instructionPointer;

					FetchReadOperand(ref instructionPointer);

					return PeekInstructionRef(localInstructionPointer).End;
				}
			}

			private sealed class LexemeStreamInstructionReader : IInstructionReader
			{
				private readonly LexemeSource<TToken> _lexemeSource;

				public LexemeStreamInstructionReader(LexemeSource<TToken> lexemeSource)
				{
					_lexemeSource = lexemeSource;
				}

				public int ReadPage(ref int position, int bufferLength, out Lexeme<TToken>[] lexemesBuffer, out int[] operandsBuffer)
				{
					RentBuffers(bufferLength, out var localLexemesBuffer, out var localOperandsBuffer);

					lexemesBuffer = localLexemesBuffer;
					operandsBuffer = localOperandsBuffer;

					return _lexemeSource.Read(ref position, localLexemesBuffer, localOperandsBuffer, 0, bufferLength, true);
				}

				public int ReadPage(ref int position, int bufferOffset, int bufferLength, Lexeme<TToken>[] lexemesBuffer, int[] operandsBuffer)
				{
					return _lexemeSource.Read(ref position, lexemesBuffer, operandsBuffer, bufferOffset, bufferLength, true);
				}

				public void ReleaseBuffers(Lexeme<TToken>[] instructionsBuffer, int[] operandsBuffer)
				{
					Lexer<TGrammar, TToken>.ReturnLexemeBuffers(instructionsBuffer, operandsBuffer);
				}

				public void RentBuffers(int bufferLength, out Lexeme<TToken>[] instructionsBuffer, out int[] operandsBuffer)
				{
					Lexer<TGrammar, TToken>.RentLexemeBuffers(bufferLength, out instructionsBuffer, out operandsBuffer);
				}

				public void Dispose()
				{
				}
			}
		}
	}
}