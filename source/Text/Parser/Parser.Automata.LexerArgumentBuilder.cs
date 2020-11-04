// <copyright file="Parser.Automata.LexerArgumentBuilder.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Reflection;
using System.Reflection.Emit;

#pragma warning disable 414

namespace Zaaml.Text
{
	internal abstract partial class Parser<TGrammar, TToken>
	{
		#region Nested Types

		private sealed partial class ParserAutomata
		{
			#region Nested Types

			private abstract class LexerArgumentBuilder : ArgumentBuilder
			{
				public MatchEntry MatchEntry { get; }

				public string StaticLexemeText { get; }

				protected LexerArgumentBuilder(MatchEntry matchEntry)
				{
					MatchEntry = matchEntry;

					if (matchEntry is SingleMatchEntry singleMatchEntry)
						StaticLexemeText = StaticLexemes[singleMatchEntry.IntOperand];
				}

				#region Properties

				public abstract Type TokenType { get; }

				#endregion

				#region Methods

				public abstract void EmitConsumeValue(IParserILBuilder builder, ILBuilderContext ilBuilderContext);

				#endregion
			}

			private abstract class LexerArgumentBuilder<TActualToken> : LexerArgumentBuilder where TActualToken : unmanaged, Enum
			{
				#region Fields

				private readonly Type _argumentType;
				private readonly BuilderImpl _builder;

				#endregion

				#region Ctors

				protected LexerArgumentBuilder(Type argumentType, MatchEntry matchEntry):base(matchEntry)
				{
					_argumentType = argumentType;

					if (argumentType == typeof(string))
						_builder = new LexemeText(this);
					else if (argumentType == typeof(TActualToken))
						_builder = new LexemeToken(this);
					else if (argumentType == typeof(string[]))
						_builder = new LexemeTextList(this);
					else if (argumentType == typeof(TActualToken[]))
						_builder = new LexemeTokenList(this);
					else if (argumentType == typeof(Lexeme<TActualToken>))
						_builder = new Lexeme(this);
					else if (argumentType == typeof(Lexeme<TActualToken>[]))
						_builder = new LexemeList(this);
					else
						throw new ArgumentOutOfRangeException();
				}

				#endregion


				#region Properties

				public override Type TokenType => typeof(TActualToken);

				#endregion

				#region Methods

				public override Argument CreateArgument(ProductionInstanceBuilder instanceBuilder, int index)
				{
					return _builder.CreateArgument(instanceBuilder, index);
				}
				
				public override void EmitConsumeValue(IParserILBuilder builder, ILBuilderContext ilBuilderContext)
				{
					_builder.EmitConsumeValue(builder, ilBuilderContext);
				}

				public override void EmitPushResetArgument(LocalBuilder argumentLocal, ILGenerator il)
				{
					_builder.EmitPushResetArgument(argumentLocal, il);
				}

				#endregion

				#region Nested Types

				private abstract class BuilderImpl
				{
					public LexerArgumentBuilder ArgumentBuilder { get; }

					protected BuilderImpl(LexerArgumentBuilder argumentBuilder)
					{
						ArgumentBuilder = argumentBuilder;
					}

					#region Methods

					public abstract void EmitConsumeValue(IParserILBuilder builder, ILBuilderContext ilBuilderContext);

					public abstract void EmitPushResetArgument(LocalBuilder argumentLocal, ILGenerator il);

					#endregion

					public abstract Argument CreateArgument(ProductionInstanceBuilder instanceBuilder, int index);
				}


				// TextList
				private sealed class LexemeTextList : BuilderImpl
				{
					#region Static Fields and Constants

					private static readonly MethodInfo ListTResultAddMethodInfo = typeof(ValueListArgument<string>).GetMethod(nameof(ValueListArgument<string>.Add), BindingFlags.Instance | BindingFlags.Public);
					private static readonly MethodInfo ListTResultToArrayMethodInfo = typeof(ValueListArgument<string>).GetMethod(nameof(ValueListArgument<string>.ToArray), BindingFlags.Instance | BindingFlags.Public);

					#endregion

					#region Methods


					public override void EmitConsumeValue(IParserILBuilder builder, ILBuilderContext ilBuilderContext)
					{
						builder.EmitGetInstructionText(ilBuilderContext);
						ilBuilderContext.IL.Emit(OpCodes.Call, ListTResultAddMethodInfo);
					}

					public override void EmitPushResetArgument(LocalBuilder argumentLocal, ILGenerator il)
					{
						il.Emit(OpCodes.Ldloc, argumentLocal);
						il.Emit(OpCodes.Call, ListTResultToArrayMethodInfo);
					}

					public override Argument CreateArgument(ProductionInstanceBuilder instanceBuilder, int index)
					{
						return new ValueListArgument<string>(instanceBuilder, index);
					}

