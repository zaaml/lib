// <copyright file="Parser.Automata.ParserProcess.IL.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Reflection.Emit;

// ReSharper disable StaticMemberInGenericType

namespace Zaaml.Text
{
	internal partial class Parser<TGrammar, TToken>
	{
		private sealed partial class ParserAutomata
		{
			private partial class ParserProcess
			{
				private partial class ParserILGenerator : ProcessILGenerator
				{
					public ParserILGenerator(Automata<Lexeme<TToken>, TToken> automata) : base(automata)
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

					private void EmitConsumeSyntaxEntryValue(ILContext context, ProductionArgument argument)
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

					protected override void EmitEnterSyntaxEntry(ILContext context, SyntaxEntry syntaxEntry)
					{
						if (syntaxEntry is not ParserSyntaxEntry parserSyntaxEntry || parserSyntaxEntry.ProductionArgument?.Binder == null)
							return;

						EmitDebugPreSyntaxEnter(context, syntaxEntry);

						context.EmitLdProcess();
						context.EmitLdProcess();
						context.IL.Emit(OpCodes.Ldfld, ProductionEntityStackTailFieldInfo);
						context.IL.Emit(OpCodes.Ldc_I4_1);
						context.IL.Emit(OpCodes.Add);
						context.IL.Emit(OpCodes.Stfld, ProductionEntityStackTailFieldInfo);

						EmitDebugPostSyntaxEnter(context, syntaxEntry);
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

					protected override void EmitValue(ILContext context, ValueEntry valueEntry)
					{
						if (valueEntry is CompositeOperandEntry compositeOperandEntry)
							EmitConsumeCompositeOperand(context, compositeOperandEntry);
					}

					private static void EmitConsumeCompositeOperand(ILContext context, CompositeOperandEntry compositeOperandEntry)
					{
						var argument = compositeOperandEntry.ProductionArgument;

						if (argument is not LexerProductionArgument lexerArgument)
							return;

						if (argument.Binder == null)
							return;

						var argumentLocal = context.IL.DeclareLocal(typeof(ProductionEntityArgument));
						var resultLocal = context.IL.DeclareLocal(typeof(Lexeme<TToken>));

						EmitPushArgument(context, lexerArgument, 0);

						context.IL.Emit(OpCodes.Stloc, argumentLocal);

						context.EmitLdProcess();
						context.IL.Emit(OpCodes.Ldc_I4, compositeOperandEntry.TokenCode);
						context.IL.Emit(OpCodes.Ldc_I4, compositeOperandEntry.SimpleTokenCount);
						context.IL.Emit(OpCodes.Call, GetCompositeTokenLexemeMethodInfo);

						context.IL.Emit(OpCodes.Stloc, resultLocal);

						lexerArgument.EmitConsumeValue(argumentLocal, resultLocal, context);
					}

					protected override void EmitInvoke(ILContext context, MatchEntry matchEntry)
					{
						var argument = matchEntry switch
						{
							ParserOperandMatchEntry s => s.ProductionArgument,
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

					protected override void EmitLeaveSyntaxEntry(ILContext context, SyntaxEntry syntaxEntry)
					{
						if (syntaxEntry is not ParserSyntaxEntry parserSyntaxEntry || parserSyntaxEntry.ProductionArgument?.Binder == null)
							return;

						EmitDebugPreSyntaxLeave(context, syntaxEntry);

						var argument = parserSyntaxEntry.ProductionArgument;

#if EMIT_CONSUME_PARSER == false
						EmitConsumeSyntaxEntryValue(context, argument);
#else
						context.EmitLdContext();
						context.IL.Emit(OpCodes.Ldc_I4, argument.ArgumentIndex);
						context.IL.Emit(OpCodes.Call, ConsumeProductionEntityMethodInfo);
#endif

						EmitLeaveSyntaxEntry(context);

						EmitDebugPostSyntaxLeave(context, syntaxEntry);
					}

					private static void EmitLeaveSyntaxEntry(ILContext context)
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