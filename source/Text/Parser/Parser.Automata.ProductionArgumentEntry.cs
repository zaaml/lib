// <copyright file="Parser.Automata.ProductionArgumentEntry.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Reflection.Emit;

namespace Zaaml.Text
{
	internal abstract partial class Parser<TGrammar, TToken>
	{
		private sealed partial class ParserAutomata
		{
			private abstract class ProductionArgument
			{
				protected ProductionArgument(string name, Entry parserEntry, int argumentIndex, Type argumentType, ParserProduction parserProduction)
				{
					Name = name;
					ParserEntry = parserEntry;
					ArgumentType = argumentType;
					ArgumentIndex = argumentIndex;
					ParserProduction = parserProduction;
				}

				public int ArgumentIndex { get; }

				public Type ArgumentType { get; }


				public string Name { get; }

				public Entry ParserEntry { get; }

				public ParserProduction ParserProduction { get; }

				public ProductionArgumentBinder Binder { get; private set; }

				public abstract ProductionEntityArgument CreateArgument(ProductionEntity entity);

				public abstract void EmitConsumeValue(LocalBuilder argumentLocal, LocalBuilder valueLocal, IParserILBuilder builder, ILBuilderContext ilBuilderContext);

				public abstract void EmitPushResetArgument(LocalBuilder argumentLocal, Type targetType, ILGenerator il, OpCode contextLdArg);

				public abstract void EmitCopyArgument(LocalBuilder argumentLocal, Type targetType, ILGenerator il, OpCode contextLdArg);

				public override string ToString()
				{
					return Name;
				}

				public void Bind(ProductionArgumentBinder argumentBinder)
				{
					if (Binder != null)
						throw new InvalidOperationException("Argument is already bound to another binder.");

					Binder = argumentBinder;
				}
			}

			private abstract class ParserProductionArgument : ProductionArgument
			{
				protected ParserProductionArgument(string name, Entry parserEntry, int argumentIndex, Type argumentType, ParserProduction parserProduction) : base(name, parserEntry, argumentIndex, argumentType, parserProduction)
				{
				}
			}

			private sealed class ParserValueArgument<TValue> : ParserProductionArgument
			{
				public ParserValueArgument(string name, Entry parserEntry, int argumentIndex, Type argumentType, ParserProduction parserProduction) : base(name, parserEntry, argumentIndex, argumentType, parserProduction)
				{
				}


				public override ProductionEntityArgument CreateArgument(ProductionEntity entity)
				{
					return new Argument<TValue>(entity, this);
				}

				public override void EmitConsumeValue(LocalBuilder argumentLocal, LocalBuilder valueLocal, IParserILBuilder builder, ILBuilderContext ilBuilderContext)
				{
					ilBuilderContext.IL.Emit(OpCodes.Ldloc, argumentLocal);
					ilBuilderContext.IL.Emit(OpCodes.Ldc_I4_1);
					ilBuilderContext.IL.Emit(OpCodes.Stfld, Argument<TValue>.CountFieldInfo);

					ilBuilderContext.IL.Emit(OpCodes.Ldloc, argumentLocal);
					ilBuilderContext.IL.Emit(OpCodes.Ldloc, valueLocal);
					ilBuilderContext.IL.Emit(OpCodes.Stfld, Argument<TValue>.ValueFieldInfo);
				}

				public override void EmitPushResetArgument(LocalBuilder argumentLocal, Type targetType, ILGenerator il, OpCode contextLdArg)
				{
					il.Emit(OpCodes.Ldloc, argumentLocal);
					il.Emit(OpCodes.Ldfld, Argument<TValue>.ValueFieldInfo);

					il.Emit(OpCodes.Ldloc, argumentLocal);
					il.Emit(OpCodes.Ldc_I4_0);
					il.Emit(OpCodes.Stfld, Argument<Lexeme<TToken>>.CountFieldInfo);

					il.Emit(OpCodes.Ldloc, argumentLocal);
					il.Emit(OpCodes.Ldnull);
					il.Emit(OpCodes.Stfld, Argument<TValue>.ValueFieldInfo);
				}

				public override void EmitCopyArgument(LocalBuilder argumentLocal, Type targetType, ILGenerator il, OpCode contextLdArg)
				{
					il.Emit(OpCodes.Call, Argument<TValue>.CopyToArrayMethodInfo);
				}
			}

			private sealed class ParserValueArrayArgument<TValue> : ParserProductionArgument
			{
				public ParserValueArrayArgument(string name, Entry parserEntry, int argumentIndex, Type argumentType, ParserProduction parserProduction) : base(name, parserEntry, argumentIndex, argumentType, parserProduction)
				{
				}

