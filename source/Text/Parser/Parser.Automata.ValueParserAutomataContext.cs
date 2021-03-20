// <copyright file="Parser.Automata.ValueParserAutomataContext.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Zaaml.Core;
using Zaaml.Core.Extensions;

// ReSharper disable StaticMemberInGenericType

namespace Zaaml.Text
{
	internal abstract partial class Parser<TGrammar, TToken>
	{
		private sealed partial class ParserAutomata
		{
			private static readonly string[] StaticLexemes;

			static ParserAutomata()
			{
				StaticLexemes = new string[InstructionsRange.Maximum + 1];

				//var grammar = Grammar.Get<TGrammar, TToken>();
				//var stringBuilder = new StringBuilder();

				//static bool BuildStaticLexeme(Grammar<TToken>.PatternCollection patternCollection, StringBuilder stringBuilder)
				//{
				//	if (patternCollection.Patterns.Count != 1)
				//		return false;

				//	var pattern = patternCollection.Patterns[0];

				//	foreach (var patternEntry in pattern.Entries)
				//	{
				//		if (patternEntry is Grammar<TToken>.CharEntry charEntry)
				//			stringBuilder.Append(charEntry.Char);
				//		else if (patternEntry is Grammar<TToken>.TokenFragment fragment)
				//		{
				//			if (BuildStaticLexeme(fragment.Pattern, stringBuilder) == false)
				//				return false;
				//		}
				//		else if (patternEntry is Grammar<TToken>.TokenFragmentEntry fragmentEntry)
				//		{
				//			if (BuildStaticLexeme(fragmentEntry.Fragment.Pattern, stringBuilder) == false)
				//				return false;
				//		}
				//		else
				//			return false;
				//	}

				//	return true;
				//}

				//foreach (var tokenRule in grammar.TokenRules)
				//{
				//	if (tokenRule.Pattern.Patterns.Count != 1)
				//		continue;

				//	stringBuilder.Clear();

				//	if (BuildStaticLexeme(tokenRule.Pattern, stringBuilder) == false)
				//		continue;

				//	if (stringBuilder.Length > 0)
				//		StaticLexemes[tokenRule.TokenCode] = stringBuilder.ToString();
				//}
			}

			private abstract class ValueParserAutomataContext : ParserAutomataContext, IParserILBuilder
			{
				private const BindingFlags Flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
				private static readonly FieldInfo TextSourceSpanFieldInfo = typeof(ValueParserAutomataContext).GetField(nameof(TextSourceSpan), Flags);
				//private static readonly MethodInfo TextSourceGetTextMethodInfo = typeof(TextSource).GetMethod(nameof(Text.TextSource.GetText), Flags);
				//private static readonly MethodInfo TextSourceGetSpanMethodInfo = typeof(TextSource).GetMethod(nameof(Text.TextSource.GetSpan), Flags);
				private static readonly FieldInfo LexemeStartFieldInfo = typeof(Lexeme<TToken>).GetField(nameof(Lexeme<TToken>.StartField), Flags);
				private static readonly FieldInfo LexemeEndFieldInfo = typeof(Lexeme<TToken>).GetField(nameof(Lexeme<TToken>.EndField), Flags);
				private static readonly FieldInfo LexemeTokenFieldInfo = typeof(Lexeme<TToken>).GetField(nameof(Lexeme<TToken>.TokenField), Flags);
				private static readonly FieldInfo ProductionInstanceBuilderArgumentsFieldInfo = typeof(ProductionInstanceBuilder).GetField(nameof(ProductionInstanceBuilder.Arguments), Flags);
				private static readonly FieldInfo StateEntryIndexFieldInfo = typeof(ValueParserAutomataContext).GetField(nameof(_stateEntryIndex), Flags);
				private static readonly FieldInfo SyntaxTreeFactoryFieldInfo = typeof(ValueParserAutomataContext).GetField(nameof(_syntaxTreeFactory), Flags);

				[UsedImplicitly] private static readonly MethodInfo DebugMethodInfo = typeof(ValueParserAutomataContext).GetMethod(nameof(Debug), BindingFlags.Instance | BindingFlags.NonPublic);

