// <copyright file="Parser.Automata.SingleArgumentBinder.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
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
			private sealed class SingleArgumentBinder : ProductionArgumentBinder
			{
				public SingleArgumentBinder(ProductionArgument productionArgument, Type argumentType) : base(argumentType)
				{
					ProductionArgument = productionArgument;
					productionArgument.Bind(this);
				}

				public ProductionArgument ProductionArgument { get; }

				public override void EmitPushResetArgument(LocalBuilder productionEntityLocal, LocalBuilder entityArgumentLocal, ILGenerator ilBuilder, OpCode processLdArg)
				{
					ilBuilder.Emit(OpCodes.Ldloc, productionEntityLocal);
					ilBuilder.Emit(OpCodes.Ldfld, ProductionEntityArgumentsFieldInfo);
					ilBuilder.Emit(OpCodes.Ldc_I4, ProductionArgument.ArgumentIndex);
					ilBuilder.Emit(OpCodes.Ldelem_Ref);
					ilBuilder.Emit(OpCodes.Stloc, entityArgumentLocal);

					ProductionArgument.EmitPushResetArgument(entityArgumentLocal, ArgumentType, ilBuilder, processLdArg);
				}
			}
		}
	}
}