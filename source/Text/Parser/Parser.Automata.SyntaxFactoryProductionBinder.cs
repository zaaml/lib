// <copyright file="Parser.Automata.SyntaxFactoryProductionBinder.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Zaaml.Text
{
	internal abstract partial class Parser<TGrammar, TToken>
	{
		private partial class ParserAutomata
		{
			private sealed class SyntaxFactoryProductionBinder : ParserProductionBinder
			{
				public SyntaxFactoryProductionBinder(ParserProduction parserProduction)
				{
					var flatEntries = parserProduction.FlatEntries;
					var binding = (Grammar<TToken>.SyntaxFactoryParserProductionBinding) parserProduction.GrammarParserProduction.Binding;
					var nodeType = binding.NodeType;

					Template = new ArgumentBuilder[flatEntries.Count];

					//var lambdaExpression = ((LambdaExpression) binding.Expression);
					//var methodCallExpression = (MethodCallExpression) lambdaExpression.Body;
					//var methodInfo = methodCallExpression.Method;
					var methodInfo = binding.Method;
					var parameters = methodInfo.GetParameters();
					var arguments = new List<int>();

					foreach (var parameterInfo in parameters)
					{
						if (typeof(SyntaxFactory).IsAssignableFrom(parameterInfo.ParameterType))
							continue;

						var flatEntry = parserProduction.GetEntry(parameterInfo.Name);

						if (flatEntry == null)
							throw new InvalidOperationException(
								$"GrammarTransition '{parserProduction.GrammarParserProduction}' node type '{nodeType.Name}' factory has argument '{parameterInfo.Name}' without corresponding transition entry.");

						var argumentBuilder = CreateArgumentBuilder(parameterInfo.ParameterType, parameterInfo, flatEntry);

						Template[flatEntry.FlatIndex] = argumentBuilder;

						arguments.Add(flatEntry.FlatIndex);
					}

					Arguments = arguments.ToArray();
					FactoryTarget = binding.MethodTarget;
					FactoryInfo = methodInfo;
				}

				public override bool IsConstValue => false;
			}
		}
	}
}