				private static readonly MethodInfo GetInstructionReferenceMethodInfo = typeof(ValueParserAutomataContext).GetProperty(nameof(InstructionReference), Flags)?.GetGetMethod();
				private static readonly MethodInfo GetInstructionTextMethodInfo = typeof(ValueParserAutomataContext).GetMethod(nameof(GetInstructionText), Flags);
				private static readonly MethodInfo GetInstructionMethodInfo = typeof(ValueParserAutomataContext).GetProperty(nameof(Instruction), Flags)?.GetGetMethod();
				private static readonly MethodInfo EnsureStateEntrySizeMethodInfo = typeof(ValueParserAutomataContext).GetMethod(nameof(EnsureStateEntrySize), Flags);
				private static readonly MethodInfo PeekStateEntryMethodInfo = typeof(ValueParserAutomataContext).GetMethod(nameof(PeekStateEntry), Flags);
				private static readonly MethodInfo PredicateResultGetResultMethodInfo = typeof(PredicateResult).GetMethod(nameof(PredicateResult.GetResult), BindingFlags.Instance | BindingFlags.NonPublic);
				private static readonly MethodInfo PushProductionInstanceBuilderMethodInfo = typeof(ValueParserAutomataContext).GetMethod(nameof(PushProductionInstanceBuilder), BindingFlags.Instance | BindingFlags.NonPublic);
				private static readonly MethodInfo ConsumeEntryMethodInfo = typeof(ValueParserAutomataContext).GetMethod(nameof(ConsumeEntry), BindingFlags.Instance | BindingFlags.NonPublic);

				private static readonly MethodInfo PushLeftRecursionProductionInstanceBuilderMethodInfo =
					typeof(ValueParserAutomataContext).GetMethod(nameof(PushLeftRecursionProductionInstanceBuilder), BindingFlags.Instance | BindingFlags.NonPublic);

				private static readonly MethodInfo ConsumeLexerEntryMethodInfo = typeof(ValueParserAutomataContext).GetMethod(nameof(ConsumeLexerEntry), BindingFlags.Instance | BindingFlags.NonPublic);
				private static readonly MethodInfo OnAfterConsumeValueMethodInfo = typeof(ValueParserAutomataContext).GetMethod(nameof(OnAfterConsumeValue), BindingFlags.Instance | BindingFlags.NonPublic);

				private readonly Stack<ParserAutomataContextState> _parserAutomataContextStatesPool = new Stack<ParserAutomataContextState>();
				private readonly ProductionInstanceBuilderPool[] _productionBuilderPool;
				private ProductionInstanceBuilder[] _stateEntryArray = new ProductionInstanceBuilder[64];
				private int _stateEntryIndex;
				private SyntaxFactory _syntaxTreeFactory;


				protected ValueParserAutomataContext(ParserState state, ParserAutomata parserAutomata) : base(state, parserAutomata)
				{
					var productionsCount = parserAutomata.Productions.Count;

					_productionBuilderPool = new ProductionInstanceBuilderPool[productionsCount];

					for (var i = 0; i < productionsCount; i++)
					{
						var automataProduction = Automata.Productions[i];

						if (automataProduction.IsInline == false)
							_productionBuilderPool[i] = new ProductionInstanceBuilderPool(automataProduction);
					}
				}

				protected override void BuildBeginExecutionPath(ILBuilderContext ilBuilderContext, int stackDelta)
				{
					if (stackDelta > 0)
					{
						ilBuilderContext.EmitLdContext();
						ilBuilderContext.IL.Emit(OpCodes.Ldc_I4, stackDelta);
						ilBuilderContext.IL.Emit(OpCodes.Call, EnsureStateEntrySizeMethodInfo);
					}
				}

				protected override void BuildConsumePredicateResult(ILBuilderContext ilBuilderContext, LocalBuilder resultLocal, PredicateEntryBase predicateEntryBase)
				{
					var parserEntry = ((IParserEntry) predicateEntryBase.GetActualPredicateEntry()).ParserEntryData;

					if (parserEntry == null || parserEntry.FlatIndex == -1)
						return;

					var parserTransition = parserEntry.ParserProduction;
					var argumentBuilder = parserTransition.Binder.Template[parserEntry.FlatIndex];

					if (argumentBuilder is ParserArgumentBuilder)
						BuildConsumeResult(ilBuilderContext, resultLocal, parserEntry);

					if (argumentBuilder is LexerArgumentBuilder)
						BuildConsumeResult(ilBuilderContext, resultLocal, parserEntry);
				}