					#endregion

					public LexemeTextList(LexerArgumentBuilder argumentBuilder) : base(argumentBuilder)
					{
					}
				}

				// LexemeTokenList
				private sealed class LexemeTokenList : BuilderImpl
				{
					#region Static Fields and Constants
			
					private static readonly MethodInfo ListTResultAddMethodInfo = typeof(ValueListArgument<TActualToken>).GetMethod(nameof(ValueListArgument<TActualToken>.Add), BindingFlags.Instance | BindingFlags.Public);
					private static readonly MethodInfo ListTResultToArrayMethodInfo = typeof(ValueListArgument<TActualToken>).GetMethod(nameof(ValueListArgument<TActualToken>.ToArray), BindingFlags.Instance | BindingFlags.Public);

					#endregion

					#region Methods

					public override void EmitConsumeValue(IParserILBuilder builder, ILBuilderContext ilBuilderContext)
					{
						builder.EmitGetInstructionToken(ilBuilderContext);
						ilBuilderContext.IL.Emit(OpCodes.Call, ListTResultAddMethodInfo);
					}

					public override void EmitPushResetArgument(LocalBuilder argumentLocal, ILGenerator il)
					{
						il.Emit(OpCodes.Ldloc, argumentLocal);
						il.Emit(OpCodes.Call, ListTResultToArrayMethodInfo);
					}

					public override Argument CreateArgument(ProductionInstanceBuilder instanceBuilder, int index)
					{
						return new ValueListArgument<TActualToken>(instanceBuilder, index);
					}

					#endregion

					public LexemeTokenList(LexerArgumentBuilder argumentBuilder) : base(argumentBuilder)
					{
					}
				}

				// Text
				private sealed class LexemeText : BuilderImpl
				{
					#region Static Fields and Constants

					private static readonly FieldInfo LexemeTextArgumentTextFieldInfo = typeof(LexemeTextArgument).GetField(nameof(LexemeTextArgument.Text), BindingFlags.Public | BindingFlags.Instance);

					#endregion

					#region Methods

					public override void EmitConsumeValue(IParserILBuilder builder, ILBuilderContext ilBuilderContext)
					{
						if (ArgumentBuilder.StaticLexemeText != null)
						{
							ilBuilderContext.IL.Emit(OpCodes.Pop);
						}
						else
						{

							builder.EmitGetInstructionText(ilBuilderContext);
							ilBuilderContext.IL.Emit(OpCodes.Stfld, LexemeTextArgumentTextFieldInfo);
						}
					}

					public override void EmitPushResetArgument(LocalBuilder argumentLocal, ILGenerator il)
					{
						il.Emit(OpCodes.Ldloc, argumentLocal);
						il.Emit(OpCodes.Ldfld, LexemeTextArgumentTextFieldInfo);

						if (ArgumentBuilder.StaticLexemeText != null) 
							return;

						il.Emit(OpCodes.Ldloc, argumentLocal);
						il.Emit(OpCodes.Ldnull);
						il.Emit(OpCodes.Stfld, LexemeTextArgumentTextFieldInfo);
					}

					public override Argument CreateArgument(ProductionInstanceBuilder instanceBuilder, int index)
					{
						return new LexemeTextArgument(instanceBuilder, index, ArgumentBuilder.StaticLexemeText);
					}

					#endregion

					#region Nested Types

					private sealed class LexemeTextArgument : Argument
					{
						#region Fields

						public string Text;
						private readonly bool _static;

						#endregion

						public override object Build()
						{
							if (_static)
								return Text;

							var text = Text;

							Text = default;

							return text;
						}

						public override void Reset()
						{
							if (_static)
								return;

							Text = default;
						}

						public LexemeTextArgument(ProductionInstanceBuilder instanceBuilder, int index, string staticText) : base(instanceBuilder, index)
						{
							Text = staticText;

							_static = staticText != null;
						}
					}

					#endregion

					public LexemeText(LexerArgumentBuilder argumentBuilder) : base(argumentBuilder)
					{
					}
				}

				// Lexeme List
				private sealed class LexemeList : BuilderImpl
				{
					#region Static Fields and Constants

					private static readonly MethodInfo ListTResultAddMethodInfo = typeof(ValueListArgument<Lexeme<TActualToken>>).GetMethod(nameof(ValueListArgument<Lexeme<TActualToken>>.Add), BindingFlags.Instance | BindingFlags.Public);
					private static readonly MethodInfo ListTResultToArrayMethodInfo = typeof(ValueListArgument<Lexeme<TActualToken>>).GetMethod(nameof(ValueListArgument<Lexeme<TActualToken>>.ToArray), BindingFlags.Instance | BindingFlags.Public);

					#endregion

					#region Methods


