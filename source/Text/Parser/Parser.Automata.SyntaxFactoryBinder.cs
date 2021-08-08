// <copyright file="Parser.Automata.SyntaxFactoryBinder.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Zaaml.Text
{
	internal abstract partial class Parser<TGrammar, TToken>
	{
		private partial class ParserAutomata
		{
			private sealed class SyntaxFactoryBinder : ProductionBinder
			{
				private static readonly FieldInfo FactoryTargetFieldInfo = typeof(SyntaxFactoryBinder).GetField(nameof(_factoryTarget), BindingFlags.Instance | BindingFlags.NonPublic);
				private readonly MethodInfo _factoryInfo;
				private object _factoryTarget;

				public SyntaxFactoryBinder(ParserProduction parserProduction)
				{
					var binding = (Grammar<TToken>.SyntaxFactoryBinding)parserProduction.GrammarParserProduction.ProductionBinding;
					var nodeType = binding.NodeType;

					var arguments = parserProduction.Arguments;
					var unwrap = parserProduction.GrammarParserProduction.Unwrap;

					TryReturn = arguments.Count > 0 && ((arguments[0].ParserEntry as ParserRuleEntry)?.TryReturn ?? false);

					if (unwrap)
					{
						Return = true;
						ArgumentBinders = new[] { CreateArgumentBinder(nodeType, parserProduction.GetArguments(arguments[0].Name)) };

						return;
					}

					////var lambdaExpression = ((LambdaExpression) binding.Expression);
					////var methodCallExpression = (MethodCallExpression) lambdaExpression.Body;
					////var methodInfo = methodCallExpression.Method;
					var methodInfo = binding.Method;
					var parameters = methodInfo.GetParameters();

					ArgumentBinders = new ProductionArgumentBinder[parameters.Length];

					for (var index = 0; index < parameters.Length; index++)
					{
						var parameterInfo = parameters[index];

						if (typeof(SyntaxFactory).IsAssignableFrom(parameterInfo.ParameterType))
						{
							ArgumentBinders[index] = new SyntaxFactoryArgumentBinder(typeof(SyntaxFactory));

							continue;
						}

						var productionArguments = parserProduction.GetArguments(parameterInfo.Name);

						if (productionArguments.Length == 0)
							throw new InvalidOperationException($"GrammarProduction '{parserProduction.GrammarParserProduction}' node type '{nodeType.Name}' factory has argument '{parameterInfo.Name}' without corresponding production entry.");

						ArgumentBinders[index] = CreateArgumentBinder(parameterInfo.ParameterType, productionArguments);
					}

					_factoryTarget = binding.MethodTarget;
					_factoryInfo = methodInfo;
				}

				protected override ProductionArgumentBinder[] ArgumentBinders { get; }

				public override bool IsFactoryBinder => true;

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

					public override void EmitPushResetArgument(LocalBuilder productionEntityLocal, LocalBuilder entityArgumentLocal, ILGenerator ilBuilder, OpCode processLdArg)
					{
						ilBuilder.Emit(processLdArg);
						ilBuilder.Emit(OpCodes.Ldfld, ParserProcess.ParserILGenerator.SyntaxTreeFactoryFieldInfo);
					}
				}
			}
		}
	}
}