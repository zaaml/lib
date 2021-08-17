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
	internal partial class Parser<TGrammar, TToken>
	{
		private partial class ParserAutomata
		{
			private sealed class ConstructorBinder : FactoryBinder
			{
				public ConstructorBinder(ParserProduction parserProduction) : base(parserProduction)
				{
					Binding = (Grammar<TGrammar, TToken>.ParserGrammar.ConstructorNodeBinding)parserProduction.GrammarParserProduction.ProductionBinding;

					var nodeType = Binding.NodeType;

					if (nodeType == null)
						throw new ArgumentNullException(nameof(nodeType));

					ConstructorInfo = nodeType.GetConstructors().SingleOrDefault();

					if (ConstructorInfo == null)
						throw new InvalidOperationException();
				}

				private Grammar<TGrammar, TToken>.ParserGrammar.ConstructorNodeBinding Binding { get; }

				private ConstructorInfo ConstructorInfo { get; }

				protected override ProductionArgumentBinder[] CreateArgumentBinders()
				{
					if (ConstructorInfo == null)
						return Array.Empty<ProductionArgumentBinder>();

					var nodeType = Binding.NodeType;
					var parameters = ConstructorInfo.GetParameters();

					var argumentBinders = new ProductionArgumentBinder[parameters.Length];

					for (var index = 0; index < parameters.Length; index++)
					{
						var parameterInfo = parameters[index];
						var productionArguments = ParserProduction.GetArguments(parameterInfo.Name);

						if (productionArguments.Length == 0)
							throw new InvalidOperationException($"GrammarProduction '{ParserProduction.GrammarParserProduction}' node type '{nodeType.Name}' ctor has argument '{parameterInfo.Name}' without corresponding production symbol.");

						argumentBinders[index] = CreateArgumentBinder(parameterInfo.ParameterType, productionArguments);
					}

					return argumentBinders;
				}

				protected override void EmitEnter(ILGenerator ilBuilder)
				{
				}

				protected override void EmitLeave(ILGenerator ilBuilder)
				{
					ilBuilder.Emit(OpCodes.Newobj, ConstructorInfo);
				}
			}
		}
	}
}