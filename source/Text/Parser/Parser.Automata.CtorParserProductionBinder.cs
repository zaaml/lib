// <copyright file="Parser.Automata.CtorParserProductionBinder.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Zaaml.Text
{
	internal abstract partial class Parser<TGrammar, TToken>
	{
		private partial class ParserAutomata
		{
			private sealed class CtorParserProductionBinder : ParserProductionBinder
			{
				public CtorParserProductionBinder(ParserProduction parserProduction)
				{
					var flatEntries = parserProduction.FlatEntries;
					var unwrap = parserProduction.GrammarParserProduction.Unwrap;
					var binding = (Grammar<TToken>.ConstructorParserProductionBinding) parserProduction.GrammarParserProduction.Binding;
					var nodeType = binding.NodeType;

					Template = new ArgumentBuilder[flatEntries.Count];

					if (unwrap)
					{
						Return = true;
						Template[0] = CreateArgumentBuilder(nodeType, null, flatEntries[0]);
						Arguments = new[] {0};

						return;
					}

					if (nodeType == null)
						return;

					TryReturn = flatEntries.Count > 0 && ((flatEntries[0].ParserEntry as ParserStateEntry)?.TryReturn ?? false);

					var ctor = nodeType.GetConstructors().SingleOrDefault();

					if (ctor == null)
					{
						var instanceProperty = nodeType.GetProperty("Instance", BindingFlags.Static | BindingFlags.Public);
						var instanceField = nodeType.GetField("Instance", BindingFlags.Static | BindingFlags.Public);

						if (instanceProperty != null && instanceProperty.PropertyType == nodeType)
							ConstValue = instanceProperty.GetValue(null);
						else if (instanceField != null && instanceField.FieldType == nodeType)
							ConstValue = instanceField.GetValue(null);
						else
							throw new InvalidOperationException("No public ctor or singleton accessor");

						return;
					}

					var parameters = ctor.GetParameters();
					var arguments = new List<int>();

					foreach (var parameterInfo in parameters)
					{
						var flatEntry = parserProduction.GetEntry(parameterInfo.Name);

						if (flatEntry == null)
							throw new InvalidOperationException(
								$"GrammarTransition '{parserProduction.GrammarParserProduction}' node type '{nodeType.Name}' ctor has argument '{parameterInfo.Name}' without corresponding transition entry.");

						var argumentBuilder = CreateArgumentBuilder(parameterInfo.ParameterType, parameterInfo, flatEntry);

						Template[flatEntry.FlatIndex] = argumentBuilder;

						arguments.Add(flatEntry.FlatIndex);
					}

					Arguments = arguments.ToArray();
					ConstructorInfo = ctor;
				}

				public override bool IsConstValue => ConstructorInfo == null && Return == false;
			}
		}
	}
}