// <copyright file="Parser.Automata.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Zaaml.Core.Converters;
using Zaaml.Core.Extensions;

namespace Zaaml.Text
{
	internal abstract partial class Parser<TGrammar, TToken> where TGrammar : Grammar<TToken> where TToken : unmanaged, Enum
	{
		private sealed partial class ParserAutomata : Automata<Lexeme<TToken>, TToken>
		{
			private readonly HashSet<Grammar<TToken>.ParserRule> _registeredRules = new();
			private int _generateProductionNameCount;

			public ParserAutomata(AutomataManager manager) : base(manager)
			{
				var grammar = Grammar.Get<TGrammar, TToken>();
				var undefinedTokenName = Enum.GetName(grammar.UndefinedToken);

				foreach (TToken token in Enum.GetValues(typeof(TToken)))
				{
					var tokenCode = (int)EnumConverter<TToken>.Convert(token);

					if (grammar.UndefinedToken.Equals(token) && Enum.GetName(token) == undefinedTokenName)
					{
						if (tokenCode != 0)
							throw new InvalidOperationException("Undefined token must have value of 0.");

						continue;
					}

					if (tokenCode == 0)
						throw new InvalidOperationException("No token except of Grammar.UndefinedToken may have value of 0.");
				}

				foreach (var parserRule in grammar.ParserRules)
					RegisterParserRule(parserRule);
			}

			protected override bool AllowParallelInstructionReader => false; //_parser.AllowParallel;

			protected override bool LookAheadEnabled => true;

			private Dictionary<Grammar<TToken>.ParserRule, ParserRule> ParserRuleDictionary { get; } = new();

			private List<ParserProduction> Productions { get; } = new();

			private static Action<AutomataContext> CreateActionDelegate(Parser<TToken>.ActionEntry actionEntry)
			{
				return c => actionEntry.Action(((ParserAutomataContextState)((ParserAutomataContext)c).ContextStateInternal)?.ParserContext);
			}

			private Entry CreateExternalParserEntry(Grammar<TToken>.ExternalParserEntry externalParserEntry)
			{
				var type = externalParserEntry.GetType();
				var typeArguments = type.GenericTypeArguments.Skip(1).ToList();

				typeArguments.Insert(0, externalParserEntry.GrammarType);

				var externalParserMethod = GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic).Single(m => m.Name == nameof(CreateExternalParserEntry) && m.GetGenericArguments().Length == typeArguments.Count);
				var externalParserGenericMethod = externalParserMethod.MakeGenericMethod(typeArguments.ToArray());

				return (Entry)externalParserGenericMethod.Invoke(this, new object[] { externalParserEntry });
			}

			private Entry CreateExternalParserEntry<TExternalGrammar, TExternalToken, TExternalNode, TExternalNodeBase>(Grammar<TToken>.ExternalParserEntry<TExternalToken, TExternalNode, TExternalNodeBase> externalParserEntry)
				where TExternalGrammar : Grammar<TExternalToken, TExternalNodeBase> where TExternalToken : unmanaged, Enum where TExternalNode : TExternalNodeBase where TExternalNodeBase : class
			{
				return new ExternalParserInvokeInfo<TExternalGrammar, TExternalToken, TExternalNode, TExternalNodeBase>(externalParserEntry).PredicateEntry;
			}

			private Entry CreateExternalParserEntry<TExternalGrammar, TExternalToken>(Grammar<TToken>.ExternalParserEntry<TExternalToken> externalParserEntry)
				where TExternalGrammar : Grammar<TExternalToken> where TExternalToken : unmanaged, Enum
			{
				return new ExternalParserInvokeInfo<TExternalGrammar, TExternalToken>(externalParserEntry).PredicateEntry;
			}

