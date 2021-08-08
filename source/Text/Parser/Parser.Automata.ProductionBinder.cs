// <copyright file="Parser.Automata.ProductionBinder.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Linq.Expressions;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

// ReSharper disable ForCanBeConvertedToForeach

namespace Zaaml.Text
{
	internal abstract partial class Parser<TGrammar, TToken>
	{
		private sealed partial class ParserAutomata
		{
			private abstract class ProductionBinder
			{
				private Func<ProductionEntity, ParserProcess, object> _createInstanceDelegate;

				protected virtual ProductionArgumentBinder[] ArgumentBinders => Array.Empty<ProductionArgumentBinder>();

				protected virtual object ConstValue => null;

				private Func<ProductionEntity, ParserProcess, object> CreateInstanceDelegate => _createInstanceDelegate ??= BuildCreateInstanceDelegate();

				public abstract bool IsFactoryBinder { get; }

				protected bool Return { get; set; }

				public bool TryReturn { get; protected set; }

				private Func<ProductionEntity, ParserProcess, object> BuildCreateInstanceDelegate()
				{
					if (ConstValue != null)
						return (_, _) => ConstValue;

					var dynMethod = new DynamicMethod("CreateInstance", typeof(object), new[] { typeof(ProductionBinder), typeof(ProductionEntity), typeof(ParserProcess) }, typeof(ProductionBinder), true);
					var ilBuilder = dynMethod.GetILGenerator();
					var productionEntityLocal = ilBuilder.DeclareLocal(typeof(ProductionEntity));
					var entityArgumentLocal = ilBuilder.DeclareLocal(typeof(ProductionEntityArgument));

					ilBuilder.Emit(OpCodes.Ldarg_1);
					ilBuilder.Emit(OpCodes.Stloc, productionEntityLocal);

					EmitEnter(ilBuilder);

					for (var index = 0; index < ArgumentBinders.Length; index++)
						ArgumentBinders[index].EmitPushResetArgument(productionEntityLocal, entityArgumentLocal, ilBuilder, OpCodes.Ldarg_2);

					if (Return)
					{
					}
					else
						EmitLeave(ilBuilder);

					ilBuilder.Emit(OpCodes.Ret);

					var delegateType = Expression.GetDelegateType(typeof(ProductionEntity), typeof(ParserProcess), typeof(object));

					return (Func<ProductionEntity, ParserProcess, object>)dynMethod.CreateDelegate(delegateType, this);
				}

				protected static ProductionArgumentBinder CreateArgumentBinder(Type argumentType, ProductionArgumentCollection productionArguments)
				{
					return productionArguments.Length == 1 ? new SingleArgumentBinder(productionArguments[0], argumentType) : new MultiArgumentBinder(productionArguments.ToArray(), argumentType);
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public object CreateInstance(ProductionEntity productionEntity, ParserProcess process)
				{
					if (TryReturn && productionEntity.ConsumeCount == 1)
						return productionEntity.Arguments[0].Build();

					return CreateInstanceDelegate(productionEntity, process);
				}

				protected virtual void EmitEnter(ILGenerator ilBuilder)
				{
				}

				protected virtual void EmitLeave(ILGenerator ilBuilder)
				{
				}
			}
		}
	}
}