// <copyright file="Parser.Automata.ParserProcess.IL.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Reflection;
using System.Reflection.Emit;
using static Zaaml.Core.Reflection.BF;

// ReSharper disable StaticMemberInGenericType

namespace Zaaml.Text
{
	internal partial class Parser<TGrammar, TToken>
	{
		private sealed partial class ParserAutomata
		{
			private partial class ParserProcess
			{
				internal partial class ParserILGenerator : ProcessILGenerator
				{
					private static readonly FieldInfo TextSourceSpanFieldInfo = ParserProcessType.GetField(nameof(_textSpan), IPNP);

					private static readonly FieldInfo LexemeStartFieldInfo = typeof(Lexeme<TToken>).GetField(nameof(Lexeme<TToken>.StartField), IPNP);
					private static readonly FieldInfo LexemeEndFieldInfo = typeof(Lexeme<TToken>).GetField(nameof(Lexeme<TToken>.EndField), IPNP);
					private static readonly FieldInfo LexemeTokenFieldInfo = typeof(Lexeme<TToken>).GetField(nameof(Lexeme<TToken>.TokenField), IPNP);
					private static readonly FieldInfo ProductionEntityArgumentsFieldInfo = typeof(ProductionEntity).GetField(nameof(ProductionEntity.Arguments), IPNP);

					private static readonly FieldInfo ProductionEntityStackFieldInfo = ParserProcessType.GetField(nameof(_productionEntityStack), IPNP);
					private static readonly FieldInfo ProductionEntityStackTailFieldInfo = ParserProcessType.GetField(nameof(_productionEntityStackTail), IPNP);


					private static readonly FieldInfo ProductionEntityResultFieldInfo = typeof(ProductionEntity).GetField(nameof(ProductionEntity.Result), IPNP);

					public static readonly FieldInfo SyntaxTreeFactoryFieldInfo = ParserProcessType.GetField(nameof(_syntaxTreeFactory), IPNP);


					public static readonly FieldInfo LexemeStringConverterFieldInfo = ParserProcessType.GetField(nameof(LexemeStringConverter), IPNP);
					public static readonly FieldInfo LexemeTokenConverterFieldInfo = ParserProcessType.GetField(nameof(LexemeTokenConverter), IPNP);

					private static readonly MethodInfo GetInstructionReferenceMethodInfo = ParserProcessType.GetProperty(nameof(InstructionReference), IPNP)?.GetGetMethod();
					private static readonly MethodInfo GetInstructionTextMethodInfo = ParserProcessType.GetMethod(nameof(GetInstructionText), IPNP);
					private static readonly MethodInfo GetInstructionMethodInfo = ParserProcessType.GetProperty(nameof(Instruction), IPNP)?.GetGetMethod();
					private static readonly MethodInfo EnsureProductionEntityStackDepthMethodInfo = ParserProcessType.GetMethod(nameof(EnsureProductionEntityStackDepth), IPNP);
					private static readonly MethodInfo PeekProductionEntityMethodInfo = ParserProcessType.GetMethod(nameof(PeekProductionEntity), IPNP);
					private static readonly MethodInfo PredicateResultGetResultMethodInfo = typeof(PredicateResult).GetMethod(nameof(PredicateResult.GetResult), IPNP);
					private static readonly MethodInfo EnterProductionMethodInfo = ParserProcessType.GetMethod(nameof(EnterProduction), IPNP);
					private static readonly MethodInfo ConsumeProductionEntityMethodInfo = ParserProcessType.GetMethod(nameof(ConsumeProductionEntity), IPNP);
					private static readonly MethodInfo LeaveRuleEntryMethodInfo = ParserProcessType.GetMethod(nameof(LeaveRuleEntry), IPNP);

					public static readonly MethodInfo GetLexemeTextMethodInfo = ParserProcessType.GetMethod(nameof(GetLexemeString), IPNP);

					private static readonly MethodInfo ProductionEntityReturnMethodInfo = typeof(ProductionEntity).GetMethod(nameof(ProductionEntity.Return), IPNP);

					private static readonly MethodInfo EnterLeftRecursionProductionMethodInfo = ParserProcessType.GetMethod(nameof(EnterLeftRecursionProduction), IPNP);
					private static readonly MethodInfo EnterLeftFactoringProductionMethodInfo = ParserProcessType.GetMethod(nameof(EnterLeftFactoringProduction), IPNP);

					private static readonly MethodInfo LeaveLeftFactoringProductionMethodInfo = ParserProcessType.GetMethod(nameof(LeaveLeftFactoringProduction), IPNP);
					private static readonly MethodInfo LeaveLeftRecursionProductionMethodInfo = ParserProcessType.GetMethod(nameof(LeaveLeftRecursionProduction), IPNP);
					private static readonly MethodInfo LeaveProductionMethodInfo = ParserProcessType.GetMethod(nameof(LeaveProduction), IPNP);

