// <copyright file="Grammar.GrammarProduction.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Zaaml.Text
{
	internal abstract partial class Grammar<TGrammar, TToken>
	{
		protected internal abstract class GrammarProduction<TGrammarSyntax, TGrammarProduction, TGrammarSymbol>
			where TGrammarSyntax : GrammarSyntax<TGrammarSyntax, TGrammarProduction, TGrammarSymbol>
			where TGrammarProduction : GrammarProduction<TGrammarSyntax, TGrammarProduction, TGrammarSymbol>
			where TGrammarSymbol : GrammarSymbol<TGrammarSyntax, TGrammarProduction, TGrammarSymbol>
		{
			protected GrammarProduction()
			{
				Symbols = new((TGrammarProduction)this, SymbolList);
			}

			private List<TGrammarSymbol> SymbolList { get; } = new();

			public GrammarSymbolCollection<TGrammarSyntax, TGrammarProduction, TGrammarSymbol> Symbols { get; }

			private protected void AddSymbolCore(TGrammarSymbol symbol)
			{
				SymbolList.Add(symbol);
			}

			private protected void InsertSymbolCore(int index, TGrammarSymbol symbol)
			{
				SymbolList.Insert(index, symbol);
			}
		}
	}
}