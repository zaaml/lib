// <copyright file="Grammar.GrammarSymbolCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections;
using System.Collections.Generic;

namespace Zaaml.Text
{
	internal abstract partial class Grammar<TGrammar, TToken>
	{
		protected internal class GrammarSymbolCollection<TGrammarSyntax, TGrammarProduction, TGrammarSymbol> : IReadOnlyList<TGrammarSymbol>
			where TGrammarSyntax : GrammarSyntax<TGrammarSyntax, TGrammarProduction, TGrammarSymbol>
			where TGrammarProduction : GrammarProduction<TGrammarSyntax, TGrammarProduction, TGrammarSymbol>
			where TGrammarSymbol : GrammarSymbol<TGrammarSyntax, TGrammarProduction, TGrammarSymbol>
		{
			public GrammarSymbolCollection(TGrammarProduction production, IReadOnlyList<TGrammarSymbol> symbolList)
			{
				Production = production;
				ReadOnlyList = symbolList;
			}

			public TGrammarProduction Production { get; }

			private IReadOnlyList<TGrammarSymbol> ReadOnlyList { get; }

			public IEnumerator<TGrammarSymbol> GetEnumerator()
			{
				return ReadOnlyList.GetEnumerator();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return ((IEnumerable)ReadOnlyList).GetEnumerator();
			}

			public int Count => ReadOnlyList.Count;

			public TGrammarSymbol this[int index] => ReadOnlyList[index];
		}
	}
}