				private void BuildConsumeResult(ILBuilderContext ilBuilderContext, LocalBuilder resultLocal, ParserEntryData parserEntry)
				{
					if (parserEntry == null || parserEntry.FlatIndex == -1)
						return;

					var parserProduction = parserEntry.ParserProduction;
					var parserTransition = parserProduction;
					var argumentBuilder = parserTransition.Binder.Template[parserEntry.FlatIndex];

					ilBuilderContext.EmitLdContext();
					ilBuilderContext.IL.Emit(OpCodes.Call, PeekStateEntryMethodInfo);
					ilBuilderContext.IL.Emit(OpCodes.Ldfld, ProductionInstanceBuilderArgumentsFieldInfo);

					ilBuilderContext.IL.Emit(OpCodes.Ldc_I4, parserEntry.FlatIndex);
					ilBuilderContext.IL.Emit(OpCodes.Ldelem_Ref);

					ilBuilderContext.IL.Emit(OpCodes.Ldloc, resultLocal);
					ilBuilderContext.IL.Emit(OpCodes.Callvirt, PredicateResultGetResultMethodInfo);

					if (argumentBuilder is ParserArgumentBuilder parserArgumentBuilder)
					{
						parserArgumentBuilder.EmitConsumeValue(ilBuilderContext);
					}
					else if (argumentBuilder is LexerArgumentBuilder lexerArgumentBuilder)
					{
						ilBuilderContext.IL.Emit(OpCodes.Unbox_Any, typeof(Lexeme<>).MakeGenericType(lexerArgumentBuilder.TokenType));
						lexerArgumentBuilder.EmitConsumeValue(this, ilBuilderContext);
					}

					if (parserProduction.Binder.TryReturn)
					{
						ilBuilderContext.EmitLdContext();
						ilBuilderContext.IL.Emit(OpCodes.Ldc_I4, parserEntry.FlatIndex);
						ilBuilderContext.IL.Emit(OpCodes.Call, OnAfterConsumeValueMethodInfo);
					}
				}

				protected override void BuildEnterProduction(ILBuilderContext ilBuilderContext, Production production)
				{
					if (!(production is ParserProduction parserProduction))
						return;

					if (parserProduction.IsInline)
						return;

					var pushMethod = parserProduction.LeftRecursionEntry != null ? PushLeftRecursionProductionInstanceBuilderMethodInfo : PushProductionInstanceBuilderMethodInfo;

					ilBuilderContext.EmitLdContext();
					ilBuilderContext.IL.Emit(OpCodes.Ldc_I4, parserProduction.ProductionIndex);
					ilBuilderContext.IL.Emit(OpCodes.Call, pushMethod);
				}

				protected override void BuildEnterStateEntry(ILBuilderContext ilBuilderContext, StateEntry stateEntry)
				{
					if (stateEntry == null || stateEntry.SkipStack)
						return;

					if (!(stateEntry is ParserStateEntry parserStateEntry) || parserStateEntry.ParserEntryData.FlatIndex == -1) 
						return;

					ilBuilderContext.EmitLdContext();
					ilBuilderContext.EmitLdContext();
					ilBuilderContext.IL.Emit(OpCodes.Ldfld, StateEntryIndexFieldInfo);
					ilBuilderContext.IL.Emit(OpCodes.Ldc_I4_1);
					ilBuilderContext.IL.Emit(OpCodes.Add);
					ilBuilderContext.IL.Emit(OpCodes.Stfld, StateEntryIndexFieldInfo);
				}

				protected override void BuildInvoke(ILBuilderContext ilBuilderContext, MatchEntry matchEntry, bool main)
				{
					if (main == false)
						return;

					var parserEntry = matchEntry switch
					{
						ParserSingleMatchEntry s => s.ParserEntryData,
						ParserSetMatchEntry s => s.ParserEntryData,
						_ => throw new ArgumentOutOfRangeException()
					};

					if (parserEntry.FlatIndex == -1)
						return;

					var parserProduction = parserEntry.ParserProduction;

					if (parserProduction == null)
						return;

					if (parserProduction.IsInline)
						return;

					var entryBuilder = parserProduction.Binder.Template[parserEntry.FlatIndex];

					if (entryBuilder == null)
						return;

					if (!(entryBuilder is LexerArgumentBuilder lexerArgumentBuilder))
						throw new InvalidOperationException();

					ilBuilderContext.EmitLdContext();
					ilBuilderContext.IL.Emit(OpCodes.Call, PeekStateEntryMethodInfo);
					ilBuilderContext.IL.Emit(OpCodes.Ldfld, ProductionInstanceBuilderArgumentsFieldInfo);
					ilBuilderContext.IL.Emit(OpCodes.Ldc_I4, parserEntry.FlatIndex);
					ilBuilderContext.IL.Emit(OpCodes.Ldelem_Ref);

					lexerArgumentBuilder.EmitConsumeValue(this, ilBuilderContext);

					if (parserProduction.Binder.TryReturn)
					{
						ilBuilderContext.EmitLdContext();
						ilBuilderContext.IL.Emit(OpCodes.Ldc_I4, parserEntry.FlatIndex);
						ilBuilderContext.IL.Emit(OpCodes.Call, OnAfterConsumeValueMethodInfo);
					}

					//ilBuilderContext.EmitLdContext();
					//ilBuilderContext.IL.Emit(OpCodes.Ldc_I4, parserStateEntry.FlatIndex);
					//ilBuilderContext.IL.Emit(OpCodes.Call, ConsumeLexerEntryMethodInfo);
				}