			private Grammar<TToken>.ParserRule CreateFragmentRule(Grammar<TToken>.ParserFragment parserFragment)
			{
				var rule = new Grammar<TToken>.ParserRule
				{
					IsInline = true
				};

				if (parserFragment.Productions.All(t => t.Entries.Length == 1 && t.Entries[0] is Grammar<TToken>.TokenRule))
				{
					var setEntry = new Grammar<TToken>.TokenRuleSet(parserFragment.Productions.Select(t => (Grammar<TToken>.TokenRule)t.Entries[0]).ToArray())
					{
						Name = parserFragment.Name
					};

					rule.Productions.Add(new Grammar<TToken>.ParserProduction(new Grammar<TToken>.ParserEntry[] { setEntry }));
				}
				else
				{
					foreach (var transition in parserFragment.Productions)
						rule.Productions.Add(new Grammar<TToken>.ParserProduction(transition.Entries));
				}

				RegisterParserRule(rule);

				return rule;
			}

			private protected override Pool<InstructionStream> CreateInstructionStreamPool()
			{
				return new(p => new LexemeStream(p));
			}

			private static ParserSingleMatchEntry CreateLexerEntry(Grammar<TToken>.ParserTokenRuleEntry tokenEntry)
			{
				return new(tokenEntry, tokenEntry.TokenRule.Token);
			}

			private static ParserSingleMatchEntry CreateLexerEntry(Grammar<TToken>.TokenRule tokenRule)
			{
				return new(tokenRule, tokenRule.Token);
			}

			private static ParserSetMatchEntry CreateLexerSetEntry(Grammar<TToken>.TokenRuleSet tokenRuleEntrySet)
			{
				return new(tokenRuleEntrySet);
			}

			private Entry CreateParserEntry(Grammar<TToken>.ParserEntry grammarParserEntry)
			{
				if (grammarParserEntry is Grammar<TToken>.ParserQuantifierEntry quantifierEntry)
				{
					var primitiveEntry = (PrimitiveEntry)CreateParserEntry(quantifierEntry.PrimitiveEntry);

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

						return new ParserRuleEntry(parserRuleEntry, GetParserRule(ruleClone), true, false);
					}

					return new ParserRuleEntry(parserRuleEntry, GetParserRule(rule), false, parserRuleEntry.IsReturn);
				}

				if (grammarParserEntry is Grammar<TToken>.ParserFragment parserFragment)
					return new ParserRuleEntry(parserFragment, GetParserRule(CreateFragmentRule(parserFragment)), true, false);

				if (grammarParserEntry is Grammar<TToken>.ParserPredicate parserPredicate)
					return new ParserPredicateEntry(parserPredicate);

				if (grammarParserEntry is Grammar<TToken>.ParserAction parserAction)
					return new ParserActionEntry(parserAction);

				if (grammarParserEntry is Grammar<TToken>.ExternalParserEntry externalParserEntry)
					return CreateExternalParserEntry(externalParserEntry);

				if (grammarParserEntry is Grammar<TToken>.ExternalLexerEntry externalLexerEntry)
					return CreateSubLexerEntry(externalLexerEntry);

				throw new ArgumentOutOfRangeException(nameof(grammarParserEntry));
			}

			private static Func<AutomataContext, PredicateResult> CreatePredicateDelegate(Parser<TToken>.PredicateEntry predicateEntry)
			{
				return c => predicateEntry.Predicate(((ParserAutomataContextState)((ParserAutomataContext)c).ContextStateInternal).ParserContext) ? PredicateResult.True : PredicateResult.False;
			}

			private ProcessAutomataContext CreateProcessContext(Grammar<TToken>.ParserRule parserRule, LexemeSource<TToken> lexemeSource, ParserContext parserContext, ProcessKind processKind, Parser<TGrammar, TToken> parser)
			{
				return new ProcessAutomataContext(GetParserRule(parserRule), lexemeSource, parserContext, processKind, parser, this);
			}

