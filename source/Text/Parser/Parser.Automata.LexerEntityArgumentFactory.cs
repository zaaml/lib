//// <copyright file="Parser.Automata.LexerEntityArgumentFactory.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
////   Copyright (c) Zaaml. All rights reserved.
//// </copyright>

//using System;
//using System.Reflection;
//using System.Reflection.Emit;

//// ReSharper disable StaticMemberInGenericType

//#pragma warning disable 414

//namespace Zaaml.Text
//{
//	internal abstract partial class Parser<TGrammar, TToken>
//	{
//		private sealed partial class ParserAutomata
//		{
//			private abstract class LexerEntityArgumentFactory : EntityArgumentFactory
//			{
//				protected LexerEntityArgumentFactory(ProductionArgument argument) : base(argument)
//				{
//				}
//			}

//			private abstract class LexerEntityArgumentFactory<TActualToken> : LexerEntityArgumentFactory where TActualToken : unmanaged, Enum
//			{
//				private readonly EntityArgumentFactoryBase _argumentFactory;

//				protected LexerEntityArgumentFactory(ProductionArgument argument) : base(argument)
//				{
//					if (argument.ArgumentType == typeof(Lexeme<TActualToken>))
//						_argumentFactory = new LexemeEntityArgumentFactory(this);
//					else if (argument.ArgumentType == typeof(Lexeme<TActualToken>[]))
//						_argumentFactory = new LexemeArrayEntityArgumentFactory(this);
//					else
//						throw new ArgumentOutOfRangeException();
//				}

//				public override ProductionEntityArgument CreateArgument(ProductionEntity entity)
//				{
//					return _argumentFactory.CreateArgument(entity);
//				}

//				public override void EmitConsumeValue(IParserILBuilder builder, ILBuilderContext ilBuilderContext)
//				{
//					ilBuilderContext.IL.Emit(OpCodes.Unbox_Any, typeof(Lexeme<>).MakeGenericType(typeof(TActualToken)));

//					_argumentFactory.EmitConsumeValue(builder, ilBuilderContext);
//				}

//				public override void EmitPushResetArgument(LocalBuilder argumentLocal, ILGenerator il)
//				{
//					_argumentFactory.EmitPushResetArgument(argumentLocal, il);
//				}

//				private abstract class EntityArgumentFactoryBase
//				{
//					protected EntityArgumentFactoryBase(LexerEntityArgumentFactory lexerEntityArgumentFactory)
//					{
//						LexerEntityArgumentFactory = lexerEntityArgumentFactory;
//					}

//					protected LexerEntityArgumentFactory LexerEntityArgumentFactory { get; }

//					public abstract ProductionEntityArgument CreateArgument(ProductionEntity entity);

//					public abstract void EmitConsumeValue(IParserILBuilder builder, ILBuilderContext ilBuilderContext);

//					public abstract void EmitPushResetArgument(LocalBuilder argumentLocal, ILGenerator il);
//				}

//				// Lexeme Array
//				private sealed class LexemeArrayEntityArgumentFactory : EntityArgumentFactoryBase
//				{
//					private static readonly MethodInfo ListTResultAddMethodInfo = typeof(ArrayArgument<Lexeme<TActualToken>>).GetMethod(nameof(ArrayArgument<Lexeme<TActualToken>>.Add), BindingFlags.Instance | BindingFlags.Public);

//					private static readonly MethodInfo ListTResultToArrayMethodInfo = typeof(ArrayArgument<Lexeme<TActualToken>>).GetMethod(nameof(ArrayArgument<Lexeme<TActualToken>>.ToArray), BindingFlags.Instance | BindingFlags.Public);

//					public LexemeArrayEntityArgumentFactory(LexerEntityArgumentFactory lexerEntityArgumentFactory) : base(lexerEntityArgumentFactory)
//					{
//					}

//					public override ProductionEntityArgument CreateArgument(ProductionEntity entity)
//					{
//						return new ArrayArgument<Lexeme<TActualToken>>(entity, LexerEntityArgumentFactory);
//					}


//					public override void EmitConsumeValue(IParserILBuilder builder, ILBuilderContext ilBuilderContext)
//					{
//						builder.EmitGetInstruction(ilBuilderContext);
//						ilBuilderContext.IL.Emit(OpCodes.Call, ListTResultAddMethodInfo);
//					}

//					public override void EmitPushResetArgument(LocalBuilder argumentLocal, ILGenerator il)
//					{
//						il.Emit(OpCodes.Ldloc, argumentLocal);
//						il.Emit(OpCodes.Call, ListTResultToArrayMethodInfo);
//					}
//				}

//				// Lexeme
//				private sealed class LexemeEntityArgumentFactory : EntityArgumentFactoryBase
//				{
//					private static readonly FieldInfo ArgumentValueFieldInfo = typeof(Argument<Lexeme<TActualToken>>).GetField(nameof(Argument<Lexeme<TActualToken>>.Value), BindingFlags.Public | BindingFlags.Instance);

//					public LexemeEntityArgumentFactory(LexerEntityArgumentFactory lexerEntityArgumentFactory) : base(lexerEntityArgumentFactory)
//					{
//					}

//					public override ProductionEntityArgument CreateArgument(ProductionEntity entity)
//					{
//						return new Argument<TActualToken>(entity, LexerEntityArgumentFactory);
//					}