					public ParserILGenerator()
					{
					}

					private static Type ParserProcessType => typeof(ParserProcess);

					protected override void EmitBeginExecutionPath(ILContext context, int stackDelta)
					{
#if AUTOMATA_VERIFY_STACK
					if (stackDelta <= 0) 
						return;

					context.EmitLdContext();
					context.IL.Emit(OpCodes.Ldc_I4, stackDelta);
					context.IL.Emit(OpCodes.Call, EnsureProductionEntityStackDepthMethodInfo);
#endif
					}

					protected override void EmitConsumePredicateResult(ILContext context, LocalBuilder predicateResultLocal, PredicateEntryBase predicateEntryBase)
					{
						var argument = ((IParserEntry)predicateEntryBase.GetActualPredicateEntry()).ProductionArgument;

						if (argument is not (ParserProductionArgument or LexerProductionArgument))
							return;

						var argumentLocal = context.IL.DeclareLocal(typeof(ProductionEntityArgument));
						var resultLocal = context.IL.DeclareLocal(typeof(object));

						EmitPushArgument(context, argument, 0);

						context.IL.Emit(OpCodes.Stloc, argumentLocal);
						context.IL.Emit(OpCodes.Ldloc, predicateResultLocal);
						context.IL.Emit(OpCodes.Callvirt, PredicateResultGetResultMethodInfo);
						context.IL.Emit(OpCodes.Stloc, resultLocal);

						argument.EmitConsumeValue(argumentLocal, resultLocal, context);
					}

					private void EmitConsumeRuleEntryValue(ILContext context, ProductionArgument argument)
					{
						if (argument is not { Binder: { ConsumeValue: true } })
							return;

						EmitDebugPreConsumeParserValue(context, argument);

						var argumentLocal = context.IL.DeclareLocal(typeof(ProductionEntityArgument));
						var resultLocal = context.IL.DeclareLocal(typeof(object));

						EmitPushArgument(context, argument, 1);

						context.IL.Emit(OpCodes.Stloc, argumentLocal);

						EmitPeekProductionEntityResult(context);

						context.IL.Emit(OpCodes.Stloc, resultLocal);

						argument.EmitConsumeValue(argumentLocal, resultLocal, context);

						EmitDebugPostConsumeParserValue(context, argument);
					}

					protected override void EmitEnterProduction(ILContext context, Production production)
					{
						if (production is not ParserProduction parserProduction)
							return;

						var method = parserProduction.Binder switch
						{
							LeftFactoringBinder => EnterLeftFactoringProductionMethodInfo,
							LeftRecursionBinder => EnterLeftRecursionProductionMethodInfo,
							{ } => EnterProductionMethodInfo,
							_ => null
						};

						if (method == null)
							return;

						EmitDebugPreProductionEnter(context, parserProduction);

						context.EmitLdProcess();
						context.IL.Emit(OpCodes.Ldc_I4, parserProduction.ProductionIndex);
						context.IL.Emit(OpCodes.Call, method);

						EmitDebugPostProductionEnter(context, parserProduction);
					}

					protected override void EmitEnterRuleEntry(ILContext context, RuleEntry ruleEntry)
					{
						if (ruleEntry is not ParserRuleEntry parserRuleEntry || parserRuleEntry.ProductionArgument?.Binder == null)
							return;

						EmitDebugPreRuleEnter(context, ruleEntry);

						context.EmitLdProcess();
						context.EmitLdProcess();
						context.IL.Emit(OpCodes.Ldfld, ProductionEntityStackTailFieldInfo);
						context.IL.Emit(OpCodes.Ldc_I4_1);
						context.IL.Emit(OpCodes.Add);
						context.IL.Emit(OpCodes.Stfld, ProductionEntityStackTailFieldInfo);

						EmitDebugPostRuleEnter(context, ruleEntry);
					}

					private static void EmitGetInstruction(ILContext context)
					{
						//context.EmitLdProcess();
						//context.IL.Emit(OpCodes.Call, GetInstructionMethodInfo);
						context.EmitGetInstruction();
					}

					private static void EmitGetInstructionText(ILContext context)
					{
						context.EmitLdProcess();
						context.IL.Emit(OpCodes.Call, GetInstructionTextMethodInfo);
					}

					private static void EmitGetInstructionToken(ILContext context)
					{
						context.EmitLdProcess();
						context.IL.Emit(OpCodes.Call, GetInstructionReferenceMethodInfo);
						context.IL.Emit(OpCodes.Ldfld, LexemeTokenFieldInfo);
					}

					protected override void EmitInvoke(ILContext context, MatchEntry matchEntry, bool main)
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

						EmitDebugPreConsumeLexerValue(context, argument);

						var argumentLocal = context.IL.DeclareLocal(typeof(ProductionEntityArgument));
						var resultLocal = context.IL.DeclareLocal(typeof(Lexeme<TToken>));

