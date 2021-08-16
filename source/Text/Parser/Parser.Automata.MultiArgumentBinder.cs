// <copyright file="Parser.Automata.MultiArgumentBinder.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;

// ReSharper disable ForCanBeConvertedToForeach

namespace Zaaml.Text
{
	internal abstract partial class Parser<TGrammar, TToken>
	{
		private sealed partial class ParserAutomata
		{
			private sealed class MultiArgumentBinder : ProductionArgumentBinder
			{
				private static readonly MethodInfo ProductionEntityArgumentGetCountMethodInfo = typeof(ProductionEntityArgument).GetMethod(nameof(ProductionEntityArgument.GetCount), BindingFlags.Instance | BindingFlags.Public);

				public MultiArgumentBinder(ProductionArgument[] productionArguments, Type argumentType) : base(argumentType)
				{
					ProductionArguments = productionArguments;

					for (var i = 0; i < ProductionArguments.Length; i++)
						ProductionArguments[i].Bind(this);
				}

				public ProductionArgument[] ProductionArguments { get; }

				public override bool ConsumeValue => true;

				public override void EmitPushResetArgument(LocalBuilder productionEntityLocal, LocalBuilder entityArgumentLocal, ILGenerator ilBuilder, OpCode processLdArg)
				{
					var elementType = ArgumentType.GetElementType();

					Debug.Assert(elementType != null);

					var indexLocal = ilBuilder.DeclareLocal(typeof(int));

					ilBuilder.Emit(OpCodes.Ldc_I4_0);
					ilBuilder.Emit(OpCodes.Stloc, indexLocal);

					for (var i = ProductionArguments.Length - 1; i >= 0; i--)
					{
						var argument = ProductionArguments[i];

						ilBuilder.Emit(OpCodes.Ldloc, productionEntityLocal);
						ilBuilder.Emit(OpCodes.Ldfld, ProductionEntityArgumentsFieldInfo);
						ilBuilder.Emit(OpCodes.Ldc_I4, argument.ArgumentIndex);
						ilBuilder.Emit(OpCodes.Ldelem_Ref);

						ilBuilder.Emit(OpCodes.Dup);

						ilBuilder.Emit(OpCodes.Callvirt, ProductionEntityArgumentGetCountMethodInfo);

						ilBuilder.Emit(OpCodes.Ldloc, indexLocal);
						ilBuilder.Emit(OpCodes.Add);
						ilBuilder.Emit(OpCodes.Stloc, indexLocal);
					}

					ilBuilder.Emit(OpCodes.Ldloc, indexLocal);
					ilBuilder.Emit(OpCodes.Newarr, elementType);

					ilBuilder.Emit(OpCodes.Ldc_I4_0);
					ilBuilder.Emit(OpCodes.Stloc, indexLocal);

					for (var i = 0; i < ProductionArguments.Length; i++)
					{
						var argument = ProductionArguments[i];

						ilBuilder.Emit(OpCodes.Ldloca, indexLocal);

						argument.EmitCopyArgument(entityArgumentLocal, ArgumentType, ilBuilder, processLdArg);
					}
				}
			}
		}
	}
}