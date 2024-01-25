// <copyright file="Parser - Copy.Automata.ParserProduction.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

// ReSharper disable StaticMemberInGenericType

namespace Zaaml.Text
{
	internal partial class Parser<TGrammar, TToken>
	{
		private sealed partial class ParserAutomata
		{
			private sealed class ProductionArgumentCollection : IReadOnlyList<ProductionArgument>
			{
				public ProductionArgumentCollection(ParserProduction parserProduction)
				{
					ParserProduction = parserProduction;
					Arguments = ArgumentsList.AsReadOnly();
				}

				private ReadOnlyCollection<ProductionArgument> Arguments { get; }

				private Dictionary<Entry, ProductionArgument> ArgumentsDictionary { get; } = new();

				private List<ProductionArgument> ArgumentsList { get; } = new();

				private ParserProduction ParserProduction { get; }

				public void AddArgument(ProductionArgument argument)
				{
					ArgumentsList.Add(argument);
					ArgumentsDictionary[argument.ParserEntry] = argument;
				}

				public bool TryGetArgument(Entry entry, out ProductionArgument productionArgument)
				{
					return ArgumentsDictionary.TryGetValue(entry, out productionArgument);
				}

				public IEnumerator<ProductionArgument> GetEnumerator()
				{
					return Arguments.GetEnumerator();
				}

				IEnumerator IEnumerable.GetEnumerator()
				{
					return ((IEnumerable)Arguments).GetEnumerator();
				}

				public int Count => Arguments.Count;

				public ProductionArgument this[int index] => Arguments[index];
			}
		}
	}
}