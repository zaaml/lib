// <copyright file="Grammar.GrammarProductionCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections;
using System.Collections.Generic;

namespace Zaaml.Text
{
	internal abstract partial class Grammar<TGrammar, TToken>
	{
		protected internal sealed class GrammarProductionCollection<TGrammarSyntax, TGrammarProduction, TGrammarSymbol> : IReadOnlyList<TGrammarProduction>
			where TGrammarSyntax : GrammarSyntax<TGrammarSyntax, TGrammarProduction, TGrammarSymbol>
			where TGrammarProduction : GrammarProduction<TGrammarSyntax, TGrammarProduction, TGrammarSymbol>
			where TGrammarSymbol : GrammarSymbol<TGrammarSyntax, TGrammarProduction, TGrammarSymbol>
		{
			public GrammarProductionCollection(TGrammarSyntax grammarSyntax, IReadOnlyList<TGrammarProduction> productionsList)
			{
				GrammarSyntax = grammarSyntax;
				ReadOnlyList = productionsList;
			}

			public TGrammarSyntax GrammarSyntax { get; }

			private IReadOnlyList<TGrammarProduction> ReadOnlyList { get; }

			public IEnumerator<TGrammarProduction> GetEnumerator()
			{
				return ReadOnlyList.GetEnumerator();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return ((IEnumerable)ReadOnlyList).GetEnumerator();
			}

			public int Count => ReadOnlyList.Count;

			public TGrammarProduction this[int index] => ReadOnlyList[index];
		}
	}
}