//					public override void EmitConsumeValue(IParserILBuilder builder, ILBuilderContext ilBuilderContext)
//					{
//						builder.EmitGetInstruction(ilBuilderContext);
//						ilBuilderContext.IL.Emit(OpCodes.Stfld, ArgumentValueFieldInfo);
//					}

//					public override void EmitPushResetArgument(LocalBuilder argumentLocal, ILGenerator il)
//					{
//						il.Emit(OpCodes.Ldloc, argumentLocal);
//						il.Emit(OpCodes.Ldfld, ArgumentValueFieldInfo);
//						il.Emit(OpCodes.Ldloc, argumentLocal);
//						il.Emit(OpCodes.Ldflda, ArgumentValueFieldInfo);
//						il.Emit(OpCodes.Initobj, typeof(Lexeme<TActualToken>));
//					}
//				}
//			}

//			private sealed class LocalLexerEntityArgumentFactory : LexerEntityArgumentFactory<TToken>
//			{
//				public LocalLexerEntityArgumentFactory(ProductionArgument argument) : base(argument)
//				{
//				}
//			}

//			private sealed class ExternalLexerEntityArgumentFactory<TExternalToken> : LexerEntityArgumentFactory<TExternalToken> where TExternalToken : unmanaged, Enum
//			{
//				private readonly ExtParserILBuilder _parserBuilder = new ExtParserILBuilder();

//				public ExternalLexerEntityArgumentFactory(ProductionArgument argument) : base(argument)
//				{
//				}

//				public override void EmitConsumeValue(IParserILBuilder builder, ILBuilderContext ilBuilderContext)
//				{
//					_parserBuilder.Enter(builder, ilBuilderContext);

//					base.EmitConsumeValue(_parserBuilder, ilBuilderContext);

//					_parserBuilder.Leave();
//				}

//				private sealed class ExtParserILBuilder : IParserILBuilder
//				{
//					private const BindingFlags Flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

//					private static readonly FieldInfo LexemeTokenFieldInfo = typeof(Lexeme<TExternalToken>).GetField(nameof(Lexeme<TExternalToken>.TokenField), Flags);
//					private static readonly FieldInfo LexemeStartFieldInfo = typeof(Lexeme<TExternalToken>).GetField(nameof(Lexeme<TExternalToken>.StartField), Flags);
//					private static readonly FieldInfo LexemeEndFieldInfo = typeof(Lexeme<TExternalToken>).GetField(nameof(Lexeme<TExternalToken>.EndField), Flags);
//					private static readonly MethodInfo TextSourceSpanGetTextMethodInfo = typeof(TextSpan).GetMethod(nameof(TextSpan.GetText), Flags);
//					private static readonly MethodInfo DebugMethodInfo = typeof(ExtParserILBuilder).GetMethod(nameof(Debug), BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);

//					private IParserILBuilder _actualBuilder;

//					private LocalBuilder _lexemeLocal;

//					public static void Debug(Lexeme<TExternalToken> lexeme)
//					{
//					}

//					public void EmitGetLexemeText(LocalBuilder lexemeLocal, ILBuilderContext ilBuilderContext)
//					{
//						EmitLdTextSourceSpan(ilBuilderContext);

//						ilBuilderContext.IL.Emit(OpCodes.Ldloc, lexemeLocal);
//						ilBuilderContext.IL.Emit(OpCodes.Ldfld, LexemeStartFieldInfo);

//						ilBuilderContext.IL.Emit(OpCodes.Ldloc, lexemeLocal);
//						ilBuilderContext.IL.Emit(OpCodes.Ldfld, LexemeEndFieldInfo);

//						ilBuilderContext.IL.Emit(OpCodes.Ldloc, lexemeLocal);
//						ilBuilderContext.IL.Emit(OpCodes.Ldfld, LexemeStartFieldInfo);

//						ilBuilderContext.IL.Emit(OpCodes.Sub);

//						ilBuilderContext.IL.Emit(OpCodes.Callvirt, TextSourceSpanGetTextMethodInfo);
//					}

//					public void Enter(IParserILBuilder builder, ILBuilderContext ilBuilderContext)
//					{
//						_lexemeLocal = ilBuilderContext.IL.DeclareLocal(typeof(Lexeme<TExternalToken>));
//						_actualBuilder = builder;

//						ilBuilderContext.IL.Emit(OpCodes.Stloc, _lexemeLocal);

//						//ilBuilderContext.IL.Emit(OpCodes.Ldloc, _lexemeLocal);
//						//ilBuilderContext.IL.Emit(OpCodes.Call, DebugMethodInfo);
//					}

//					public void Leave()
//					{
//						_lexemeLocal = null;
//						_actualBuilder = null;
//					}

//					public void EmitGetInstruction(ILBuilderContext ilBuilderContext)
//					{
//						ilBuilderContext.IL.Emit(OpCodes.Ldloc, _lexemeLocal);
//					}

//					public void EmitGetInstructionText(ILBuilderContext ilBuilderContext)
//					{
//						EmitGetLexemeText(_lexemeLocal, ilBuilderContext);
//					}

//					public void EmitGetInstructionToken(ILBuilderContext ilBuilderContext)
//					{
//						ilBuilderContext.IL.Emit(OpCodes.Ldfld, LexemeTokenFieldInfo);
//					}

//					public void EmitLdTextSourceSpan(ILBuilderContext ilBuilderContext)
//					{
//						_actualBuilder.EmitLdTextSourceSpan(ilBuilderContext);
//					}
//				}
//			}
//		}
//	}
//}