//// <copyright file="Parser.Automata.ParserEntityArgumentFactory.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
////   Copyright (c) Zaaml. All rights reserved.
//// </copyright>

//using System;
//using System.Reflection;
//using System.Reflection.Emit;

//namespace Zaaml.Text
//{
//	internal abstract partial class Parser<TGrammar, TToken>
//	{
//		private sealed partial class ParserAutomata
//		{
//			private abstract class ParserEntityArgumentFactory : EntityArgumentFactory
//			{
//				protected ParserEntityArgumentFactory(ProductionArgument argument) : base(argument)
//				{
//				}
//			}

//			private sealed class ParserEntityArgumentFactory<TResult> : ParserEntityArgumentFactory
//			{
//				private readonly EntityArgumentFactoryBase _argumentFactory;

//				public ParserEntityArgumentFactory(ProductionArgument argument) : base(argument)
//				{
//					if (argument.ArgumentType == typeof(TResult))
//						_argumentFactory = new ValueEntityArgumentFactory(this);
//					else if (argument.ArgumentType == typeof(TResult[]))
//						_argumentFactory = new ValueArrayEntityArgumentFactory(this);
//					else
//						throw new ArgumentOutOfRangeException();
//				}

//				public override ProductionEntityArgument CreateArgument(ProductionEntity entity)
//				{
//					return _argumentFactory.CreateArgument(entity);
//				}

//				public override void EmitConsumeValue(IParserILBuilder builder, ILBuilderContext ilBuilderContext)
//				{
//					_argumentFactory.EmitConsumeValue(ilBuilderContext);
//				}

//				public override void EmitPushResetArgument(LocalBuilder argumentLocal, ILGenerator il)
//				{
//					_argumentFactory.EmitPushResetArgument(argumentLocal, il);
//				}

//				private abstract class EntityArgumentFactoryBase
//				{
//					protected EntityArgumentFactoryBase(ParserEntityArgumentFactory<TResult> entityArgumentFactory)
//					{
//						EntityArgumentFactory = entityArgumentFactory;
//					}

//					protected ParserEntityArgumentFactory<TResult> EntityArgumentFactory { get; }

//					public abstract ProductionEntityArgument CreateArgument(ProductionEntity entity);

//					public abstract void EmitConsumeValue(ILBuilderContext ilBuilderContext);

//					public abstract void EmitPushResetArgument(LocalBuilder argumentLocal, ILGenerator il);
//				}

//				private sealed class ValueEntityArgumentFactory : EntityArgumentFactoryBase
//				{
//					private static readonly FieldInfo ValueArgumentResultFieldInfo = typeof(Argument<TResult>).GetField(nameof(Argument<TResult>.Value), BindingFlags.Public | BindingFlags.Instance);

//					public ValueEntityArgumentFactory(ParserEntityArgumentFactory<TResult> entityArgumentFactory) : base(entityArgumentFactory)
//					{
//					}

//					public override ProductionEntityArgument CreateArgument(ProductionEntity entity)
//					{
//						return new Argument<TResult>(entity, EntityArgumentFactory);
//					}

//					public override void EmitConsumeValue(ILBuilderContext ilBuilderContext)
//					{
//						ilBuilderContext.IL.Emit(OpCodes.Stfld, ValueArgumentResultFieldInfo);
//					}

//					public override void EmitPushResetArgument(LocalBuilder argumentLocal, ILGenerator il)
//					{
//						il.Emit(OpCodes.Ldloc, argumentLocal);
//						il.Emit(OpCodes.Ldfld, ValueArgumentResultFieldInfo);
//						il.Emit(OpCodes.Ldloc, argumentLocal);
//						il.Emit(OpCodes.Ldnull);
//						il.Emit(OpCodes.Stfld, ValueArgumentResultFieldInfo);
//					}
//				}

//				private sealed class ValueArrayEntityArgumentFactory : EntityArgumentFactoryBase
//				{
//					private static readonly MethodInfo ListTResultAddMethodInfo = typeof(ArrayArgument<TResult>).GetMethod(nameof(ArrayArgument<TResult>.Add), BindingFlags.Instance | BindingFlags.Public);

//					private static readonly MethodInfo ListTResultToArrayMethodInfo =
//						typeof(ArrayArgument<TResult>).GetMethod(nameof(ArrayArgument<TResult>.ToArray), BindingFlags.Instance | BindingFlags.Public);

//					public ValueArrayEntityArgumentFactory(ParserEntityArgumentFactory<TResult> entityArgumentFactory) : base(entityArgumentFactory)
//					{
//					}

//					public override ProductionEntityArgument CreateArgument(ProductionEntity entity)
//					{
//						return new ArrayArgument<TResult>(entity, EntityArgumentFactory);
//					}

//					public override void EmitConsumeValue(ILBuilderContext ilBuilderContext)
//					{
//						ilBuilderContext.IL.Emit(OpCodes.Call, ListTResultAddMethodInfo);
//					}

//					public override void EmitPushResetArgument(LocalBuilder argumentLocal, ILGenerator il)
//					{
//						il.Emit(OpCodes.Ldloc, argumentLocal);
//						il.Emit(OpCodes.Call, ListTResultToArrayMethodInfo);
//					}
//				}
//			}
//		}
//	}
//}