				protected override void BuildLeaveProduction(ILBuilderContext ilBuilderContext, Production production)
				{
				}

				protected override void BuildLeaveStateEntry(ILBuilderContext ilBuilderContext, StateEntry stateEntry)
				{
					if (stateEntry == null || stateEntry.SkipStack)
						return;

					var parserStateEntry = stateEntry as ParserStateEntry;
					var parserEntry = parserStateEntry?.ParserEntryData;

					if (parserEntry != null && parserEntry.FlatIndex != -1)
					{
						var parserProduction = parserEntry.ParserProduction;
						var argumentBuilder = parserProduction.Binder.Template[parserEntry.FlatIndex];

						if (!(argumentBuilder is ParserArgumentBuilder parserArgumentBuilder))
							throw new InvalidOperationException();

						ilBuilderContext.EmitLdContext();
						ilBuilderContext.IL.Emit(OpCodes.Ldc_I4, parserEntry.FlatIndex);
						ilBuilderContext.IL.Emit(OpCodes.Call, ConsumeEntryMethodInfo);

						if (parserProduction.Binder.TryReturn)
						{
							ilBuilderContext.EmitLdContext();
							ilBuilderContext.IL.Emit(OpCodes.Ldc_I4, parserEntry.FlatIndex);
							ilBuilderContext.IL.Emit(OpCodes.Call, OnAfterConsumeValueMethodInfo);
						}
					}
				}

				protected sealed override AutomataContextState CloneContextState(AutomataContextState contextState)
				{
					if (contextState == null)
						return null;

					var parserContextState = (ParserAutomataContextState) contextState;
					var cloneContextState = _parserAutomataContextStatesPool.Count > 0 ? _parserAutomataContextStatesPool.Pop() : new ParserAutomataContextState();
					var parserContextClone = parserContextState.ParserContext.Clone();

					parserContextClone.ParserAutomataContext = this;
					cloneContextState.ParserContext = parserContextClone;

					return cloneContextState;
				}

				private void ConsumeEntry(int entryIndex)
				{
					var value = _stateEntryArray[_stateEntryIndex--];
					var consume = _stateEntryArray[_stateEntryIndex];
					var argument = consume.Arguments[entryIndex];

					argument.ConsumeParserValue(value.CreateInstance(_syntaxTreeFactory));
				}

				private void ConsumeLexerEntry(int entryIndex)
				{
					_stateEntryArray[_stateEntryIndex].Arguments[entryIndex].ConsumeLexerEntry(this);
				}

				protected sealed override AutomataContextState CreateContextState()
				{
					if (ParserContext == null)
						return null;

					var contextState = _parserAutomataContextStatesPool.Count > 0 ? _parserAutomataContextStatesPool.Pop() : new ParserAutomataContextState();

					contextState.ParserContext = ParserContext;

					return contextState;
				}

				[UsedImplicitly]
				private void Debug(object val)
				{
				}

				public override void Dispose()
				{
					while (_stateEntryIndex >= 0)
					{
						_stateEntryArray[_stateEntryIndex].Reset();
						_stateEntryIndex--;
					}

					_stateEntryIndex = 0;

					_syntaxTreeFactory?.DetachParserContextInternal(ParserContext);
					_syntaxTreeFactory = _syntaxTreeFactory.DisposeExchange();

					base.Dispose();
				}

				protected sealed override void DisposeContextState(AutomataContextState contextState)
				{
					if (contextState == null)
						return;

					var parserContextState = (ParserAutomataContextState) contextState;

					parserContextState.ParserContext.Dispose();
					parserContextState.ParserContext = null;

					_parserAutomataContextStatesPool.Push(parserContextState);
				}

