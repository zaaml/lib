// <copyright file="Parser.Automata.ProductionArgument.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Reflection.Emit;

namespace Zaaml.Text
{
	internal partial class Parser<TGrammar, TToken>
	{
		private sealed partial class ParserAutomata
		{
			private abstract class ProductionArgument
			{
				protected ProductionArgument()
				{
				}

				protected ProductionArgument(string name, Entry parserEntry, int argumentIndex, Type argumentType, ParserProduction parserProduction)
				{
					Name = name;
					ParserEntry = parserEntry;
					ArgumentType = argumentType;
					ArgumentIndex = argumentIndex;
					ParserProduction = parserProduction;

					//if (((IParserEntry)parserEntry).ProductionArgument != null)
					//{

					//}

					((IParserEntry)parserEntry).ProductionArgument = this;
				}

				protected ProductionArgument(string name, Entry parserEntry, int argumentIndex, Type argumentType, ParserProduction parserProduction, ProductionArgument originalArgument)
				{
					Name = name;
					ParserEntry = parserEntry;
					ArgumentType = argumentType;
					ArgumentIndex = argumentIndex;
					ParserProduction = parserProduction;
					OriginalArgument = originalArgument;

					//if (((IParserEntry)parserEntry).ProductionArgument != null)
					//{

					//}

					((IParserEntry)parserEntry).ProductionArgument = this;

					Bind(OriginalArgument.Binder);
				}

				public int ArgumentIndex { get; }

				public Type ArgumentType { get; }

				public ProductionArgumentBinder Binder { get; private set; }

				public string Name { get; }

				public ProductionArgument OriginalArgument { get; }

				public Entry ParserEntry { get; }

				public ParserProduction ParserProduction { get; }

				public void Bind(ProductionArgumentBinder argumentBinder)
				{
					if (Binder != null)
						throw new InvalidOperationException("Argument is already bound to another binder.");

					Binder = argumentBinder;
				}

				public abstract ProductionEntityArgument CreateArgument(ProductionEntity entity);

				public abstract void EmitConsumeValue(LocalBuilder argumentLocal, LocalBuilder valueLocal, Process.ILContext context);

				public abstract void EmitCopyArgument(LocalBuilder argumentLocal, Type targetType, ILGenerator il, OpCode processLdArg);

				public abstract void EmitPushResetArgument(LocalBuilder argumentLocal, Type targetType, ILGenerator il, OpCode processLdArg);

				public abstract ProductionArgument MapArgument(int index, Entry parserEntry, ParserProduction parserProduction);

				public override string ToString()
				{
					return Name;
				}
			}

			private abstract class ParserProductionArgument : ProductionArgument
			{
				protected ParserProductionArgument(string name, Entry parserEntry, int argumentIndex, Type argumentType, ParserProduction parserProduction) : base(name, parserEntry, argumentIndex, argumentType, parserProduction)
				{
				}

				protected ParserProductionArgument(string name, Entry parserEntry, int argumentIndex, Type argumentType, ParserProduction parserProduction, ParserProductionArgument originalArgument) : base(name, parserEntry, argumentIndex,
					argumentType, parserProduction, originalArgument)
				{
				}
			}

			private sealed class NullArgument : ProductionArgument
			{
				public static readonly NullArgument Instance = new();

				private NullArgument()
				{
				}

				public NullArgument(string name, Entry parserEntry, int argumentIndex, ParserProduction parserProduction) : base(name, parserEntry, argumentIndex, null, parserProduction)
				{
					Bind(NullArgumentBinder.Instance);
				}

				public override ProductionEntityArgument CreateArgument(ProductionEntity entity)
				{
					return new NullEntityArgument(entity, this);
				}

				public override void EmitConsumeValue(LocalBuilder argumentLocal, LocalBuilder valueLocal, Process.ILContext context)
				{
				}

				public override void EmitCopyArgument(LocalBuilder argumentLocal, Type targetType, ILGenerator il, OpCode processLdArg)
				{
				}