						EmitPushArgument(context, lexerArgument, 0);

						context.IL.Emit(OpCodes.Stloc, argumentLocal);

						EmitGetInstruction(context);

						context.IL.Emit(OpCodes.Stloc, resultLocal);

						lexerArgument.EmitConsumeValue(argumentLocal, resultLocal, context);

						EmitDebugPostConsumeLexerValue(context, argument);
					}

					private static void EmitLdTextSourceSpan(ILContext context)
					{
						context.EmitLdProcess();
						context.IL.Emit(OpCodes.Ldfld, TextSourceSpanFieldInfo);
					}

					protected override void EmitLeaveProduction(ILContext context, Production production)
					{
						if (production is not ParserProduction parserProduction)
							return;

						var method = parserProduction.Binder switch
						{
							LeftFactoringBinder => LeaveLeftFactoringProductionMethodInfo,
							LeftRecursionBinder => LeaveLeftRecursionProductionMethodInfo,
							{ } => LeaveProductionMethodInfo,
							_ => null
						};

						if (method == null)
							return;

						EmitDebugPreProductionLeave(context, parserProduction);

						context.EmitLdProcess();
						context.IL.Emit(OpCodes.Ldc_I4, parserProduction.ProductionIndex);
						context.IL.Emit(OpCodes.Call, method);

						EmitDebugPostProductionLeave(context, parserProduction);
					}

					protected override void EmitLeaveRuleEntry(ILContext context, RuleEntry ruleEntry)
					{
						if (ruleEntry is not ParserRuleEntry parserRuleEntry || parserRuleEntry.ProductionArgument?.Binder == null)
							return;

						EmitDebugPreRuleLeave(context, ruleEntry);

						var argument = parserRuleEntry.ProductionArgument;

#if EMIT_COMSUME_PARSER == false
						EmitConsumeRuleEntryValue(context, argument);
#else
						context.EmitLdContext();
						context.IL.Emit(OpCodes.Ldc_I4, argument.ArgumentIndex);
						context.IL.Emit(OpCodes.Call, ConsumeProductionEntityMethodInfo);
#endif
						EmitLeaveRuleEntry(context);

						EmitDebugPostRuleLeave(context, ruleEntry);
					}

					private static void EmitLeaveRuleEntry(ILContext context)
					{
						EmitPeekProductionEntity(context, 0);

						context.IL.Emit(OpCodes.Call, ProductionEntityReturnMethodInfo);

#if DEBUG
						// _productionEntityStack[_productionEntityStackTail] = null;
						{
							context.EmitLdProcess();
							context.IL.Emit(OpCodes.Ldfld, ProductionEntityStackFieldInfo);
							context.EmitLdProcess();
							context.IL.Emit(OpCodes.Ldfld, ProductionEntityStackTailFieldInfo);
							context.IL.Emit(OpCodes.Ldnull);
							context.IL.Emit(OpCodes.Stelem_Ref);
						}
#endif

						context.EmitLdProcess();
						context.EmitLdProcess();

						context.IL.Emit(OpCodes.Ldfld, ProductionEntityStackTailFieldInfo);
						context.IL.Emit(OpCodes.Ldc_I4_1);

						context.IL.Emit(OpCodes.Sub);
						context.IL.Emit(OpCodes.Stfld, ProductionEntityStackTailFieldInfo);
					}

					private static void EmitPeekProductionEntity(ILContext context, int tailDelta)
					{
						context.EmitLdProcess();
						context.IL.Emit(OpCodes.Ldfld, ProductionEntityStackFieldInfo);
						context.EmitLdProcess();
						context.IL.Emit(OpCodes.Ldfld, ProductionEntityStackTailFieldInfo);

						if (tailDelta > 0)
						{
							if (tailDelta == 1)
								context.IL.Emit(OpCodes.Ldc_I4_1);
							else
								context.IL.Emit(OpCodes.Ldc_I4, tailDelta);

							context.IL.Emit(OpCodes.Sub);
						}

						context.IL.Emit(OpCodes.Ldelem_Ref);
					}

					private static void EmitPeekProductionEntityResult(ILContext context)
					{
						EmitPeekProductionEntity(context, 0);
						context.IL.Emit(OpCodes.Ldfld, ProductionEntityResultFieldInfo);
					}

					private static void EmitPushArgument(ILContext context, ProductionArgument productionArgument, int tailDelta)
					{
						EmitPeekProductionEntity(context, tailDelta);
						context.IL.Emit(OpCodes.Ldfld, ProductionEntityArgumentsFieldInfo);
						context.IL.Emit(OpCodes.Ldc_I4, productionArgument.ArgumentIndex);
						context.IL.Emit(OpCodes.Ldelem_Ref);
					}
				}
			}
		}
	}
}