				private void EmitConsumeEntry(ILBuilderContext ilBuilderContext, ParserStateEntry parserStateEntry, LocalBuilder resultLocal)
				{
					var parserEntry = parserStateEntry?.ParserEntryData;

					if (parserEntry == null || parserEntry.FlatIndex == -1)
						return;

					var parserProduction = parserEntry.ParserProduction;
					var argumentBuilder = parserProduction.Binder.Template[parserEntry.FlatIndex];

					if (!(argumentBuilder is ParserArgumentBuilder parserArgumentBuilder))
						throw new InvalidOperationException();

					ilBuilderContext.EmitLdContext();
					ilBuilderContext.IL.Emit(OpCodes.Call, PeekStateEntryMethodInfo);
					ilBuilderContext.IL.Emit(OpCodes.Ldfld, ProductionInstanceBuilderArgumentsFieldInfo);
					ilBuilderContext.IL.Emit(OpCodes.Ldc_I4, parserEntry.FlatIndex);
					ilBuilderContext.IL.Emit(OpCodes.Ldelem_Ref);

					ilBuilderContext.IL.Emit(OpCodes.Ldloc, resultLocal);
					parserArgumentBuilder.EmitConsumeValue(ilBuilderContext);

					if (parserProduction.Binder.TryReturn)
					{
						ilBuilderContext.EmitLdContext();
						ilBuilderContext.IL.Emit(OpCodes.Ldc_I4, parserEntry.FlatIndex);
						ilBuilderContext.IL.Emit(OpCodes.Call, OnAfterConsumeValueMethodInfo);
					}
				}

				private void EmitDebugValue(ILBuilderContext ilBuilderContext, object value)
				{
					ilBuilderContext.EmitLdContext();
					ilBuilderContext.EmitLdValue(value);
					ilBuilderContext.IL.Emit(OpCodes.Call, DebugMethodInfo);
				}

				private static void EmitGetInstruction(ILBuilderContext ilBuilderContext)
				{
					ilBuilderContext.EmitLdContext();
					ilBuilderContext.IL.Emit(OpCodes.Call, GetInstructionMethodInfo);
				}

				//private static void EmitGetInstructionSpan(ILBuilderContext ilBuilderContext)
				//{
				//	var lexemeLocal = ilBuilderContext.IL.DeclareLocal(typeof(Lexeme<TToken>*));

				//	ilBuilderContext.EmitLdContext();
				//	ilBuilderContext.IL.Emit(OpCodes.Call, GetInstructionReferenceMethodInfo);
				//	ilBuilderContext.IL.Emit(OpCodes.Stloc, lexemeLocal);
				//	ilBuilderContext.EmitLdContext();
				//	ilBuilderContext.IL.Emit(OpCodes.Ldfld, TextSourceFieldInfo);

				//	ilBuilderContext.IL.Emit(OpCodes.Ldloc, lexemeLocal);
				//	ilBuilderContext.IL.Emit(OpCodes.Ldfld, LexemeStartFieldInfo);
				//	ilBuilderContext.IL.Emit(OpCodes.Ldloc, lexemeLocal);
				//	ilBuilderContext.IL.Emit(OpCodes.Ldfld, LexemeEndFieldInfo);

				//	ilBuilderContext.IL.Emit(OpCodes.Callvirt, TextSourceGetSpanMethodInfo);
				//}

				private static void EmitGetInstructionText(ILBuilderContext ilBuilderContext)
				{
					ilBuilderContext.EmitLdContext();
					ilBuilderContext.IL.Emit(OpCodes.Call, GetInstructionTextMethodInfo);
				}

				private static void EmitGetInstructionToken(ILBuilderContext ilBuilderContext)
				{
					ilBuilderContext.EmitLdContext();
					ilBuilderContext.IL.Emit(OpCodes.Call, GetInstructionReferenceMethodInfo);
					ilBuilderContext.IL.Emit(OpCodes.Ldfld, LexemeTokenFieldInfo);
				}

				//private static void EmitGetLexemeSpan(LocalBuilder lexemeLocal, ILBuilderContext ilBuilderContext)
				//{
				//	ilBuilderContext.EmitLdContext();
				//	ilBuilderContext.IL.Emit(OpCodes.Ldfld, TextSourceFieldInfo);

				//	ilBuilderContext.IL.Emit(OpCodes.Ldloc, lexemeLocal);
				//	ilBuilderContext.IL.Emit(OpCodes.Ldfld, LexemeStartFieldInfo);
				//	ilBuilderContext.IL.Emit(OpCodes.Ldloc, lexemeLocal);
				//	ilBuilderContext.IL.Emit(OpCodes.Ldfld, LexemeEndFieldInfo);

