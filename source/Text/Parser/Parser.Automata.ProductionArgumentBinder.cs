// <copyright file="Parser.Automata.ProductionArgumentBinder.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Zaaml.Text
{
	internal abstract partial class Parser<TGrammar, TToken>
	{
		private sealed partial class ParserAutomata
		{
			private abstract class ProductionArgumentBinder
			{
				protected static readonly FieldInfo ProductionEntityArgumentsFieldInfo = typeof(ProductionEntity).GetField(nameof(ProductionEntity.Arguments), BindingFlags.Instance | BindingFlags.Public);

				protected ProductionArgumentBinder(Type argumentType)
				{
					ArgumentType = argumentType;
				}

				public Type ArgumentType { get; }

				public abstract void EmitPushResetArgument(LocalBuilder productionEntityLocal, LocalBuilder entityArgumentLocal, ILGenerator ilBuilder, OpCode contextLdArg);
			}
		}
	}
}