				public override void EmitPushResetArgument(LocalBuilder argumentLocal, Type targetType, ILGenerator il, OpCode processLdArg)
				{
				}

				public override ProductionArgument MapArgument(int index, Entry parserEntry, ParserProduction parserProduction)
				{
					return new NullArgument(Name, parserEntry, index, parserProduction);
				}

				private sealed class NullEntityArgument : ProductionEntityArgument
				{
					public NullEntityArgument(ProductionEntity entity, ProductionArgument argument) : base(entity, argument)
					{
					}

					public override object Build()
					{
						return null;
					}

					public override int GetCount()
					{
						return 0;
					}

					public override void Reset()
					{
					}

					public override void TransferValue(ProductionEntityArgument argument)
					{
					}
				}

				private sealed class NullArgumentBinder : ProductionArgumentBinder
				{
					public static readonly NullArgumentBinder Instance = new();

					private NullArgumentBinder() : base(typeof(void))
					{
					}

					public override bool ConsumeValue => false;

					public override void EmitPushResetArgument(LocalBuilder productionEntityLocal, LocalBuilder entityArgumentLocal, ILGenerator ilBuilder, OpCode processLdArg)
					{
					}
				}
			}

			private sealed class ParserValueArgument<TValue> : ParserProductionArgument
			{
				public ParserValueArgument(string name, Entry parserEntry, int argumentIndex, Type argumentType, ParserProduction parserProduction) : base(name, parserEntry, argumentIndex, argumentType, parserProduction)
				{
				}

				private ParserValueArgument(string name, Entry parserEntry, int argumentIndex, Type argumentType, ParserProduction parserProduction, ParserValueArgument<TValue> originalArgument) : base(name, parserEntry, argumentIndex,
					argumentType, parserProduction, originalArgument)
				{
				}

				public override ProductionEntityArgument CreateArgument(ProductionEntity entity)
				{
					return new Argument<TValue>(entity, this);
				}

				public override void EmitConsumeValue(LocalBuilder argumentLocal, LocalBuilder valueLocal, Process.ILContext context)
				{
					var il = context.IL;

					il.Emit(OpCodes.Ldloc, argumentLocal);
					il.Emit(OpCodes.Ldc_I4_1);
					il.Emit(OpCodes.Stfld, Argument<TValue>.CountFieldInfo);

					il.Emit(OpCodes.Ldloc, argumentLocal);
					il.Emit(OpCodes.Ldloc, valueLocal);
					il.Emit(OpCodes.Stfld, Argument<TValue>.ValueFieldInfo);
				}

				public override void EmitCopyArgument(LocalBuilder argumentLocal, Type targetType, ILGenerator il, OpCode processLdArg)
				{
					il.Emit(OpCodes.Call, Argument<TValue>.CopyToArrayMethodInfo);
				}

				public override void EmitPushResetArgument(LocalBuilder argumentLocal, Type targetType, ILGenerator il, OpCode processLdArg)
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

				public override ProductionArgument MapArgument(int index, Entry parserEntry, ParserProduction parserProduction)
				{
					return new ParserValueArgument<TValue>(Name, parserEntry, index, ArgumentType, parserProduction, this);
				}
			}

			private sealed class ParserValueArrayArgument<TValue> : ParserProductionArgument
			{
				public ParserValueArrayArgument(string name, Entry parserEntry, int argumentIndex, Type argumentType, ParserProduction parserProduction) : base(name, parserEntry, argumentIndex, argumentType, parserProduction)
				{
				}

				private ParserValueArrayArgument(string name, Entry parserEntry, int argumentIndex, Type argumentType, ParserProduction parserProduction, ParserValueArrayArgument<TValue> originalArgument) : base(name, parserEntry, argumentIndex,
					argumentType, parserProduction, originalArgument)
				{
				}

				public override ProductionEntityArgument CreateArgument(ProductionEntity entity)
				{
					return new ArrayArgument<TValue>(entity, this);
				}

