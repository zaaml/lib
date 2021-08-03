// <copyright file="Parser.Automata.ValueParserAutomataContext.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Zaaml.Core;
using Zaaml.Core.Extensions;
using Zaaml.Core.Utils;

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
				private const BindingFlags InstancePublicPrivate = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

				private static readonly FieldInfo TextSourceSpanFieldInfo = typeof(ValueParserAutomataContext).GetField(nameof(TextSourceSpan), InstancePublicPrivate);
				//private static readonly MethodInfo TextSourceGetTextMethodInfo = typeof(TextSource).GetMethod(nameof(Text.TextSource.GetText), InstancePublicPrivate);
				//private static readonly MethodInfo TextSourceGetSpanMethodInfo = typeof(TextSource).GetMethod(nameof(Text.TextSource.GetSpan), InstancePublicPrivate);
				private static readonly FieldInfo LexemeStartFieldInfo = typeof(Lexeme<TToken>).GetField(nameof(Lexeme<TToken>.StartField), InstancePublicPrivate);
				private static readonly FieldInfo LexemeEndFieldInfo = typeof(Lexeme<TToken>).GetField(nameof(Lexeme<TToken>.EndField), InstancePublicPrivate);
				private static readonly FieldInfo LexemeTokenFieldInfo = typeof(Lexeme<TToken>).GetField(nameof(Lexeme<TToken>.TokenField), InstancePublicPrivate);
				private static readonly FieldInfo ProductionEntityArgumentsFieldInfo = typeof(ProductionEntity).GetField(nameof(ProductionEntity.Arguments), InstancePublicPrivate);
				private static readonly FieldInfo ProductionEntityStackFieldInfo = typeof(ValueParserAutomataContext).GetField(nameof(_productionEntityStack), InstancePublicPrivate);
				private static readonly FieldInfo ProductionEntityStackTailFieldInfo = typeof(ValueParserAutomataContext).GetField(nameof(_productionEntityStackTail), InstancePublicPrivate);


				private static readonly FieldInfo ProductionEntityResultFieldInfo = typeof(ProductionEntity).GetField(nameof(ProductionEntity.Result), InstancePublicPrivate);

				public static readonly FieldInfo SyntaxTreeFactoryFieldInfo = typeof(ValueParserAutomataContext).GetField(nameof(_syntaxTreeFactory), InstancePublicPrivate);


				public static readonly FieldInfo LexemeStringConverterFieldInfo = typeof(ValueParserAutomataContext).GetField(nameof(LexemeStringConverter), InstancePublicPrivate);
				public static readonly FieldInfo LexemeTokenConverterFieldInfo = typeof(ValueParserAutomataContext).GetField(nameof(LexemeTokenConverter), InstancePublicPrivate);


				[UsedImplicitly] private static readonly MethodInfo DebugMethodInfo = typeof(ValueParserAutomataContext).GetMethod(nameof(Debug), InstancePublicPrivate);

				private static readonly MethodInfo GetInstructionReferenceMethodInfo = typeof(ValueParserAutomataContext).GetProperty(nameof(InstructionReference), InstancePublicPrivate)?.GetGetMethod();
				private static readonly MethodInfo GetInstructionTextMethodInfo = typeof(ValueParserAutomataContext).GetMethod(nameof(GetInstructionText), InstancePublicPrivate);
				private static readonly MethodInfo GetInstructionMethodInfo = typeof(ValueParserAutomataContext).GetProperty(nameof(Instruction), InstancePublicPrivate)?.GetGetMethod();
				private static readonly MethodInfo EnsureProductionEntityStackDepthMethodInfo = typeof(ValueParserAutomataContext).GetMethod(nameof(EnsureProductionEntityStackDepth), InstancePublicPrivate);
				private static readonly MethodInfo PeekProductionEntityMethodInfo = typeof(ValueParserAutomataContext).GetMethod(nameof(PeekProductionEntity), InstancePublicPrivate);
				private static readonly MethodInfo PredicateResultGetResultMethodInfo = typeof(PredicateResult).GetMethod(nameof(PredicateResult.GetResult), InstancePublicPrivate);
				private static readonly MethodInfo EnterProductionMethodInfo = typeof(ValueParserAutomataContext).GetMethod(nameof(EnterProduction), InstancePublicPrivate);
				private static readonly MethodInfo ConsumeProductionEntityMethodInfo = typeof(ValueParserAutomataContext).GetMethod(nameof(ConsumeProductionEntity), InstancePublicPrivate);
				private static readonly MethodInfo LeaveRuleEntryMethodInfo = typeof(ValueParserAutomataContext).GetMethod(nameof(LeaveRuleEntry), InstancePublicPrivate);
				private static readonly MethodInfo ProductionEntityReturnMethodInfo = typeof(ProductionEntity).GetMethod(nameof(ProductionEntity.Return), InstancePublicPrivate);

				private static readonly MethodInfo EnterLeftRecursionProductionMethodInfo = typeof(ValueParserAutomataContext).GetMethod(nameof(EnterLeftRecursionProduction), InstancePublicPrivate);
				private static readonly MethodInfo PushLeftFactorProductionInstanceBuilderMethodInfo = typeof(ValueParserAutomataContext).GetMethod(nameof(PushLeftFactorProductionInstanceBuilder), InstancePublicPrivate);

				private static readonly MethodInfo LeaveLeftFactorProductionMethodInfo = typeof(ValueParserAutomataContext).GetMethod(nameof(LeaveLeftFactorProduction), InstancePublicPrivate);
				private static readonly MethodInfo LeaveProductionMethodInfo = typeof(ValueParserAutomataContext).GetMethod(nameof(LeaveProduction), InstancePublicPrivate);

				private static readonly MethodInfo OnAfterConsumeValueMethodInfo = typeof(ValueParserAutomataContext).GetMethod(nameof(OnAfterConsumeValue), InstancePublicPrivate);

				private readonly Stack<ParserAutomataContextState> _parserAutomataContextStatesPool = new();
				private readonly ParserProduction[] _productions;
				private ProductionEntity[] _productionEntityStack = new ProductionEntity[64];
				private int _productionEntityStackTail;
				private SyntaxFactory _syntaxTreeFactory;

				public readonly Converter<Lexeme<TToken>, string> LexemeStringConverter;
				public readonly Converter<Lexeme<TToken>, TToken> LexemeTokenConverter;

				protected ValueParserAutomataContext(ParserRule rule, ParserAutomata parserAutomata) : base(rule, parserAutomata)
				{
					_productions = Automata.Productions.ToArray();

					LexemeStringConverter = lexeme => TextSourceSpan.GetText(lexeme.StartField, lexeme.EndField - lexeme.StartField);
					LexemeTokenConverter = lexeme => lexeme.TokenField;
				}

				protected override void BuildBeginExecutionPath(ILBuilderContext ilBuilderContext, int stackDelta)
				{
					if (stackDelta <= 0) 
						return;

					ilBuilderContext.EmitLdContext();
					ilBuilderContext.IL.Emit(OpCodes.Ldc_I4, stackDelta);
					ilBuilderContext.IL.Emit(OpCodes.Call, EnsureProductionEntityStackDepthMethodInfo);
				}

				protected override void BuildConsumePredicateResult(ILBuilderContext ilBuilderContext, LocalBuilder predicateResultLocal, PredicateEntryBase predicateEntryBase)
				{
					var argument = ((IParserEntry) predicateEntryBase.GetActualPredicateEntry()).ProductionArgument;
					
					if (argument is not (ParserProductionArgument or LexerProductionArgument)) 
						return;

					var argumentLocal = ilBuilderContext.IL.DeclareLocal(typeof(ProductionEntityArgument));
					var resultLocal = ilBuilderContext.IL.DeclareLocal(typeof(object));

					EmitPushArgument(ilBuilderContext, argument, 0);

					ilBuilderContext.IL.Emit(OpCodes.Stloc, argumentLocal);

					ilBuilderContext.IL.Emit(OpCodes.Ldloc, predicateResultLocal);
					ilBuilderContext.IL.Emit(OpCodes.Callvirt, PredicateResultGetResultMethodInfo);
					ilBuilderContext.IL.Emit(OpCodes.Stloc, predicateResultLocal);

					argument.EmitConsumeValue(argumentLocal, resultLocal, this, ilBuilderContext);

					EmitTryReturn(ilBuilderContext, argument);
				}

				private static void EmitTryReturn(ILBuilderContext ilBuilderContext, ProductionArgument argument)
				{
					if (argument.ParserProduction.Binder.TryReturn == false)
						return;

					ilBuilderContext.EmitLdContext();
					ilBuilderContext.IL.Emit(OpCodes.Ldc_I4, argument.ArgumentIndex);
					ilBuilderContext.IL.Emit(OpCodes.Call, OnAfterConsumeValueMethodInfo);
				}

				protected override void BuildEnterProduction(ILBuilderContext ilBuilderContext, Production production)
				{
					if (production is not ParserProduction parserProduction)
						return;

					MethodInfo method = null;

					if (parserProduction.LeftFactorProduction != null || parserProduction.LeftFactorEntry != null) 
						method = PushLeftFactorProductionInstanceBuilderMethodInfo;
					else if (parserProduction.IsInline)
						return;
					
					if (method == null)
						method = parserProduction.LeftRecursionEntry != null ? EnterLeftRecursionProductionMethodInfo : EnterProductionMethodInfo;

					ilBuilderContext.EmitLdContext();
					ilBuilderContext.IL.Emit(OpCodes.Ldc_I4, parserProduction.ProductionIndex);
					ilBuilderContext.IL.Emit(OpCodes.Call, method);
				}

				protected override void BuildEnterRuleEntry(ILBuilderContext ilBuilderContext, RuleEntry ruleEntry)
				{
					if (ruleEntry == null || ruleEntry.SkipStack)
						return;

					if (ruleEntry is not ParserRuleEntry parserStateEntry || parserStateEntry.ProductionArgument == null) 
						return;

					ilBuilderContext.EmitLdContext();
					ilBuilderContext.EmitLdContext();
					ilBuilderContext.IL.Emit(OpCodes.Ldfld, ProductionEntityStackTailFieldInfo);
					ilBuilderContext.IL.Emit(OpCodes.Ldc_I4_1);
					ilBuilderContext.IL.Emit(OpCodes.Add);
					ilBuilderContext.IL.Emit(OpCodes.Stfld, ProductionEntityStackTailFieldInfo);
				}

				private void LeaveRuleEntry()
				{
					_productionEntityStack[_productionEntityStackTail--].Return();
				}

				protected override void BuildInvoke(ILBuilderContext ilBuilderContext, MatchEntry matchEntry, bool main)
				{
					if (main == false)
						return;

					var argument = matchEntry switch
					{
						ParserSingleMatchEntry s => s.ProductionArgument,
						ParserSetMatchEntry s => s.ProductionArgument,
						_ => throw new ArgumentOutOfRangeException()
					};

					if (argument is not LexerProductionArgument lexerArgument)
						return;

					if (argument.Binder == null)
						return;

					var argumentLocal = ilBuilderContext.IL.DeclareLocal(typeof(ProductionEntityArgument));
					var resultLocal = ilBuilderContext.IL.DeclareLocal(typeof(Lexeme<TToken>));

					EmitPushArgument(ilBuilderContext, lexerArgument, 0);

					ilBuilderContext.IL.Emit(OpCodes.Stloc, argumentLocal);

					EmitGetInstruction(ilBuilderContext);
					ilBuilderContext.IL.Emit(OpCodes.Stloc, resultLocal);

					lexerArgument.EmitConsumeValue(argumentLocal, resultLocal, this, ilBuilderContext);

					EmitTryReturn(ilBuilderContext, argument);
				}

				private static void EmitPushArgument(ILBuilderContext ilBuilderContext, ProductionArgument productionArgument, int tailDelta)
				{
					EmitPeekProductionEntity(ilBuilderContext, tailDelta);
					ilBuilderContext.IL.Emit(OpCodes.Ldfld, ProductionEntityArgumentsFieldInfo);
					ilBuilderContext.IL.Emit(OpCodes.Ldc_I4, productionArgument.ArgumentIndex);
					ilBuilderContext.IL.Emit(OpCodes.Ldelem_Ref);
				}

				protected override void BuildLeaveProduction(ILBuilderContext ilBuilderContext, Production production)
				{
					if (production is not ParserProduction parserProduction)
						return;

					var method = LeaveProductionMethodInfo;

					if (parserProduction.LeftFactorProduction != null || parserProduction.LeftFactorEntry != null)
						method = LeaveLeftFactorProductionMethodInfo;

					ilBuilderContext.EmitLdContext();
					ilBuilderContext.IL.Emit(OpCodes.Ldc_I4, parserProduction.ProductionIndex);
					ilBuilderContext.IL.Emit(OpCodes.Call, method);
				}

				protected override void BuildLeaveRuleEntry(ILBuilderContext ilBuilderContext, RuleEntry ruleEntry)
				{
					if (ruleEntry == null || ruleEntry.SkipStack)
						return;

					var parserStateEntry = ruleEntry as ParserRuleEntry;
					var argument = parserStateEntry?.ProductionArgument;

					if (argument is not ParserProductionArgument) 
						return;

					if (argument.Binder == null)
						return;

#if EMIT_COMSUME_PARSER == false
					var argumentLocal = ilBuilderContext.IL.DeclareLocal(typeof(ProductionEntityArgument));
					var resultLocal = ilBuilderContext.IL.DeclareLocal(typeof(object));

					EmitPushArgument(ilBuilderContext, argument, 1);

					ilBuilderContext.IL.Emit(OpCodes.Stloc, argumentLocal);

					EmitPeekProductionEntityResult(ilBuilderContext);

					ilBuilderContext.IL.Emit(OpCodes.Stloc, resultLocal);

					argument.EmitConsumeValue(argumentLocal, resultLocal, this, ilBuilderContext);

					EmitLeaveRuleEntry(ilBuilderContext);
#else
					ilBuilderContext.EmitLdContext();
					ilBuilderContext.IL.Emit(OpCodes.Ldc_I4, argument.ArgumentIndex);
					ilBuilderContext.IL.Emit(OpCodes.Call, ConsumeProductionEntityMethodInfo);

					//ilBuilderContext.EmitLdContext();
					//ilBuilderContext.IL.Emit(OpCodes.Call, LeaveRuleEntryMethodInfo);
#endif
					EmitTryReturn(ilBuilderContext, argument);
				}

				private static void EmitLeaveRuleEntry(ILBuilderContext ilBuilderContext)
				{
					EmitPeekProductionEntity(ilBuilderContext, 0);
					ilBuilderContext.IL.Emit(OpCodes.Call, ProductionEntityReturnMethodInfo);

					ilBuilderContext.EmitLdContext();
					ilBuilderContext.EmitLdContext();
					ilBuilderContext.IL.Emit(OpCodes.Ldfld, ProductionEntityStackTailFieldInfo);
					ilBuilderContext.IL.Emit(OpCodes.Ldc_I4_1);
					ilBuilderContext.IL.Emit(OpCodes.Sub);
					ilBuilderContext.IL.Emit(OpCodes.Stfld, ProductionEntityStackTailFieldInfo);
				}

				private static void EmitPeekProductionEntity(ILBuilderContext ilBuilderContext, int tailDelta)
				{
					ilBuilderContext.EmitLdContext();
					ilBuilderContext.IL.Emit(OpCodes.Ldfld, ProductionEntityStackFieldInfo);
					ilBuilderContext.EmitLdContext();
					ilBuilderContext.IL.Emit(OpCodes.Ldfld, ProductionEntityStackTailFieldInfo);
					
					if (tailDelta > 0)
					{
						if (tailDelta == 1)
							ilBuilderContext.IL.Emit(OpCodes.Ldc_I4_1);
						else
							ilBuilderContext.IL.Emit(OpCodes.Ldc_I4, tailDelta);

						ilBuilderContext.IL.Emit(OpCodes.Sub);
					}

					ilBuilderContext.IL.Emit(OpCodes.Ldelem_Ref);
				}

				private static void EmitPeekProductionEntityResult(ILBuilderContext ilBuilderContext)
				{
					EmitPeekProductionEntity(ilBuilderContext, 0);
					ilBuilderContext.IL.Emit(OpCodes.Ldfld, ProductionEntityResultFieldInfo);
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

				private void ConsumeProductionEntity(int entryIndex)
				{
					var value = _productionEntityStack[_productionEntityStackTail].Result;

					_productionEntityStack[_productionEntityStackTail - 1].Arguments[entryIndex].ConsumeValue(value);
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
					while (_productionEntityStackTail >= 0)
					{
						_productionEntityStack[_productionEntityStackTail].Reset();
						_productionEntityStackTail--;
					}

					_productionEntityStackTail = 0;

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

				private void EnsureProductionEntityStackDepth(int delta)
				{
					if (_productionEntityStackTail + delta >= _productionEntityStack.Length)
						ArrayUtils.ExpandArrayLength(ref _productionEntityStack, true);
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
					if (_productionEntityStackTail != 0)
						throw new InvalidOperationException();

					return (TResult) _productionEntityStack[0].Result;
				}

				public override void Mount(LexemeSource<TToken> lexemeSource, ParserContext parserContext, Parser<TGrammar, TToken> parser)
				{
					base.Mount(lexemeSource, parserContext, parser);

					_syntaxTreeFactory = parser.CreateSyntaxFactoryInternal();
					_syntaxTreeFactory?.AttachParserContextInternal(parserContext);
				}

				private void OnAfterConsumeValue(int entryIndex)
				{
					_productionEntityStack[_productionEntityStackTail].OnAfterConsumeValue(entryIndex);
				}

				private ProductionEntity PeekProductionEntity()
				{
					return _productionEntityStack[_productionEntityStackTail];
				}

				private void EnterLeftRecursionProduction(int productionIndex)
				{
					ref var productionEntity = ref _productionEntityStack[_productionEntityStackTail];
					productionEntity.BuildEntity(this);

					var value = productionEntity.Result;

					productionEntity.Return();

					productionEntity = _productions[productionIndex].RentEntity();

					var leftRecursionArgumentIndex = productionEntity.ParserProduction.LeftRecursionEntry?.ProductionArgument.ArgumentIndex ?? -1;

					if (leftRecursionArgumentIndex == -1)
						return;

					var leftRecursionArgument = productionEntity.Arguments[leftRecursionArgumentIndex];
					
					leftRecursionArgument.ConsumeValue(value);

					productionEntity.OnAfterConsumeValue(leftRecursionArgument.ArgumentIndex);
				}

				private void PushLeftFactorProductionInstanceBuilder(int productionIndex)
				{
					var parserProduction = _productions[productionIndex];

					if (parserProduction.LeftFactorProduction != null)
						_productionEntityStackTail++;
					
					_productionEntityStack[_productionEntityStackTail] = _productions[productionIndex].RentEntity();
				}

				private void LeaveProduction(int productionIndex)
				{
					if (_productions[productionIndex].IsInline)
						return;

					_productionEntityStack[_productionEntityStackTail].BuildEntity(this);
				}

				private void LeaveLeftFactorProduction(int productionIndex)
				{
					var parserProduction = _productions[productionIndex];

					if (parserProduction.LeftFactorProduction == null) 
						return;

					var productionInstanceBuilder = _productionEntityStack[_productionEntityStackTail--];

					_productionEntityStack[_productionEntityStackTail].UnwindLeftFactorBuilder(productionInstanceBuilder);
				}

				private void EnterProduction(int productionIndex)
				{
					_productionEntityStack[_productionEntityStackTail] = _productions[productionIndex].RentEntity();
				}

				void IParserILBuilder.EmitGetInstructionText(ILBuilderContext ilBuilderContext) => EmitGetInstructionText(ilBuilderContext);

				void IParserILBuilder.EmitGetInstructionToken(ILBuilderContext ilBuilderContext) => EmitGetInstructionToken(ilBuilderContext);

				void IParserILBuilder.EmitGetInstruction(ILBuilderContext ilBuilderContext) => EmitGetInstruction(ilBuilderContext);

				void IParserILBuilder.EmitLdTextSourceSpan(ILBuilderContext ilBuilderContext) => EmitLdTextSourceSpan(ilBuilderContext);
			}
		}
	}
}