					public override void EmitConsumeValue(IParserILBuilder builder, ILBuilderContext ilBuilderContext)
					{
						builder.EmitGetInstruction(ilBuilderContext);
						ilBuilderContext.IL.Emit(OpCodes.Call, ListTResultAddMethodInfo);
					}

					public override void EmitPushResetArgument(LocalBuilder argumentLocal, ILGenerator il)
					{
						il.Emit(OpCodes.Ldloc, argumentLocal);
						il.Emit(OpCodes.Call, ListTResultToArrayMethodInfo);
					}

					public override Argument CreateArgument(ProductionInstanceBuilder instanceBuilder, int index)
					{
						return new ValueListArgument<Lexeme<TActualToken>>(instanceBuilder, index);
					}

					#endregion

					public LexemeList(LexerArgumentBuilder argumentBuilder) : base(argumentBuilder)
					{
					}
				}

				// Lexeme
				private sealed class Lexeme : BuilderImpl
				{
					#region Static Fields and Constants

					private static readonly FieldInfo LexemeArgumentLexemeFieldInfo = typeof(LexemeArgument).GetField(nameof(LexemeArgument.Lexeme), BindingFlags.Public | BindingFlags.Instance);

					#endregion

					#region Methods

					public override void EmitConsumeValue(IParserILBuilder builder, ILBuilderContext ilBuilderContext)
					{
						builder.EmitGetInstruction(ilBuilderContext);
						ilBuilderContext.IL.Emit(OpCodes.Stfld, LexemeArgumentLexemeFieldInfo);
					}

					public override void EmitPushResetArgument(LocalBuilder argumentLocal, ILGenerator il)
					{
						il.Emit(OpCodes.Ldloc, argumentLocal);
						il.Emit(OpCodes.Ldfld, LexemeArgumentLexemeFieldInfo);
						il.Emit(OpCodes.Ldloc, argumentLocal);
						il.Emit(OpCodes.Ldflda, LexemeArgumentLexemeFieldInfo);
						il.Emit(OpCodes.Initobj, typeof(Lexeme<TActualToken>));
					}

					public override Argument CreateArgument(ProductionInstanceBuilder instanceBuilder, int index)
					{
						return new LexemeArgument(instanceBuilder, index);
					}

					#endregion

					#region Nested Types

					private sealed class LexemeArgument : Argument
					{
						#region Fields

						public Lexeme<TActualToken> Lexeme;

						#endregion

						public override object Build()
						{
							var lexeme = Lexeme;

							Lexeme = default;

							return lexeme;
						}

						public override void Reset()
						{
							Lexeme = default;
						}

						public LexemeArgument(ProductionInstanceBuilder instanceBuilder, int index) : base(instanceBuilder, index)
						{
						}
					}

					#endregion

					public Lexeme(LexerArgumentBuilder argumentBuilder) : base(argumentBuilder)
					{
					}
				}

				// LexemeToken
				private sealed class LexemeToken : BuilderImpl
				{
					#region Static Fields and Constants

					
					private static readonly FieldInfo LexemeTokenArgumentTokenFieldInfo = typeof(LexemeTokenArgument).GetField(nameof(LexemeTokenArgument.Token), BindingFlags.Public | BindingFlags.Instance);

					#endregion

					#region Methods


					public override void EmitConsumeValue(IParserILBuilder builder, ILBuilderContext ilBuilderContext)
					{
						builder.EmitGetInstructionToken(ilBuilderContext);
						ilBuilderContext.IL.Emit(OpCodes.Stfld, LexemeTokenArgumentTokenFieldInfo);
					}

					public override void EmitPushResetArgument(LocalBuilder argumentLocal, ILGenerator il)
					{
						il.Emit(OpCodes.Ldloc, argumentLocal);
						il.Emit(OpCodes.Ldfld, LexemeTokenArgumentTokenFieldInfo);
						il.Emit(OpCodes.Ldloc, argumentLocal);
						il.Emit(OpCodes.Ldc_I4_0);
						il.Emit(OpCodes.Stfld, LexemeTokenArgumentTokenFieldInfo);
					}

					public override Argument CreateArgument(ProductionInstanceBuilder instanceBuilder, int index)
					{
						return new LexemeTokenArgument(instanceBuilder, index);
					}

					#endregion

					#region Nested Types

					private sealed class LexemeTokenArgument : Argument
					{
						#region Fields

						public TActualToken Token;

						#endregion

						public override object Build()
						{
							var token = Token;

							Token = default;

							return token;
						}

						public override void Reset()
						{
							Token = default;
						}

						public LexemeTokenArgument(ProductionInstanceBuilder instanceBuilder, int index) : base(instanceBuilder, index)
						{
						}
					}

					#endregion

					public LexemeToken(LexerArgumentBuilder argumentBuilder) : base(argumentBuilder)
					{
					}
				}