				public override void EmitConsumeValue(LocalBuilder argumentLocal, LocalBuilder valueLocal, Process.ILContext context)
				{
					var il = context.IL;

					il.Emit(OpCodes.Ldloc, argumentLocal);
					il.Emit(OpCodes.Ldloc, valueLocal);
					il.Emit(OpCodes.Call, ArrayArgument<TValue>.AddMethodInfo);
				}

				public override void EmitCopyArgument(LocalBuilder argumentLocal, Type targetType, ILGenerator il, OpCode processLdArg)
				{
					il.Emit(OpCodes.Call, ArrayArgument<TValue>.CopyToArrayMethodInfo);
				}

				public override void EmitPushResetArgument(LocalBuilder argumentLocal, Type targetType, ILGenerator il, OpCode processLdArg)
				{
					il.Emit(OpCodes.Ldloc, argumentLocal);
					il.Emit(OpCodes.Call, ArrayArgument<TValue>.ToArrayMethodInfo);
				}

				public override ProductionArgument MapArgument(int index, Entry parserEntry, ParserProduction parserProduction)
				{
					return new ParserValueArrayArgument<TValue>(Name, parserEntry, index, ArgumentType, parserProduction, this);
				}
			}

			private abstract class LexerProductionArgument : ProductionArgument
			{
				protected LexerProductionArgument(string name, Entry parserEntry, int argumentIndex, Type argumentType, ParserProduction parserProduction) : base(name, parserEntry, argumentIndex, argumentType, parserProduction)
				{
				}

				protected LexerProductionArgument(string name, Entry parserEntry, int argumentIndex, Type argumentType, ParserProduction parserProduction, LexerProductionArgument originalArgument) : base(name, parserEntry, argumentIndex,
					argumentType, parserProduction, originalArgument)
				{
				}
			}

			private sealed class LexerValueArgument : LexerProductionArgument
			{
				public LexerValueArgument(string name, Entry parserEntry, int argumentIndex, Type argumentType, ParserProduction parserProduction) : base(name, parserEntry, argumentIndex, argumentType, parserProduction)
				{
				}

				private LexerValueArgument(string name, Entry parserEntry, int argumentIndex, Type argumentType, ParserProduction parserProduction, LexerValueArgument originalArgument) : base(name, parserEntry, argumentIndex, argumentType,
					parserProduction, originalArgument)
				{
				}

				public override ProductionEntityArgument CreateArgument(ProductionEntity entity)
				{
					return new Argument<Lexeme<TToken>>(entity, this);
				}

				public override void EmitConsumeValue(LocalBuilder argumentLocal, LocalBuilder valueLocal, Process.ILContext context)
				{
					var il = context.IL;

					il.Emit(OpCodes.Ldloc, argumentLocal);
					il.Emit(OpCodes.Ldc_I4_1);
					il.Emit(OpCodes.Stfld, Argument<Lexeme<TToken>>.CountFieldInfo);

					il.Emit(OpCodes.Ldloc, argumentLocal);
					il.Emit(OpCodes.Ldloc, valueLocal);
					il.Emit(OpCodes.Stfld, Argument<Lexeme<TToken>>.ValueFieldInfo);
				}

				public override void EmitCopyArgument(LocalBuilder argumentLocal, Type targetType, ILGenerator il, OpCode processLdArg)
				{
					if (ArgumentType != targetType)
					{
						if (targetType == typeof(string[]))
						{
							il.Emit(processLdArg);
							il.Emit(OpCodes.Ldfld, ParserProcess.LexemeStringConverterFieldInfo);
							il.Emit(OpCodes.Call, Argument<Lexeme<TToken>>.CopyToArrayConvertMethodInfo.MakeGenericMethod(typeof(string)));
						}
						else if (targetType == typeof(TToken[]))
						{
							il.Emit(processLdArg);
							il.Emit(OpCodes.Ldfld, ParserProcess.LexemeTokenConverterFieldInfo);
							il.Emit(OpCodes.Call, Argument<Lexeme<TToken>>.CopyToArrayConvertMethodInfo.MakeGenericMethod(typeof(TToken)));
						}
						else if (targetType == typeof(SyntaxToken<TToken>[]))
						{
							il.Emit(processLdArg);
							il.Emit(OpCodes.Ldfld, ParserProcess.SyntaxTokenConverterFieldInfo);
							il.Emit(OpCodes.Call, Argument<Lexeme<TToken>>.CopyToArrayConvertMethodInfo.MakeGenericMethod(typeof(SyntaxToken<TToken>)));
						}
						else
							throw new InvalidOperationException("Invalid target type.");
					}
					else
						il.Emit(OpCodes.Call, Argument<Lexeme<TToken>>.CopyToArrayMethodInfo);
				}

