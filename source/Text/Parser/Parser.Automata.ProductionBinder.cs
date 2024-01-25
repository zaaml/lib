// <copyright file="Parser.Automata.ProductionBinder.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

// ReSharper disable ForCanBeConvertedToForeach

namespace Zaaml.Text
{
	internal partial class Parser<TGrammar, TToken>
	{
		private sealed partial class ParserAutomata
		{
			private abstract class ProductionBinder
			{
				public virtual object Bind(ProductionEntity productionEntity, ParserProcess process)
				{
					throw new NotSupportedException();
				}

				public void Build()
				{
					BuildCore();
				}

				protected abstract void BuildCore();
			}

			private abstract class ValueBinder : ProductionBinder
			{
				protected ValueBinder(ParserProduction parserProduction)
				{
					ParserProduction = parserProduction;
				}

				protected ParserProduction ParserProduction { get; }
			}

			private sealed class ConstValueBinder : ValueBinder
			{
				public ConstValueBinder(ParserProduction parserProduction) : base(parserProduction)
				{
					ConstValue = parserProduction.GrammarParserProduction.ProductionBinding.ConstValue;
				}

				private object ConstValue { get; }

				public override object Bind(ProductionEntity productionEntity, ParserProcess process)
				{
					return ConstValue;
				}

				protected override void BuildCore()
				{
				}
			}

			private sealed class ReturnValueBinder : ValueBinder
			{
				public ReturnValueBinder(ParserProduction parserProduction) : base(parserProduction)
				{
				}

				private SingleArgumentBinder ArgumentBinder { get; set; }

				public override object Bind(ProductionEntity productionEntity, ParserProcess process)
				{
					return productionEntity.Arguments[ArgumentBinder.ProductionArgument.ArgumentIndex].Build();
				}

				protected override void BuildCore()
				{
					var node = (Grammar<TGrammar, TToken>.ParserGrammar.NodeSyntax)ParserProduction.GrammarParserSyntax;
					var productionArgument = ParserProduction.Arguments.Single(a => node.NodeType.IsAssignableFrom(a.ArgumentType));

					ArgumentBinder = new SingleArgumentBinder(productionArgument, productionArgument.ArgumentType);
				}
			}

			private abstract class FactoryBinder : ValueBinder
			{
				private Func<ProductionEntity, ParserProcess, object> _createInstanceDelegate;

				protected FactoryBinder(ParserProduction parserProduction) : base(parserProduction)
				{
					_createInstanceDelegate = DeferCreateInstanceDelegate;
				}

				private object DeferCreateInstanceDelegate(ProductionEntity productionEntity, ParserProcess parserProcess)
				{
					_createInstanceDelegate = BuildCreateInstanceDelegate();

					return _createInstanceDelegate(productionEntity, parserProcess);
				}

				private ProductionArgumentBinder[] ArgumentBinders { get; set; }

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public sealed override object Bind(ProductionEntity productionEntity, ParserProcess process)
				{
					return _createInstanceDelegate(productionEntity, process);
				}

				protected override void BuildCore()
				{
					ArgumentBinders = CreateArgumentBinders();
				}

				private Func<ProductionEntity, ParserProcess, object> BuildCreateInstanceDelegate()
				{
					var dynMethod = new DynamicMethod("CreateInstance", typeof(object), new[] { typeof(ProductionBinder), typeof(ProductionEntity), typeof(ParserProcess) }, typeof(ProductionBinder), true);
					var ilBuilder = dynMethod.GetILGenerator();
					var productionEntityLocal = ilBuilder.DeclareLocal(typeof(ProductionEntity));
					var entityArgumentLocal = ilBuilder.DeclareLocal(typeof(ProductionEntityArgument));

					ilBuilder.Emit(OpCodes.Ldarg_1);
					ilBuilder.Emit(OpCodes.Stloc, productionEntityLocal);

					EmitEnter(ilBuilder);

					for (var index = 0; index < ArgumentBinders.Length; index++)
						ArgumentBinders[index].EmitPushResetArgument(productionEntityLocal, entityArgumentLocal, ilBuilder, OpCodes.Ldarg_2);

					EmitLeave(ilBuilder);

					ilBuilder.Emit(OpCodes.Ret);

					var delegateType = Expression.GetDelegateType(typeof(ProductionEntity), typeof(ParserProcess), typeof(object));

					return (Func<ProductionEntity, ParserProcess, object>)dynMethod.CreateDelegate(delegateType, this);
				}

				protected static ProductionArgumentBinder CreateArgumentBinder(Type argumentType, ProductionNamedArgumentCollection productionArguments)
				{
					switch (productionArguments.Length)
					{
						case 1:
							return new SingleArgumentBinder(productionArguments[0], argumentType);
						default:
							return argumentType.IsArray ? new MultiArgumentBinder(productionArguments.ToArray(), argumentType) : new SwitchArgumentBinder(productionArguments.ToArray(), argumentType);
					}
				}

				protected abstract ProductionArgumentBinder[] CreateArgumentBinders();

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