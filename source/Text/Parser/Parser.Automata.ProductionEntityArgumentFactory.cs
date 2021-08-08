// <copyright file="Parser.Automata.ProductionEntityArgumentFactory.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

//using System;
//using System.Reflection.Emit;

//#pragma warning disable 414

//namespace Zaaml.Text
//{
//	internal abstract partial class Parser<TGrammar, TToken>
//	{
//		private sealed partial class ParserAutomata
//		{
//			private abstract class EntityArgumentFactory
//			{
//				protected EntityArgumentFactory(ProductionArgument argument)
//				{
//					ProductionArgument = argument;
//				}

//				public Type ArgumentType => ProductionArgument.ArgumentType;

//				public int ArgumentIndex => ProductionArgument.ArgumentIndex;

//				public ProductionArgument ProductionArgument { get; }

//				public abstract ProductionEntityArgument CreateArgument(ProductionEntity entity);

//				public abstract void EmitPushResetArgument(LocalBuilder argumentLocal, ILGenerator il);

//				public abstract void EmitConsumeValue(IParserILBuilder builder, context context);
//			}
//		}
//	}
//}

