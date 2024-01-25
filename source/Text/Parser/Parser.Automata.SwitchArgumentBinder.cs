// <copyright file="Parser.Automata.SwitchArgumentBinder.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Reflection;
using System.Reflection.Emit;

// ReSharper disable ForCanBeConvertedToForeach

namespace Zaaml.Text
{
	internal partial class Parser<TGrammar, TToken>
	{
		private sealed partial class ParserAutomata
		{
			private sealed class SwitchArgumentBinder : ProductionArgumentBinder
			{
				private static readonly MethodInfo ProductionEntityArgumentGetCountMethodInfo = typeof(ProductionEntityArgument).GetMethod(nameof(ProductionEntityArgument.GetCount), BindingFlags.Instance | BindingFlags.Public);

				public SwitchArgumentBinder(ProductionArgument[] productionArguments, Type argumentType) : base(argumentType)
				{
					ProductionArguments = productionArguments;

					for (var i = 0; i < ProductionArguments.Length; i++)
						ProductionArguments[i].Bind(this);
				}

				public override bool ConsumeValue => true;

				public ProductionArgument[] ProductionArguments { get; }

				public override void EmitPushResetArgument(LocalBuilder productionEntityLocal, LocalBuilder entityArgumentLocal, ILGenerator ilBuilder, OpCode processLdArg)
				{
					var exitLabel = ilBuilder.DefineLabel();

					for (var i = 0; i < ProductionArguments.Length; i++)
					{
						var nextLabel = ilBuilder.DefineLabel();
						var argument = ProductionArguments[i];

						ilBuilder.Emit(OpCodes.Ldloc, productionEntityLocal);
						ilBuilder.Emit(OpCodes.Ldfld, ProductionEntityArgumentsFieldInfo);
						ilBuilder.Emit(OpCodes.Ldc_I4, argument.ArgumentIndex);
						ilBuilder.Emit(OpCodes.Ldelem_Ref);

						ilBuilder.Emit(OpCodes.Dup);

						ilBuilder.Emit(OpCodes.Callvirt, ProductionEntityArgumentGetCountMethodInfo);
						ilBuilder.Emit(OpCodes.Ldc_I4_0);

						ilBuilder.Emit(OpCodes.Beq, nextLabel);

						ilBuilder.Emit(OpCodes.Stloc, entityArgumentLocal);
						argument.EmitPushResetArgument(entityArgumentLocal, ArgumentType, ilBuilder, processLdArg);
						ilBuilder.Emit(OpCodes.Br, exitLabel);

						ilBuilder.MarkLabel(nextLabel);

						ilBuilder.Emit(OpCodes.Pop);
					}

					if (ArgumentType.IsValueType)
					{
						var defaultValue = ilBuilder.DeclareLocal(ArgumentType);

						ilBuilder.Emit(OpCodes.Ldloca, defaultValue);
						ilBuilder.Emit(OpCodes.Initobj, ArgumentType);
						ilBuilder.Emit(OpCodes.Ldloc, defaultValue);
					}
					else
						ilBuilder.Emit(OpCodes.Ldnull);

					ilBuilder.MarkLabel(exitLabel);
				}
			}
		}
	}
}