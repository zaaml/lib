// <copyright file="Parser.Automata.ProductionArgumentCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

// ReSharper disable ForCanBeConvertedToForeach

namespace Zaaml.Text
{
	internal partial class Parser<TGrammar, TToken>
	{
		private sealed partial class ParserAutomata
		{
			private readonly struct ProductionNamedArgumentCollection
			{
				private string Name { get; }

				private ParserProduction Production { get; }

				private StringComparison ComparisonType { get; }

				public ProductionNamedArgumentCollection(string name, ParserProduction production, StringComparison comparisonType)
				{
					Name = name;
					Production = production;
					ComparisonType = comparisonType;
					Length = 0;

					for (var index = 0; index < production.Arguments.Count; index++)
					{
						var argument = production.Arguments[index];

						if (string.Equals(name, argument.Name, comparisonType))
							Length++;
					}
				}

				public int Length { get; }

				public ProductionArgument this[int index]
				{
					get
					{
						var name = Name;

						for (var i = 0; i < Production.Arguments.Count; i++)
						{
							var argument = Production.Arguments[i];

							if (string.Equals(name, argument.Name, ComparisonType) == false)
								continue;

							if (index-- == 0)
								return argument;
						}

						throw new InvalidOperationException();
					}
				}

				public ProductionArgument[] ToArray()
				{
					var name = Name;
					var arguments = new ProductionArgument[Length];

					var index = 0;

					for (var i = 0; i < Production.Arguments.Count; i++)
					{
						var argument = Production.Arguments[i];

						if (string.Equals(name, argument.Name, ComparisonType) == false)
							continue;

						arguments[index++] = argument;
					}

					return arguments;
				}
			}
		}
	}
}