// <copyright file="Parser.Automata.SyntaxFactoryBinder.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Zaaml.Text
{
	internal partial class Parser<TGrammar, TToken>
	{
		private partial class ParserAutomata
		{
			private sealed class SyntaxNodeFactoryBinder : FactoryBinder
			{
				private static readonly FieldInfo FactoryTargetFieldInfo = typeof(SyntaxNodeFactoryBinder).GetField(nameof(_factoryTarget), BindingFlags.Instance | BindingFlags.NonPublic);
				private readonly MethodInfo _factoryInfo;
				private object _factoryTarget;

				public SyntaxNodeFactoryBinder(ParserProduction parserProduction) : base(parserProduction)
				{
					Binding = (Grammar<TGrammar, TToken>.ParserGrammar.SyntaxNodeFactoryBinding)parserProduction.GrammarParserProduction.ProductionBinding;

					////var lambdaExpression = ((LambdaExpression) binding.Expression);
					////var methodCallExpression = (MethodCallExpression) lambdaExpression.Body;
					////var methodInfo = methodCallExpression.Method;

					_factoryTarget = Binding.MethodTarget;
					_factoryInfo = Binding.Method;
				}

				private Grammar<TGrammar, TToken>.ParserGrammar.SyntaxNodeFactoryBinding Binding { get; }

				protected override ProductionArgumentBinder[] CreateArgumentBinders()
				{
					var nodeType = Binding.NodeType;
					var parameters = _factoryInfo.GetParameters();
					var argumentBinders = new ProductionArgumentBinder[parameters.Length];

					for (var index = 0; index < parameters.Length; index++)
					{
						var parameterInfo = parameters[index];

						if (typeof(SyntaxNodeFactory).IsAssignableFrom(parameterInfo.ParameterType))
						{
							argumentBinders[index] = new SyntaxFactoryArgumentBinder(typeof(SyntaxNodeFactory));

							continue;
						}

						var productionArguments = ParserProduction.GetArguments(parameterInfo.Name);

						if (productionArguments.Length == 0)
							throw new InvalidOperationException($"GrammarProduction '{ParserProduction.GrammarParserProduction}' node type '{nodeType.Name}' factory has argument '{parameterInfo.Name}' without corresponding production production.");

						argumentBinders[index] = CreateArgumentBinder(parameterInfo.ParameterType, productionArguments);
					}

					return argumentBinders;
				}

				protected override void EmitEnter(ILGenerator ilBuilder)
				{
					if (_factoryTarget == null)
						return;

					ilBuilder.Emit(OpCodes.Ldarg_0);
					ilBuilder.Emit(OpCodes.Ldfld, FactoryTargetFieldInfo);
				}

				protected override void EmitLeave(ILGenerator ilBuilder)
				{
					ilBuilder.Emit(OpCodes.Call, _factoryInfo);
				}

				private sealed class SyntaxFactoryArgumentBinder : ProductionArgumentBinder
				{
					public SyntaxFactoryArgumentBinder(Type syntaxFactoryType) : base(syntaxFactoryType)
					{
					}

					public override bool ConsumeValue => false;

					public override void EmitPushResetArgument(LocalBuilder productionEntityLocal, LocalBuilder entityArgumentLocal, ILGenerator ilBuilder, OpCode processLdArg)
					{
						ilBuilder.Emit(processLdArg);
						ilBuilder.Emit(OpCodes.Ldfld, ParserProcess.SyntaxTreeFactoryFieldInfo);
					}
				}
			}
		}
	}
}