				public override void EmitPushResetArgument(LocalBuilder argumentLocal, Type targetType, ILGenerator il, OpCode processLdArg)
				{
					if (ArgumentType != targetType)
					{
						if (targetType == typeof(string))
						{
							il.Emit(processLdArg);
							il.Emit(OpCodes.Ldloc, argumentLocal);
							il.Emit(OpCodes.Ldflda, Argument<Lexeme<TToken>>.ValueFieldInfo);

							il.Emit(OpCodes.Call, ParserProcess.GetLexemeTextMethodInfo);
						}
						else if (targetType == typeof(TToken))
						{
							il.Emit(OpCodes.Ldloc, argumentLocal);

							il.Emit(processLdArg);
							il.Emit(OpCodes.Ldfld, ParserProcess.LexemeTokenConverterFieldInfo);
							il.Emit(OpCodes.Call, Argument<Lexeme<TToken>>.ConvertValueMethodInfo.MakeGenericMethod(typeof(TToken)));
						}
						else if (targetType == typeof(SyntaxToken<TToken>))
						{
							il.Emit(OpCodes.Ldloc, argumentLocal);

							il.Emit(processLdArg);
							il.Emit(OpCodes.Ldfld, ParserProcess.SyntaxTokenConverterFieldInfo);
							il.Emit(OpCodes.Call, Argument<Lexeme<TToken>>.ConvertValueMethodInfo.MakeGenericMethod(typeof(SyntaxToken<TToken>)));
						}
						else if (targetType == typeof(SyntaxToken<TToken>?))
						{
							il.Emit(OpCodes.Ldloc, argumentLocal);

							il.Emit(processLdArg);
							il.Emit(OpCodes.Ldfld, ParserProcess.NullableSyntaxTokenConverterFieldInfo);
							il.Emit(OpCodes.Call, Argument<Lexeme<TToken>>.ConvertValueMethodInfo.MakeGenericMethod(typeof(SyntaxToken<TToken>?)));
						}
						else
							throw new InvalidOperationException("Invalid target type.");
					}
					else
					{
						il.Emit(OpCodes.Ldloc, argumentLocal);
						il.Emit(OpCodes.Ldfld, Argument<Lexeme<TToken>>.ValueFieldInfo);
					}

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

				public override ProductionArgument MapArgument(int index, Entry parserEntry, ParserProduction parserProduction)
				{
					return new LexerValueArgument(Name, parserEntry, index, ArgumentType, parserProduction, this);
				}
			}

			private sealed class LexerValueArrayArgument : LexerProductionArgument
			{
				public LexerValueArrayArgument(string name, Entry parserEntry, int argumentIndex, Type argumentType, ParserProduction parserProduction) : base(name, parserEntry, argumentIndex, argumentType, parserProduction)
				{
				}

				private LexerValueArrayArgument(string name, Entry parserEntry, int argumentIndex, Type argumentType, ParserProduction parserProduction, LexerValueArrayArgument originalArgument) : base(name, parserEntry, argumentIndex,
					argumentType, parserProduction, originalArgument)
				{
				}

				public override ProductionEntityArgument CreateArgument(ProductionEntity entity)
				{
					return new ArrayArgument<Lexeme<TToken>>(entity, this);
				}

