// <copyright file="Parser.Automata.ConstructorBinder.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

// ReSharper disable ForCanBeConvertedToForeach

namespace Zaaml.Text
{
	internal abstract partial class Parser<TGrammar, TToken>
	{
		private partial class ParserAutomata
		{
			private sealed class ConstructorBinder : ProductionBinder
			{
				public ConstructorBinder(Type nodeType, ParserProduction parserProduction)
				{
					if (nodeType == null)
						throw new ArgumentNullException(nameof(nodeType));
					
					ConstructorInfo = nodeType.GetConstructors().SingleOrDefault();
					ConstValue = ConstructorInfo == null ? GetConstValue(nodeType) : null;

					var arguments = parserProduction.Arguments;
					var unwrap = parserProduction.GrammarParserProduction.Unwrap;

					TryReturn = arguments.Count > 0 && ((arguments[0].ParserEntry as ParserRuleEntry)?.TryReturn ?? false);

					if (unwrap)
					{
						Return = true;
						ArgumentBinders = new[] { CreateArgumentBinder(nodeType, parserProduction.GetArguments(arguments[0].Name)) };

						return;
					}

					if (ConstructorInfo == null)
					{
						ArgumentBinders = Array.Empty<ProductionArgumentBinder>();

						return;
					}

					var parameters = ConstructorInfo.GetParameters();

					ArgumentBinders = new ProductionArgumentBinder[parameters.Length];

					for (var index = 0; index < parameters.Length; index++)
					{
						var parameterInfo = parameters[index];
						var productionArguments = parserProduction.GetArguments(parameterInfo.Name);

						if (productionArguments.Length == 0)
							throw new InvalidOperationException($"GrammarProduction '{parserProduction.GrammarParserProduction}' node type '{nodeType.Name}' ctor has argument '{parameterInfo.Name}' without corresponding production entry.");

						ArgumentBinders[index] = CreateArgumentBinder(parameterInfo.ParameterType, productionArguments);
					}
				}

				protected override ProductionArgumentBinder[] ArgumentBinders { get; }

				private ConstructorInfo ConstructorInfo { get; }

				protected override object ConstValue { get; }

				protected override void EmitEnter(ILGenerator ilBuilder)
				{
				}

				protected override void EmitLeave(ILGenerator ilBuilder)
				{
					ilBuilder.Emit(OpCodes.Newobj, ConstructorInfo);
				}

				private static object GetConstValue(Type nodeType)
				{
					var instanceProperty = nodeType.GetProperty("Instance", BindingFlags.Static | BindingFlags.Public);
					var instanceField = nodeType.GetField("Instance", BindingFlags.Static | BindingFlags.Public);

					if (instanceProperty != null && instanceProperty.PropertyType == nodeType)
						return instanceProperty.GetValue(null);

					if (instanceField != null && instanceField.FieldType == nodeType)
						return instanceField.GetValue(null);

					throw new InvalidOperationException("No public ctor or singleton accessor");
				}
			}
		}
	}
}