				//	ilBuilderContext.IL.Emit(OpCodes.Callvirt, TextSourceGetSpanMethodInfo);
				//}

				//private static void EmitGetLexemeText(LocalBuilder lexemeLocal, ILBuilderContext ilBuilderContext)
				//{
				//	ilBuilderContext.EmitLdContext();
				//	ilBuilderContext.IL.Emit(OpCodes.Ldfld, TextSourceFieldInfo);

				//	ilBuilderContext.IL.Emit(OpCodes.Ldloc, lexemeLocal);
				//	ilBuilderContext.IL.Emit(OpCodes.Ldfld, LexemeStartFieldInfo);
				//	ilBuilderContext.IL.Emit(OpCodes.Ldloc, lexemeLocal);
				//	ilBuilderContext.IL.Emit(OpCodes.Ldfld, LexemeEndFieldInfo);

				//	ilBuilderContext.IL.Emit(OpCodes.Callvirt, TextSourceGetTextMethodInfo);
				//}

				private static void EmitLdTextSourceSpan(ILBuilderContext ilBuilderContext)
				{
					ilBuilderContext.EmitLdContext();
					ilBuilderContext.IL.Emit(OpCodes.Ldfld, TextSourceSpanFieldInfo);
				}

				private void EnsureStateEntrySize(int delta)
				{
					if (_stateEntryIndex + delta >= _stateEntryArray.Length)
						ResizeStateEntryArrays();
				}

				internal string GetInstructionText()
				{
					ref var lexeme = ref GetInstructionOperand(out var operand);

					if (operand < 0)
						return null;

					return StaticLexemes[operand] ?? TextSourceSpan.GetText(lexeme.StartField, lexeme.EndField - lexeme.StartField);
				}

				public TResult GetResult<TResult>()
				{
					if (_stateEntryIndex != 0)
						throw new InvalidOperationException();

					return (TResult) _stateEntryArray[0].CreateInstance(_syntaxTreeFactory);
				}

				public override void Mount(LexemeSource<TToken> lexemeSource, ParserContext parserContext, Parser<TGrammar, TToken> parser)
				{
					base.Mount(lexemeSource, parserContext, parser);

					_syntaxTreeFactory = parser.CreateSyntaxFactoryInternal();
					_syntaxTreeFactory?.AttachParserContextInternal(parserContext);
				}

				private void OnAfterConsumeValue(int entryIndex)
				{
					_stateEntryArray[_stateEntryIndex].OnAfterConsumeValue(entryIndex);
				}

				private object PeekStateEntry()
				{
					return _stateEntryArray[_stateEntryIndex];
				}

				private void PushLeftRecursionProductionInstanceBuilder(int productionIndex)
				{
					ref var instanceBuilder = ref _stateEntryArray[_stateEntryIndex];
					var value = instanceBuilder.CreateInstance(_syntaxTreeFactory);

					instanceBuilder = _productionBuilderPool[productionIndex].Rent();

					if (instanceBuilder.LeftRecursionArgument == null) 
						return;

					instanceBuilder.LeftRecursionArgument.ConsumeParserValue(value);
					instanceBuilder.OnAfterConsumeValue(instanceBuilder.LeftRecursionArgument.Index);
				}

				private void PushProductionInstanceBuilder(int productionIndex)
				{
					_stateEntryArray[_stateEntryIndex] = _productionBuilderPool[productionIndex].Rent();
				}

				private void ResizeStateEntryArrays()
				{
					var stateEntryArray = new ProductionInstanceBuilder[_stateEntryArray.Length * 2];

					Array.Copy(_stateEntryArray, stateEntryArray, _stateEntryArray.Length);

					_stateEntryArray = stateEntryArray;
				}

				void IParserILBuilder.EmitGetInstructionText(ILBuilderContext ilBuilderContext) => EmitGetInstructionText(ilBuilderContext);

				void IParserILBuilder.EmitGetInstructionToken(ILBuilderContext ilBuilderContext) => EmitGetInstructionToken(ilBuilderContext);

				void IParserILBuilder.EmitGetInstruction(ILBuilderContext ilBuilderContext) => EmitGetInstruction(ilBuilderContext);

				void IParserILBuilder.EmitLdTextSourceSpan(ILBuilderContext ilBuilderContext) => EmitLdTextSourceSpan(ilBuilderContext);
			}
		}
	}
}