				public override void EmitConsumeValue(LocalBuilder argumentLocal, LocalBuilder valueLocal, Process.ILContext context)
				{
					var il = context.IL;

					il.Emit(OpCodes.Ldloc, argumentLocal);
					il.Emit(OpCodes.Ldloc, valueLocal);
					il.Emit(OpCodes.Call, ArrayArgument<Lexeme<TToken>>.AddMethodInfo);
				}

				public override void EmitCopyArgument(LocalBuilder argumentLocal, Type targetType, ILGenerator il, OpCode processLdArg)
				{
					if (ArgumentType != targetType)
					{
						if (targetType == typeof(string[]))
						{
							il.Emit(processLdArg);
							il.Emit(OpCodes.Ldfld, ParserProcess.LexemeStringConverterFieldInfo);
							il.Emit(OpCodes.Call, ArrayArgument<Lexeme<TToken>>.CopyToArrayConvertMethodInfo.MakeGenericMethod(typeof(string)));
						}
						else if (targetType == typeof(TToken[]))
						{
							il.Emit(processLdArg);
							il.Emit(OpCodes.Ldfld, ParserProcess.LexemeTokenConverterFieldInfo);
							il.Emit(OpCodes.Call, ArrayArgument<Lexeme<TToken>>.CopyToArrayConvertMethodInfo.MakeGenericMethod(typeof(TToken)));
						}						
						else if (targetType == typeof(SyntaxToken<TToken>[]))
						{
							il.Emit(processLdArg);
							il.Emit(OpCodes.Ldfld, ParserProcess.SyntaxTokenConverterFieldInfo);
							il.Emit(OpCodes.Call, ArrayArgument<Lexeme<TToken>>.CopyToArrayConvertMethodInfo.MakeGenericMethod(typeof(SyntaxToken<TToken>)));
						}
						else
							throw new InvalidOperationException("Invalid target type.");
					}
					else
						il.Emit(OpCodes.Call, ArrayArgument<Lexeme<TToken>>.CopyToArrayMethodInfo);
				}

				public override void EmitPushResetArgument(LocalBuilder argumentLocal, Type targetType, ILGenerator il, OpCode processLdArg)
				{
					il.Emit(OpCodes.Ldloc, argumentLocal);

					if (ArgumentType != targetType)
					{
						if (targetType == typeof(string[]))
						{
							il.Emit(processLdArg);
							il.Emit(OpCodes.Ldfld, ParserProcess.LexemeStringConverterFieldInfo);
							il.Emit(OpCodes.Call, ArrayArgument<Lexeme<TToken>>.ToArrayConvertMethodInfo.MakeGenericMethod(typeof(string)));
						}
						else if (targetType == typeof(TToken[]))
						{
							il.Emit(processLdArg);
							il.Emit(OpCodes.Ldfld, ParserProcess.LexemeTokenConverterFieldInfo);
							il.Emit(OpCodes.Call, ArrayArgument<Lexeme<TToken>>.ToArrayConvertMethodInfo.MakeGenericMethod(typeof(TToken)));
						}						
						else if (targetType == typeof(SyntaxToken<TToken>[]))
						{
							il.Emit(processLdArg);
							il.Emit(OpCodes.Ldfld, ParserProcess.SyntaxTokenConverterFieldInfo);
							il.Emit(OpCodes.Call, ArrayArgument<Lexeme<TToken>>.ToArrayConvertMethodInfo.MakeGenericMethod(typeof(SyntaxToken<TToken>)));
						}
						else
							throw new InvalidOperationException("Invalid target type.");
					}
					else
						il.Emit(OpCodes.Call, ArrayArgument<Lexeme<TToken>>.ToArrayMethodInfo);
				}

				public override ProductionArgument MapArgument(int index, Entry parserEntry, ParserProduction parserProduction)
				{
					return new LexerValueArrayArgument(Name, parserEntry, index, ArgumentType, parserProduction, this);
				}
			}
		}
	}
}