				public override ProductionEntityArgument CreateArgument(ProductionEntity entity)
				{
					return new ArrayArgument<TValue>(entity, this);
				}

				public override void EmitConsumeValue(LocalBuilder argumentLocal, LocalBuilder valueLocal, IParserILBuilder builder, ILBuilderContext ilBuilderContext)
				{
					ilBuilderContext.IL.Emit(OpCodes.Ldloc, argumentLocal);
					ilBuilderContext.IL.Emit(OpCodes.Ldloc, valueLocal);
					ilBuilderContext.IL.Emit(OpCodes.Call, ArrayArgument<TValue>.AddMethodInfo);
				}

				public override void EmitPushResetArgument(LocalBuilder argumentLocal, Type targetType, ILGenerator il, OpCode contextLdArg)
				{
					il.Emit(OpCodes.Ldloc, argumentLocal);
					il.Emit(OpCodes.Call, ArrayArgument<TValue>.ToArrayMethodInfo);
				}

				public override void EmitCopyArgument(LocalBuilder argumentLocal, Type targetType, ILGenerator il, OpCode contextLdArg)
				{
					il.Emit(OpCodes.Call, ArrayArgument<TValue>.CopyToArrayMethodInfo);
				}
			}

			private abstract class LexerProductionArgument : ProductionArgument
			{
				protected LexerProductionArgument(string name, Entry parserEntry, int argumentIndex, Type argumentType, ParserProduction parserProduction) : base(name, parserEntry, argumentIndex, argumentType, parserProduction)
				{
				}
			}

			private sealed class LexerValueArgument : LexerProductionArgument
			{
				public LexerValueArgument(string name, Entry parserEntry, int argumentIndex, Type argumentType, ParserProduction parserProduction) : base(name, parserEntry, argumentIndex, argumentType, parserProduction)
				{
				}

				public override ProductionEntityArgument CreateArgument(ProductionEntity entity)
				{
					return new Argument<Lexeme<TToken>>(entity, this);
				}

				public override void EmitConsumeValue(LocalBuilder argumentLocal, LocalBuilder valueLocal, IParserILBuilder builder, ILBuilderContext ilBuilderContext)
				{
					ilBuilderContext.IL.Emit(OpCodes.Ldloc, argumentLocal);
					ilBuilderContext.IL.Emit(OpCodes.Ldc_I4_1);
					ilBuilderContext.IL.Emit(OpCodes.Stfld, Argument<Lexeme<TToken>>.CountFieldInfo);

					ilBuilderContext.IL.Emit(OpCodes.Ldloc, argumentLocal);
					ilBuilderContext.IL.Emit(OpCodes.Ldloc, valueLocal);
					ilBuilderContext.IL.Emit(OpCodes.Stfld, Argument<Lexeme<TToken>>.ValueFieldInfo);
				}

				public override void EmitPushResetArgument(LocalBuilder argumentLocal, Type targetType, ILGenerator il, OpCode contextLdArg)
				{
					il.Emit(OpCodes.Ldloc, argumentLocal);

					if (ArgumentType != targetType)
					{
						if (targetType == typeof(string))
						{
							il.Emit(contextLdArg);
							il.Emit(OpCodes.Ldfld, ValueParserAutomataContext.LexemeStringConverterFieldInfo);

							il.Emit(OpCodes.Call, Argument<Lexeme<TToken>>.ConvertValueMethodInfo.MakeGenericMethod(typeof(string)));
						}
						else if (targetType == typeof(TToken))
						{
							il.Emit(contextLdArg);
							il.Emit(OpCodes.Ldfld, ValueParserAutomataContext.LexemeTokenConverterFieldInfo);
							il.Emit(OpCodes.Call, Argument<Lexeme<TToken>>.ConvertValueMethodInfo.MakeGenericMethod(typeof(TToken)));
						}
						else
							throw new InvalidOperationException("Invalid target type.");
					}
					else
						il.Emit(OpCodes.Ldfld, Argument<Lexeme<TToken>>.ValueFieldInfo);

					il.Emit(OpCodes.Ldloc, argumentLocal);
					il.Emit(OpCodes.Ldc_I4_0);
					il.Emit(OpCodes.Stfld, Argument<Lexeme<TToken>>.CountFieldInfo);

					EmitReset(argumentLocal, il);
				}

				private void EmitReset(LocalBuilder argumentLocal, ILGenerator il)
				{
					il.Emit(OpCodes.Ldloc, argumentLocal);
					il.Emit(OpCodes.Ldflda, Argument<Lexeme<TToken>>.ValueFieldInfo);
					il.Emit(OpCodes.Initobj, typeof(Lexeme<TToken>));
				}