				#endregion
			}

			private sealed class LocalLexerArgumentBuilder : LexerArgumentBuilder<TToken>
			{
				#region Ctors

				public LocalLexerArgumentBuilder(Type argumentType, MatchEntry matchEntry) : base(argumentType, matchEntry)
				{
				}

				#endregion
			}

			private sealed class ExternalLexemeArgumentBuilder<TExternalToken> : LexerArgumentBuilder<TExternalToken> where TExternalToken : unmanaged, Enum
			{
				#region Fields

				private readonly ExtParserILBuilder _parserBuilder = new ExtParserILBuilder();

				#endregion

				#region Ctors

				public ExternalLexemeArgumentBuilder(Type argumentType, MatchEntry matchEntry) : base(argumentType, matchEntry)
				{
				}

				#endregion

				#region Methods

				public override void EmitConsumeValue(IParserILBuilder builder, ILBuilderContext ilBuilderContext)
				{
					_parserBuilder.Enter(builder, ilBuilderContext);

					base.EmitConsumeValue(_parserBuilder, ilBuilderContext);

					_parserBuilder.Leave();
				}

				#endregion

				#region Nested Types

				private sealed class ExtParserILBuilder : IParserILBuilder
				{
					#region Static Fields and Constants

					private const BindingFlags Flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

					private static readonly FieldInfo LexemeTokenFieldInfo = typeof(Lexeme<TExternalToken>).GetField(nameof(Lexeme<TExternalToken>.TokenField), Flags);
					private static readonly FieldInfo LexemeStartFieldInfo = typeof(Lexeme<TExternalToken>).GetField(nameof(Lexeme<TExternalToken>.StartField), Flags);
					private static readonly FieldInfo LexemeEndFieldInfo = typeof(Lexeme<TExternalToken>).GetField(nameof(Lexeme<TExternalToken>.EndField), Flags);
					private static readonly MethodInfo TextSourceGetTextMethodInfo = typeof(TextSource).GetMethod(nameof(TextSource.GetText), Flags);
					private static readonly MethodInfo DebugMethodInfo = typeof(ExtParserILBuilder).GetMethod(nameof(Debug), BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);

					#endregion

					#region Fields

					private IParserILBuilder _actualBuilder;

					private LocalBuilder _lexemeLocal;

					#endregion

					#region Methods

					public static void Debug(Lexeme<TExternalToken> lexeme)
					{
					}

					public void EmitGetLexemeText(LocalBuilder lexemeLocal, ILBuilderContext ilBuilderContext)
					{
						EmitLdTextSource(ilBuilderContext);

						ilBuilderContext.IL.Emit(OpCodes.Ldloc, lexemeLocal);
						ilBuilderContext.IL.Emit(OpCodes.Ldfld, LexemeStartFieldInfo);
						ilBuilderContext.IL.Emit(OpCodes.Ldloc, lexemeLocal);
						ilBuilderContext.IL.Emit(OpCodes.Ldfld, LexemeEndFieldInfo);

						ilBuilderContext.IL.Emit(OpCodes.Callvirt, TextSourceGetTextMethodInfo);
					}

					public void Enter(IParserILBuilder builder, ILBuilderContext ilBuilderContext)
					{
						_lexemeLocal = ilBuilderContext.IL.DeclareLocal(typeof(Lexeme<TExternalToken>));
						_actualBuilder = builder;

						ilBuilderContext.IL.Emit(OpCodes.Stloc, _lexemeLocal);

						//ilBuilderContext.IL.Emit(OpCodes.Ldloc, _lexemeLocal);
						//ilBuilderContext.IL.Emit(OpCodes.Call, DebugMethodInfo);
					}

					public void Leave()
					{
						_lexemeLocal = null;
						_actualBuilder = null;
					}

					#endregion

					#region Interface Implementations

					#region Parser<TGrammar,TToken>.ParserAutomata.IParserILBuilder

					public void EmitGetInstruction(ILBuilderContext ilBuilderContext)
					{
						ilBuilderContext.IL.Emit(OpCodes.Ldloc, _lexemeLocal);
					}

					public void EmitGetInstructionText(ILBuilderContext ilBuilderContext)
					{
						EmitGetLexemeText(_lexemeLocal, ilBuilderContext);
					}

					public void EmitGetInstructionToken(ILBuilderContext ilBuilderContext)
					{
						ilBuilderContext.IL.Emit(OpCodes.Ldfld, LexemeTokenFieldInfo);
					}

					public void EmitLdTextSource(ILBuilderContext ilBuilderContext)
					{
						_actualBuilder.EmitLdTextSource(ilBuilderContext);
					}

					#endregion

					#endregion
				}

				#endregion
			}

			#endregion
		}

		#endregion
	}
}