			private Entry CreateSubLexerEntry(Grammar<TToken>.ExternalLexerEntry externalLexerEntry)
			{
				var type = externalLexerEntry.GetType();
				var typeArguments = type.GenericTypeArguments.Skip(1).ToArray();
				var externalParserMethod = GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic).Single(m => m.Name == nameof(CreateSubLexerEntry) && m.GetGenericArguments().Length == typeArguments.Length);
				var externalParserGenericMethod = externalParserMethod.MakeGenericMethod(typeArguments);

				return (Entry)externalParserGenericMethod.Invoke(this, new object[] { externalLexerEntry });
			}

			private Entry CreateSubLexerEntry<TExternalToken>(Grammar<TToken>.ExternalLexerEntry<TExternalToken> externalLexerEntry) where TExternalToken : unmanaged, Enum
			{
				var externalLexerInvokeInfo = new ExternalLexerInvokeInfo<TExternalToken>(externalLexerEntry);

				return new ParserPredicateEntry<Lexeme<TExternalToken>>(externalLexerEntry, c => externalLexerInvokeInfo.ExternalLex(c), ParserPredicateKind.ExternalLexer);
			}

			private SyntaxTreeAutomataContext CreateSyntaxTreeContext(Grammar<TToken>.ParserRule parserRule, LexemeSource<TToken> lexemeSource, ParserContext parserContext, ProcessKind processKind, Parser<TGrammar, TToken> parser)
			{
				return new SyntaxTreeAutomataContext(GetParserRule(parserRule), lexemeSource, parserContext, processKind, parser, this);
			}

			private VisitorAutomataContext CreateVisitorContext<TResult>(Visitor<TResult> visitor, Grammar<TToken>.ParserRule parserRule, LexemeSource<TToken> lexemeSource, ParserContext parserContext, ProcessKind processKind,
				Parser<TGrammar, TToken> parser)
			{
				return new VisitorAutomataContext(visitor, GetParserRule(parserRule), lexemeSource, parserContext, processKind, parser, this);
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
					Grammar<TToken>.ExternalParserEntry externalParserEntry => externalParserEntry.Name,
					Grammar<TToken>.ExternalLexerEntry externalLexerEntry => externalLexerEntry.Name,
					_ => null
				};
			}

			private string GenerateProductionName()
			{
				return $"GeneratedTransition{_generateProductionNameCount++}";
			}

			private static ProductionArgument GetParserEntryArgument(Entry entry)
			{
				return ((IParserEntry)entry).ProductionArgument;
			}

			private ParserRule GetParserRule(Grammar<TToken>.ParserRule parserRule)
			{
				return ParserRuleDictionary.GetValueOrCreate(parserRule, pr => new ParserRule(pr.Name, parserRule.IsInline));
			}

			public TResult Parse<TResult>(Grammar<TToken>.ParserRule parserRule, LexemeSource<TToken> lexemeSource, ParserContext parserContext, Parser<TGrammar, TToken> parser)
			{
				using var context = CreateSyntaxTreeContext(parserRule, lexemeSource, parserContext, ProcessKind.Process, parser);
				using var result = context.Process.Run().Verify();

				return context.GetResult<TResult>();
			}

			public void Parse(Grammar<TToken>.ParserRule parserRule, LexemeSource<TToken> lexemeSource, ParserContext parserContext, Parser<TGrammar, TToken> parser)
			{
				using var context = CreateProcessContext(parserRule, lexemeSource, parserContext, ProcessKind.Process, parser);
				using var result = context.Process.Run().Verify();
			}

			public TResult Parse<TResult>(Visitor<TResult> visitor, Grammar<TToken>.ParserRule parserRule, LexemeSource<TToken> lexemeSource, ParserContext parserContext, Parser<TGrammar, TToken> parser)
			{
				using var context = CreateVisitorContext(visitor, parserRule, lexemeSource, parserContext, ProcessKind.Process, parser);
				using var result = context.Process.Run().Verify();

				return context.GetResult<TResult>();
			}

			private void RegisterParserRule(Grammar<TToken>.ParserRule grammarParserRule)
			{
				if (_registeredRules.Add(grammarParserRule) == false)
					throw new InvalidOperationException();

				var parserRule = GetParserRule(grammarParserRule);
				var parserProductions = grammarParserRule.Productions.Select(t => new ParserProduction(this, grammarParserRule, t)).ToList();

				//parserState.Inline |= parserRule.AggressiveInlining;

				EliminateLeftFactoring(parserRule, parserProductions);
				EliminateLeftRecursion(parserRule, parserProductions);

				AddRule(parserRule, parserProductions);
			}

			private int RegisterProduction(ParserProduction parserProduction)
			{
				var productionIndex = Productions.Count;

				Productions.Add(parserProduction);

				return productionIndex;
			}

			private sealed class LexemeStream : InstructionStream
			{
				private int _instructionReaderPosition;

				public LexemeStream(Pool<InstructionStream> pool) : base(pool)
				{
				}

				private ISeekableInstructionReader SeekableInstructionReader => (ISeekableInstructionReader)InstructionReader;

				public override InstructionStream Advance(int instructionPointer, Automata<Lexeme<TToken>, TToken> automata)
				{
					var copy = (LexemeStream)Pool.Get().Mount(InstructionReader, automata);

					copy.AdvanceInstructionPosition(this, instructionPointer);

					return copy;
				}

				private void AdvanceInstructionPosition(LexemeStream lexemeStream, int instructionPointer)
				{
					StartPosition = SeekableInstructionReader.Position;
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

					return PeekInstruction(localInstructionPointer).End;
				}

				protected override void LoadPosition()
				{
					if (SeekableInstructionReader.Position != _instructionReaderPosition)
						SeekableInstructionReader.Position = _instructionReaderPosition;
				}

				public override InstructionStream Mount(IInstructionReader instructionReader, Automata<Lexeme<TToken>, TToken> automata)
				{
					var instructionQueue = base.Mount(instructionReader, automata);

					_instructionReaderPosition = SeekableInstructionReader.Position;

					return instructionQueue;
				}

				protected override void OnReleased()
				{
					_instructionReaderPosition = -1;

					base.OnReleased();
				}

				protected override void SavePosition()
				{
					_instructionReaderPosition = SeekableInstructionReader.Position;
				}
			}

			private sealed class LexemeStreamInstructionReader : ISeekableInstructionReader
			{
				private readonly LexemeSource<TToken> _lexemeSource;

				public LexemeStreamInstructionReader(LexemeSource<TToken> lexemeSource)
				{
					_lexemeSource = lexemeSource;
				}

				public int ReadPage(int bufferLength, out Lexeme<TToken>[] lexemesBuffer, out int[] operandsBuffer)
				{
					RentBuffers(bufferLength, out var localLexemesBuffer, out var localOperandsBuffer);

					lexemesBuffer = localLexemesBuffer;
					operandsBuffer = localOperandsBuffer;

					return _lexemeSource.Read(localLexemesBuffer, localOperandsBuffer, 0, bufferLength, true);
				}

				public int ReadPage(int bufferOffset, int bufferLength, Lexeme<TToken>[] lexemesBuffer, int[] operandsBuffer)
				{
					return _lexemeSource.Read(lexemesBuffer, operandsBuffer, bufferOffset, bufferLength, true);
				}

				public void ReleaseBuffers(Lexeme<TToken>[] instructionsBuffer, int[] operandsBuffer)
				{
					Lexer<TGrammar, TToken>.ReturnLexemeBuffers(instructionsBuffer, operandsBuffer);
				}

				public void RentBuffers(int bufferLength, out Lexeme<TToken>[] instructionsBuffer, out int[] operandsBuffer)
				{
					Lexer<TGrammar, TToken>.RentLexemeBuffers(bufferLength, out instructionsBuffer, out operandsBuffer);
				}

				public int Position
				{
					get => _lexemeSource.Position;
					set => _lexemeSource.Position = value;
				}

				public void Dispose()
				{
				}
			}
		}
	}
}