				public override void EmitCopyArgument(LocalBuilder argumentLocal, Type targetType, ILGenerator il, OpCode contextLdArg)
				{
					if (ArgumentType != targetType)
					{
						if (targetType == typeof(string[]))
						{
							il.Emit(contextLdArg);
							il.Emit(OpCodes.Ldfld, ValueParserAutomataContext.LexemeStringConverterFieldInfo);
							il.Emit(OpCodes.Call, Argument<Lexeme<TToken>>.CopyToArrayConvertMethodInfo.MakeGenericMethod(typeof(string)));
						}
						else if (targetType == typeof(TToken[]))
						{
							il.Emit(contextLdArg);
							il.Emit(OpCodes.Ldfld, ValueParserAutomataContext.LexemeTokenConverterFieldInfo);
							il.Emit(OpCodes.Call, Argument<Lexeme<TToken>>.CopyToArrayConvertMethodInfo.MakeGenericMethod(typeof(TToken)));
						}
						else
							throw new InvalidOperationException("Invalid target type.");
					}
					else
						il.Emit(OpCodes.Call, Argument<Lexeme<TToken>>.CopyToArrayMethodInfo);

					EmitReset(argumentLocal, il);
				}
			}

			private sealed class LexerValueArrayArgument : LexerProductionArgument
			{
				public LexerValueArrayArgument(string name, Entry parserEntry, int argumentIndex, Type argumentType, ParserProduction parserProduction) : base(name, parserEntry, argumentIndex, argumentType, parserProduction)
				{
				}

				public override ProductionEntityArgument CreateArgument(ProductionEntity entity)
				{
					return new ArrayArgument<Lexeme<TToken>>(entity, this);
				}


				public override void EmitConsumeValue(LocalBuilder argumentLocal, LocalBuilder valueLocal, IParserILBuilder builder, ILBuilderContext ilBuilderContext)
				{
					ilBuilderContext.IL.Emit(OpCodes.Ldloc, argumentLocal);
					ilBuilderContext.IL.Emit(OpCodes.Ldloc, valueLocal);
					ilBuilderContext.IL.Emit(OpCodes.Call, ArrayArgument<Lexeme<TToken>>.AddMethodInfo);
				}

				public override void EmitPushResetArgument(LocalBuilder argumentLocal, Type targetType, ILGenerator il, OpCode contextLdArg)
				{
					il.Emit(OpCodes.Ldloc, argumentLocal);

					if (ArgumentType != targetType)
					{
						if (targetType == typeof(string[]))
						{
							il.Emit(contextLdArg);
							il.Emit(OpCodes.Ldfld, ValueParserAutomataContext.LexemeStringConverterFieldInfo);
							il.Emit(OpCodes.Call, ArrayArgument<Lexeme<TToken>>.ToArrayConvertMethodInfo);
						}
						else if (targetType == typeof(TToken[]))
						{
							il.Emit(contextLdArg);
							il.Emit(OpCodes.Ldfld, ValueParserAutomataContext.LexemeTokenConverterFieldInfo);
							il.Emit(OpCodes.Call, ArrayArgument<Lexeme<TToken>>.ToArrayConvertMethodInfo);
						}
						else
							throw new InvalidOperationException("Invalid target type.");
					}
					else
						il.Emit(OpCodes.Call, ArrayArgument<Lexeme<TToken>>.ToArrayMethodInfo);
				}

				public override void EmitCopyArgument(LocalBuilder argumentLocal, Type targetType, ILGenerator il, OpCode contextLdArg)
				{
					if (ArgumentType != targetType)
					{
						if (targetType == typeof(string[]))
						{
							il.Emit(contextLdArg);
							il.Emit(OpCodes.Ldfld, ValueParserAutomataContext.LexemeStringConverterFieldInfo);
							il.Emit(OpCodes.Call, ArrayArgument<Lexeme<TToken>>.CopyToArrayConvertMethodInfo.MakeGenericMethod(typeof(string)));
						}
						else if (targetType == typeof(TToken[]))
						{
							il.Emit(contextLdArg);
							il.Emit(OpCodes.Ldfld, ValueParserAutomataContext.LexemeTokenConverterFieldInfo);
							il.Emit(OpCodes.Call, ArrayArgument<Lexeme<TToken>>.CopyToArrayConvertMethodInfo.MakeGenericMethod(typeof(TToken)));
						}
						else
							throw new InvalidOperationException("Invalid target type.");
					}
					else
						il.Emit(OpCodes.Call, ArrayArgument<Lexeme<TToken>>.CopyToArrayMethodInfo);
				}
			}
		}
	}
}